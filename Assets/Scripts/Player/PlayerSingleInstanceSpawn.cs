using UnityEngine;

namespace Player
{[CreateAssetMenu(fileName = "New Single instance spawn", menuName = "Single instance player Spawn")]

	public class PlayerSingleInstanceSpawn : SingleInstanceSpawn
	{
		public override void Setup(GameObject obj)
		{
			var pm = obj.GetComponent<PlayerMovement>();
			if (pm == null) Debug.Log("missing player movement");
			else pm.SetCanMove(true);
			
		}
	}
}