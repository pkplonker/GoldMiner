using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiggableTerrain))]
public class DigVFX : MonoBehaviour
{
	private DiggableTerrain diggableTerrain;

	[SerializeField] private ParticleSystem particleSystem;

	// Start is called before the first frame update
	void Start()
	{
		diggableTerrain = GetComponent<DiggableTerrain>();
		diggableTerrain.OnDig += OnDig;
	}

	private void OnDig(RaycastHit hit)
	{
		var vfx = Instantiate(particleSystem, hit.point, Quaternion.identity, transform);
		vfx.gameObject.AddComponent<DestroyParticleWhenDone>();
	}
}

[RequireComponent(typeof(ParticleSystem))]
public class DestroyParticleWhenDone : MonoBehaviour
{
	private ParticleSystem particle;

	private void Start() => particle = GetComponent<ParticleSystem>();

	private void Update()
	{
		if (particle != null && !particle.IsAlive()) Destroy(gameObject);
	}
}