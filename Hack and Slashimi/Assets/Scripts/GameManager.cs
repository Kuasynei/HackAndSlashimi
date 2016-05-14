using UnityEngine;
using System.Collections;

public static class GameManager {
	static GameObject playerReference;
	static GameObject pColossusReference;

	public static void SetPlayer (GameObject playerGameobject)
	{
		playerReference = playerGameobject;
	}

	public static GameObject GetPlayer()
	{
		return playerReference;
	}

	public static void SetPColossus (GameObject colossusGameobject)
	{
		pColossusReference = colossusGameobject;
	}

	public static GameObject GetPColossus()
	{
		return pColossusReference;
	}
}
