using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : NetworkBehaviour {

    public Camera Cam;
    public float RotateSpeed;
    public float FullAutoTimeout;
    public GameObject BulletPrefab;
    public float BulletSpeed;

    [SyncVar]
    public bool HasController;
    public bool Controlling;

    private Ship _ship;
    private Transform _inner;
    private Transform _bulletSpawn;

    private bool _shotReady = true;

    private void Start() {
        _ship = GetComponentInParent<Ship>();
        _inner = transform.GetChild(0);
        _bulletSpawn = _inner.Find("bullet-spawn");
        Cam.enabled = false;
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
            transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0f) * Time.deltaTime * RotateSpeed);
            transform.GetChild(0).Rotate(new Vector3(Input.GetAxis("Vertical") * -1f, 0f, 0f) * Time.deltaTime * RotateSpeed);
            if (Input.GetKey(KeyCode.LeftShift) && _shotReady) {
                // shoot
                GameObject g = Instantiate(BulletPrefab, _bulletSpawn.position, _inner.rotation);
                g.GetComponent<Rigidbody>().velocity = g.transform.forward * BulletSpeed;
                g.GetComponent<Bullet>().LocalOwned = true;
                CmdShoot(_bulletSpawn.position, _inner.eulerAngles, g.transform.forward * BulletSpeed);
                _shotReady = false;
                Invoke("ReadyShot", FullAutoTimeout);
            }
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

    private void ReadyShot() {
        _shotReady = true;
    }

    [Command]
    private void CmdShoot(Vector3 pos, Vector3 euler, Vector3 vel) {
        Debug.Log("cmd");
        RpcShoot(pos, euler, vel);
    }

    [ClientRpc]
    private void RpcShoot(Vector3 pos, Vector3 euler, Vector3 vel) {
        Debug.Log("rpc");
        if (hasAuthority)
            return;
        Debug.Log("spawn");
        GameObject g = Instantiate(BulletPrefab, pos, Quaternion.Euler(euler));
        g.GetComponent<Rigidbody>().velocity = vel;
    }
}
