using UnityEngine;
using System.Collections;

public class ActorController : MonoBehaviour
{
	public Puppet puppet;
	public LayerMask mask;
	Vector3 goal;

	[System.Serializable]
	public class Puppet
	{
		public GameObject head;
		public GameObject body;
		public GameObject leftHand;
		public GameObject rightHand;

	}

	void Start()
	{
		goal = transform.position;
		if(xa.emptyObj == null) {xa.emptyObj = new GameObject("emptyObj"); }
	}

	void Update()
	{
		//Get goal
		if (Input.GetMouseButtonDown(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 999, mask))
			{
				goal = hit.point;
			}
		}

		MoveToGoal();
	}

	Vector3 oldAngles; 
	Vector3 oldAngles2; 

	void MoveToGoal()
	{

		oldAngles = transform.localEulerAngles;
		transform.localEulerAngles = oldAngles2;

		if (Vector3.Distance(transform.position, goal) > 0.3f)
		{
			goal = new Vector3(goal.x,goal.y,0);

			//Slow turn at a flat rate
			//transform.rotation = Setup.SlowTurn(transform.position, transform.rotation, goal, 555, new Vector3(0,0,1));

			//Vector3 lookPos = goal - transform.position;
			//lookPos.y = 0;
			//	Quaternion rotation = Quaternion.LookRotation(lookPos);
			//transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.5f);

			//Quaternion rotation = Quaternion.LookRotation(goal - transform.position, transform.TransformDirection(Vector3.up));
			//transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
			
			transform.LookAt(goal);

			transform.Translate(new Vector3(-1 * Time.deltaTime, 0, 0));
		}

		oldAngles2 = transform.localEulerAngles;
		transform.localEulerAngles = oldAngles;

	}

}
