using System.CodeDom.Compiler;

namespace Cazzar.Deck.Generators;

internal static class IndentedTextWriterExtensions
{
    public static IDisposable Block(this IndentedTextWriter writer, string suffix = "")
    {
        writer.WriteLine("{");
        writer.Indent++;

        return new Closer(writer, suffix);
    }

    private sealed class Closer(IndentedTextWriter writer, string suffix) : IDisposable
    {
        public void Dispose()
        {
            writer.Indent--;
            writer.WriteLine("}" + suffix);
        }
    }
}
