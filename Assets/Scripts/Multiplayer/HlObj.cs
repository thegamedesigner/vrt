using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HlObj : NetworkBehaviour
{
	/* This script is for: */
	/* All RPC/Command functions, inside a NetworkBehaviour */



	/* Command is called on the client & happens on the server, ClientRPC is called on the server & happens on all clients */

	//Chat
	[Command]
	public void CmdChat(string str) { RpcChat(str); if (isServer && !isClient) { ChatScript.ChatLocallyAndLog(str); } }

	[ClientRpc]
	public void RpcChat(string str) { ChatScript.ChatLocallyAndLog(str); }

	//Ticks & gamestate
	[ClientRpc]
	public void RpcBroadcastGameState(float[] gameState)
	{
		Ticks.ApplyTickOnClient(gameState);

		//Now, on each client, store the gamestate
		Ticks.receivedGameStates.Add(gameState);

		//then apply the gamestate to those objects that have changed

	}

	//Create Obj
	[Command]
	public void CmdCreateObj(int clId, Vector3 pos, int objType)//Sends an order from the client to the server
	{
		Debug.Log("Trying to buy " + (ObjFuncs.Type)objType);
		//Can the player afford this object?
		hl.Peer p = hl.GetPeerForUID(clId);
		if (p == null) { Debug.Log("Peer (" + clId + ") was null, failed to buy " + (ObjFuncs.Type)objType); return; }//Make sure the peer still exists
		if (!ObjFuncs.CheckCost((ObjFuncs.Type)objType, p)) { Debug.Log("Couldnt afford " + (ObjFuncs.Type)objType); return; }//Make sure they can afford this obj

		ObjFuncs.SubtractCost((ObjFuncs.Type)objType, p);//Spend the money to buy this object

		hl.uIds++;//Only ever do this on the server
		int uId = hl.uIds;

		ChatScript.ChatLocally("CreateObj on Sv. uId: " + uId);
		RpcCreateObj(clId, pos, objType, uId);
	}
	[ClientRpc]
	public void RpcCreateObj(int clId, Vector3 pos, int objType, int uId)
	{
		//create the object on all clients
		ChatScript.ChatLocally("CreateObj on Cl. uId: " + uId);
		//Trigger local effect
		Effects.CircleBlip(pos, 3, 6, 1);

		//Create the correct object of type
		ObjFuncs.CreateObj((ObjFuncs.Type)objType, pos, Vector3.zero, clId, uId);
	}

	//Move orders
	[Command]
	public void CmdMoveOrder(int clId, Vector3 pos, int[] unitIds)//Sends an order from the client to the server
	{
		Orders.MoveOrder(pos, clId, unitIds);

		RpcMoveOrder(clId, pos, unitIds);
	}

	[ClientRpc]
	public void RpcMoveOrder(int clId, Vector3 pos, int[] unitIds)
	{
		Orders.MoveOrder(pos, clId, unitIds);

		//Trigger local effect
		Effects.MoveOrderEffect(pos);
	}

	//update health (health only changes on the server, so it only needs a RPC)
	[ClientRpc]
	public void RpcUpdateHealth(int clId, int uId, int hp)//Sv to clients
	{
		//Added this to the gamestate tick. Don't think this is needed.
		int index = ObjFuncs.GetObjIndexForUID(uId);
		ObjFuncs.objs[index].health = hp;
	}

	//Join as Peer
	[Command]
	public void CmdRequestJoinAsPeer(string myName)
	{
		//A client has asked to become a new peer.
		hl.Peer peer = new hl.Peer();
		hl.uIds++;
		peer.uId = hl.uIds;
		peer.myName = myName;
		peer.team = ServerSideLoop.DetermineBestTeam(peer.uId);//Only called on the server. Auto picks team 1 or 2
		peer.teamPlayerNum = ServerSideLoop.DetermineTeamPlacement(peer.uId);
		Resources.InitResources(peer);
		hl.peers.Add(peer);
		RpcGrantJoinAsPeer(peer);

		//tell all the clients, including the one that just joined, all the peer info.
		hl.Peer[] list = new hl.Peer[hl.peers.Count];
		for (int i = 0; i < list.Length; i++) { list[i] = hl.peers[i]; }
		RpcSendAllPeers(list);

		//Also update everyone's global info
		hl.globalInfo.currentPlayers++;
		RpcBroadcastGlobalInfo(hl.globalInfo);
	}

	[ClientRpc]
	public void RpcGrantJoinAsPeer(hl.Peer peer)
	{
		if (isLocalPlayer)
		{
			if (hl.peers == null) { hl.peers = new List<hl.Peer>(); }
			//check that this peer doesn't already exist
			int result = hl.GetIndexForUID(peer.uId);
			if(result == -1)//Doesn't exist here yet
			{
				hl.peers.Add(peer);
				Debug.Log("Just added Peer" + peer.uId + " to hl.peers, count: " + hl.peers.Count);
			}

			//At this point, it either exists here already, or was just added. Since this is
			//the local player, set hl.local_uId
			hl.local_uId = peer.uId;
		}
	}

	[ClientRpc]
	public void RpcSendAllPeers(hl.Peer[] arrayOfPeers)//Sends a fairly large packet of all peer info, because it doesn't know what this player has
	{
		if (hl.peers == null) { hl.peers = new List<hl.Peer>(); }

		for (int i = 0; i < arrayOfPeers.Length; i++)
		{
			hl.OverwritePeer(arrayOfPeers[i]);
		}

	}

	[ClientRpc]
	public void RpcUpdateTeam(int uId, int team)//Updates a peer's team on all clients
	{
		//Is this peer already on the list?
		int result = hl.GetIndexForUID(uId);
		if (result != -1)//Peer doesn't exist
		{
			//Ah here it is, update it's details
			hl.peers[result].team = team;
		}
	}

	//Request team change
	[Command]
	public void CmdRequestChangeTeam(int clId, int desiredTeam)//Happens on the server
	{
		//A client has asked to change team
		hl.Peer peer = hl.GetPeerForUID(clId);
		if (peer != null)
		{
			//Grant team change request
			peer.team = desiredTeam;
		}

		//Update everyone's peers (not added yet)
		//RpcUpdateTeam(clId, desiredTeam);
	}

	//Update global info
	[ClientRpc]
	public void RpcBroadcastGlobalInfo(hl.GlobalInfo g)
	{
		hl.globalInfo = g;
	}

	/* Local functions.
	 * The Awake/Start/Update that run on the script that is on the gameobject that has the Network Identity component on it.
	 */

	void Awake()
	{
		Debug.Log("hlObj exists!");
		DontDestroyOnLoad(this.gameObject);
	}

	void Start()
	{
		if (isServer && isLocalPlayer)//Are you the server and the local player? Then setup some stuff! 
		{
			//Setup server info
			hl.globalInfo = new hl.GlobalInfo();
			hl.globalInfo.currentLevel = "Level1";
			hl.peers = new List<hl.Peer>();//This has to ONLY happen on the LocalPlayer Server, or client's joining will wipe the peers list.
		}

		if (isLocalPlayer)
		{
			hl.hlObj = this;
			CmdRequestJoinAsPeer("DefaultPlayer");
		}
	}

	void Update()
	{
		if (isLocalPlayer)
		{
			if (Input.GetKeyDown(KeyCode.G)) { ChatScript.ChatLocally("G key pressed! " + hl.local_uId); }

			Cl_Orders.InputOrders();//detect keyboard & mouse input, as orders, and send them to the server

		}

		if (isServer)
		{
			hl.svTime = Time.realtimeSinceStartup;//This is the server. Update this every frame
			ServerSideLoop.UpdateServer();
			Ticks.HandleTicks();//Send out ticks
		}

		if (isClient)
		{
			ClientSideLoop.UpdateClient();
		}
	}
}
