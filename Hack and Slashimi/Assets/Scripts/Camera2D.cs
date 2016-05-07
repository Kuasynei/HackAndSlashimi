using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	[SerializeField] Transform target;
	[SerializeField] float lerpSpeed;
	[SerializeField] float zLayer = -11;
	[SerializeField] float groundLayer = 0;

	// Update is called once per frame
	void Update () {
		Vector3 lockedTargetTransform = new Vector3 (target.position.x, target.position.y, zLayer);
		Vector3 lockedCurrentTransform = new Vector3 (transform.position.x, transform.position.y, zLayer);

		transform.position = Vector3.Lerp (lockedCurrentTransform, lockedTargetTransform, lerpSpeed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider otherColl)
	{

	}
}
