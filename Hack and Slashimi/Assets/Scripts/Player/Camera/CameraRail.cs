using UnityEngine;
using System.Collections;

public class CameraRail : MonoBehaviour {

	Vector3[] nodes;
	int nodeCount;

	void Awake()
	{
		nodeCount = transform.childCount;
		nodes = new Vector3[nodeCount];

		for (int i = 0; i < nodeCount; i++)
		{
			nodes[i] = transform.GetChild(i).position;
		}
	}

	void Update()
	{
		if (nodeCount > 1)
		{
			for (int i = 0; i < nodeCount - 1; i++)
			{
				Debug.DrawLine (nodes[i], nodes[i + 1], Color.blue);
			}
		}
	}
}
