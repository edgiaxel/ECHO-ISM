namespace ECHOISM;

public interface IInteractable
{
    string StableId { get; }
    string GetPrompt(GregCharacter actor);
    bool TryInteract(GregCharacter actor);
}
