using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	public Camera Cam;
    public Gun Gun1;

	[SyncVar]
	public bool HasDriver;
	public bool Driving;

    public float Speed = 30;
    public float RollSpeed = 5;
    public float VertSpeed = 5;

    private Vector3 _correctPosition;
    private Vector3 _correctRotation;

    private Transform _camTarget;
    private bool _alive = true;

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();
        
        Cam.enabled = false;
        _camTarget = transform.Find("cam-target");
	}

	private void StartServer() {

	}

	private void StartClient() {

	}

	private void Update() {
        if (!_alive)
            return;
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
            if (Vector3.Distance(Cam.transform.position, _camTarget.transform.position) < 1f)
                Cam.transform.position = _camTarget.transform.position;
            Cam.transform.position = Vector3.Lerp(Cam.transform.position, _camTarget.transform.position, 0.1f);
            if (Vector3.Distance(Cam.transform.eulerAngles, _camTarget.transform.eulerAngles) < 1f)
                Cam.transform.rotation = _camTarget.transform.rotation;
            Cam.transform.rotation = Quaternion.Lerp(Cam.transform.rotation, _camTarget.transform.rotation, 0.1f);
        }
	}

	public void SetCameraActive(bool active) {
        Cam.enabled = active;
        Cam.transform.position = _camTarget.transform.position;
        Cam.transform.rotation = _camTarget.transform.rotation;
	}

    public void OnTriggerEnter(Collider col) {
        if (!hasAuthority || !_alive)
            return;
        if (col.gameObject.tag == "rock") {
            Debug.Log("boom boom");
            Instantiate(GameController.Instance.ExplosionParticles, transform.position, Quaternion.identity);
            _alive = false;
        }
    }

	[Server]
	public void SetDriver(GameObject player) {
		HasDriver = true;
		GetComponent<NetworkIdentity> ().AssignClientAuthority (player.GetComponent<NetworkIdentity> ().connectionToClient);
	}

    [Server]
    public void UnSetDriver(GameObject player) {
        _correctPosition = transform.position;
        _correctRotation = transform.eulerAngles;
        HasDriver = false;
        GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
        GetComponent<LilNetTransform>().OverridePosition(_correctPosition, _correctRotation);
    }

	
}
