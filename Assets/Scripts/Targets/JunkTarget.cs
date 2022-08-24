using Player;
using UnityEngine;

namespace Targets
{
    public class JunkTarget : Target
    {
        public override bool Interact(PlayerInteractionStateMachine player)
        {
            Debug.Log("Interacted");
            DisableObject();
            return true;
        }
    }
}
