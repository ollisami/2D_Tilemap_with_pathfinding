using UnityEngine;
using System.Collections;

public class SampleDisplay : MonoBehaviour {


	public ColorPicker colorPicker;
	private Color color;
	// Use this for initialization
	void Start () {
		Texture2D t = this.gameObject.GetComponent<MeshRenderer> ().material.mainTexture as Texture2D;
		color = t.GetPixel (0, 0);
		setTexture ();
		Debug.Log (Mathf.Clamp ((Mathf.Max (7, 6) / 10.0F), 0.00F, 1.0F));
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -10) {
			Destroy (this.gameObject);
		}
	}

	public void OnMouseDown () {
		//colorPicker.setColor (color);
	}

	private void setTexture() {
		int size = 200;
		Texture2D t = new Texture2D (size, size);
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
				t.SetPixel (x, y, new Color (color.r, color.g, color.b,a));	
			}
		}
		t.Apply();
		this.gameObject.GetComponent<MeshRenderer> ().material.mainTexture = t;
		//this.gameObject.GetComponent<MeshRenderer> ().material.color = Color.white;
	}
}
