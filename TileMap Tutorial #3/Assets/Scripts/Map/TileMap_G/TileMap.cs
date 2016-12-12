using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x = 100;
	public int size_y = 50;
	public float tileSize = 2.0f;

	public Texture2D terrainTiles;
	public int tileResolution;

	public GameObject player;
	public GameObject[] wallPrefab;
	public GameObject goalPrefab;

	private GameObject selectedUnit;
	
	Node[ , ] graph;

	public DTileMap map;

	private int maxMovementDistance = 30;

	private bool debug_mode = false;


	public GameObject[] enemies;
	private int enemyCount = 10;
	
	// Use this for initialization
	void Start () {
		CreateMap ();
	}

	public void CreateMap() {
		if(debug_mode)
			Debug.Log ("creating map");
		if (selectedUnit != null) {
			Destroy (selectedUnit);
			selectedUnit = null;
	}

		GameObject[] walls = GameObject.FindGameObjectsWithTag ("Wall");
		foreach (GameObject g in walls) {
			Destroy (g);
		}

		GameObject[] goal = GameObject.FindGameObjectsWithTag ("Goal");
		foreach (GameObject g in goal) {
			Destroy (g);
		}

		GameObject[] enemy = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject g in enemy) {
			Destroy (g);
		}

		GameObject[] player = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject g in player) {
			Destroy (g);
		}

		enemyCount = 10;

		map = new DTileMap(size_x, size_y);
		GeneratePathFindingGraph ();
		BuildMesh();


		if(debug_mode)
			Debug.Log ("end creating map");
	}

