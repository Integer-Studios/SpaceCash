using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	public static Ship Instance;
	[SerializeField] public MouseLook m_MouseLook;

	public Camera Camera;

	// Use this for initialization
	void Start () {
		if (Instance == null)
			Instance = this;


		Camera = GetComponentInChildren<Camera> ();
		Camera.enabled = false;
		m_MouseLook.Init(transform , Camera.transform, true);

	}
	
	// Update is called once per frame
	void Update () {
//		transform.Translate(0, 0, 0.5f);

	}

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
	}

}
