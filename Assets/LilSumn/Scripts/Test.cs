using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public bool type1;
    public Vector3 force;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (!type1)
            transform.Translate(Vector3.forward * 5f * Time.deltaTime);
        if (type1)
            GetComponent<Rigidbody>().velocity = new Vector3(Input.GetAxis("Horizontal")*4f, GetComponent<Rigidbody>().velocity.y, Input.GetAxis("Vertical")* 4f);
	}
}
