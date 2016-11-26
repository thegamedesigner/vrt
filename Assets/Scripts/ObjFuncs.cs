using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjFuncs : MonoBehaviour
{
	public static List<Obj> objs = new List<Obj>();

	public enum Type
	{
		None = 0,
		Marine = 1,
		End
	}

	public class Obj//Not a sendable class
	{
		//Sync'd variables (have to be added to Ticks.cs by hand, but seperated here for clarity)
		public Vector3 goal = Vector3.zero;
		public int health = 10;

		public Type type;
		public GameObject go;
		public GameObject selectionCircle;
		public int uId = -1;//This object's uid
		public int ownedByClId = -1;//The uId of the peer that owns this object
		public bool selected = false;
		public bool isUnit = false;
		public float stopDist = 1;
		public TextMesh textMesh;
		public bool isMoving = false;
		public float turnSpd = 55;
		public float spd = 5;
		public float desiredAngle = 0;
		public float range = 0;
		public float firingConeAngle = 0;//doubled, so 15 is a 30 degree cone
		public int priorityValue = 0;//The higher this is, the more likely an AI gun will shoot at it
		public int minDam;
		public int maxDam;
		public float firingDelay;
		public float firingTimeSet;

		public int popCost = 0;
		public int moneyCost = 0;
		public int muntionsCost = 0;
		public int fuelCost = 0;


		public int[] visualState = new int[3];
		public int[] oldVisualState = new int[3];
		public Renderer[] mainBodyRenderers;

		public Animator animator;

		public Animation animation;
		public AnimationClip animClip;
		/*
		 * 
		 */
		public void KillMeDead()
		{
			//detach this obj from the world, in all ways. Don't have to actually destroy any of the GameObjects.
			Effects.Bubbles(go.transform.position);
			Destroy(go);
		}

		public void UpdateObjVisualState()
		{
			//This function is called every frame on the client, and changes the materials/colors/shaders of the object,
			//based on what's happening to it. (example, it's behind terrain, out of the fog of war & being dragged).

			//Has the visual state changed?
			bool update = false;
			for (int i = 0; i < visualState.Length; i++)
			{
				if (visualState[i] != oldVisualState[i]) { update = true; break; }
			}

			//sync the states
			for (int i = 0; i < visualState.Length; i++)
			{
				oldVisualState[i] = visualState[i];
			}
		}
	}

	public static void CreateObj(Type type, Vector3 pos, Vector3 ang, int clId, int uId)
	{
		//Create the go
		hl.Peer peer = hl.GetPeerForUID(clId);
		Obj obj = new Obj();
		obj.type = type;
		obj.go = Instantiate(PrefabLibrary.GetPrefabForType(type));
		obj.go.transform.position = pos;
		obj.go.transform.localEulerAngles = ang;
		obj.ownedByClId = clId;
		obj.uId = uId;
		obj.goal = pos;

		switch (type)
		{
			case Type.Marine:
				obj.isUnit = true;//This obj is a unit, and can be selected.
								  //create selectionCircle
				obj.selectionCircle = (GameObject)Instantiate(xa.de.selectionCirclePrefab, pos, new Quaternion(0, 0, 0, 0));
				obj.selectionCircle.transform.parent = obj.go.transform;
				obj.selectionCircle.transform.localPosition = Vector3.zero;
				obj.selectionCircle.SetActive(false);
				obj.textMesh = obj.go.GetComponentInChildren<TextMesh>();
				obj.textMesh.text = "";//uId: " + obj.uId + "\nClId: " + obj.ownedByClId;
				obj.go.GetComponent<Info>().uId = obj.uId;
				obj.animator = obj.go.GetComponent<Info>().animator;
				int[] cost = GetCostForType(type);
				obj.popCost = cost[0];
				obj.moneyCost = cost[1];
				obj.muntionsCost = cost[2];
				obj.fuelCost = cost[3];
				obj.priorityValue = 1;
				obj.health = 100;
				break;

		}

		objs.Add(obj);
	}

	public static int[] GetCostForType(Type type)
	{
		int[] result = null;
		switch (type)
		{
			case Type.Marine:
				result = new int[4];
				result[0] = 1;//Pop
				result[1] = 50;//Money
				result[2] = 0;//Muntions
				result[3] = 0;//Fuel
				break;
		}
		return result;
	}


	public static bool CheckCost(Type type, hl.Peer peer)//Returns false if this peer can't afford it
	{
		int[] cost = GetCostForType(type);
		if (cost == null) { return false; }
		if (peer.pop + cost[0] <= peer.maxPop &&
			peer.money >= cost[1] &&
			peer.munitions >= cost[2] &&
			peer.fuel >= cost[3])
		{
			return true;
		}

		return false;
	}

	public static void SubtractCost(Type type, hl.Peer peer)//Subtracts the cost of this object from this peer
	{
		int[] cost = GetCostForType(type);
		if (cost == null) { return; }
		peer.pop += cost[0];
		peer.money -= cost[1];
		peer.munitions -= cost[2];
		peer.fuel -= cost[3];

	}

	public static void UpdateObjsOnServer()
	{
		//MoveUnits();//Move the units
		if (hl.hlObj != null)
		{
			if (hl.hlObj.isServer)
			{
				Combat.SvSideCombat();
			}
		}

	}

	public static void UpdateObjsLocally()
	{
		DrawFiringCones();

		MoveUnits();//Move the units

		//Handle selection
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];

			//is it a unit?
			if (o.isUnit)
			{
				if (o.selected != o.selectionCircle.activeSelf)
				{
					o.selectionCircle.SetActive(o.selected);
				}
			}
		}

		for (int i = 0; i < objs.Count; i++)
		{
			objs[i].UpdateObjVisualState();
		}

		//death


		//Animate objects
		AnimateObjects();
	}

	public static void AnimateObjects()
	{
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];

			if (o.animator != null)
			{
				o.animator.SetBool("isMoving", o.isMoving);
			}
		}
	}

	public static void KillDeadObjects()
	{
		//Loop through all of the objects and kill the dead ones.
		//I forget how I'm supposed to solve this particuler problem. I want to remove from a list while looping through it. So... I'm going to once for each dead object to delete, then start the for loop again each time.
		for (int i = 0; i < 1000; i++)
		{
			bool foundADeado = false;
			for (int a = 0; a < objs.Count; a++)
			{
				if (objs[a].health <= 0)
				{
					foundADeado = true;
					objs[a].KillMeDead();
					objs.RemoveAt(a);
					break;
				}
			}
			if (!foundADeado) { break; }
		}
	}

	public static void DrawFiringCones()
	{
		//draw firing cones from guns
		for (int i = 0; i < objs.Count; i++)
		{
			/*
			Obj o = objs[i];
			if (o.range > 0 && o.attachedTo != -1)//Is a gun, and is attached to a unit
			{
				Vector3 origin;
				Vector3 ang;
				Vector3 leftPos;
				Vector3 rightPos;
				Vector3 centerPos;

				origin = o.go.transform.position;
				ang = o.go.transform.eulerAngles;
				centerPos = MathFuncs.ProjectVec(origin, ang, o.range, Vector3.forward);
				leftPos = MathFuncs.ProjectVec(origin, new Vector3(ang.x, ang.y + o.firingConeAngle, ang.z), o.range, Vector3.forward);
				rightPos = MathFuncs.ProjectVec(origin, new Vector3(ang.x, ang.y - o.firingConeAngle, ang.z), o.range, Vector3.forward);
				Effects.DrawLine(origin, leftPos, 0.1f, Effects.Colors.White);
				Effects.DrawLine(leftPos, centerPos, 0.1f, Effects.Colors.White);
				Effects.DrawLine(centerPos, rightPos, 0.1f, Effects.Colors.White);
				Effects.DrawLine(rightPos, origin, 0.1f, Effects.Colors.White);
			}
			*/
		}

	}

	public static void MoveUnits()
	{
		//Move & turn units
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];
			if (o.isUnit)
			{
				o.isMoving = false;

				//Debug.DrawLine(o.go.transform.position,o.goal, Color.green);

				//Is the unit far enough away from it's goal, that it should go there?
				if (Vector3.Distance(o.go.transform.position, o.goal) > o.stopDist)
				{
					Effects.DrawLine(o.go.transform.position, o.goal, 0.2f, Effects.Colors.Red);
					o.isMoving = true;
					//Slow turn at a flat rate
					Vector3 pos = o.go.transform.position;
					float tempSpd = o.spd;

					o.go.transform.rotation = Setup.SlowTurn(pos, o.go.transform.rotation, o.goal, o.turnSpd);

					//Is the unit pointing at it's goal?
					if (MathFuncs.CheckCone(-1, 15, o.go.transform.localEulerAngles.y, o.go.transform.position, o.goal, true))
					{
						Effects.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(o.goal.x, o.goal.y + 1, o.goal.z), 0.3f, Effects.Colors.Cyan);
						tempSpd = o.spd;
					}
					else
					{
						//tempSpd = o.spd;// * 0.25f;//turning. Move at 1/4 speed
						tempSpd = 0;
					}

					o.go.transform.Translate(new Vector3(0, 0, tempSpd * Time.deltaTime));
				}
				else
				{
				}
				
				o.textMesh.text = "isMoving: " + o.isMoving;
			}
		}
	}

	public static int GetObjIndexForUID(int uId)
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i].uId == uId) { return i; }
		}
		return -1;
	}

}
