using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ColorPickPanel : MonoBehaviour {

	public int size_x = 20;
	public int size_y = 5;
	public float tileSize = 2.0f;
	public int tileResolution = 50;

	// Use this for initialization
	void Start () {
		BuildMesh();
	}


	Color[,] ChopUpColors() { 
		int texWidth = size_x * tileResolution;
		int texHeight = size_y * tileResolution;
		Color[,] colors = new Color[texWidth, texHeight];
		int loopLength = texWidth/6;
		float r = 1.0F;
		float g = 0.1F;
		float b = 0.1F;
		float shader = 1F;
		for(int x=0; x<loopLength; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			g += 1.0F / loopLength;
		} 

		r = 1.0F;
		g = 1.0F;
		b = 0.0F;
		for(int x=loopLength; x<loopLength *2; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			r -= 1.0F / texHeight;
		} 

		r = 0.0F;
		g = 1.0F;
		b = 0.0F;
		for(int x=loopLength *2; x<loopLength *3; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			b += 1.0F / texHeight;
		} 

		r = 0.0F;
		g = 1.0F;
		b = 1.0F;
		for(int x=loopLength * 3; x<loopLength *4; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			g -= 1.0F / texHeight;
		} 

		r = 0.0F;
		g = 0.0F;
		b = 1.0F;
		for(int x=loopLength * 4; x<loopLength *5; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			r += 1.0F / texHeight;

		} 

		r = 1.0F;
		g = 0.0F;
		b = 1.0F;
		for(int x=loopLength * 5; x<texWidth; x++) {
			shader = 0.3F;
			for(int y=0; y<texHeight; y++) {
				colors [x, y] = new Color (Mathf.Clamp(r+shader,0F,1F),Mathf.Clamp(g+shader,0F,1F),Mathf.Clamp(b+shader,0F,1F)) ;
				shader -= (1.4F/(float)texHeight);
			}
			b -= 1.0F / texHeight;
		} 
		return colors; 
	}

	void BuildTexture() {

		int texWidth = size_x * tileResolution;
		int texHeight = size_y * tileResolution;
		Texture2D texture = new Texture2D(texWidth, texHeight);

		Color[,] colors = ChopUpColors();
		float f = 0.0F;
		for(int py=0; py < size_y; py++) {
			
			for(int px=0; px < size_x; px++) {
				Color color = colors [px, py];
				texture.SetPixel (px, py, new Color (f + color.r , f+color.g, f+color.b));//,a));	

				/*
				int size = 200;
				int radius = 50;
				float multiplier = 0.5F;
				float diff = size - radius;
				for (int y = 0; y < size; y++) {
					for (int x = 0; x < size; x++) {
						float a = 1.0F;
						Vector2 vec = new Vector2 (x, y);
						if (Vector2.Distance (vec, new Vector2(size/2, size/2)) > radius) {
							if (x < diff || y < diff)
								a =  Mathf.Clamp((Mathf.Min (x, y) / diff) * multiplier, 0.00F,1.0F);
							if(x > radius || y > radius)
								a =  Mathf.Min(Mathf.Clamp((1.0F - ((Mathf.Max (x, y) - radius) / diff)) * multiplier, 0.00F,1.0F), a);
							if (x == 0 || y == 0)
								a = 0.0F; 
						}
						//texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, p);
						texture.SetPixel (px*tileResolution, py*tileResolution, new Color (color.r, color.g, color.b,a));	
					}
				}

				*/


			

			}
			//f -= (1.0f/(float)size_y)*2F;
		}
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();

		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture; 
	}

	public void BuildMesh() {

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

		float x_curve = -5F;
		int x, z;
		for(z=0; z < vsize_y; z++) {
			float y_curve = -30F;
			for(x=0; x < vsize_x; x++) {
				vertices [z * vsize_x + x] = new Vector3 (x * tileSize, -z * tileSize, 0);//x_curve + y_curve);
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2((float)x / size_x,1f - (float)z / size_y );
				y_curve *= Mathf.Sin(2.0F);// y_curve*0.1F;
			}
			x_curve = x_curve*=0.5F;
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


	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3 (x + (tileSize/2), y + (tileSize/2), -0.5F);
	}
		
}