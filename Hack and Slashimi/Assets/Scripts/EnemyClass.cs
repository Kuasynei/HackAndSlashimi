using UnityEngine;
using System.Collections;

public class EnemyClass : EntityClass {

	/* This is flash on damage code I worked on, and then stopped working on because I realized that it wasn't a good use of time, at the time. - Tony
	[Header ("Aesthetic")]
	[SerializeField] Color damageFlashColor;
	[SerializeField] MeshRenderer[] damageFlashMeshes;
	Color[] originalColors;

	protected override void Awake()
	{
		base.Start ();
		originalColors = new Color[damageFlashMeshes.Length];
		for (int i = 0; i > damageFlashMeshes.Length; i++)
		{
			originalColors [i] = damageFlashMeshes [i].material.color;
		}
	}

	public override float TakeDamage (DamageInfo damagePackage) //Causes enemies to flash the same color when taking damage.
	{
		StartCoroutine ("damageFlash");
		return base.TakeDamage (damagePackage);
	}

	IEnumerator damageFlash () //Flash when taking damage.
	{
		if (damageFlashMeshes.Length > 0)
		{
			for (int i = 0; i < damageFlashMeshes.Length; i++)
			{
				damageFlashMeshes [i].material.color = damageFlashColor;
			}

			yield return new WaitForSeconds (0.1f);

			for (int i = 0; i < damageFlashMeshes.Length; i++)
			{
				damageFlashMeshes [i].material.color = originalColors [i];
			}

		}
	}*/
}
