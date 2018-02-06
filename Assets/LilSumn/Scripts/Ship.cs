using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	public Camera Camera;

	[SyncVar]
	public bool HasDriver;

	public bool Driving;

    public float Speed = 30;
    public float RollSpeed = 5;
    public float VertSpeed = 5;

    private Transform _camTarget;

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();
        
		Camera.enabled = false;
        _camTarget = transform.Find("cam-target");
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
        if (hasAuthority && Driving) {
            transform.Rotate(new Vector3(Input.GetAxis("Vertical") * VertSpeed, 0, Input.GetAxis("Horizontal") * -1f * RollSpeed)* Time.deltaTime);
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            // cam lerping
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, _camTarget.transform.position, 0.1f);
            Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, _camTarget.transform.rotation, 0.1f);
        }
	}

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
        Camera.transform.position = _camTarget.transform.position;
        Camera.transform.rotation = _camTarget.transform.rotation;
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
