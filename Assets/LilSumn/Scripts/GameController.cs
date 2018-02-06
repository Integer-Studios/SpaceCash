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
	}

	public void RegisterLocalPlayer(Player p) {
		LocalPlayer = p;
		CmdCenter = p.GetComponent<CommandCenter> ();
	}

}
