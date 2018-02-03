using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 
using UnityStandardAssets.CrossPlatformInput;

public class Player : NetworkBehaviour {

	public float Threshold;

	private Ship _ship;
	private Animator _animator;
	private Vector3 _prevPos;
	private float _displacement;
	[SyncVar]
	private bool _running;
	private bool _clientRunning;

	void Start() {
		
		_ship = FindObjectOfType<Ship> ();
		_animator = GetComponentInChildren<Animator> ();
		transform.parent = _ship.transform;

		if (!isLocalPlayer) {
			GetComponentInChildren<Camera> ().enabled = false;
			GetComponent<CharacterController> ().enabled = false;
			GetComponentInChildren<AudioListener> ().enabled = false;
		}
	}
	
	void Update() {
		_displacement = Vector3.Distance (_prevPos, transform.localPosition) * 0.9f;
		_prevPos = transform.localPosition;
		if (isLocalPlayer && _clientRunning)
			_animator.SetFloat ("Speed_f", 1f);
		else if (_running)
			_animator.SetFloat ("Speed_f", 1f);
		else if (_displacement > Threshold)
			_animator.SetFloat ("Speed_f", 0.5f);
		else
			_animator.SetFloat ("Speed_f", 0f);

		if (!Physics.Raycast (transform.position, transform.up * -1, 2f)) {
			_animator.SetBool ("Grounded", false);
		} else {
			_animator.SetBool ("Grounded", true);
		}

		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKey (KeyCode.LeftShift) && !_clientRunning) {
			_clientRunning = true;
			CmdRun (_clientRunning);
		}
		if (!Input.GetKey (KeyCode.LeftShift) && _clientRunning) {
			_clientRunning = false;
			CmdRun (_clientRunning);
		}

		if (CrossPlatformInputManager.GetButton ("Jump")) {
			StartCoroutine(Jump ());
			CmdJump ();
		}
		
	}

	private IEnumerator Jump() {
		_animator.SetBool ("Jump_b", true);
		yield return new WaitForSeconds (0.2f);
		_animator.SetBool ("Jump_b", false);
	}


	[Command]
	void CmdRun(bool b) {
		_running = b;
	}

	[Command]
	void CmdJump() {
		RpcJump ();
	}
	[ClientRpc]
	void RpcJump() {
		if (!isLocalPlayer)
			StartCoroutine(Jump ());
	}
}