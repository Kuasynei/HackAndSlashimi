using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    //[SerializeField] GameObject enemyToSpawn;
	[SerializeField] bool debugMode;
    [SerializeField] int nEnemiesToSpawn;
    [SerializeField] int MAXSpawnedEnemies;

    private float spawnTimer;
	private float distToPlayer;
    public static int enemiesSpawned;
    private GameObject enemy;
	private GameObject colossus;
	private GameObject player;
    private EnemyPool oP;

	// Use this for initialization
	void Start ()
    {
        spawnTimer = 3;

        oP = GetComponent<EnemyPool>();

		colossus = GameManager.GetPColossus ();
		player = GameManager.GetPlayer ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

		if (distToPlayer >= 30)
        {
            if (spawnTimer <= 0 && enemiesSpawned < MAXSpawnedEnemies)
            {
                //Debug.Log("Ding");
                for (int i = 0; i < nEnemiesToSpawn; i++)
                {
                    GameObject temp = oP.GetFromPool();

                    temp.SetActive(true);
                    temp.transform.position = new Vector3(transform.position.x * i / 4, transform.position.y , 0);
                    temp.transform.rotation = transform.rotation;

					Footsoldier tempFootsoldier = temp.GetComponent<Footsoldier> ();
					tempFootsoldier.setPlayer(player);
					tempFootsoldier.setPlayerColossus(colossus);
					tempFootsoldier.setResetHealth(); 
					tempFootsoldier.debugMode = debugMode;

                    enemiesSpawned++;

                    //Debug.Log("Dong");
                }

                spawnTimer = 3;
            }
        }

        spawnTimer -= Time.deltaTime;
	}
}
