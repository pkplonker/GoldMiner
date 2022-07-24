 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using UnityEngine;

 namespace Terrain
 {
	 /// <summary>
	 ///LevelGeneration full description
	 /// </summary>
    
	 public class LevelGeneration : MonoBehaviour {
		 [SerializeField] private int mapWidthInTiles;
		 [SerializeField] private GameObject tilePrefab;
		 [Range(1,12)][SerializeField] private int octaves;
		 [Range(0,1)] [SerializeField] private float persistance;
		 [Range(0.001f,2f)] [SerializeField] private float lacunarity;
		 [SerializeField] private int seed;
		 [SerializeField] private float heightMultiplier;
		 [SerializeField] private AnimationCurve heightCurve;
		 [Range(1,100)][SerializeField] private float mapScale;
		 void Start() {
			 GenerateMap ();
		 }
		 public void GenerateMap() {
			 // get the tile dimensions from the tile Prefab
			 Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer> ().bounds.size;
			 int tileWidth = (int)tileSize.x;
			 int tileDepth = (int)tileSize.z;
			 // for each Tile, instantiate a Tile in the correct position
			 for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
				 for (int zTileIndex = 0; zTileIndex < mapWidthInTiles; zTileIndex++) {
					 // calculate the tile position based on the X and Z indices
					 Vector3 tilePosition = new Vector3(gameObject.transform.position.x + xTileIndex * tileWidth, 
						 gameObject.transform.position.y, 
						 gameObject.transform.position.z + zTileIndex * tileDepth);
					 // instantiate a new Tile
					 GameObject tile = Instantiate (tilePrefab, tilePosition, Quaternion.identity) ;
					 // set the Tile's parent to be the LevelGeneration game object
					 tile.transform.SetParent(transform);
					 var tg= tile.GetComponent<TileGeneration>();
					 tg.GenerateTile(octaves,persistance,lacunarity,seed,heightCurve,heightMultiplier,mapScale);
				 }
			 }
		 }
	 }
 }