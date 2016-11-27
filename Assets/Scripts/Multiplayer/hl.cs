using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class hl : MonoBehaviour
{
	/* This script is for: */
	/*A collection of multiplayer data structures */
	
	public static float svTime;//Time on the server
	public static float clTime;//time as recived via tick (so on clients, and on the client part of sv/cls)
	public static HlObj hlObj;
	public static GlobalInfo globalInfo;
	public static List<Peer> peers;
	public static int uIds;//Used in many places. Always unique
	public static int team = 1;//Can be 0 (observer), 1 or 2.
	public static string gameName = "DefaultGame";
	public static int maxPlayers = 6;
	public static NetworkManager manager;

	public static int local_uId = -1;//The local uId

	[System.Serializable]
	public class GlobalInfo
	{
		//This is a sendable class. Only basic types of variables
		public string currentLevel;
		public int currentPlayers = 0;
	}

	[System.Serializable]
	public class Peer
	{
		//This is a sendable class. Only basic types of variables
		public string myName = "DefaultPlayer";
		public int uId = -1;
		public int team = -1;//0, 1 or 2 (observer, team 1, team 2)
		public int playerNum = -1;//Player1, player2, player3, etc
		public int teamPlayerNum = -1;//PlayerSlot1 on team X, PlayerSlot2 on team X, PlayerSlot3 on team X, 
		public string key = "";

		public int pop = 0;
		public int maxPop = 0;
		public int money = 0;
		public int moneyIncrease = 0;
		public int munitions = 0;
		public int munitionsIncrease = 0;
		public int fuel = 0;
		public int fuelIncrease = 0;
		public float resourceUpdateTimeSet = 0;
		public float resourceUpdateDelay = 1;
	}

	public static Peer GetPeerForUID(int uId)
	{
		for (int i = 0; i < peers.Count; i++)
		{
			if (peers[i].uId == uId) { return peers[i]; }
		}
		return null;
	}
}
