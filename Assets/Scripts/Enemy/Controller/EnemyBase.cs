using UnityEngine;
using System.Collections.Generic;

abstract public class EnemyBase : MonoBehaviour
{
	//! a prefab that will be instantiated when spawning
	public GameObject mSpawnAnimationPrefab;
	//! a prefab that will be instantiated upon death
	public GameObject mDeathAnimationPrefab;
	
	//! behaviour containers
	public BehaviourBase[] mBehaviourList;
	public Dictionary<object, object> mCustomData = new Dictionary<object, object>();
	
	//! the turning force of the enemy
	public float mSteeringForce;
	
	//! EnemyStats like health and movespeed
	public StatsEnemy mStatEnemy;
	
	//! preset radius checking of the enemy
	public float mDetectionRadius;
	public float mAttackRadius;
	
	//! the path for pathfinding
	public List<Vector3> mPath = new List<Vector3>();
	//! keeps track of player that was targeted
	public GameObject mTargetPlayer;
	//! cache the child transform (the mesh)
	Transform mChildTransform;
	
	public float mMaxSpeed;
	public float mCurrSpeed;
	
	//! for now
	[HideInInspector]
	public CharacterController charController;
	
	void Awake()
	{
		charController = GetComponent<CharacterController>();
		mStatEnemy = GetComponent<StatsEnemy>();
		mChildTransform = GetComponentInChildren<Animation>().transform;
		//mCurrSpeed = mMaxSpeed;
	}
	
	public void InstantiateDeathAni()
	{
		if(mDeathAnimationPrefab == null)return;
		GameObject obj = (GameObject)PoolManager.pools["EnemyVisualPool"].Spawn(mDeathAnimationPrefab,transform.position,transform.rotation);
		obj.transform.localScale = mChildTransform.localScale;
	}
	
	public void InstantiateSpawnAni()
	{
		
	}
	
	//! call all the death function in all the current listed behaviours of this enemy
	public void ActivateDeath()
	{
		foreach(BehaviourBase behaviourBase in mBehaviourList)
		{
			behaviourBase.Death(this);
		}
	}
	
	public virtual void RefreshBehaviour(BehaviourState state)
	{
		//Replace the behavior list based on the current state
		//Debug.Log("RefreshBehaviour");
		mBehaviourList = state.mBehaviourList;
		foreach (BehaviourBase behaviourBase in mBehaviourList)
		{
			behaviourBase.Init(this);
		}
	}
	
	public BehaviourManager GetBehaviourManager()
	{
		BehaviourManager manager = GetComponent<BehaviourManager>();
		return manager;
	}
	
	public virtual void GetFinalSteerDirection()
	{
		Vector3 resultantDirection = Vector3.zero;
		
		foreach(BehaviourBase behaviourBase in mBehaviourList)
		{
			if(behaviourBase == null)continue;
			
			resultantDirection += behaviourBase.UpdateBehaviour(this).normalized * behaviourBase.mInfluenceWeight;
		}
		
		
		//! debug the front of the obj(!REMOVE)
		//Debug.DrawRay(transform.position, transform.forward * 3.0f, Color.blue);
		
		Steer(resultantDirection);
	}
	
	public abstract void Steer(Vector3 direction);
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Debug.LogError("The enemy path: " + path.Count);
		for(int i = 0; i < mPath.Count - 1; i++)
		{
			//Gizmos.DrawIcon(mPath[i],"BullEye_Icon");
			Gizmos.DrawLine(mPath[i], mPath[i + 1]);
		}
	}
}
