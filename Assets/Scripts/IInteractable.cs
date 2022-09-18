
using Player;

public interface IInteractable
{
    public void Interact(PlayerInteractionStateMachine player);
    public string GetInteractMessage();
}
