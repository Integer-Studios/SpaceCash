using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LilNetTransformGun : NetworkBehaviour {

    public float Interval = 9f;
    public float Smoothing = 0.5f;
    public float RotationAllowance = 5f;

    private Vector3 _correctInnerRotation;
    private Vector3 _correctRotation;
    private Vector3 _innerEulers;
    private Vector3 _eulers;
    private GameObject _parent;

    // start updating when you get authority
    void Start() {
        if (hasAuthority)
            StartCoroutine(NetworkTransformUpdate());
    }
    public override void OnStartAuthority() {
        StartCoroutine(NetworkTransformUpdate());
    }

    public void OverridePosition(Vector3 inner, Vector3 rotation) {
        _correctInnerRotation = inner;
        _correctRotation = rotation;
    }

    // update transform if you don't have authority
    private void Update() {
        if (hasAuthority)
            return;

        // lerp to correct rotations
        if (Vector3.Distance(_correctInnerRotation, transform.GetChild(0).localEulerAngles) > RotationAllowance)
            _innerEulers = Quaternion.Lerp(Quaternion.Euler(_innerEulers), Quaternion.Euler(_correctInnerRotation), Smoothing).eulerAngles;

        if (Vector3.Distance(_correctRotation, transform.localEulerAngles) > RotationAllowance)
            _eulers = Quaternion.Lerp(Quaternion.Euler(_eulers), Quaternion.Euler(_correctRotation), Smoothing).eulerAngles;

        transform.localEulerAngles = _eulers;
        transform.GetChild(0).localEulerAngles = _innerEulers;

    }

    // update loop
    private IEnumerator NetworkTransformUpdate() {
        while (enabled && hasAuthority) {
            if (NetworkServer.active) // no need for command u are the server
                RpcTransform(transform.GetChild(0).localEulerAngles, transform.localEulerAngles, Parent());
            else // arent the server but have authority so use relay
                CmdTransform(transform.GetChild(0).localEulerAngles, transform.localEulerAngles, Parent());
            yield return new WaitForSeconds(1 / Interval);
        }
    }

    // networking

    [Command]
    private void CmdTransform(Vector3 iRot, Vector3 rot, GameObject parent) {
        if (!hasAuthority)
            Sync(iRot, rot, parent);
        // relay to rpc
        RpcTransform(_correctInnerRotation, _correctRotation, parent);
    }

    [ClientRpc]
    private void RpcTransform(Vector3 iRot, Vector3 rot, GameObject parent) {
        if (!hasAuthority)
            Sync(iRot, rot, parent);
    }

    // sync target variable
    private void Sync(Vector3 innerRot, Vector3 rot, GameObject parent) {
        // make sure parent is correct
        if (parent == gameObject && transform.parent != null)
            transform.parent = null;
        if (parent != gameObject && transform.parent != parent)
            transform.parent = parent.transform;
        // initial set shouldnt use lerp
        if (innerRot == Vector3.zero) {
            _innerEulers = innerRot;
            transform.GetChild(0).localEulerAngles = _innerEulers;
            _eulers = rot;
            transform.localEulerAngles = _eulers;
        }
        // set lerp
        _correctInnerRotation = innerRot;
        _correctRotation = rot;

    }

    // get parent
    private GameObject Parent() {
        if (transform.parent == null)
            return gameObject;
        else
            return transform.parent.gameObject;
    }
}
