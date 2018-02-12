using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZoneRigidbody : MonoBehaviour {

    private Rigidbody _rigidbody;
    private GravityZone _gZone;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _gZone = GameController.Instance.GetComponentInParents<GravityZone>(gameObject); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        _rigidbody.AddForce(_gZone.GetGravity(), ForceMode.Acceleration);
	}

    public void RefreshZone() {
        _gZone = GameController.Instance.GetComponentInParents<GravityZone>(gameObject); 
    }
}
