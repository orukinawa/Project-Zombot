using UnityEngine;
using System.Collections.Generic;

public class EventSpawnEnemies : MonoBehaviour
{
	public enum SPAWN_STATE
	{
		ALLOW_SPAWN,
		CANCEL_SPAWN
	}
	
	public SPAWN_STATE mSpawnState;
	public int mMaxSpawnNum = 1;
	List<GameObject> mEnemyListPrefab = new List<GameObject>();

	float mSpawnTimer;
	public float mSpawnDuration = 1.0f;
	
	SpawnManager mSpawnManager;
	
	List<GameObject> mSpawnLocationList = new List<GameObject>();
	
	LayerMask mWallLayer;
	
	void Awake()
	{
		mSpawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
		mWallLayer = 1 << LayerMask.NameToLayer("Environment");
		SpawnLocation[] tempSpawnList;
		tempSpawnList = gameObject.GetComponentsInChildren<SpawnLocation>();
		
		for(int i = 0; i < tempSpawnList.Length; i++)
		{
			if(tempSpawnList[i].mSpawnType == SpawnLocation.TYPE.ENEMY)	
			{
				mSpawnLocationList.Add(tempSpawnList[i].gameObject);
			}
		}
			
	}
	
	//! update  the range of enemy to spawn
	public void UpdatePrefabList(List<GameObject> enemyList)
	{
		//! if the list recently updated, don't update it again
		if(enemyList.Count == mEnemyListPrefab.Count && mEnemyListPrefab.Count > 0)
		{
			return;
		}
		mEnemyListPrefab = enemyList;
	}
	
	//! for the wave type
	public void SpawnEnemy(string state, GameObject target)
	{
		int counter = 0;
		bool validSpawn = false;
		//! scale to only lesser zombot and spitter
		int rand = 0;
		//! do spawn here
		if(mEnemyListPrefab.Count > 1)
		{
			rand = Random.Range(0,2);
		}
		float colliderRad = mEnemyListPrefab[rand].GetComponent<CharacterController>().radius;
		
		//! get the spawning location
		int spawnRand = Random.Range(0, mSpawnLocationList.Count);
		Vector3 min = mSpawnLocationList[spawnRand].collider.bounds.min;
		Vector3 max = mSpawnLocationList[spawnRand].collider.bounds.max;
			
		Vector3 pos = Vector3.zero;
		
		//! to check whether the spawn area is applicable
		while(!validSpawn)
		{
			float randX = Random.Range(min.x,max.x);
			float randZ = Random.Range(min.z,max.z);
		
			pos = new Vector3(randX, 2.0f, randZ);
			if(!Physics.CheckSphere(pos,colliderRad,mWallLayer))
			{
				validSpawn = true;
			}
			
			counter++;
			if(counter > 1000)
			{
				Debug.LogError("InfiniteLoop");
				break;
			}
		}
		//! spawn enemy
		mSpawnManager.SpawnEnemy(mEnemyListPrefab[rand],pos,Quaternion.identity,target,state);
	}
	
	//! calls per frame
	public void SpawnEnemy()
	{
		if(mMaxSpawnNum > 0)
		{
			mSpawnTimer += Time.deltaTime;
		}
		else
		{
			//! disallow any spawning because it met it's spawning quota
			mSpawnState = SPAWN_STATE.CANCEL_SPAWN;
		}
		
		if(mSpawnTimer >= mSpawnDuration)
		{
			//! if the list is not update yet
			if(mEnemyListPrefab.Count <= 0)return;
			
			int counter = 0;
			bool validSpawn = false;
			//! do spawn here
			int rand = Random.Range(0, mEnemyListPrefab.Count);
			
			float colliderRad = mEnemyListPrefab[rand].GetComponent<CharacterController>().radius;
			
			//! get the spawning location
			int spawnRand = Random.Range(0, mSpawnLocationList.Count);
			Vector3 min = mSpawnLocationList[spawnRand].collider.bounds.min;
			Vector3 max = mSpawnLocationList[spawnRand].collider.bounds.max;
				
			Vector3 pos = Vector3.zero;
			
			//! to check whether the spawn area is applicable
			while(!validSpawn)
			{
				float randX = Random.Range(min.x,max.x);
				float randZ = Random.Range(min.z,max.z);
			
				pos = new Vector3(randX, 2.0f, randZ);
				if(!Physics.CheckSphere(pos,colliderRad,mWallLayer))
				{
					validSpawn = true;
				}
				
				counter++;
				if(counter > 1000)
				{
					Debug.LogError("InfiniteLoop");
					break;
				}
			}
			
			mSpawnManager.SpawnEnemy(mEnemyListPrefab[rand],pos,Quaternion.identity);
			mSpawnTimer = 0.0f;
			mMaxSpawnNum -= 1;
		}
	}
	
}
