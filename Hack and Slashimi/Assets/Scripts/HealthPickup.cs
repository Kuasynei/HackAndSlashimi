using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
	PlayerClass playerScript;

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			playerScript = (PlayerClass)col.gameObject.GetComponent("PlayerClass");
			playerScript.Heal(5.0f);
			Destroy(gameObject);
		}
	}
}
