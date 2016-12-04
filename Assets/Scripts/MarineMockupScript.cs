using UnityEngine;
using System.Collections;

public class MarineMockupScript : MonoBehaviour
{



	Animator animator;
	int ani = 0;
	float rotationSpd = 0;
	void Start()
	{
		animator = gameObject.GetComponent<Info>().animator;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			if (rotationSpd == 0) { rotationSpd = 35f; }
			else if (rotationSpd == 35f) { rotationSpd = -35; }
			else if (rotationSpd == -35f) { rotationSpd = 75; }
			else if (rotationSpd == 75f) { rotationSpd = 0; }
		}

		transform.AddAngY(rotationSpd * Time.deltaTime);
		if (Input.GetKeyDown(KeyCode.B))
		{
			ani++;
			if (ani > 1) { ani = 0; }

			switch (ani)
			{
				case 0:
					animator.SetBool("isMoving", true);
					break;
				case 1:
					animator.SetBool("isMoving", false);
					break;
			}
		}
		if (animator != null)
		{
		}
	}
}
