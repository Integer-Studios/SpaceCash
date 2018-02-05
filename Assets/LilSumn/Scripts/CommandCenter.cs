﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CommandCenter : NetworkBehaviour {

	[Command]
	public void CmdDriveShip(GameObject g) {
		g.GetComponent<Ship> ().SetDriver (gameObject);
	}
		
}
