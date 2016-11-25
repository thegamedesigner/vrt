using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerSideLoop : MonoBehaviour
{
	public static void UpdateServer()//Called by the server's version of the network manager object
	{
		for(int i = 0;i < hl.peers.Count;i++)
		{
			Resources.UpdateResources(hl.peers[i]);
		}
		ObjFuncs.UpdateObjsOnServer();
	}

}
