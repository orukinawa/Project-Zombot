using UnityEngine;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
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
	
	//! Animation controller
	AnimationController mAnimationController;
	
	public Renderer[] mMeshRenderers;
	
	//! for now
	[HideInInspector]
	public CharacterController charController;
	
	void Awake()
	{
		charController = GetComponent<CharacterController>();
		mStatEnemy = GetComponent<StatsEnemy>();
		mChildTransform = GetComponentInChildren<Animation>().transform;
		mAnimationController = GetComponent<AnimationController>();
		mMeshRenderers = GetComponentsInChildren<Renderer>();
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
	
	public AnimationController Animator{
		get{	
			return mAnimationController;	
		}
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
	
	public virtual void UpdateBehaviours()
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
	
	public void Steer(Vector3 resultantVector)
	{
		Vector3 targetVelocity;
		float maxSpeed = mCurrSpeed;
		targetVelocity = resultantVector.normalized * maxSpeed;
		targetVelocity.y = 0;
		if(targetVelocity.sqrMagnitude > Mathf.Epsilon)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetVelocity),mSteeringForce * Time.deltaTime);
			//transform.position += transform.forward * mMaxSpeed * Time.deltaTime;
			charController.SimpleMove(transform.forward * maxSpeed);
		}
		else
		{
			charController.SimpleMove(Vector3.zero);
		}
	}
	
	void Update()
	{
		UpdateBehaviours();
	}
	
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
