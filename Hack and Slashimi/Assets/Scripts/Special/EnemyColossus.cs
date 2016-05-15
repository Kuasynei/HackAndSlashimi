using UnityEngine;
using System.Collections;

public class EnemyColossus : EntityClass 
{
	[SerializeField] float maxHealth = 10000;
	[SerializeField] float shockwaveTime = 5.0f;
	[SerializeField] GameObject shockwaveObject;
	GameObject player;
	bool gotPlayer;

	void Awake () 
	{
		maxH = maxHealth;
	}

//	void Start()
//	{
//		player = GameManager.GetPlayer();
//	}
	void Update()
	{
		if(!gotPlayer)
		{
			player = GameManager.GetPlayer();
			if(player != null)
			{
				gotPlayer = true;
				//Debug.Log("Player Received!");
			}
		}
	}

	void FixedUpdate()
	{
		//Debug.Log(Vector3.Distance(transform.position, player.transform.position));

		if(health <= 0)
		{
			Destroy(gameObject);
		}

		if(Vector3.Distance(transform.position, player.transform.position) < 20.0f)
		{
			shockwaveTime += Time.deltaTime;

			if(shockwaveTime > 5)
			{
				Debug.Log("SHOCKWAVE");
				shockwaveTime = 0;
			}
		}
	}
}
