using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 
using UnityStandardAssets.CrossPlatformInput;

public class Player : NetworkBehaviour {

	private Ship _ship;
	private Animator _animator;

	[SyncVar]
	private int _walkSpeed;

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
		_animator.SetFloat ("Speed_f", ((float)_walkSpeed)/2f);

		if (!isLocalPlayer) {
			return;
		}
		
		int i = 0;
		if (Mathf.Abs(CrossPlatformInputManager.GetAxis ("Vertical")) > 0.5f)
			i = 1;
		if (Input.GetKey (KeyCode.LeftShift))
			i = 2;

		if (_walkSpeed != i) {
			_walkSpeed = i;
			CmdSetWalkSpeed (_walkSpeed);
		}
		
	}

	[Command]
	void CmdSetWalkSpeed(int f) {
		if (!isLocalPlayer)
			_walkSpeed = f;
	}
}