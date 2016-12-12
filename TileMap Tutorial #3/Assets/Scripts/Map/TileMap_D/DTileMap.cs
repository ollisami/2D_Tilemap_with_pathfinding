using UnityEngine;
using System.Collections.Generic;

public class DTileMap {

	
	int size_x;
	int size_y;
	
	int[,] map_data;
	
	List<DRoom> rooms;

	/////////////////////// MAKE A LIST OF DTILES////////////////////////////
	List<DTile> tileType  = new List<DTile>();

	/////////////////////// DEFINE WHAT A DTILE IS////////////////////////////
	protected class DTile {
		public string name = "Unknown";
		public bool isWalkable = false;
		public int tileGraphicId = 0;
		public float movementCost = 1F;
		
	}

	///////////////////////// MAKE A CLASS FOR DROOM ///////////////////
	
	protected class DRoom {
		public int left;
		public int top;
		public int width;
		public int height;
		
		public bool isConnected=false;
		
		public int right {
			get {return left + width - 1;}
		}
		
		public int bottom {
			get { return top + height - 1; }
		}
		
		public int center_x {
			get { return left + width/2; }
		}
		
		public int center_y {
			get { return top - height/2; }
		}
		
		public bool CollidesWith(DRoom other) {
			if( left > other.right-1 )
				return false;
			
			if( top > other.bottom-1 )
				return false;
			
			if( right < other.left+1 )
				return false;
			
			if( bottom < other.top+1 )
				return false;
			
			return true;
		}
		
		
	}

	

	/////////////////////// make a new DTileMap ////////////////////////////////
	public DTileMap(int size_x, int size_y) {
		DTile unknown = new DTile();
		DTile Floor = new DTile();
		DTile wall = new DTile();
		DTile stone = new DTile();
		
		tileType.Add(unknown);
		tileType.Add(Floor);
		tileType.Add(wall);
		tileType.Add(stone);
		
		tileType[1].name = "Floor";
		tileType[1].isWalkable = true;
		tileType[1].tileGraphicId = 1;
		tileType[1].movementCost = 1;
		
		tileType[2].name = "Wall";
		tileType[2].isWalkable = false;
		tileType[2].tileGraphicId = 2;
		tileType[2].movementCost = 999;
		
		tileType[3].name = "Stone";
		tileType[3].isWalkable = false;
		tileType[3].tileGraphicId = 3;
		tileType[3].movementCost = 999;
		Debug.Log ("data initilialised!");
		//Debug.Log (" 1= " + tileType [1].name + "2= " + tileType [2].tileGraphicId + " 3= " + tileType [3].tileGraphicId);
		//Debug.Log ("Tile type 1 iswalkable: " + tileType [1].isWalkable);
		//tileType[1].damagePerTurn = 0;
		
		DRoom r;
		this.size_x = size_x;
		this.size_y = size_y;

		/// MAKE A INT [,] AND PUT RVERYTHING TO TYPE 3 (STONE)
		map_data = new int[size_x,size_y];
		
		for(int x=0;x<size_x;x++) {
			for(int y=0;y<size_y;y++) {
				map_data[x,y] = tileType[3].tileGraphicId;
			}
		}
		
		rooms = new List<DRoom>();
		
		int maxFails = 10;
		
		while(rooms.Count < 10) {
			//room sizes
			int rsx = 10;//Random.Range(6,18);
			int rsy = 10;//Random.Range(6,14);
			
			r = new DRoom();
			r.left = Random.Range(0, size_x - rsx);
			r.top = (Random.Range(rsy, size_y));
			r.width = rsx;
			r.height = rsy;

			if(!RoomCollides(r)) {			
				rooms.Add (r);
			} else {
				maxFails--;
				if(maxFails <=0)
					break;
			}
			/*else {
				int failCount = 0;
				bool loop = true;
				while(loop){
					
					Mathf.Clamp(r.left + 1, 0, size_x - rsx);
					
					Mathf.Clamp(r.top + 1, 0, size_y - rsy);
					
					if(!RoomCollides(r)) {			
						rooms.Add (r);
						loop = false;
					} else {
						failCount++;
						if(failCount >= 5)
							loop = false;
					}
				}
				maxFails--;
				if(maxFails <=0)
					break;
			}*/
			
		}
		
		foreach(DRoom r2 in rooms) {
			MakeRoom(r2);
		}
		
		
		for(int i=0; i < rooms.Count; i++) {
			if(!rooms[i].isConnected) {
				int j = Random.Range(1, rooms.Count);
				MakeCorridor(rooms[i], rooms[(i + j) % rooms.Count ]);
			}
		}
		
		MakeWalls();
		
	}

	




	
	bool RoomCollides(DRoom r) {
		foreach(DRoom r2 in rooms) {
			if(r.CollidesWith(r2)) {
				return true;
			}
		}
		
		return false;
	}

	
	void MakeRoom(DRoom r) {
		for(int x=0; x < r.width; x++) {
			for(int y=0; y < r.height; y++){
				if(x==0 || x == r.width-1 || y==0 || y == r.height-1) {
					map_data[r.left+x,r.top-y] = tileType[2].tileGraphicId; // wall
				}
				else {
					map_data[r.left+x,r.top-y] = tileType[1].tileGraphicId; // floor
					//Debug.Log ("x " + (r.left+x) + "y " + (r.top-y) + "is " + map_data[r.left+x,r.top-y]);
				}
			}
		}
		
	}
	
