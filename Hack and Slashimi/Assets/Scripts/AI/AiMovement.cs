using UnityEngine;
using System.Collections;

public class AiMovement : EntityClass {

    [SerializeField] GameObject player;
    [SerializeField] GameObject playerColossus;
    [SerializeField] WeaponClass enemyWeapon;

    private CharacterController charController;
    //THIS WILL BE CHANGED TO AXE
    private Sword enemySword;
    

    private float distToPlayer;
    private float distToColossus;
    private float weaponLock = 0;

	
    // Use this for initialization
	void Start () 
    {
        //Store the character controller and the enemy sword/weapon
        charController = GetComponent<CharacterController>();
        enemySword = (Sword)enemyWeapon;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Check distance from the enemy to the player
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //Check distance from the enemy to the player colossus
        distToColossus = Vector3.Distance(playerColossus.transform.position, transform.position);

        //Ememy Attack Funtionality
        //The enemy AI is either going to hit the player or the colossus. The colossus has priority.
        //The idea is that the enemy will rotate and look at the thing it wants to hit and then run the function
        //@note: I have no idea how to do this without breaking it
        if (enemySword && weaponLock <= 0)
        {
            if (distToColossus <= 2 || distToPlayer <= 1.5f)
            {
                if (distToColossus <= 2)
                {
                    //Rotate and smack the colossus
                }
                else if (distToPlayer <= 1.5f)
                {
                    //Rotate and smack the player
                }
                enemySword.BasicAttack(0.4f, 1);
                weaponLock = 1f;
            }
        }

        //This entire area is basically a behavior tree. This prioritizes running towards the colossus then towards the player else it will just
        //run to the left
        //@note: the basic else functionality will have to change because enemies will not only spawn on the right of the map
        if(distToColossus < 10)
        {
            if(distToColossus > 2)
            {
                //Move toward the colossus
                charController.SimpleMove(new Vector3(3, 0, 0));
            }
            else if( distToColossus < 2)
            {
                //Stop and look at the colossus
                charController.SimpleMove(Vector3.zero);
            }
        }
        else if(distToPlayer < 10)
        {
            if (distToPlayer > 1.5f)
            {
                Vector3 headingDir = player.transform.position - transform.position;
                float leftOrRightValue = AngleDir(transform.forward, headingDir, transform.up);
                if(leftOrRightValue != 0)
                {
                    transform.rotation = Quaternion.LookRotation(leftOrRightValue * Vector3.right);
                }

                charController.SimpleMove(new Vector3(leftOrRightValue * 3, 0, 0));
            }
            else if (distToPlayer < 1.5f)
            {
                charController.SimpleMove(Vector3.zero);
            }
        }
        else
        {
            Vector3 headingDir = playerColossus.transform.position - transform.position;
            float leftOrRightValue = AngleDir(transform.forward, headingDir, transform.up);
            if (leftOrRightValue != 0)
            charController.SimpleMove(new Vector3(leftOrRightValue * 1, 0, 0));
            if (leftOrRightValue != 0)
            {
                transform.rotation = Quaternion.LookRotation(leftOrRightValue * Vector3.right);
            }

        }

        //Ticks down the weapon lock allowing the enemy to attack again
        weaponLock -= Time.deltaTime;

        //Checks the health value that is on the EntityClass and destroys the object
        if(health <= 0)
        {
            Destroy(gameObject);
        }
	}

    //Solves the problem of "Is the object left or right of me"
    //This was obtained from a thread on a forum because I need to git gud.
    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return -1f;
        }
        else if (dir < 0f)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }
}
