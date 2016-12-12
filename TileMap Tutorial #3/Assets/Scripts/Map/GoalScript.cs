using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log ("trigger");
		if (other.gameObject.tag == "PlayerUnit") {
			Debug.Log ("GOAL!");
			other.gameObject.SendMessageUpwards("LevelComplited");
		}
	}
}
