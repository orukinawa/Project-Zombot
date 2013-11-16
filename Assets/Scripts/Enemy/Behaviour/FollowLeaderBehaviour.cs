using UnityEngine;
using System.Collections;

class FollowLeaderBehaviourData
{
	//! the leader as target
	public GameObject mLeaderTarget;
}

public class FollowLeaderBehaviour : BehaviourBase
{
	//! checks for ally
	public LayerMask mAllyLayer;
	public float mInfluenceRadius = 3;
	public float mInfluenceAngle = 90.0f;
	//! the viewing angle of the follower towards the leader
	public float mViewAngle = 90.0f;
	public float mDistFromLeader = 1;
	//! connect the state which uses pathfinding via inspector
	public State mSearchState;
	
	public float mMinDist = 1.5f;
	float mMinDistSqr;
	public override void Init (EnemyBase enemyBase)
	{
		FollowLeaderBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new FollowLeaderBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (FollowLeaderBehaviourData)enemyBase.mCustomData[this];
		}
		data.mLeaderTarget = null;
		mMinDistSqr = mMinDist * mMinDist;
	}
	
	bool IsLeaderOnSight(GameObject leader, Transform trans) 
	{
		Vector3 targetDir = leader.transform.position - trans.position;
		float angle = Vector3.Angle(trans.forward, targetDir);
		
		if(Physics.CheckSphere(trans.position, mInfluenceRadius))
		{
			return true;
		}
		return false;
	}
	
	bool SearchForLeader(Collider[] colliders, Vector3 position, FollowLeaderBehaviourData data)
	{
		foreach(Collider col in colliders)
		{
			//! found a leader
			if(col.GetComponent<BehaviourManager>().mCurrentState == mSearchState)
			{
				Vector3 targetDir = position - col.transform.position;
				float angle = Vector3.Angle(-col.transform.forward, targetDir); 
				//! you're behind the leader backwards angle
				if(angle < mInfluenceAngle)
				{
					data.mLeaderTarget = col.gameObject;
					return true;
				}
			}
		}
		return false;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		FollowLeaderBehaviourData data = (FollowLeaderBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		float colliderRad = enemyBase.charController.radius;
		Collider[] colliders = Physics.OverlapSphere(pos,mInfluenceRadius,mAllyLayer);
		
		Vector3 result = Vector3.zero;
		
		if(data.mLeaderTarget == null)
		{
			if(!SearchForLeader(colliders,enemyBase.transform.position,data))
			{
				//! when no leader is found you path search your own and act as a leader
				ExecuteTransition(enemyBase);
			}
		}
		else
		{
			if(IsLeaderOnSight(data.mLeaderTarget,enemyBase.transform))
			{
				result = data.mLeaderTarget.transform.position - pos;
				if(result.sqrMagnitude < mMinDistSqr + colliderRad)	
				{
					result = Vector3.zero;
				}
			}
			else
			{
				ExecuteTransition(enemyBase);
			}
		}
		return result;
	}
}
