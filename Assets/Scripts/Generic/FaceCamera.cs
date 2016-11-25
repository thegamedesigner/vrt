using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	void Update()
	{
		if(xa.de == null) {return; }
		transform.LookAt(Camera.main.transform.position, Vector3.back);

	}
}
