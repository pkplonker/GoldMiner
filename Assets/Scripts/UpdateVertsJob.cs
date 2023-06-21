using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
struct UpdateVertsJob : IJobParallelFor
{
	[ReadOnly] public NativeArray<Vector3> hitVerts;
	[ReadOnly] public float digAmount;
	public NativeArray<Vector3> verts;

	public void Execute(int i)
	{
		var v = verts[i];
		for (int j = 0; j < hitVerts.Length; j++)
		{
			if (v.Equals(hitVerts[j]))
			{
				verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
				break;
			}
		}
	}
}