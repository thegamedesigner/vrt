using UnityEngine;
using System.Collections;

public class DrawSkeleton : MonoBehaviour
{
	public Bone[] bones = new Bone[0];

	[System.Serializable]
	public class Bone
	{
		public string label = "";
		public Joint joint1;
		public Joint joint2;
	}
	
	[System.Serializable]
	public class Joint
	{
		public GameObject go;
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < bones.Length; i++)
		{
			Debug.DrawLine(bones[i].joint1.go.transform.position,bones[i].joint2.go.transform.position,Color.blue);
		}
		}
}
