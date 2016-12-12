using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public bool isEnemy = false;
	public float speed = 5F;

	public int tileX;
	public int tileY;
	public TileMap map;

	public List<Node> currentPath = null;

	public bool canFinishTheLevel = false;

	public GameObject model;
	public Sprite[] modelSprites;     // 0=down, 1 = right, 2 = up , 3= left
	private SpriteRenderer modelRend;

	void Start() {
		modelRend = model.GetComponent<SpriteRenderer> ();

		if (modelRend == null || modelSprites.Length < 3)
			Debug.LogError ("error with model sprites!");
	}

	void Update() {
		if (currentPath != null) {

		/*	int currNode = 0;

			while (currNode < currentPath.Count -1) {

				Vector3 start = map.TileCoordToWorldCoord (currentPath [currNode].x, currentPath [currNode].y);
				start.z = -1;
				Vector3 end = map.TileCoordToWorldCoord (currentPath [currNode + 1].x, currentPath [currNode + 1].y);
				end.z = -1;
				Debug.DrawLine (start, end);
				currNode++;
			}*/

			// Have we moved our visible piece close enough to the target tile that we can
			// advance to the next step in our pathfinding?
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f)
				AdvancePathing ();
		
			// Smoothly animate towards the correct map tile.

			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), speed * Time.deltaTime);
		}

	}

	// Advances our pathfinding progress by one tile.
	private void AdvancePathing() {

		if (currentPath == null)
			return;
		if (currentPath.Count > 2) {
			currentPath.RemoveAt (0);

			tileX = currentPath [1].x;
			tileY = currentPath [1].y;
			modelRend.sprite = modelSprites[CalculateSprite((int)transform.position.x, (int) transform.position.y, tileX, tileY)];
			//transform.position = map.TileCoordToWorldCoord (currentPath [0].x, currentPath [0].y);

		} else {
			//only one tile left, must be our final destination and we are on it
			currentPath = null;
		}
	}

	//recreate map
	public void LevelComplited() {
		map.CreateMap ();
	}

	private int CalculateSprite (int posX, int posY, int targetX, int targetY) {

		int dir = 0;

		if (targetY < posY)
			dir = 0;
		if (targetX > posX)
			dir = 1;
		if (targetY > posY)
			dir = 2;
		if (targetX < posX)
			dir = 3;
		return dir;

	}

	void OnTriggerStay(Collider other) {

		if(isEnemy) {
			if (other.gameObject.tag == "PlayerUnit") {
				if(currentPath == null) {
					map.SetSelectedUnit(this.gameObject);
					int tarX = other.gameObject.GetComponentInParent<Unit>().tileX;
					int tarY = other.gameObject.GetComponentInParent<Unit>().tileY;
					if(tileX != tarX || tileY != tarY) {
						map.GeneratePathTo(tarX, tarY, false);
					}
				}
			}
		}
	}

}
