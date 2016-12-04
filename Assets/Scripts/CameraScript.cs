﻿using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
	GameObject CameraNode;
	GameObject puppet;

	float turnSpd = 45;
	float verTurnSpd = 25;
	float zoomSpd = 111;
	float spdAdd = 1;
	float scrollZoomFactor = 1;
	float dist = 250;
	float minDist = 3;
	float maxDist = 600;
	float tilt = 0;
	float minTilt = -10;
	float maxTilt = -75;
	Vector3 vel = new Vector3(0, 0, 0);

	void Start()
	{
		dist = 42;
		tilt = -55;
		//puppet.SetActive(false);

	}

	void Update()
	{
		scrollZoomFactor = dist * 0.001f;
		if (scrollZoomFactor < 0.1f) { scrollZoomFactor = 0.1f; }
		if (scrollZoomFactor > 1f) { scrollZoomFactor = 1f; }

		if (CameraNode == null)
		{
			//try to find a camera node
			CameraNode = GameObject.FindGameObjectWithTag("CameraNode");
			if (CameraNode != null)
			{
				puppet = CameraNode.transform.Find("puppet").gameObject;
				puppet.SetActive(false);
			}
		}
		if (CameraNode == null) { return; }
		if (ChatScript.typing) { return; }
		//Zoom
		dist += -Input.GetAxis("MouseScrollWheel") * zoomSpd * Time.deltaTime;
		if (dist < minDist) { dist = minDist; }
		if (dist > maxDist) { dist = maxDist; }

		//Rotate node
		if (Input.GetKey(KeyCode.Space))
		{
			//spin the cameraNode horizontally
			CameraNode.transform.AddAngY(Input.GetAxis("Horizontal") * turnSpd * Time.deltaTime);

			//tilt the cameraNode vertically
			tilt += Input.GetAxis("Vertical") * verTurnSpd * Time.deltaTime;
			if (tilt < -75) { tilt = maxTilt; }
			if (tilt > -10) { tilt = minTilt; }
		}
		CameraNode.transform.SetAngX(tilt);
		//Debug.Log("tilt: " + tilt + ", y: " + CameraNode.transform.localEulerAngles.y + ", dist: " + dist);


		//Move node
		Vector3 oldAngs = CameraNode.transform.localEulerAngles;
		CameraNode.transform.SetAngX(0);

		if (Input.GetKey(KeyCode.W)) { vel.z -= spdAdd * scrollZoomFactor; }
		if (Input.GetKey(KeyCode.S)) { vel.z += spdAdd * scrollZoomFactor; }
		if (Input.GetKey(KeyCode.A)) { vel.x += spdAdd * scrollZoomFactor; }
		if (Input.GetKey(KeyCode.D)) { vel.x -= spdAdd * scrollZoomFactor; }
		CameraNode.transform.Translate(vel);

		CameraNode.transform.SetAngX(oldAngs.x);
		vel *= 0.5f;

		//Place camera
		transform.position = MathFuncs.ProjectVec(CameraNode.transform.position, CameraNode.transform.localEulerAngles, dist, Vector3.forward);

		//Look at CameraNode
		transform.LookAt(CameraNode.transform.position);
	}
}
