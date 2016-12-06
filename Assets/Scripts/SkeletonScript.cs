using UnityEngine;
using System.Collections;

public class SkeletonScript : MonoBehaviour
{
	public enum RotationMethod
	{
		None,
		MatchUnitsAngle,
		LookAtJoint,
		End
	}

	public enum AttachMethod
	{
		None,
		OnePoint,
		End
	}

	public Bone[] bones;

	[System.Serializable]
	public class Bone
	{
		public string label = "";//what this thing is
		public string notes = "";//just notes
		public GameObject go;//The thing that is attached
		public AttachMethod attachMethod = AttachMethod.None;
		public RotationMethod rotationMethod = RotationMethod.None;
		public GameObject[] attachPoints;
		public GameObject[] rotationPoints;
	}

	void Start()
	{

	}

	void Update()
	{
		for (int i = 0; i < bones.Length; i++)
		{
			switch (bones[i].attachMethod)
			{
				case AttachMethod.OnePoint:
					bones[i].go.transform.position = bones[i].attachPoints[0].transform.position;
					break;
			}
			
			
			switch (bones[i].rotationMethod)
			{
				case RotationMethod.MatchUnitsAngle:
					bones[i].go.transform.SetAngY(gameObject.transform.localEulerAngles.y);
					break;
				case RotationMethod.LookAtJoint:
					bones[i].go.transform.LookAt(bones[i].rotationPoints[0].transform.position);
					break;
			}
		}
	}
}
