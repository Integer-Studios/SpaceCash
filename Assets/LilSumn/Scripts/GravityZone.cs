using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour {

    public float LocalGravity;

    public Vector3 GetGravity() {
        return transform.up * LocalGravity;
    }
}
