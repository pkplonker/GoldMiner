using TerrainGeneration;
using UnityEngine;

[CreateAssetMenu(fileName = "KillZoneSpawner", menuName = "Props/Spawns/Kill Zone Spawner")]
public class KillZoneSpawner : SingleInstanceSpawn
{
	[SerializeField] private float killHeight = -10f;
	[SerializeField] private Vector3 boxSize = new Vector3(10000f, 1f, 10000f);

	public override bool Spawn(MapData mapData, out GameObject killZone)
	{
		killZone = new GameObject("KillZone");
		var boxCollider = killZone.AddComponent<BoxCollider>();
		killZone.AddComponent<KillPlayerCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size = boxSize;
		killZone.transform.position = new Vector3(0f, killHeight, 0f);

		return true;
	}

	public override string GetName() => "Kill Zone";
}