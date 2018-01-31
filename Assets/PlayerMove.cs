using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class PlayerMove : NetworkBehaviour
{
	public Camera _camera; 
	[SerializeField] private MouseLook m_MouseLook;

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

		var x = Input.GetAxis("Horizontal")*0.1f;
		var z = Input.GetAxis("Vertical")*0.1f;

		transform.Translate(x, 0, z);
	}

	private void RotateView()
	{
		m_MouseLook.LookRotation (transform, _camera.transform);
	}

}