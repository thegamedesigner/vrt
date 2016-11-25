using UnityEngine;
using System.Collections;

public class Resources : MonoBehaviour
{
	public static void WipeStaticVars()
	{
	}

	public static void InitResources(hl.Peer peer)
	{
		peer.pop = 0;
		peer.maxPop = 10;
		peer.money = 300;
		peer.moneyIncrease = 1;
		peer.munitions = 0;
		peer.munitionsIncrease = 0;
		peer.fuel = 0;
		peer.fuelIncrease = 1;
		peer.resourceUpdateDelay = 1;
		peer.resourceUpdateTimeSet = 1;
	}

	public static void UpdateResources(hl.Peer peer)//Called only on the server, clients get this information via the tick
	{
		if (hl.svTime >= (peer.resourceUpdateTimeSet + peer.resourceUpdateDelay))
		{
			peer.resourceUpdateTimeSet = hl.svTime;
			peer.money += peer.moneyIncrease;
			peer.munitions += peer.munitionsIncrease;
			peer.fuel += peer.fuelIncrease;
		}

	}

}
