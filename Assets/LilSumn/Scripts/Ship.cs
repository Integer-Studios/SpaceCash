using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson; 

public class Ship : NetworkBehaviour {

	[SerializeField] public MouseLook m_MouseLook;
	public Camera Camera;

	[SyncVar]
	public bool HasDriver;

	public bool Driving;

    float Speed = 30;
    float MaxTurn = 0.45f;
    float MaxRotation = 12;
    float TurnSmoothness = 0.08f;
    float TurnResetSmoothness = 0.07f;

    float _yVel = 0f;
    float _xVel = 0f;
    float _rot = 0f;
    float _yRot = 0f;
    float _timeSincePoint = 0f;

	private void Start() {
		if (NetworkServer.active)
			StartServer ();
		if (NetworkClient.active)
			StartClient ();


		Camera = GetComponentInChildren<Camera> ();
		Camera.enabled = false;
		m_MouseLook.Init(transform , Camera.transform, true);

	}

	private void StartServer() {

	}

	private void StartClient() {

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
        if (hasAuthority && Driving) {
            //transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 5f * Time.deltaTime);
            if (TapLeft()) {
                _xVel = Mathf.Lerp(_xVel, MaxTurn * -1, TurnSmoothness);
                _rot = Mathf.Lerp(_rot, MaxRotation, TurnSmoothness);
            }
            if (TapRight()) {
                _xVel = Mathf.Lerp(_xVel, MaxTurn, TurnSmoothness);
                _rot = Mathf.Lerp(_rot, MaxRotation * -1, TurnSmoothness);
            }
            if (!TapLeft() && !TapRight()) {
                _xVel = Mathf.Lerp(_xVel, 0, TurnResetSmoothness);
                _rot = Mathf.Lerp(_rot, 0, TurnResetSmoothness);
            }

            if (TapUp()) {
                _yVel = Mathf.Lerp(_yVel, MaxTurn, TurnSmoothness);
                _yRot = Mathf.Lerp(_yRot, MaxRotation * -1, TurnSmoothness);
            }
            if (TapDown()) {
                _yVel = Mathf.Lerp(_yVel, MaxTurn * -1, TurnSmoothness);
                _yRot = Mathf.Lerp(_yRot, MaxRotation, TurnSmoothness);
            }
            if (!TapUp() && !TapDown()) {
                _yVel = Mathf.Lerp(_yVel, 0, TurnResetSmoothness);
                _yRot = Mathf.Lerp(_yRot, 0, TurnResetSmoothness);
            }



            transform.position += new Vector3(_xVel * Speed, _yVel * Speed, Speed) * Time.deltaTime;

            transform.eulerAngles = new Vector3(_yRot, 0, _rot);
        }
	}

    private bool TapLeft() {
      
        return Input.GetKey(KeyCode.A);
    }

    private bool TapRight() {
     
        return Input.GetKey(KeyCode.D);
    }

    private bool TapUp() {

        return Input.GetKey(KeyCode.W);
    }

    private bool TapDown() {

        return Input.GetKey(KeyCode.S);
    }

	public void SetCameraActive(bool active) {
		Camera.enabled = active;
	}

	[Server]
	public void SetDriver(GameObject player) {
		HasDriver = true;
		GetComponent<NetworkIdentity> ().AssignClientAuthority (player.GetComponent<NetworkIdentity> ().connectionToClient);
	}

    [Server]
    public void UnSetDriver(GameObject player) {
        HasDriver = false;
        GetComponent<NetworkIdentity>().RemoveClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

	
}
