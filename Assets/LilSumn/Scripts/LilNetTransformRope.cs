using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LilNetTransformRope : NetworkBehaviour {

    public float Interval = 9f;
    public float Smoothing = 0.5f;
    public float PositionAllowance = 0.1f;
    public float RotationAllowance = 5f;

    private Vector3 _correctPositionRoot;
    private Vector3 _correctRotationRoot;
    private Vector3 _correctPositionEnd;
    private Vector3 _correctRotationEnd;
    private Vector3 _eulersRoot;
    private Vector3 _eulersEnd;
    private GameObject _parent;

    private Transform _root;
    private Transform _end;

    void Start() {
        if (!hasAuthority) {
            Initialize();
        }
    }

    public override void OnStartAuthority() {
        Debug.Log("starting updates");
        StartCoroutine(NetworkTransformUpdate());
    }

    public void GetAuth(GameObject player) {
        if (player != GameController.Instance.LocalPlayer.gameObject)
            GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    public void GiveupAuth(GameObject player) {
        if (player != GameController.Instance.LocalPlayer.gameObject)
            GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    private void Initialize() {
        _root = transform.Find("root");
        _end = transform.Find("end");
        _correctPositionRoot = _root.localPosition;
        _correctRotationRoot = _root.localEulerAngles;
        _correctPositionEnd = _end.localPosition;
        _correctRotationEnd = _end.localEulerAngles;
    }

    // update transform if you don't have authority
    private void Update() {
        if (hasAuthority)
            return;

        // lerp to correct transform
        if (Vector3.Distance(_correctPositionRoot, _root.localPosition) > PositionAllowance)
            _root.GetComponent<Rigidbody>().velocity = (_correctPositionRoot - _root.localPosition);

        // lerp to correct transform
        if (Vector3.Distance(_correctPositionEnd, _end.localPosition) > PositionAllowance)
            _end.GetComponent<Rigidbody>().velocity = (_correctPositionEnd - _end.localPosition);

        if (Vector3.Distance(_correctRotationRoot, _root.localEulerAngles) > RotationAllowance)
            _eulersRoot = Quaternion.Lerp(Quaternion.Euler(_eulersRoot), Quaternion.Euler(_correctRotationRoot), Smoothing).eulerAngles;

        _root.localEulerAngles = _eulersRoot;

        if (Vector3.Distance(_correctRotationEnd, _end.localEulerAngles) > RotationAllowance)
            _eulersEnd = Quaternion.Lerp(Quaternion.Euler(_eulersEnd), Quaternion.Euler(_correctRotationEnd), Smoothing).eulerAngles;

        _end.localEulerAngles = _eulersEnd;
    }

    // update loop
    private IEnumerator NetworkTransformUpdate() {
        Initialize();
        while (enabled && hasAuthority) {
            if (NetworkServer.active) // no need for command u are the server
                RpcTransform(_root.localPosition, _root.localEulerAngles, _end.localPosition, _end.localEulerAngles, Parent());
            else // arent the server but have authority so use relay
                CmdTransform(_root.localPosition, _root.localEulerAngles, _end.localPosition, _end.localEulerAngles, Parent());
            yield return new WaitForSeconds(1 / Interval);
        }
        Debug.Log("ending updates");
    }

    // networking

    [Command]
    private void CmdTransform(Vector3 posr, Vector3 rotr, Vector3 pose, Vector3 rote, GameObject parent) {
        if (!hasAuthority)
            Sync(posr, rotr, pose, rote, parent);

        // relay to rpc
        RpcTransform(_correctPositionRoot, _correctRotationRoot, _correctPositionEnd, _correctRotationEnd, parent);
    }

    [ClientRpc]
    private void RpcTransform(Vector3 posr, Vector3 rotr, Vector3 pose, Vector3 rote, GameObject parent) {
        if (!hasAuthority)
            Sync(posr, rotr, pose, rote, parent);
    }

    // sync target variable
    private void Sync(Vector3 posr, Vector3 rotr, Vector3 pose, Vector3 rote, GameObject parent) {
        // make sure parent is correct
        if (parent == gameObject && transform.parent != null)
            transform.parent = null;
        if (parent != gameObject && transform.parent != parent.transform)
            transform.parent = parent.transform;
        // initial set shouldnt use lerp
        if (_correctPositionRoot == Vector3.zero) {
            _root.localPosition = posr;
            _eulersRoot = rotr;
            _root.localEulerAngles = _eulersRoot;

            _end.localPosition = pose;
            _eulersEnd = rote;
            _end.localEulerAngles = _eulersEnd;
        }
        // set lerp
        _correctPositionRoot = posr;
        _correctRotationRoot = rotr;
        _correctPositionEnd = pose;
        _correctRotationEnd = rote;
    }

    // get parent
    private GameObject Parent() {
        if (transform.parent == null)
            return gameObject;
        else
            return transform.parent.gameObject;
    }

}