	void MakeCorridor(DRoom r1, DRoom r2) {
		int x = r1.center_x;
		int y = r1.center_y;
		
		while( x != r2.center_x) {
			map_data[x,y] = tileType[1].tileGraphicId;
			map_data[x +1 ,y+1] = tileType[1].tileGraphicId;
			map_data[x -1 ,y-1] = tileType[1].tileGraphicId;
			
			x += x < r2.center_x ? 1 : -1;
		}
		
		while( y != r2.center_y ) {
			map_data[x,y] = tileType[1].tileGraphicId;
			map_data[x +1,y +1] = tileType[1].tileGraphicId;
			map_data[x -1,y -1] = tileType[1].tileGraphicId;
			
			y += y < r2.center_y ? 1 : -1;
		}

		
		r1.isConnected = true;
		r2.isConnected = true;
		
	}
	
	void MakeWalls() {
		for(int x=0; x< size_x;x++) {
			for(int y=0; y< size_y;y++) {
				if(map_data[x,y]==3 && HasAdjacentFloor(x,y)) {
					map_data[x,y] = 2;
				}
				// check if border
				if(map_data[x,y] == 2 && x-1 > 0 && x+1 < size_x && y-1 > 0 && y+1 < size_y) { 
					
					if(map_data[x,y+1] == 1 && map_data[x,y-1] == 1){
						map_data[x,y] = 1;
					}
					else if(map_data[x +1,y] == 1 && map_data[x -1,y] == 1){
						map_data[x,y]=1;
					}
				}
			}
		}
	}
	
	bool HasAdjacentFloor(int x, int y) {
		if( x > 0 && map_data[x-1,y] == 1 )
			return true;
		if( x < size_x-1 && map_data[x+1,y] == 1 )
			return true;
		if( y > 0 && map_data[x,y-1] == 1 )
			return true;
		if( y < size_y-1 && map_data[x,y+1] == 1 )
			return true;

		if( x > 0 && y > 0 && map_data[x-1,y-1] == 1 )
			return true;
		if( x < size_x-1 && y > 0 && map_data[x+1,y-1] == 1 )
			return true;
		
		if( x > 0 && y < size_y-1 && map_data[x-1,y+1] == 1 )
			return true;
		if( x < size_x-1 && y < size_y-1 && map_data[x+1,y+1] == 1 )
			return true;
		
		
		return false;
	}

	
	
	public Vector2[] GetSpawnPoint(){
		Vector2[] pos = new Vector2[2];

		if (rooms != null) {
			pos[0].x = rooms[0].center_x;
			pos[0].y = rooms[0].center_y;

			pos[1].x = rooms[rooms.Count -1].center_x;
			pos[1].y = rooms[rooms.Count -1].center_y;

		}
		return pos;
	}

	/////////////////////////////////////// GET DATA ////////////////////////////////

	
	public int GetTileGraphicIDAt(int x, int y) {
		return tileType[map_data[x,y]].tileGraphicId;
		
	}
	
	public float GetTileMovementCost(int x, int y) {
		return tileType[map_data[x,y]].movementCost;
		
	}

	public bool IsTileWalkable (int x, int y) {
		return tileType [map_data [x, y]].isWalkable;

	}
	
	public int[ , ] GetMapData() {
		return map_data;
	}


}
