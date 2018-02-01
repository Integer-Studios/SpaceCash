using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ship : NetworkBehaviour {

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();
	}

	private void StartServer() {

	}

	private void StartClient() {

	}

	private void Update() {
		if (NetworkServer.active)
			UpdateServer ();
		if (NetworkClient.active)
			UpdateClient ();
	}

	private void UpdateServer() {
		transform.Translate (Vector3.forward * Time.deltaTime * 3f);
	}

	private void UpdateClient() {
		
	}
	
}
