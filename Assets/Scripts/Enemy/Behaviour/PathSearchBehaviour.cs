using UnityEngine;
using System.Collections.Generic;

class PathSearchBehaviourData
{
	public float mMemoryTimer;
}

public class PathSearchBehaviour : BehaviourBase
{
	public float mMemoryDuration;
	public float mPathSearchDist;
	public float mMinDist = 1.5f;
	float mMinDistSqr;
	float mPathSearchDistSqr;
	
	public override void Init (EnemyBase enemyBase)
	{
		PathSearchBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new PathSearchBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (PathSearchBehaviourData)enemyBase.mCustomData[this];
		}
		data.mMemoryTimer = mMemoryDuration;
		mMinDistSqr = mMinDist * mMinDist;
		mPathSearchDistSqr = mPathSearchDist * mPathSearchDist;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		PathSearchBehaviourData data = (PathSearchBehaviourData)enemyBase.mCustomData[this];
		float colliderRad = enemyBase.charController.radius;
		
		//! haven't see any player
		if(enemyBase.mTargetPlayer == null)
		{
			return Vector3.zero;
		}
		
		data.mMemoryTimer += Time.deltaTime;
		
		//! the time to check a path
		if(data.mMemoryTimer >= mMemoryDuration)
		{
			Vector3 targetPos = enemyBase.mTargetPlayer.transform.position;
			int colliderSize =(int)Mathf.Round(enemyBase.charController.radius * 2);
			//! get distance between player and self 
			float distance = (targetPos - enemyBase.transform.position).sqrMagnitude;
			
			if(distance < mPathSearchDistSqr)
			{
				//! use small nodes(only close by)(not using clearance)
				enemyBase.mPath = EventMap.GetPathAi(enemyBase.transform.position,targetPos,
					EventMap.sSmallAiNodes,0,EventMap.AI_NODE_TYPE.SMALL);
			}
			else
			{
				//! use big nodes(big nodes have less node to check, improve performance)
				enemyBase.mPath = EventMap.GetPathAi(enemyBase.transform.position,targetPos,
					EventMap.sBigAiNodes,0,EventMap.AI_NODE_TYPE.BIG);
			}
			data.mMemoryTimer = 0.0f;
		}
		
		Vector3 targetDir = Vector3.zero;
		
		if(enemyBase.mPath.Count > 0)
		{
			targetDir = enemyBase.mPath[enemyBase.mPath.Count - 1] - enemyBase.transform.position;
			
			//! if reach the point
			if(targetDir.sqrMagnitude < mMinDistSqr + colliderRad)
			{
				enemyBase.mPath.RemoveAt(enemyBase.mPath.Count - 1);
			}
		}
		
		return targetDir;
	}
}
