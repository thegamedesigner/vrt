using UnityEngine;
using System.Collections;

public class PrefabLibrary : MonoBehaviour
{
	public GameObject MarinePrefab;



	public static GameObject GetPrefabForType(ObjFuncs.Type type)
	{
		if(type == ObjFuncs.Type.Marine) {return xa.pr.MarinePrefab; }
		return null;
	}

}
