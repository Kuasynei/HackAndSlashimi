using UnityEngine;
using System.Collections;

public class CameraRail : MonoBehaviour {

	[SerializeField] bool debugMode = true;
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
				if (debugMode)
				{
					Debug.DrawLine (nodes [i], nodes [i + 1], Color.blue);
				}
			}
		}
	}

	public Vector3 ProjectPositionOnRail(Vector3 pos)
	{
		int closestNodeIndex = GetClosestNode(pos); 

		if (closestNodeIndex == 0)
		{
			return ProjectOnSegment (nodes[0], nodes[1], pos);
		}
		else if (closestNodeIndex == nodeCount - 1)
		{
			return ProjectOnSegment (nodes[nodeCount - 1], nodes[nodeCount - 2], pos);
		}
		else
		{
			Vector3 leftSeg = ProjectOnSegment (nodes[closestNodeIndex - 1], nodes[closestNodeIndex], pos);
			Vector3 rightSeg = ProjectOnSegment (nodes[closestNodeIndex + 1], nodes[closestNodeIndex], pos);

			if (debugMode)
			{
				Debug.DrawLine (pos, leftSeg, Color.blue);
				Debug.DrawLine (pos, rightSeg, Color.blue);
			}

			if ((pos - leftSeg).sqrMagnitude <= (pos - rightSeg).sqrMagnitude)
			{
				return leftSeg;
			}
			else
			{
				return rightSeg;
			}
		}
	}

	int GetClosestNode(Vector3 pos)
	{
		int closestNodeIndex = -1;
		float shortestDist = 0;

		for (int i = 0; i < nodeCount; i++)
		{
			float sqrDist = (nodes [i] - pos).sqrMagnitude;
			if (shortestDist == 0 || sqrDist < shortestDist)
			{
				shortestDist = sqrDist;
				closestNodeIndex = i;
			}
		}

		return closestNodeIndex;
	}

	Vector3 ProjectOnSegment(Vector3 v1, Vector3 v2, Vector3 pos)
	{
		Vector3 v1ToPos = pos - v1;
		Vector3 segDirection = (v2 - v1).normalized;

		float distanceFromV1 = Vector3.Dot (segDirection, v1ToPos);
		if (distanceFromV1 < 0)
		{
			return v1;
		}
		else if (Mathf.Pow (distanceFromV1, 2) > (v2 - v1).sqrMagnitude)
		{
			return v2;
		}
		else
		{
			Vector3 fromV1 = segDirection * distanceFromV1;
			return v1 + fromV1;
		}
	}
}
