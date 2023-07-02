using Player;
using UnityEngine;

namespace Props
{[CreateAssetMenu(fileName = "Player Single instance spawn", menuName = "Player instance player Spawn")]

	public class PlayerMultiAttemptSingleInstanceSpawn : MultiAttemptSingleInstanceSpawn
	{
		protected override void Setup(GameObject obj)
		{
			var pm = obj.GetComponent<PlayerMovement>();
			if (pm == null) Debug.Log("missing player movement");
			else pm.SetCanMove(true);
			
		}
	}
}