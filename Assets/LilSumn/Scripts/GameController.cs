using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController Instance;
	[HideInInspector]
	public Player LocalPlayer;
	[HideInInspector]
	public CommandCenter CmdCenter;

    public GameObject ExplosionParticles;

	void Awake () {
		Instance = this;
        // wires with wires
        Physics.IgnoreLayerCollision(11,11);
        // remote player and wires
        Physics.IgnoreLayerCollision(10, 11);
        // local player and wires
        Physics.IgnoreLayerCollision(12, 11);

	}

	public void RegisterLocalPlayer(Player p) {
		LocalPlayer = p;
		CmdCenter = p.GetComponent<CommandCenter> ();
	}

    public T GetComponentInParents<T>(GameObject g) {
        T t = g.GetComponent<T>();
        if (EqualityComparer<T>.Default.Equals(t, default(T))) {
            if (g.transform.parent != null)
                return GetComponentInParents<T>(g.transform.parent.gameObject);
            else
                return default(T);
        } else {
            return t;
        }
            
    }

}
