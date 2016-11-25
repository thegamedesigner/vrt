using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ticks : MonoBehaviour
{
	/* A system that checks if certain variables in Objs have changed, and includes them in an update tick if they have 
	 */

	public static List<float[]> completeGameStates = new List<float[]>();//a record of all complete gameStates, on the server
	public static List<float[]> receivedGameStates = new List<float[]>();//a record of all recived game states, on each cl.

	public enum Type //Can't have any gaps between None & End here, or ticks unwrap check will fail.
	{
		None = 0,
		ObjGoal = 2,
		ObjHealth = 3,
		Pop = 4,
		Money = 5,
		Muntions = 6,
		Fuel = 7,
		End
	}

	public static float tickRate = 0.05f;
	public static float tickRateTimeSet;

	public static void HandleTicks()
	{
		//Send ticks to all clients, based on the tickRate
		if (hl.svTime >= (tickRate + tickRateTimeSet))
		{
			tickRateTimeSet = hl.svTime;

			if (hl.hlObj != null)
			{
				float[] cgs = CreateCompleteGameState();

				if (cgs != null)
				{
					//send this tick
					hl.hlObj.RpcBroadcastGameState(cgs);
				}
			}
		}

	}

	public static float[] CreateCompleteGameState()//Returns entire game state. Does zero paring down.
	{
		//Prepare a gameState
		List<float> s = new List<float>();


		//Add svTime
		s.Add(hl.svTime);//The first float is always the time, so it functions as a time stamp

		//loop through every peer
		for (int i = 0; i < hl.peers.Count; i++)
		{
			//Pop
			s.Add((float)Type.Pop);
			s.Add(5);//Number of floats, including this one
			s.Add(hl.peers[i].uId);
			s.Add(hl.peers[i].pop);
			s.Add(hl.peers[i].maxPop);

			//Money
			s.Add((float)Type.Money);
			s.Add(5);//Number of floats, including this one
			s.Add(hl.peers[i].uId);
			s.Add(hl.peers[i].money);
			s.Add(hl.peers[i].moneyIncrease);

			//Muntions
			s.Add((float)Type.Muntions);
			s.Add(5);//Number of floats, including this one
			s.Add(hl.peers[i].uId);
			s.Add(hl.peers[i].munitions);
			s.Add(hl.peers[i].munitionsIncrease);

			//Fuel
			s.Add((float)Type.Fuel);
			s.Add(5);//Number of floats, including this one
			s.Add(hl.peers[i].uId);
			s.Add(hl.peers[i].fuel);
			s.Add(hl.peers[i].fuelIncrease);

		}

		//loop through every object
		for (int i = 0; i < ObjFuncs.objs.Count; i++)
		{
			//do this part
			ObjFuncs.Obj o = ObjFuncs.objs[i];

			//Goal 
			s.Add((float)Type.ObjGoal);
			s.Add(6);//Number of floats, including this one
			s.Add(o.uId);
			s.Add(o.goal.x);
			s.Add(o.goal.y);
			s.Add(o.goal.z);

			//Health
			s.Add((float)Type.ObjHealth);
			s.Add(4);//Number of floats, including this one
			s.Add(o.uId);
			s.Add(o.health);
		}

		//Done. Return this as a float array
		float[] cgs = new float[s.Count];
		for (int i = 0; i < cgs.Length; i++)
		{
			cgs[i] = s[i];
		}
		return cgs;
	}

	//Called on the client, applys these changes to the objects
	public static void ApplyTickOnClient(float[] gs)
	{
		//Should I use this tick?
		if (hl.clTime > gs[0])
		{
			//This tick's timestamp is older than my own. It's old, throw it out
			Debug.Log("Old timestamp. Throwing out this tick. " + gs[0]);
			return;
		}

		hl.clTime = gs[0];//Apply time

		//Print entire tick
		/*
		string s = "Received game state:";
		for (int a = 0; a < gs.Length; a++)
		{
			s += "\n" + gs[a];
		}
		Debug.Log(s);*/

		//Check this tick
		int i = 1;//1 to skip the timestamp at the start
		int infLoopCheck = 1;
		while (i < gs.Length && infLoopCheck < gs.Length)
		{
			//What type is this chunk?
			bool found = false;

			//Does this match any type?
			for (int a = 1; a < (int)Type.End; a++)
			{
				if ((int)gs[i] == a)
				{
					i += (int)gs[i + 1];
					found = true;
					break;
				}
			}
			if (!found)
			{
				//Didn't find a correct type. Um... I think this means this ENTIRE tick is garbage. Throw it out.
				Debug.Log("Unable to read tick. Throwing out the entire tick. " + gs[0]);
				return;
			}
			infLoopCheck++;//Make sure this can't get in a weird loop
		}

		//Unwravel and apply this game state
		i = 1;//1 to skip the timestamp at the start
		infLoopCheck = 1;
		while (i < gs.Length && infLoopCheck < gs.Length)
		{
			//What type is this chunk?
			switch ((Type)(int)gs[i])
			{
				case Type.ObjGoal:
					ApplyObjGoal((int)gs[i + 2], new Vector3(gs[i + 3], gs[i + 4], gs[i + 5]));
					i += (int)gs[i + 1];
					break;

				case Type.ObjHealth:
					ApplyObjHealth((int)gs[i + 2], (int)gs[i + 3]);
					i += (int)gs[i + 1];
					break;
					
				case Type.Pop:
					ApplyPeerPop((int)gs[i + 2], (int)gs[i + 3], (int)gs[i + 4]);
					i += (int)gs[i + 1];
					break;
					
				case Type.Money:
					ApplyPeerMoney((int)gs[i + 2], (int)gs[i + 3], (int)gs[i + 4]);
					i += (int)gs[i + 1];
					break;

				case Type.Muntions:
					ApplyPeerMuntions((int)gs[i + 2], (int)gs[i + 3], (int)gs[i + 4]);
					i += (int)gs[i + 1];
					break;

				case Type.Fuel:
					ApplyPeerFuel((int)gs[i + 2], (int)gs[i + 3], (int)gs[i + 4]);
					i += (int)gs[i + 1];
					break;
			}
			infLoopCheck++;//Make sure this can't get in a weird loop
		}


	}

	public static void ApplyObjGoal(int uId, Vector3 goal)
	{
		//Apply this to an object's goal
		int index = ObjFuncs.GetObjIndexForUID(uId);
		if (index == -1) { return; }//Couldn't find the obj
		if (ObjFuncs.objs[index] == null) { return; }//Somehow, this obj[index] contains a null?

		ObjFuncs.objs[index].goal = goal;
		xa.goal = goal;
		xa.debugStr += "\nReceived goal";
	}

	public static void ApplyObjHealth(int uId, int health)
	{
		//Apply this to an object's goal
		int index = ObjFuncs.GetObjIndexForUID(uId);
		if (index == -1) { return; }//Couldn't find the obj
		if (ObjFuncs.objs[index] == null) { return; }//Somehow, this obj[index] contains a null?

		ObjFuncs.objs[index].health = health;
	}
	
	public static void ApplyPeerPop(int uId, int pop, int maxPop)
	{
		hl.Peer p = hl.GetPeerForUID(uId);
		if (p == null) { return; }

		p.pop = pop;
		p.maxPop = maxPop;

		UIController.displayPop = pop;
		UIController.displayMaxPop = maxPop;
	}

	public static void ApplyPeerMoney(int uId, int money, int moneyIncrease)
	{
		hl.Peer p = hl.GetPeerForUID(uId);
		if (p == null) { return; }

		p.money = money;
		p.moneyIncrease = moneyIncrease;

		UIController.displayMoney = money;
		UIController.displayMoneyIncrease = moneyIncrease;
	}

	public static void ApplyPeerMuntions(int uId, int muntions, int muntionsIncrease)
	{
		hl.Peer p = hl.GetPeerForUID(uId);
		if (p == null) { return; }

		p.munitions = muntions;
		p.munitionsIncrease = muntionsIncrease;

		UIController.displayMunitions = muntions;
		UIController.displayMunitionsIncrease = muntionsIncrease;
	}

	public static void ApplyPeerFuel(int uId, int fuel, int fuelIncrease)
	{
		hl.Peer p = hl.GetPeerForUID(uId);
		if (p == null) { return; }

		p.fuel = fuel;
		p.fuelIncrease = fuelIncrease;

		UIController.displayFuel = fuel;
		UIController.displayFuelIncrease = fuelIncrease;
	}


}
