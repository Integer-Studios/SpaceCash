using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	[SerializeField] public MouseLook m_MouseLook;
	public Camera Camera;

	[SyncVar]
	public bool HasDriver;

	public bool Driving;

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
		
	}

	private void UpdateClient() {
		if (hasAuthority && Driving)
			transform.Translate (new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"))*5f * Time.deltaTime);
	}

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
	}

	[Server]
	public void SetDriver(GameObject player) {
		HasDriver = true;
		GetComponent<NetworkIdentity> ().AssignClientAuthority (player.GetComponent<NetworkIdentity> ().connectionToClient);
	}

    [Server]
    public void UnSetDriver(GameObject player) {
        HasDriver = false;
        GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

	
}
