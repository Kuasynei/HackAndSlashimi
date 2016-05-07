using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	[SerializeField] Transform target;
	[SerializeField] float lerpSpeed;
	[SerializeField] float zLayer = -11;
	[SerializeField] float groundLayer = 0;

	// Update is called once per frame
	void Update () {
		//Horizontal Lerping
		Vector3 lockedTargetTransform = new Vector3 (target.position.x, transform.position.y, zLayer);
		Vector3 lockedCurrentTransform = new Vector3 (transform.position.x, transform.position.y, zLayer);
		transform.position = Vector3.Lerp (lockedCurrentTransform, lockedTargetTransform, lerpSpeed * Time.deltaTime);

		//Vertical Lerping
		Vector3 targetTransformV = new Vector3 (transform.position.x, target.position.y / 2 + groundLayer, transform.position.z);
		Vector3 currentTransformV = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		transform.position = Vector3.Lerp (currentTransformV, targetTransformV, lerpSpeed * Time.deltaTime);

	}

	void OnTriggerStay(Collider otherColl) {
		groundLayer = otherColl.transform.position.y;
		Debug.Log ("TA-DA");
	}
}
