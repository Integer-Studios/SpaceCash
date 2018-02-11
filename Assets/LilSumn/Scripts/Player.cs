using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 
using UnityStandardAssets.CrossPlatformInput;

public class Player : NetworkBehaviour {

	public float Threshold;
    public float DragSpeed;
    public float DragLerp;
    public Transform DragTarget;

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
    private Camera _camera;
    private float ArmLength = 5f;
    private Rigidbody _dragObject;

	void Start() {
		
		_ship = FindObjectOfType<Ship> ();
		_rigidbody = GetComponent<Rigidbody> ();
		_animator = GetComponentInChildren<Animator> ();
		_cmd = GetComponent<CommandCenter> ();
        _camera = GetComponentInChildren<Camera>();
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
        if (transform.parent == null) {
            _ship = null;

        }
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



        if (Input.GetKeyDown(KeyCode.E)) {

            if (_ship.HasDriver && _ship.Driving)
                StopDriveShip();

            if (_ship.Gun1.HasController && _ship.Gun1.Controlling)
                StopControlGun();
        }

		if (!isLocalPlayer || _inputDisabled) {
			return;
		}

        if (Input.GetMouseButtonDown(0)) {
            if (_dragObject != null) {
                DragStop();
            } else {
                Interact();
            }
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

        if (_dragObject != null) {
            _dragObject.velocity = Vector3.Lerp(_dragObject.velocity, (DragTarget.position - _dragObject.transform.position) * DragSpeed, DragLerp);
            _dragObject.transform.rotation = Quaternion.Lerp(_dragObject.transform.rotation, DragTarget.rotation, DragLerp);
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
				_ship = c.gameObject.transform.parent.GetComponentInParent<Ship> ();
				transform.SetParent(_ship.transform);
				GetComponent<FirstPersonController> ().SetParent (_ship.transform);
			} else {
                transform.SetParent(null);

				GetComponent<FirstPersonController> ().RemoveParent (_ship.transform);
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

    private void StopDriveShip() {
        if (_ship.HasDriver) {
            _ship.Driving = false;
            EnableCharacter();
            _ship.SetCameraActive(false);
            _cmd.CmdStopDriveShip(_ship.gameObject);
        }
 
    }

    private void AttemptControlGun() {
        if (!_ship.Gun1.HasController) {
            _cmd.CmdControlGun(_ship.Gun1.gameObject);
            _ship.Gun1.Controlling = true;
            DisableCharacter();
            _ship.Gun1.SetCameraActive(true);
        }
    }

    private void StopControlGun() {
        if (_ship.Gun1.HasController) {
            _ship.Gun1.Controlling = false;
            EnableCharacter();
            _ship.Gun1.SetCameraActive(false);
            _cmd.CmdStopControlGun(_ship.Gun1.gameObject);
        }

    }

	private void DisableCharacter() {
		_inputDisabled = true;
        _camera.enabled = false;
		//GetComponent<CharacterController> ().enabled = false;
		GetComponent<FirstPersonController> ().enabled = false;
	}

    private void EnableCharacter() {
        _inputDisabled = false;
        _camera.enabled = true;
        //GetComponent<CharacterController>().enabled = true;
        GetComponent<FirstPersonController>().enabled = true;
    }

    private void Interact() {
        RaycastHit hit = GetHit();
        if (hit.collider != null) {
            if (hit.collider.gameObject.tag == "steering-console") {
                AttemptDriveShip();
                return;
            } else if (hit.collider.gameObject.tag == "gun-controls") {
                AttemptControlGun();
                return;
            } else if (hit.collider.gameObject.tag == "draggable") {
                DragStart(hit.collider.gameObject);
                return;
            }
        }
    }

    private RaycastHit GetHit() {
        RaycastHit hit;
       
        Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, ArmLength);
        return hit;
    }

    private void DragStart(GameObject g) {
        DragTarget.position = g.transform.position;
        DragTarget.eulerAngles = g.transform.eulerAngles;

        _dragObject = g.GetComponent<Rigidbody>();
        _dragObject.freezeRotation = true;
    }

    private void DragStop() {
        _dragObject.freezeRotation = false;
        _dragObject = null;
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