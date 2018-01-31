using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ship : NetworkBehaviour {

	public static Ship Instance;

	private Camera _camera;

	// Use this for initialization
	void Start () {
		if (Instance == null)
			Instance = this;

		_camera = GetComponentInChildren<Camera> ();
		_camera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
//		transform.Translate(0, 0, 0.5f);

	}

	public void SetCameraActive(bool active) {
		_camera.enabled = active;
	}

}
