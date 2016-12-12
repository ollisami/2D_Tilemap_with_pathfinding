using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {
	
	TileMap _tileMap;
	
	Vector3 currentTileCoord;
	
	public Transform selectionCube;
	public GameObject playerObj;

	void Start() {
		_tileMap = GetComponent<TileMap>();
	}

	// Update is called once per frame
	void Update () {
		if (playerObj == null)
			playerObj = GameObject.FindGameObjectWithTag ("Player");

		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			int x = Mathf.FloorToInt( hitInfo.point.x / _tileMap.tileSize);
			int y = Mathf.FloorToInt( hitInfo.point.y / _tileMap.tileSize);
			//Debug.Log ("Tile: " + x + ", " + y + " type is " + _tileMap.map.GetTileGraphicIDAt(x, y) + " is walkable: " + _tileMap.map.IsTileWalkable(x,y));
			
			currentTileCoord.x = x;
			currentTileCoord.y = y;
			
			selectionCube.transform.position = currentTileCoord*1f;
		}
		else {
			// Hide selection cube?
		}
		
		if (Input.GetMouseButtonUp (0)) {
			int posX = Mathf.FloorToInt (currentTileCoord.x * 1F);
			int posY = Mathf.FloorToInt (currentTileCoord.y * 1F);
			if (_tileMap.Walkable (posX, posY)) {
				_tileMap.SetSelectedUnit(playerObj);
				_tileMap.GeneratePathTo (posX, posY, false);
			}
		}
	}
}
