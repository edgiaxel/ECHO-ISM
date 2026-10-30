namespace ECHOISM;

public interface IInteractable
{
    string StableId { get; }
    bool AcceptsEchoes { get; }
    string GetPrompt(GregCharacter actor);
    bool TryInteract(GregCharacter actor);
}
