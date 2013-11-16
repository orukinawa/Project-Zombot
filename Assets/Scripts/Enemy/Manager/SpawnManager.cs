using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnDictionary
{
	public GameObject mEnemyPrefab;
	public GameObject mStateMachine;
}

public class SpawnManager : MonoBehaviour 
{
	public SpawnDictionary[] mSpawnDictionary = new SpawnDictionary[0];
	 
	//! spawn enemy based on the given prefab
	public void SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		//! check for the state machine and spawn it
		for(int i = 0; i < mSpawnDictionary.Length; i++)
		{
			if(prefab == mSpawnDictionary[i].mEnemyPrefab)
			{
				//! spawn the mStateMachine
				GameObject stateMachine = SpawnStateMachine(mSpawnDictionary[i].mStateMachine);
				GameObject enemyObj =(GameObject)PoolManager.pools["Enemy Pool"].Spawn(prefab, position, rotation);
				//! Spawn enemy with idle state
				enemyObj.GetComponent<BehaviourManager>().mStartState = GetEnemyState("IDLE",stateMachine);
				enemyObj.GetComponent<BehaviourManager>().mCurrentState = GetEnemyState("IDLE",stateMachine);
			}
		}
	}
	
	//! spawn enemy with target given and state
	public void SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation, GameObject target, string state)
	{
		//! check for the state machine and spawn it
		for(int i = 0; i < mSpawnDictionary.Length; i++)
		{
			if(prefab == mSpawnDictionary[i].mEnemyPrefab)
			{
				//! spawn the mStateMachine
				GameObject stateMachine = SpawnStateMachine(mSpawnDictionary[i].mStateMachine);
				GameObject enemyObj =(GameObject)PoolManager.pools["Enemy Pool"].Spawn(prefab, position, rotation);
				enemyObj.GetComponent<EnemyBase>().mTargetPlayer = target;
				//! Spawn enemy with idle state
				enemyObj.GetComponent<BehaviourManager>().mStartState = GetEnemyState(state,stateMachine);
				enemyObj.GetComponent<BehaviourManager>().mCurrentState = GetEnemyState(state,stateMachine);
			}
		}
	} 
	
	BehaviourState GetEnemyState(string stateName, GameObject stateMachine)
	{
		BehaviourState[] states = stateMachine.GetComponentsInChildren<BehaviourState>();
		
		foreach(BehaviourState state in states)
		{
			if(stateName == state.mStateName)
			{
				return state;
			}
		}
		return null;
	}
	
	GameObject SpawnStateMachine(GameObject stateMachinePrefab)
	{
		//! check in the scene if the stateMachine is spawn before
		GameObject obj = GameObject.FindGameObjectWithTag(stateMachinePrefab.tag);
		if(obj == null)
		{
			obj = (GameObject)Instantiate(stateMachinePrefab);
			return obj;
		}
		//Debug.LogWarning("State Machine was manually instatiated via editor! Please refrain from doing that unless for test purposes");
		return obj;
	}
}
