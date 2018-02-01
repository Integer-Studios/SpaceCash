using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Player : NetworkBehaviour {

	public float Speed;
	public Camera _camera; 
	[SerializeField] private MouseLook m_MouseLook;

	private bool _steering = false;
	private Ship _ship;

	void Start() {
		m_MouseLook.Init(transform , _camera.transform, false);
		transform.parent = FindObjectOfType<Ship> ().transform;
		_ship = transform.parent.gameObject.GetComponent<Ship> ();
		if (!isLocalPlayer)
			GetComponentInChildren<Camera> ().enabled = false;
	}
	
	void Update() {
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyUp (KeyCode.P)) {
			if (_steering) {
				_ship.SetCameraActive (false);
				_camera.enabled = true;

				_steering = false;
			} else {
				_ship.SetCameraActive (true);
				_camera.enabled = false;
				_steering = true;
			}
		}
		RotateView();

		if (!_steering) {
			var x = Input.GetAxis ("Horizontal") * 0.1f;
			var z = Input.GetAxis ("Vertical") * 0.1f;
			transform.Translate(x, 0, z);

		} else {

			var x = Input.GetAxis ("Horizontal") * 0.5f;
			var z = Input.GetAxis ("Vertical") * 0.5f;
//			_ship.transform.Translate(x, 0, z);

		}

//		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * Time.deltaTime * Speed);
	}

	private void RotateView()
	{
		if (!_steering)
			m_MouseLook.LookRotation (transform, _camera.transform);
		else
			_ship.m_MouseLook.LookRotation (_ship.transform,_ship.Camera.transform);


	}

}