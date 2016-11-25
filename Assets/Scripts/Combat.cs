using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{
	public static void SvSideCombat()
	{
		//Figure out combat
		List<ObjFuncs.Obj> objs = ObjFuncs.objs;
		List<int> possibleTargets = new List<int>();
		RaycastHit hit;

		for (int i = 0; i < objs.Count; i++)
		{
			ObjFuncs.Obj o = objs[i];
			/*
			if (o.range > 0 && o.attachedTo != -1)//Is a gun, and is attached to a unit
			{
				if (Time.timeSinceLevelLoad > (o.firingTimeSet + o.firingDelay))
				{
					o.firingTimeSet = Time.timeSinceLevelLoad;

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

					//Loop through every other object
					for (int a = 0; a < objs.Count; a++)
					{
						ObjFuncs.Obj o2 = objs[a];
						if (o2.ownedByClId != o.ownedByClId && o2.health > 0)//If owned by other player
						{
							if (MathFuncs.DistXZ(o.go.transform.position, o2.go.transform.position) <= o.range)
							{
								//Check if it's within the firing cone
								if (MathFuncs.CheckCone(o.range, o.firingConeAngle * 0.5f, ang.y, origin, o2.go.transform.position, true))
								{
									//Can I raytrace to this point?
									LayerMask mask = xa.de.CombatMask;//Layers.ReturnMaskForClId(o2.ownedByClId);
									//Effects.DrawLine(origin, o2.go.transform.position, 0.05f, Effects.Colors.Orange);
									if (Physics.Linecast(origin, o2.go.transform.position, out hit, mask))
									{
										//If this object is in range
										possibleTargets.Add(a);
									}
								}
							}
						}
					}


					int target = -1;
					if (possibleTargets.Count > 0)
					{
						//Now pick something from the list of possible targets
						int highestValue = -1;
						for (int a = 0; a < possibleTargets.Count; a++)
						{
							if (objs[possibleTargets[a]].priorityValue > highestValue)
							{
								target = possibleTargets[a];
								highestValue = objs[possibleTargets[a]].priorityValue;
							}
						}
					}
					else
					{
						//Nothing is viable
					}

					if (target != -1)
					{
						//Draw a line
						Debug.Log("BANG! " + Time.timeSinceLevelLoad);
						Effects.DrawLine(o.go.transform.position, ObjFuncs.objs[target].go.transform.position, 0.2f, Effects.Colors.Yellow);
						ObjFuncs.objs[target].health -= Random.Range(o.minDam,o.maxDam);
						if(ObjFuncs.objs[target].health < 0) {ObjFuncs.objs[target].health = 0; }
						
						ObjFuncs.objs[target].textMesh.text = "" + ObjFuncs.objs[target].health;
					}
				}
			}
			*/
		}

	}

}
