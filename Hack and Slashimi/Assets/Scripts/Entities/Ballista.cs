using UnityEngine;
using System.Collections;

public class Ballista : EnemyClass {

	[SerializeField] bool debugMode = true;
	[SerializeField] GameObject bolt;
	[SerializeField] Transform boltSpawnPoint;
	[SerializeField] float secondsToReload = 7;
	[SerializeField] float rotationSpeed = 10;
	[SerializeField] float projectileSpeed = 50;
	[SerializeField] float leadTarget = 10;

	[Range (0,1)]
	[SerializeField] float arcBias = 0; //0 minimum arc, 1 maximum arc.

	float fireCooldown = 0;
	Transform targetTransform = null;

	void Update()
	{
		if (fireCooldown > 0)
			fireCooldown -= Time.deltaTime;

		//if (targetTransform != null  && fireCooldown <= 0)
		if (targetTransform != null && fireCooldown <= 0)
		{
			fireBolt ();
			fireCooldown = secondsToReload;
		}
	}

	void OnTriggerEnter(Collider otherColl)
	{
		//Finding the colossus.
		if (otherColl.CompareTag ("Colossus"))
		{
			targetTransform = otherColl.transform;
		}
	}

	void fireBolt()
	{
		StartCoroutine ("execFireBolt");
	}

	IEnumerator execFireBolt()
	{
		float timeFrame = Time.time;
		GameObject newBolt = Instantiate (bolt, boltSpawnPoint.position, boltSpawnPoint.rotation) as GameObject;
		Rigidbody newBoltRB = newBolt.GetComponent<Rigidbody> ();
		newBoltRB.isKinematic = true;
		newBolt.transform.SetParent (transform);

		while ((Time.time - timeFrame) < 2)
		{
			Vector3 ballisticDirection = calculateBallisticAngle();

			//Rotate the ballista horizontally.
			Quaternion desiredBallistaRot = Quaternion.LookRotation (new Vector3(ballisticDirection.x, 0, ballisticDirection.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredBallistaRot, Time.fixedDeltaTime * rotationSpeed);

			//Rotate the bolt vertically.
			Quaternion desiredBoltRot = Quaternion.LookRotation (ballisticDirection);
			newBolt.transform.rotation = Quaternion.Slerp(newBolt.transform.rotation, desiredBoltRot, Time.fixedDeltaTime * rotationSpeed);

			//Keep the bolt attached to the ballista at the correct location.
			newBolt.transform.position = boltSpawnPoint.position;

			yield return new WaitForFixedUpdate();
		}

		newBoltRB.isKinematic = false;
		newBolt.transform.SetParent (null);

		newBoltRB.velocity = calculateBallisticAngle();
	}

	//Ballistic Arc Code
	Vector3 calculateBallisticAngle ()
	{
		
		//The target's position, offset to include the target's velocity.
		Vector3 targetOffsetPosition = targetTransform.position + (targetVelocity() * Time.fixedDeltaTime * leadTarget * projectileSpeed);
		Vector3 targetOffsetPositionXZ = new Vector3(targetOffsetPosition.x, boltSpawnPoint.position.y, targetOffsetPosition.z);

		float distToTargetXZ = Vector3.Distance(boltSpawnPoint.position, targetOffsetPositionXZ); //X
		float heightRelativeToBallista = targetOffsetPosition.y - boltSpawnPoint.position.y; //Y

		// 0 = arctan( (v^2 +- sqr[ v^4 - g(gx^2 + 2y*v^2) ])/g*x )
		//				    ^^
		//		Change this to plus, or minus, to attain the minimum, and maximum arc possible.

		float minBallisticAngle = Mathf.Atan ((Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - (-Physics.gravity.magnitude) *
			(Physics.gravity.magnitude * Mathf.Pow(distToTargetXZ, 2) + 2 * heightRelativeToBallista * Mathf.Pow(projectileSpeed, 2)))) / (Physics.gravity.magnitude * distToTargetXZ));

		float maxBallisticAngle = Mathf.Atan ((Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - (-Physics.gravity.magnitude) *
			(Physics.gravity.magnitude * Mathf.Pow(distToTargetXZ, 2) + 2 * heightRelativeToBallista * Mathf.Pow(projectileSpeed, 2)))) / (Physics.gravity.magnitude * distToTargetXZ));

		arcBias = Mathf.Clamp (arcBias, 0, 1);
		float mixedBallisticAngle = minBallisticAngle * (1 - arcBias) - maxBallisticAngle * arcBias;

		//Setting ballistic 
		Vector3 ballisticDirection = Quaternion.Euler (mixedBallisticAngle * Mathf.Rad2Deg, 0, 0) * Vector3.forward * projectileSpeed;

		//We exclude the height of the target here so that we can aim at the XZ position. We don't need the Y position here because the
		//ballista is firing at an angle that does not point directly at the colossus Y position.
		ballisticDirection = Quaternion.LookRotation (targetOffsetPositionXZ - boltSpawnPoint.position, Vector3.up) * ballisticDirection;

		if (debugMode)
		{
			Debug.DrawRay (boltSpawnPoint.position, ballisticDirection, Color.magenta);
			Debug.DrawRay (targetOffsetPosition, targetOffsetPosition - targetTransform.position, Color.magenta);
		}

		return ballisticDirection;
	}

	Vector3 targetVelocity()
	{
		if (targetTransform.GetComponent<Rigidbody> ())
		{
			return targetTransform.GetComponent<Rigidbody> ().velocity;
		}
		else
		{
			Debug.LogWarning ("Ballista Target has no rigidbody!");
			return Vector3.zero;
		}
	}
}
