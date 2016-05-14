using UnityEngine;
using System.Collections;

public class SetPositionTo : MonoBehaviour {

	[SerializeField] Transform target;
	[SerializeField] Vector3 offset;

	// Update is called once per frame
	void Update () {
		transform.position = target.position + offset;
	}
}
