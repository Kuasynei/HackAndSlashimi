using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
	PlayerClass playerScript;
	Colossus colScript;

	[SerializeField] float playerHealAmount = 5.0f;
	[SerializeField] float colossusHealAmount = 5.0f;

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			playerScript = (PlayerClass)col.gameObject.GetComponent("PlayerClass");
			playerScript.Heal(playerHealAmount);

			colScript = (Colossus)FindObjectOfType(typeof(Colossus));
			colScript.Heal(colossusHealAmount);

			Destroy(gameObject);
		}
	}
}
