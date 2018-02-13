using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour {

    public bool IsRoot;
    public Joint JointToMassScale;
    public bool PluggedIn = false;

    private Outlet _outlet;
    private Cord _cord;

    public bool AttemptPlugin() {
        _cord = GameController.Instance.GetComponentInParents<Cord>(gameObject);
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
            LocalPlugin(outlets[0]);
            return true;
        }
        return false;
    }

    public void LocalPlugin(Outlet o) {
        _cord.CmdPlugin(o.gameObject, IsRoot);
        StartCoroutine(Plugin(o, true));
    }

    public void RemotePlugin(Outlet o) {
        StartCoroutine(Plugin(o, false));
    }

    public IEnumerator Plugin(Outlet o, bool local) {
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
        if (local)
            GameController.Instance.CmdCenter.CmdGiveupCordAuth(GameController.Instance.GetComponentInParents<Cord>(gameObject).gameObject);
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

