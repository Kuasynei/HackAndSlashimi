using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Colossus : EntityClass 
{
	[SerializeField] bool debugMode = true;
	[SerializeField] float movementSpeed;
	[SerializeField] float maxHealth = 100;
	[SerializeField] float damage = 1000;
	[SerializeField] float gateDetectionLength = 5;
	[SerializeField] Text uI_HP;

    bool gateToDestroy;
	Rigidbody rB;
	float timeSinceLastHit = 0.0f;
	Gate gateScript;
    Vector3 deathPosition;

    //Colossus death variables
    private bool isDead = false;

	protected override void Awake () 
	{
		maxH = maxHealth;
		base.Awake ();

		rB = GetComponent<Rigidbody> ();

		GameManager.SetPColossus (this.gameObject);
	}

	void Update()
	{
		//UI STUFF
		if (uI_HP != null)
			uI_HP.text = Mathf.Round(health).ToString();
	}

	void FixedUpdate()
	{
        if(health <= 0)
        {
            isDead = true;
        }

        if(isDead)
        {
            //gameObject.SetActive(false);

            if(health > 20)
            {
                isDead = false;
                transform.position = deathPosition;
                deathPosition = Vector3.zero;
                //gameObject.SetActive(true);
            }

            deathPosition = transform.position;
            transform.position = new Vector3(0, 1000, 0);
        }
        else
        {
            timeSinceLastHit += Time.fixedDeltaTime;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.right, out hit, gateDetectionLength))
            {
                if (hit.collider.CompareTag("Gate"))
                {
                    gateToDestroy = true;
                    rB.velocity = new Vector3(0, 0, 0);
                    if (timeSinceLastHit > 5)
                    {
                        gateScript = (Gate)hit.collider.GetComponent("Gate");

						DamageInfo breakTheGatePackage = new DamageInfo (damage, this.gameObject, faction.goodGuys);
						float gateHealth = gateScript.TakeDamage(breakTheGatePackage);
                        timeSinceLastHit = 0;
                        Debug.DrawLine(transform.position, hit.point, Color.red, .3f);

                        if (debugMode)
                        {
                            Debug.Log("Colossus dealt " + damage + " to a Gate!\n" + "The gate has " + gateHealth + " health remaining.");
                        }
                    }
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
            }
            else
            {
                gateToDestroy = false;
            }

            if (!gateToDestroy)
            {
                rB.velocity = new Vector3(movementSpeed / 10, rB.velocity.y, rB.velocity.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DamageInfo suicidePackage = new DamageInfo(1, this.gameObject, faction.neutral);
            TakeDamage(suicidePackage);
            Debug.Log("LIFE did " + health + " damage to " + name + "! " + health + " health remaining.");
        }


    }

    public bool getIsDead()
    {
        //Return true if dead, false if alive
        return isDead;
    }
}
