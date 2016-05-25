using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class ExtendedChildCollider : MonoBehaviour {


	void OnCollisionEnter(Collision collInfo)
	{
		transform.parent.SendMessage ("OnCollisionEnter", collInfo, SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionStay(Collision collInfo)
	{
		transform.parent.SendMessage ("OnCollisionStay", collInfo, SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionExit(Collision collInfo)
	{
		transform.parent.SendMessage ("OnCollisionExit", collInfo, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerEnter(Collider otherColl)
	{
		transform.parent.SendMessage ("OnTriggerEnter", otherColl, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerStay(Collider otherColl)
	{
		transform.parent.SendMessage ("OnTriggerStay", otherColl, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerExit(Collider otherColl)
	{
		transform.parent.SendMessage ("OnTriggerExit", otherColl, SendMessageOptions.DontRequireReceiver);
	}
}
