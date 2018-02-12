using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour {

    public Joint JointToMassScale;
    public bool PluggedIn = false;

    private Outlet _outlet;

    public bool AttemptPlugin() {
        Collider[] cols = Physics.OverlapSphere(transform.position, 1f);
        List<Outlet> outlets = new List<Outlet>();
        foreach (Collider c in cols) {
            Outlet o = c.GetComponent<Outlet>();
            if (o != null && !o.HasCord) {
                outlets.Add(o);
            }
        }
        if (outlets.Count > 0) {
            outlets.Sort(new DistanceComparer(transform.position));
            StartCoroutine(Plugin(outlets[0]));
            return true;
        }
        return false;
    }

    private IEnumerator Plugin(Outlet o) {
        _outlet = o;
        o.HasCord = true;
        PluggedIn = true;
        GetComponent<Rigidbody>().isKinematic = true;
        while (Vector3.Distance(transform.position, o.transform.position) > 0.1f) {
            transform.position = Vector3.Lerp(transform.position, o.transform.position, 0.2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, o.transform.rotation, 0.2f);
            yield return null;
        }
        Debug.Log("done");
        transform.position = o.transform.position;
        transform.rotation = o.transform.rotation;
    }

    public void Unplug() {
        Debug.Log("unlpug");
        PluggedIn = false;
        GetComponent<Rigidbody>().isKinematic = false;
        _outlet.HasCord = false;
        _outlet = null;
    }

    public class DistanceComparer : IComparer<Outlet> {

        private Vector3 position;

        public DistanceComparer(Vector3 p) {
            position = p;
        }

        public int Compare(Outlet x, Outlet y) {
            return  Mathf.RoundToInt(100 * (Vector3.Distance(x.transform.position, position)-Vector3.Distance(y.transform.position, position)));
        }

    }
}

