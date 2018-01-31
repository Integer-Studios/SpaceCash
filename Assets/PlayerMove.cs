using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class PlayerMove : NetworkBehaviour
{
	public Camera _camera; 
	[SerializeField] private MouseLook m_MouseLook;

	private bool _steering = false;

	void Start() {
		m_MouseLook.Init(transform , _camera.transform);

		transform.parent = FindObjectOfType<Ship> ().transform;
		NetworkTransformChild[] childs = Ship.Instance.GetComponents<NetworkTransformChild> ();
		foreach (NetworkTransformChild t in childs) {
			if (t.target.gameObject.tag != "Player") {
				t.target = transform;
				break;
			}
		}
		if(!isLocalPlayer)
			_camera.enabled = false;
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;
		RotateView();

		if (Input.GetKey (KeyCode.P)) {
			if (_steering) {
				Ship.Instance.SetCameraActive (false);
				_camera.enabled = true;

				_steering = false;
			} else {
				Ship.Instance.SetCameraActive (true);
				_camera.enabled = false;
				_steering = true;
			}
		}

		if (!_steering) {

			var x = Input.GetAxis ("Horizontal") * 0.1f;
			var z = Input.GetAxis ("Vertical") * 0.1f;
			transform.Translate (x, 0, z);
		} else {

			var x = Input.GetAxis ("Horizontal") * 0.5f;
			var z = Input.GetAxis ("Vertical") * 0.5f;
			Ship.Instance.transform.Translate(x, 0, z);

		}
	}

	private void RotateView()
	{
		if (!_steering)
			m_MouseLook.LookRotation (transform, _camera.transform);
		else
			m_MouseLook.LookRotation (Ship.Instance.transform, Ship.Instance.Camera.transform);
		
	}

}