using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 
using UnityStandardAssets.CrossPlatformInput;

public class Player : NetworkBehaviour {

	public float Threshold;

	private Ship _ship;
	private Rigidbody _rigidbody;
	private Animator _animator;
	private Vector3 _prevPos;
	private CommandCenter _cmd;
	private float _displacement;
	[SyncVar]
	private bool _running;
	private bool _clientRunning;
	private bool _parentingCooldown = true;
	private bool _inputDisabled;

	void Start() {
		
		_ship = FindObjectOfType<Ship> ();
		_rigidbody = GetComponent<Rigidbody> ();
		_animator = GetComponentInChildren<Animator> ();
		_cmd = GetComponent<CommandCenter> ();
		transform.parent = _ship.transform;

		if (!isLocalPlayer) {
			DisableCharacter ();
			GetComponentInChildren<AudioListener> ().enabled = false;
		} else {
			_animator.gameObject.GetComponentInChildren<SkinnedMeshRenderer> ().gameObject.layer = 10;
			GetComponentInChildren<Camera> ().cullingMask = ~(1 << 10);
			GameController.Instance.RegisterLocalPlayer (this);
		}
	}
	
	void Update() {

		_displacement = Vector3.Distance (_prevPos, transform.localPosition);
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

		if (!isLocalPlayer || _inputDisabled) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.P))
			AttemptDriveShip ();

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

	public Vector3 GetGravity() {
		if (_ship == null)
			return Vector3.zero;
		return _ship.transform.up * -9.81f;
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "doorway" && _parentingCooldown) {
			_parentingCooldown = false;
			StartCoroutine (CooldownParenting ());
			if (transform.parent == null) {
				Debug.Log ("enter");
				_ship = c.gameObject.transform.parent.GetComponentInParent<Ship> ();
				transform.SetParent(_ship.transform);
				GetComponent<FirstPersonController> ().SetParent (_ship.transform);
			} else {
				Debug.Log ("exit");
				GetComponent<FirstPersonController> ().RemoveParent (_ship.transform);
				transform.SetParent(null);
				_ship = null;
			}
		}
	}

	private IEnumerator CooldownParenting() {
		yield return new WaitForSeconds (0.2f);
		_parentingCooldown = true;
	}

	private void AttemptDriveShip() {
		if (!_ship.HasDriver) {
			_cmd.CmdDriveShip (_ship.gameObject);
			_ship.Driving = true;
			DisableCharacter ();
			_ship.SetCameraActive (true);
		}
	}

	private void DisableCharacter() {
		_inputDisabled = true;
		GetComponentInChildren<Camera> ().enabled = false;
		GetComponent<CharacterController> ().enabled = false;
		GetComponent<FirstPersonController> ().enabled = false;
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