using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	[SerializeField] public MouseLook m_MouseLook;
	public Camera Camera;

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();


		Camera = GetComponentInChildren<Camera> ();
		Camera.enabled = false;
		m_MouseLook.Init(transform , Camera.transform, true);

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
		transform.Translate (Vector3.forward * Time.deltaTime * 1f);
	}

	private void UpdateClient() {
		
	}

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
	}


	
}
