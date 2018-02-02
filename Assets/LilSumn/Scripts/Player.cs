using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Player : NetworkBehaviour {

	private Ship _ship;

	void Start() {
		
		_ship = FindObjectOfType<Ship> ();
		transform.parent = _ship.transform;

		if (!isLocalPlayer) {
			GetComponentInChildren<Camera> ().enabled = false;
			GetComponent<CharacterController> ().enabled = false;
			GetComponent<AudioListener> ().enabled = false;
		}
	}
	
	void Update() {
		if (!isLocalPlayer)
			return;
	}

}