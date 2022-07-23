using System;
using UnityEngine;

namespace Terrain
{
	public class MapDisplay : MonoBehaviour
	{
		[SerializeField] private Renderer renderer;


		public void DrawTexture(Texture2D texture2D)
		{
			renderer.material.mainTexture = texture2D;
			//renderer.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
		}

	
	}
}