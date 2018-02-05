using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LilNetTransformOld : NetworkBehaviour {

	public float Interval = 9f;
	public float Smoothing = 0.5f;
	public float PositionAllowance = 0.1f;
	public float RotationAllowance = 5f;

	public bool LocalPlayerAuthoritive;

	private Vector3 _correctPosition;
	private Vector3 _correctRotation;
	private Vector3 _eulers;
	private GameObject _parent;

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();
	}

	private void StartServer() {
		if (!LocalPlayerAuthoritive)
			StartCoroutine (NetworkTransformUpdate ());
	}

	private void StartClient() {
		if (LocalPlayerAuthoritive && isLocalPlayer)
			StartCoroutine (NetworkTransformUpdate ());
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
		// local player on LPA doesnt need to lerp cause it is sending positions
		if ((!LocalPlayerAuthoritive && !NetworkServer.active) || (LocalPlayerAuthoritive && !isLocalPlayer)) {
			// lerp to correct transform
			if (Vector3.Distance (_correctPosition, transform.localPosition) > PositionAllowance)
				transform.localPosition = Vector3.Lerp (transform.localPosition, _correctPosition, Smoothing);

			if (Vector3.Distance(_correctRotation, transform.localEulerAngles) > RotationAllowance)
				_eulers = Quaternion.Lerp (Quaternion.Euler(_eulers), Quaternion.Euler(_correctRotation), Smoothing).eulerAngles;
				
			transform.localEulerAngles = _eulers;
		}
	}

	private IEnumerator NetworkTransformUpdate() {
		Debug.Log ("I'm firing updates " + gameObject.name);
		while (enabled) {
			if (NetworkServer.active) // either its local player auth and u are the host, or its server auth, either way use rpc
				RpcTransform (transform.localPosition, transform.localEulerAngles, Parent());
			else // this means u called it as local player auth and ur a client so use command
				CmdTransform (transform.localPosition, transform.localEulerAngles, Parent());
			yield return new WaitForSeconds (1 / Interval);
		}
	}

	[Command]
	private void CmdTransform(Vector3 pos, Vector3 rot, GameObject parent) {
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
		// relay to rpc
		RpcTransform (_correctPosition, _correctRotation, parent);
	}

	[ClientRpc]
	private void RpcTransform(Vector3 pos, Vector3 rot, GameObject parent) {
		// net server doesnt need it cause it sent the command, local auth doesnt cause it sent the command
		if (NetworkServer.active || (LocalPlayerAuthoritive && isLocalPlayer))
			return;
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

	private GameObject Parent() {
		if (transform.parent == null)
			return gameObject;
		else
			return transform.parent.gameObject;
	}
}
