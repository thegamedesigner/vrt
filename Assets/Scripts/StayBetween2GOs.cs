using UnityEngine;
using System.Collections;

public class StayBetween2GOs : MonoBehaviour
{
	public GameObject go1;
	public GameObject go2;

	void Start()
	{

	}

	void Update()
	{
		Debug.DrawLine(go1.transform.position,gameObject.transform.position,Color.white);
		Debug.DrawLine(go2.transform.position,gameObject.transform.position,Color.white);

		float dist = Vector3.Distance(go1.transform.position,go2.transform.position);
		gameObject.transform.position = go1.transform.position;
		gameObject.transform.LookAt(go2.transform.position);
		gameObject.transform.Translate(dist * 0.5f,0,0);
	}
}
