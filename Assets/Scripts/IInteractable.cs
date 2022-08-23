
using Player;

public interface IInteractable
{
    public bool Interact(PlayerInteractionStateMachine player);
    public string GetInteractMessage();
}
