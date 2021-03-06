﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CommandCenter : NetworkBehaviour {

	[Command]
	public void CmdDriveShip(GameObject g) {
		g.GetComponent<Ship> ().SetDriver (gameObject);
	}

    [Command]
    public void CmdStopDriveShip(GameObject g) {
        g.GetComponent<Ship>().UnSetDriver(gameObject);
    }

    [Command]
    public void CmdControlGun(GameObject g) {
        g.GetComponent<Gun>().SetController(gameObject);
    }

    [Command]
    public void CmdStopControlGun(GameObject g) {
        g.GetComponent<Gun>().UnSetController(gameObject);
    }

    [Command]
    public void CmdGetCordAuth(GameObject g, bool IsRight) {
        g.GetComponent<Cord>().GetAuth(gameObject, IsRight);
    }

    [Command]
    public void CmdGiveupCordAuth(GameObject g) {
        g.GetComponent<Cord>().GiveupAuth(gameObject);
    }
		
}
