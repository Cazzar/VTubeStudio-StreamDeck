using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Cazzar.Deck.Shared;

namespace Cazzar.Deck.Generators;

[Generator]
public sealed class DeckActionGenerator : IIncrementalGenerator
{
    private const string ActionAttribute = "Cazzar.Deck.Abstractions.DeckActionAttribute";
    private const string FeatureAttribute = "Cazzar.Deck.Abstractions.RequiresFeatureAttribute";
    private const string CommandAttribute = "Cazzar.Deck.Abstractions.Actions.PropertyViewCommandAttribute";
    private const string SerializableAttribute = "System.Text.Json.Serialization.JsonSerializableAttribute";
    private const string ActionBase = "Cazzar.Deck.Core.Actions.DeckAction`1";
    private const int EncoderFeature = DeckHostFlags.EncoderFeature;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var actions = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ActionAttribute,
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) => Describe(ctx))
            .Where(static a => a is not null)
            .Select(static (a, _) => a!);

        context.RegisterSourceOutput(
            actions.Collect().Combine(context.CompilationProvider),
            static (spc, pair) => Emit(spc, pair.Left, pair.Right));
    }

    private sealed record ActionModel(
        string ActionId,
        string TypeName,
        string HostFlags,
        bool NeedsEncoder,
        bool ExcludesEncoderHosts,
        ImmutableArray<string> ConstructorArguments,
        ImmutableArray<CommandModel> Commands,
        string? SettingsType,
        Location Location);

    private sealed record CommandModel(string Command, string Method, bool TakesPayload);

    private static ActionModel? Describe(GeneratorAttributeSyntaxContext ctx)
    {
        if (ctx.TargetSymbol is not INamedTypeSymbol type || type.IsAbstract) return null;

        var attribute = ctx.Attributes[0];
        var actionId = attribute.ConstructorArguments.Length > 0
            ? attribute.ConstructorArguments[0].Value as string
            : DeckActionNaming.Derive(type.Name);

        if (actionId is null or "") return null;

        var hosts = attribute.NamedArguments
            .FirstOrDefault(a => a.Key == "Hosts").Value.Value as int? ?? DeckHostFlags.All;

        var constructor = type.InstanceConstructors
            .Where(c => c.DeclaredAccessibility == Accessibility.Public)
            .OrderByDescending(c => c.Parameters.Length)
            .FirstOrDefault();

        return new(
            actionId,
            type.ToDisplayString(),
            HostFlags(hosts),
            NeedsEncoder(type),
            hosts != 0 && (hosts & DeckHostFlags.EncoderHosts) == 0,
            constructor?.Parameters.Select(Argument).ToImmutableArray() ?? [],
            Commands(type),
            SettingsType(type),
            ctx.TargetNode.GetLocation());
    }

    private static string? SettingsType(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.OriginalDefinition.MetadataName is "DeckAction`1" or "DeckAction`2" &&
                current.OriginalDefinition.ToDisplayString().StartsWith("Cazzar.Deck.Core.Actions.DeckAction") &&
                current.TypeArguments.Length >= 1)
            {
                return current.TypeArguments[0].ToDisplayString();
            }
        }

        return null;
    }

    private static HashSet<string> SerializableTypes(Compilation compilation)
    {
        var covered = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<INamespaceOrTypeSymbol>();
        queue.Enqueue(compilation.Assembly.GlobalNamespace);

        while (queue.Count > 0)
        {
            foreach (var member in queue.Dequeue().GetMembers())
            {
                if (member is INamespaceOrTypeSymbol child) queue.Enqueue(child);
                if (member is not INamedTypeSymbol type) continue;

                foreach (var attribute in type.GetAttributes())
                {
                    if (attribute.AttributeClass?.ToDisplayString() != SerializableAttribute) continue;

                    if (attribute.ConstructorArguments.FirstOrDefault().Value is INamedTypeSymbol serializable)
                        covered.Add(serializable.ToDisplayString());
                }
            }
        }

        return covered;
    }

    private static ImmutableArray<CommandModel> Commands(INamedTypeSymbol type)
    {
        var commands = new Dictionary<string, CommandModel>();

        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var method in current.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal)) continue;

                foreach (var attribute in method.GetAttributes())
                {
                    if (attribute.AttributeClass?.ToDisplayString() != CommandAttribute) continue;
                    if (attribute.ConstructorArguments.FirstOrDefault().Value is not string command) continue;

                    var key = command.ToLowerInvariant();

                    if (!commands.ContainsKey(key))
                        commands[key] = new(key, method.Name, method.Parameters.Length > 0);
                }
            }
        }

        return commands.Values.OrderBy(c => c.Command, StringComparer.Ordinal).ToImmutableArray();
    }

    private static string HostFlags(int hosts)
    {
        const string host = "global::Cazzar.Deck.Abstractions.DeckHost.";

        if (hosts == DeckHostFlags.All) return host + "All";

        var names = DeckHostFlags.Names(hosts).Select(n => host + n).ToList();

        return names.Count > 0 ? string.Join(" | ", names) : host + "None";
    }

    private static bool NeedsEncoder(INamedTypeSymbol type) =>
        type.AllInterfaces.Concat([type]).Any(t => t.GetAttributes().Any(a =>
            a.AttributeClass?.ToDisplayString() == FeatureAttribute &&
            a.ConstructorArguments.Length > 0 &&
            a.ConstructorArguments[0].Value is int feature &&
            (feature & EncoderFeature) == EncoderFeature));

    private static string Argument(IParameterSymbol parameter)
    {
        var type = parameter.Type.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString();
        var optional = parameter.HasExplicitDefaultValue || parameter.NullableAnnotation == NullableAnnotation.Annotated;
        var resolve = optional ? "GetService" : "GetRequiredService";

        return $"global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.{resolve}<global::{type}>(sp)";
    }

    private static void Emit(SourceProductionContext spc, ImmutableArray<ActionModel> actions, Compilation compilation)
    {
        if (actions.IsDefaultOrEmpty) return;

        foreach (var duplicate in actions.GroupBy(a => a.ActionId).Where(g => g.Count() > 1))
        {
            foreach (var action in duplicate.Skip(1))
                spc.ReportDiagnostic(Diagnostic.Create(Diagnostics.DuplicateActionId, action.Location, action.ActionId));
        }

        foreach (var action in actions.Where(a => a.NeedsEncoder && a.ExcludesEncoderHosts))
            spc.ReportDiagnostic(Diagnostic.Create(Diagnostics.ContradictoryHosts, action.Location, action.TypeName));

        var covered = SerializableTypes(compilation);

        foreach (var action in actions.Where(a => a.SettingsType is not null && !covered.Contains(a.SettingsType!)))
            spc.ReportDiagnostic(Diagnostic.Create(Diagnostics.UncoveredSettings, action.Location, action.SettingsType));

        using var buffer = new StringWriter { NewLine = "\n" };
        using var source = new IndentedTextWriter(buffer, "    ") { NewLine = "\n" };

        source.WriteLine("// <auto-generated/>");
        source.WriteLine("#nullable enable");
        source.WriteLine();
        source.WriteLine($"namespace {compilation.AssemblyName?.Replace(" ", "_") ?? "Generated"};");
        source.WriteLine();

        source.WriteLine("internal sealed class GeneratedActionProvider : global::Cazzar.Deck.Abstractions.Actions.IActionProvider");
        using (source.Block())
        {
            source.WriteLine("public global::System.Collections.Generic.IEnumerable<global::Cazzar.Deck.Abstractions.Actions.DeckActionDescriptor> GetActions()");
            using (source.Block())
            {
                foreach (var action in actions.OrderBy(a => a.ActionId))
                {
                    source.WriteLine("yield return new global::Cazzar.Deck.Abstractions.Actions.DeckActionDescriptor");
                    using (source.Block(";"))
                    {
                        source.WriteLine($"ActionId = \"{action.ActionId}\",");
                        source.WriteLine($"ActionType = typeof(global::{action.TypeName}),");
                        source.WriteLine($"Hosts = {action.HostFlags},");
                        source.WriteLine($"RequiredFeatures = {(action.NeedsEncoder ? "global::Cazzar.Deck.Abstractions.DeckFeature.Encoder" : "global::Cazzar.Deck.Abstractions.DeckFeature.None")},");
                        source.WriteLine($"Activate = static sp => new global::{action.TypeName}({string.Join(", ", action.ConstructorArguments)}),");
                    }
                }
            }
        }

        source.WriteLine();
        source.WriteLine("internal static class GeneratedPropertyViewCommands");
        using (source.Block())
        {
            source.WriteLine("public static bool Dispatch(global::Cazzar.Deck.Abstractions.Actions.IPropertyViewCommands target, string command, global::Cazzar.Deck.Abstractions.IPayload body)");
            using (source.Block())
            {
                source.WriteLine("switch (target)");
                using (source.Block())
                {
                    foreach (var action in actions.Where(a => !a.Commands.IsDefaultOrEmpty).OrderBy(a => a.ActionId))
                    {
                        source.WriteLine($"case global::{action.TypeName} a:");
                        source.Indent++;
                        source.WriteLine("switch (command)");
                        using (source.Block())
                        {
                            foreach (var command in action.Commands)
                                source.WriteLine($"case \"{command.Command}\": a.{command.Method}({(command.TakesPayload ? "body" : "")}); return true;");
                        }

                        source.WriteLine("return false;");
                        source.Indent--;
                    }
                }

                source.WriteLine();
                source.WriteLine("return false;");
            }
        }

        spc.AddSource("GeneratedActionProvider.g.cs", buffer.ToString());
    }
}
