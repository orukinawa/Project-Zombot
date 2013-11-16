using UnityEngine;
using System.Collections;

public class SpawnLocation : MonoBehaviour
{
	public enum TYPE
	{
		PLAYER,
		ENEMY,
		ENVIRONMENT,
	}
	
	public TYPE mSpawnType; 
	
}