/// C///////////////////////////////////////////////////////////////////////////////////////////////////////
/// create map


	Color[][] ChopUpTiles() {
		if(debug_mode)
			Debug.Log ("chop tiles");
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;
		
		Color[][] tiles = new Color[numTilesPerRow*numRows][];
		
		for(int y=0; y<numRows; y++) {
			for(int x=0; x<numTilesPerRow; x++) {
				tiles[y*numTilesPerRow + x] = terrainTiles.GetPixels( x*tileResolution , y*tileResolution, tileResolution, tileResolution );
			}
		}
		if(debug_mode)
			Debug.Log ("end chop tiles");
		return tiles;
	}
	
	void BuildTexture() {
		if(debug_mode)
			Debug.Log ("BuildTexture");

		int texWidth = size_x * tileResolution;
		int texHeight = size_y * tileResolution;
		Texture2D texture = new Texture2D(texWidth, texHeight);
		
		Color[][] tiles = ChopUpTiles();
		
		for(int y=0; y < size_y; y++) {
			for(int x=0; x < size_x; x++) {

				Color[] p = tiles[ map.GetTileGraphicIDAt(x,y) ];
				texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, p);

			}
		}
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;
		
		Debug.Log ("Done Texture!");


		Vector2[] pos = map.GetSpawnPoint ();
		SpawnPlayer (pos);
	}
	
	public void BuildMesh() {
		if(debug_mode)
			Debug.Log ("BuildMesh");

		int numTiles = size_x * size_y;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_y = size_y + 1;
		int numVerts = vsize_x * vsize_y;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for(z=0; z < vsize_y; z++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, -z*tileSize , 0);
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2((float)x / size_x,1f - (float)z / size_y );
			}
		}
		Debug.Log ("Done Verts!");
		
		for(z=0; z < size_y; z++) {
			for(x=0; x < size_x; x++) {
				int squareIndex = z * size_x + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 5] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 4] = z * vsize_x + x + 		   1;
			}
		}
		
		Debug.Log ("Done Triangles!");
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		Debug.Log ("Done Mesh!");
		
		BuildTexture();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////


	float CostToEnterTile(int x, int y) {
		//DTileMap map = new DTileMap(size_x, size_y);
		//tiles[ map.GetTileGraphicIDAt(x,y) ]
		float tt = map.GetTileMovementCost (x, y);
		return tt;
	}

	void GeneratePathFindingGraph(){
		if(debug_mode)
			Debug.Log ("GeneratePathFinfGraph");
		// initiolize the array
		graph = new Node[size_x, size_y];
		//initiolize a node for each spot in the array
		for (int y=0; y < size_y; y++) {
			for (int x=0; x < size_x; x++) {
				// make a node before putting info to it
				graph [x, y] = new Node ();
				graph [x, y].x = x;
				graph [x, y].y = y;
			}
		}

		// now that all nodes exsist, calculate their neighbours
		for (int y=0; y < size_y; y++) {
			for (int x=0; x < size_x; x++) {

				if(x > 0)
					graph[x,y].neighbours.Add(graph[x-1, y]);
				if(x < size_x -1)
					graph[x,y].neighbours.Add(graph[x+1, y]);
				if(y > 0)
					graph[x,y].neighbours.Add(graph[x, y-1]);
				if(y < size_y -1)
					graph[x,y].neighbours.Add(graph[x, y+1]);
			}
		}
		if(debug_mode)
			Debug.Log ("end GeneratePathFindingGraph");
	}

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3 (x + (tileSize/2), y + (tileSize/2), -0.5F);
	}

	public void GeneratePathTo(int targetX, int targetY, bool isPathCheck){
		if(debug_mode)
			Debug.Log ("GeneratePathTo");

		if (selectedUnit != null) {
			//Debug.Log ("x = " + posX + " y: " + posY);
			//clear out our units old path
			selectedUnit.GetComponent<Unit> ().currentPath = null;

			Dictionary<Node, float> dist = new Dictionary<Node, float> ();
			Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

			List<Node> unvisited = new List<Node> ();

			Node source = graph [
		                   selectedUnit.GetComponent<Unit> ().tileX,
		                   selectedUnit.GetComponent<Unit> ().tileY
			];
			Node target = graph [
		                    targetX, 
		                    targetY
			];

//			Debug.Log ("Trying to find path from " + selectedUnit.GetComponent<Unit> ().tileX + " , " + selectedUnit.GetComponent<Unit> ().tileY + " to: " + targetX + " , " + targetY);
			dist [source] = 0;
			prev [source] = null;

			dist [source] = 0;
			prev [source] = null;
		
			// Initialize everything to have INFINITY distance, since
			// we don't know any better right now. Also, it's possible
			// that some nodes CAN'T be reached from the source,
			// which would make INFINITY a reasonable value
			foreach (Node v in graph) {
				if (v != source) {
					dist [v] = Mathf.Infinity;
					prev [v] = null;
				}
			
				unvisited.Add (v);
			}
		
			while (unvisited.Count > 0) {
				// "u" is going to be the unvisited node with the smallest distance.
				Node u = null;
			
				foreach (Node possibleU in unvisited) {
					if (u == null || dist [possibleU] < dist [u]) {
						u = possibleU;
					}
				}
			
				if (u == target) {
					break;	// Exit the while loop!
				}
			
				unvisited.Remove (u);
			
				foreach (Node v in u.neighbours) {
					//Debug.Log("tile " + v.x + " , " + v.y + "is walkable");
					if (map.IsTileWalkable (u.x, u.y)) {
						//float alt = dist[u] + CostToEnterTile(v.x, v.y);
						float alt = dist [u] + u.DistanceTo (v);
						if (alt < dist [v]) {
							dist [v] = alt;
							prev [v] = u;
						}
					} else {
						unvisited.Remove (u);
					}
				}
			}

		
			// If we get there, the either we found the shortest route
			// to our target, or there is no route at ALL to our target.
		
			if (prev [target] == null) {
				// No route between our target and the source
				return;
			}
		
			List<Node> currentPath = new List<Node> ();
		
			Node curr = target;
		
			// Step through the "prev" chain and add it to our path
			while (curr != null) {
				currentPath.Add (curr);
				curr = prev [curr];
			}
		
			// Right now, currentPath describes a route from out target to our source
			// So we need to invert it!
			if (isPathCheck == false) {
				if (currentPath.Count <= maxMovementDistance) {
					currentPath.Reverse ();
		
					selectedUnit.GetComponent<Unit> ().currentPath = currentPath;
				} else {
					Debug.Log ("Too long way!");
					return;
				}
			} else {
				Debug.Log ("player can finish this level!");
				selectedUnit.GetComponent<Unit> ().canFinishTheLevel = true;
				;
			}
		} else {
			return;
		}
		if(debug_mode)
			Debug.Log ("End GeneratePathTO");
	}
	
	//////////////////////// check if tile is walkable//////////////

	public bool Walkable (int posX, int posY) {
		return map.IsTileWalkable (posX, posY);
	}


	/////////////////////////////////////////////////////////////////////
	/// spawn player
	
	void SpawnPlayer(Vector2[] spawnPos) {
		if(debug_mode)
			Debug.Log ("SpawnPlayer");

		spawnPos[0].x =  (spawnPos[0].x * tileSize) + (tileSize / 2);
		spawnPos[0].y = (spawnPos[0].y * tileSize) - (tileSize / 2);
		if (player != null) {
			selectedUnit = (GameObject)Instantiate (player, new Vector3(spawnPos[0].x, spawnPos[0].y, -0.5F), Quaternion.identity); 
			selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
			selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
			selectedUnit.GetComponent<Unit>().map = this;
		}

		GeneratePathTo ((int)spawnPos[1].x, (int)spawnPos[1].y, true);

		if(selectedUnit.GetComponent<Unit> ().canFinishTheLevel) {
			Instantiate (goalPrefab, new Vector3(spawnPos[1].x, spawnPos[1].y, -0.5F), Quaternion.identity); 
			SpawnWallsAndEnemies ();
		} else {
			Debug.Log("no way found from start to end, re-creating the map");
			CreateMap();
		}
		if(debug_mode)
			Debug.Log ("end SpawnPlayer");
	}

	/////// spawn walls ///////////////////////////////////////////////

	void SpawnWallsAndEnemies() {
		if(debug_mode)
			Debug.Log ("SpawnWalls");


		if(wallPrefab != null) {
			float wallOffset = 0.5F;
			int [ , ] data = map.GetMapData();

			for (int x = 0; x < size_x; x++) {
				for (int y = 0; y < size_y; y++) {
					if(data[x,y] == 2) {
						Instantiate (wallPrefab[Random.Range(0, wallPrefab.Length)], new Vector3(x + wallOffset, y + wallOffset, -0.5F), Quaternion.Euler(270, 0, 0));
					} else if (enemyCount > 0 && data[x,y] == 1) {

						bool createEnemy = false;
						if(x < (size_x - enemyCount)) {

							int rnd = Random.Range(0, 100);
							if(rnd == 5)
								createEnemy = true;
						} else {
							createEnemy = true;
						}
						if(createEnemy) {
							selectedUnit = (GameObject)Instantiate (enemies[Random.Range(0, enemies.Length)], new Vector3(x + wallOffset, y+ wallOffset , -0.5F), Quaternion.identity);
							selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
							selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
							selectedUnit.GetComponent<Unit>().map = this;
						enemyCount--;
						}
					}
				}
			}
		}
		if(debug_mode)
			Debug.Log ("End SpawnWalls");
	}


	////////////////////////////////// SET SELECTED UNIT ////////////////////////
	/// 
	public void SetSelectedUnit(GameObject unit) {
		selectedUnit = unit;
	}
}
