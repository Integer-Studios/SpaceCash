using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : NetworkBehaviour {

    public Camera Cam;
    public float RotateSpeed;

    [SyncVar]
    public bool HasController;
    public bool Controlling;

    private Ship _ship;

    private void Start() {
        _ship = GetComponentInParent<Ship>();
    }

    private void Update() {
        if (NetworkServer.active)
            UpdateServer();
        if (NetworkClient.active)
            UpdateClient();
    }

    private void UpdateServer() {

    }

    private void UpdateClient() {
        if (hasAuthority && Controlling) {
            transform.Rotate(new Vector3(Input.GetAxis("Vertical") * RotateSpeed, Input.GetAxis("Horizontal") * -1f * RotateSpeed, 0f) * Time.deltaTime);
        }
    }

    [Server]
    public void SetController(GameObject player) {
        HasController = true;
        GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Server]
    public void UnSetController(GameObject player) {
        HasController = false;
        GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    public void SetCameraActive(bool active) {
        Cam.enabled = active;
    }
}
