using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class Cone : MonoBehaviour
{
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private const float TAU = 6.283185307179586f;


	public void GenerateCone(Vector3 pos, int sides, float height, float topRadius, float bottomRadius)
	{
		pos = transform.position;
		GenerateVertices(pos, bottomRadius, topRadius, sides, height, vertices);
		GenerateTriangles(triangles, sides, vertices.Count);
		GetComponent<MeshFilter>().mesh = GenerateMesh(vertices, triangles, "cone");
	}


	private static void GenerateTriangles(List<int> triangles, int sides, int vertexCount)
	{
		for (int i = 0; i < sides - 1; i++)
		{
			var root = i + 1;
			var rootLeft = root + 1;
			var rootUpleft = root + sides + 1;
			var rootUp = root + sides;


			triangles.Add(root);
			triangles.Add(rootUpleft);
			triangles.Add(rootLeft);
			triangles.Add(root);
			triangles.Add(rootUp);
			triangles.Add(rootUpleft);
		}

		var start = sides;
		var startLeft = 1;
		var startLeftUp = start + 1;
		var startup = start + sides;

		triangles.Add(start);
		triangles.Add(startLeftUp);
		triangles.Add(startLeft);
		triangles.Add(start);
		triangles.Add(startup);
		triangles.Add(startLeftUp);

		//top vertices
		for (var i = 0; i < sides; i++)
		{
			triangles.Add(0);
			triangles.Add(i + 1);
			triangles.Add(((i + 1) % sides) + 1);
		}

		//bottom vertices
		for (var i = 0; i < sides; i++)
		{
			triangles.Add(vertexCount - sides);
			triangles.Add(sides + ((i + 1) % sides) + 1);
			triangles.Add(sides + i + 1);
		}
	}

	private static void GenerateVertices(Vector3 startPos, float bottomRadius, float topRadius, int sides, float height,
		List<Vector3> vertices)
	{
		vertices.Add(Vector3.zero);
		for (var i = 0; i < sides; i++)
		{
			var angle = TAU * i / sides;
			var x = Mathf.Cos(angle);
			var y = Mathf.Sin(angle);
			vertices.Add(new Vector3( (x * (bottomRadius / 2)),0,  (y * (bottomRadius / 2))));
		}

		for (var i = 0; i < sides; i++)
		{
			var angle = TAU * i / sides;
			var x = Mathf.Cos(angle);
			var y = Mathf.Sin(angle);
			vertices.Add(new Vector3( (x * (topRadius / 2)), height, (y * (topRadius / 2)) ));
		}
		vertices.Add( new Vector3(0,height,0));
		
		
	}


	private static Mesh GenerateMesh(List<Vector3> vertices, List<int> triangles, string name)
	{
		var mesh = new Mesh
		{
			name = name,
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray()
		};
		mesh.RecalculateNormals();
		return mesh;
	}
}