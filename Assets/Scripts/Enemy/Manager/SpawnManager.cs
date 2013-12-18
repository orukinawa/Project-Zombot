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
	
	public List<GameObject> StateMachines = new List<GameObject>();
	
	void Awake()
	{
		InitStateMachines();
	}
	
	void InitStateMachines()
	{
		GameObject StateMachineObj = new GameObject("StateMachines");
		for(int i = 0; i < mSpawnDictionary.Length; i++)
		{
			GameObject go = SpawnStateMachine(mSpawnDictionary[i].mStateMachine);
			StateMachines.Add(go);
			go.transform.parent = StateMachineObj.transform;
		}
	}
	
	//! spawn enemy based on the given prefab
	public GameObject SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		//! check for the state machine and spawn it
		for(int i = 0; i < mSpawnDictionary.Length; i++)
		{
			if(prefab == mSpawnDictionary[i].mEnemyPrefab)
			{
				//! spawn the mStateMachine
				//GameObject stateMachine = SpawnStateMachine(mSpawnDictionary[i].mStateMachine);
				
				GameObject stateMachine = StateMachines[i];
				GameObject enemyObj =(GameObject)PoolManager.pools["Enemy Pool"].Spawn(prefab, position, rotation);
				//! Spawn enemy with idle state
				BehaviourManager behaviourManager = enemyObj.GetComponent<BehaviourManager>();
				behaviourManager.ResetStateMachine();
				behaviourManager.mStartState = GetEnemyState("SPAWN",stateMachine);
				behaviourManager.RequestState(Transition.MODE.PUSH,behaviourManager.mStartState);
				
				return enemyObj;
			}
		}
		
		Debug.LogWarning("[SpawnManager]Cannot spawnEnemy!!");
		return null;
	}
	
	//! spawn enemy with target given and state
	public GameObject SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation, GameObject target, string state)
	{
		//! check for the state machine and spawn it
		for(int i = 0; i < mSpawnDictionary.Length; i++)
		{
			if(prefab == mSpawnDictionary[i].mEnemyPrefab)
			{
				//! spawn the mStateMachine
				GameObject stateMachine =  StateMachines[i];
				GameObject enemyObj =(GameObject)PoolManager.pools["Enemy Pool"].Spawn(prefab, position, rotation);
				enemyObj.GetComponent<EnemyBase>().mTargetPlayer = target;
				//! Spawn enemy with idle state
				BehaviourManager behaviourManager = enemyObj.GetComponent<BehaviourManager>();
				behaviourManager.ResetStateMachine();
				behaviourManager.mStartState = GetEnemyState(state,stateMachine);
				behaviourManager.RequestState(Transition.MODE.PUSH,behaviourManager.mStartState);
				return enemyObj;
			}
		}
		
		Debug.LogWarning("[SpawnManager]Cannot spawnEnemy!!");
		return null;
	} 
	
	BehaviourState GetEnemyState(string stateName, GameObject stateMachine)
	{
		BehaviourState[] states = stateMachine.GetComponentsInChildren<BehaviourState>();
		BehaviourState defaultState = null;
			
		foreach(BehaviourState state in states)
		{
			// just cache the default state
			if(state.mStateName == "SPAWN")
			{
				defaultState = state;
			}
			
			if(stateName == state.mStateName)
			{
				return state;
			}
		}
		
		// if there isn't such request state found
		Debug.LogWarning("No such state found for the particular spawn!");
		
		return defaultState;
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
