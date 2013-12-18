using UnityEngine;
using System.Collections.Generic;

public class EventSpawnEnemies : MonoBehaviour
{
	public enum SPAWN_STATE
	{
		ALLOW_SPAWN,
		CANCEL_SPAWN
	}
	
	// the key whether this block can produce any enemy any further
	public SPAWN_STATE mSpawnState;
	// the maximum number of enemy can be produce by this block
	public int mMaxSpawnNum = 1;
	// the type of enemy prefab available to spawn
	List<GameObject> mEnemyListPrefab = new List<GameObject>();
	
	// used by the function that spawn the non aggro enemy
	float mSpawnTimer;
	public float mSpawnDuration = 1.0f;
	
	SpawnManager mSpawnManager;
	
	// the list of spawn location in current block
	List<GameObject> mSpawnLocationList = new List<GameObject>();
	// the environment layer to prevent spawning in between environmental objects like wall, props, etc
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
		// compare with the list of enemy types count with this instance enemy type list
		// if the count is the same means it's updated
		// if the count of the list in the parameter is higher means this instance list is outdated
		if(enemyList.Count == mEnemyListPrefab.Count && mEnemyListPrefab.Count > 0)
		{
			return;
		}
		// update the list
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
		
			pos = new Vector3(randX, 5.0f, randZ);
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
	
	//! Spawn non aggro enemy (this function will called in an update function!)
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
			
			// infinite counter check
			int counter = 0;
			// counter to tell whether the location can be spawn
			bool validSpawn = false;
			//! do spawn here
			int rand = Random.Range(0, mEnemyListPrefab.Count);
			
			float colliderRad = mEnemyListPrefab[rand].GetComponent<CharacterController>().radius;
			
			//! in a block there could be 1 or more gameobject with a script SpawnLocation(enemy) script attached 
			//! get the spawning location, will randomize if have more than 2 spawning location
			int spawnRand = Random.Range(0, mSpawnLocationList.Count);
			Vector3 min = mSpawnLocationList[spawnRand].collider.bounds.min;
			Vector3 max = mSpawnLocationList[spawnRand].collider.bounds.max;
				
			Vector3 pos = Vector3.zero;
			
			//! to check whether the spawn area is applicable
			while(!validSpawn)
			{
				// random the area of the block to see there is a space to spawn based on the size of the enemy
				float randX = Random.Range(min.x,max.x);
				float randZ = Random.Range(min.z,max.z);
				
				pos = new Vector3(randX, 5.0f, randZ);
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
			
			// instantiate the obj
			mSpawnManager.SpawnEnemy(mEnemyListPrefab[rand],pos,Quaternion.identity);
			mSpawnTimer = 0.0f;
			mMaxSpawnNum -= 1;
		}
	}
	
}
