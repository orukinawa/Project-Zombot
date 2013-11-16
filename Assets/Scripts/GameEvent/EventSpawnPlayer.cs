using UnityEngine;
using System.Collections;

public class EventSpawnPlayer : MonoBehaviour 
{
	public GameObject mEffectSpawn;
	public GameObject mSpawnerLocation;
	
	//! spawn the location
	public void SpawnPlayer(GameObject prefab, EventManager evtManager) 
	{
		//Debug.Log(mSpawnerLocation);
		Vector3 pos = mSpawnerLocation.transform.position;
		GameObject obj = (GameObject)Instantiate(prefab,pos,Quaternion.identity);
		PlayerData data = new PlayerData();
		data.mPlayerObj = obj;
		evtManager.mPlayerInGame.Add(data);
	}
}
