using System.Collections;
using System.Collections.Generic;
using Props;
using TerrainGeneration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Multi-attempt Single instance spawn with terrain parenting",
	menuName = "Props/Spawns/New Multi-attempt Single instance spawn - Parented")]
public class MultiAttemptSingleInstanceSpawnerWithTerrainParent : MultiAttemptSingleInstanceSpawn
{
	public override bool Spawn(MarchingCubeMapData mapData, out GameObject currentInstance)
	{
		var result = base.Spawn(mapData, out currentInstance);
		if (result)
		{
			currentInstance.transform.SetParent(ServiceLocator.Instance.GetService<ChunkManager>()
				.GetChunkFromPosition(currentInstance.transform.position).transform);
		}

		return result;
	}
}