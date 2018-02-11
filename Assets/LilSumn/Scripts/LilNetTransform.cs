using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LilNetTransform : NetworkBehaviour {

	public float Interval = 9f;
	public float Smoothing = 0.5f;
	public float PositionAllowance = 0.1f;
	public float RotationAllowance = 5f;

	private Vector3 _correctPosition;
	private Vector3 _correctRotation;
	private Vector3 _eulers;
	private GameObject _parent;

	// start updating when you get authority
	void Start() {
		if (hasAuthority)
			StartCoroutine (NetworkTransformUpdate ());
	}
	public override void OnStartAuthority() {
		StartCoroutine (NetworkTransformUpdate ());
	}

    public void OverridePosition(Vector3 position, Vector3 rotation) {
        _correctPosition = position;
        _correctRotation = rotation;
    }

	// update transform if you don't have authority
	private void Update() {
		if (hasAuthority)
			return;
		
		// lerp to correct transform
		if (Vector3.Distance (_correctPosition, transform.localPosition) > PositionAllowance)
			transform.localPosition = Vector3.Lerp (transform.localPosition, _correctPosition, Smoothing);

		if (Vector3.Distance(_correctRotation, transform.localEulerAngles) > RotationAllowance)
			_eulers = Quaternion.Lerp (Quaternion.Euler(_eulers), Quaternion.Euler(_correctRotation), Smoothing).eulerAngles;
			
		transform.localEulerAngles = _eulers;
	}

	// update loop
	private IEnumerator NetworkTransformUpdate() {
		while (enabled && hasAuthority) {
			if (NetworkServer.active) // no need for command u are the server
				RpcTransform (transform.localPosition, transform.localEulerAngles, Parent());
			else // arent the server but have authority so use relay
				CmdTransform (transform.localPosition, transform.localEulerAngles, Parent());
			yield return new WaitForSeconds (1 / Interval);
		}
	}

	// networking

	[Command]
	private void CmdTransform(Vector3 pos, Vector3 rot, GameObject parent) {
		if (!hasAuthority)
			Sync (pos, rot, parent);
		// relay to rpc
		RpcTransform (_correctPosition, _correctRotation, parent);
	}

	[ClientRpc]
	private void RpcTransform(Vector3 pos, Vector3 rot, GameObject parent) {
		if (!hasAuthority)
			Sync (pos, rot, parent);
	}

	// sync target variable
	private void Sync(Vector3 pos, Vector3 rot, GameObject parent) {
		// make sure parent is correct
		if (parent == gameObject && transform.parent != null)
			transform.parent = null;
		if (parent != gameObject && transform.parent != parent)
			transform.parent = parent.transform;
		// initial set shouldnt use lerp
		if (_correctPosition == Vector3.zero) {
			transform.localPosition = pos;
			_eulers = rot;
			transform.localEulerAngles = _eulers;
		}
		// set lerp
		_correctPosition = pos;
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
