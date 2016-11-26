using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{
	public bool SnapZToZero = false;
	// Use this for initialization
	void Start()
	{

	}

	void Update()
	{
		if(xa.de == null) {return; }
		transform.LookAt(Camera.main.transform.position, Vector3.back);
		if(SnapZToZero) { transform.SetAngZ(0); }

	}
}
