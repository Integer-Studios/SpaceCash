using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class PlayerMove : NetworkBehaviour {

	public float Speed;

	void Start() {
		transform.parent = FindObjectOfType<Ship> ().transform;
		if (!isLocalPlayer)
			GetComponentInChildren<Camera> ().enabled = false;
	}
	
	void Update() {
		if (!isLocalPlayer)
			return;
		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * Time.deltaTime * Speed);
	}
}