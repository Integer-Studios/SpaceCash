using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ship : NetworkBehaviour {

	public static Ship Instance;

	public Camera Camera;

	// Use this for initialization
	void Start () {
		if (Instance == null)
			Instance = this;

		Camera = GetComponentInChildren<Camera> ();
		Camera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
//		transform.Translate(0, 0, 0.5f);

	}

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
	}

}
