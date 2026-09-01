namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.Encoder)]
public sealed record SetFeedbackLayout(ActionRef Ref, string LayoutId) : IDeckCommand;
