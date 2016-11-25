using UnityEngine;
using System.Collections;

public class Layers : MonoBehaviour
{
	public static LayerMask ReturnMaskForClId(int clId)
	{
		LayerMask mask;

		mask = xa.de.CombatMask;//I think I might be able to get away with just one mask for everyone, not 1 per player

		return mask;
	}

}
