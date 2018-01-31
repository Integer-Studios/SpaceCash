using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ship : NetworkBehaviour {

	public static Ship Instance;

	// Use this for initialization
	void Start () {
		if (Instance == null)
			Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0, 0, 0.5f);

	}
}
