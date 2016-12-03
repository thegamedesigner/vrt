using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerSideLoop : MonoBehaviour
{
	public static void UpdateServer()//Called by the server's version of the network manager object
	{
		if(hl.peers != null)
		{
			for (int i = 0; i < hl.peers.Count; i++)
			{
				Resources.UpdateResources(hl.peers[i]);
			}
		}
		ObjFuncs.UpdateObjsOnServer();
	}
	
	public static int DetermineBestTeam(int clId)//Only called on the server
	{
		int t1 = 0;
		int t2 = 0;
		if(hl.peers == null) { return 1; }//If peers doesn't exist, then assume you should join team 1
		//Loop through all the peers, and count how many are on each team
		Debug.Log("Peer count:" + hl.peers.Count);
		for (int i = 0; i < hl.peers.Count; i++)
		{
			Debug.Log("Peer " + hl.peers[i].uId + ". Team: " + hl.peers[i].team);
			if (hl.peers[i].uId != clId)//Don't count myself
			{
				if (hl.peers[i].team == 1) { t1++; }//Only count the 2 important teams
				if (hl.peers[i].team == 2) { t2++; }
			}
		}


		Debug.Log("Counting teams for Peer " + clId + ". Team1: " + t1 + ", Team2: " + t2);

		//Which team has less?
		if (t1 < t2) { return 1; }
		else if (t2 < t1) { return 2; }
		else { return 1; }//If a tie, then just put them on team 1
	}

	public static int DetermineTeamPlacement(int clId)//Only called on the server
	{
		//Count how many people are on your team, that are not you, and return that number +1
		
		int result = 0;
		for (int i = 0; i < hl.peers.Count; i++)
		{
			if (hl.peers[i].uId != clId)//Don't count myself
			{
				if (hl.peers[i].team == 1) { result++; }
			}
		}
		return result + 1;
	}
}
