using UnityEngine;
using System.Collections;

public class Player_Sword : WeaponClass 
{
	[SerializeField] Color debugDeadlinessColor;

	MeshRenderer mR;
	PlayerClass wieldingPlayer;

	protected override void Awake() 
	{
		base.Awake ();

		mR = GetComponentInChildren<MeshRenderer> ();
	}

	protected override void Start()
	{
		base.Start ();

		wieldingPlayer = GameManager.GetPlayer ().GetComponent<PlayerClass> ();
	}

	//BASIC ATTACK//////////////////////////////////////////////////////////////////////////////////////////////////

	//This attack a certain amount of damage numerous times over its attack duration, changing color when active.
	public void BasicAttack() 
	{ 
		StartCoroutine (execBasicAttack()); 
	}

	//Handle how the actual attack plays out step-by-step here.
	IEnumerator execBasicAttack() 
	{
		wieldingPlayer.SetWeaponLock (0.3f); //The time it will take for this attack to finish. 


			//Movement
				wieldingPlayer.ManualBrake (0.1f, 600); //Brake for a short time.

		yield return new WaitForSeconds (0.1f); //Wind up Delay
			//Appearance
				Color originalColor = mR.material.color;
				mR.material.color = debugDeadlinessColor;
				transform.Rotate (new Vector3(75, 0, 0), Space.Self); //Delete once we get animations.
			
			//Movement
				wieldingPlayer.Dash (0.1f, Vector3.right * wieldingPlayer.GetFacingDirection() * 15, false); //Dash in the direction you are facing. Gravity applies.
				wieldingPlayer.ManualBrake (0.1f, 600); //We brake at the same time to decellerate the player quickly during this dash.
			
			//Weaponization
				OneShotWeaponize (new OmniAttackInfo(wieldingPlayer.gameObject, wieldingPlayer.GetFaction(), 5, 0.5f, Vector3.zero));

		yield return new WaitForSeconds (0.1f); //Attack finishing up
			//Appearance
				mR.material.color = originalColor;
				transform.Rotate (new Vector3(-75, 0, 0), Space.Self); //Delete once we get animations.
			
			//Weaponization
				OneShotWeaponize (new OmniAttackInfo(wieldingPlayer.gameObject, wieldingPlayer.GetFaction(), 5, 0.5f, Vector3.zero));
	}

	//LAUNCHING ATTACK//////////////////////////////////////////////////////////////////////////////////////////////////

	//An attack that launches nearby enemies into the air and deals damage.
	public void LaunchingAttack()
	{
		StartCoroutine (execLaunchingAttack());
	}

	//Handle how the actual attack plays out step-by-step here.
	IEnumerator execLaunchingAttack()
	{
		wieldingPlayer.SetWeaponLock (0.6f);


			//Movement
				wieldingPlayer.ManualBrake (0.3f, 300); //Brake for a short time.

		yield return new WaitForSeconds(0.3f);
			//Movement
				//Dash in the direction you are facing over the span of 0.4 seconds. Gravity applies.
				wieldingPlayer.Dash (0.4f, Vector3.right * wieldingPlayer.GetFacingDirection() * 15, false); 

		yield return new WaitForSeconds(0.1f);
			//Appearance
				Color originalColor = mR.material.color;
				mR.material.color = debugDeadlinessColor;

			//Movement
				wieldingPlayer.ManualBrake (0.5f, 1000);

			//Weaponization
				OneShotWeaponize(new OmniAttackInfo(wieldingPlayer.gameObject, wieldingPlayer.GetFaction(), 10, 1, Vector3.up * 7));

		yield return null;
			//Appearance
				mR.material.color = originalColor;
				PacifyContact ();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
}
