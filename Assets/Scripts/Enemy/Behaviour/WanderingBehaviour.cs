using UnityEngine;
using System.Collections;

public class WanderingBehaviourData
{
	public Vector3 mTarget;
	public float mPauseTimer;
}

public class WanderingBehaviour : BehaviourBase
{
	public float mPauseDuration;
	public float mMinDistance;
	public float mMaxDistance;
	public float mRandomAngle;
	public LayerMask mAllyLayer;
	public LayerMask mWallLayer;
	float mMinDistanceSqr;
	
	public override void Init (EnemyBase enemyBase)
	{
		WanderingBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new WanderingBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (WanderingBehaviourData)enemyBase.mCustomData[this];
		}
		data.mPauseTimer = mPauseDuration;
		data.mTarget = Vector3.zero;
		
		mMinDistanceSqr = mMinDistance * mMinDistance;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		WanderingBehaviourData data = (WanderingBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		Vector3 dir = enemyBase.transform.forward;
		float rad = enemyBase.charController.radius;
		
		data.mPauseTimer += Time.deltaTime;
		
		if(data.mPauseTimer >= mPauseDuration)
		{
			//! prepare to change course
			float randomAngle = Random.Range(-mRandomAngle, mRandomAngle);
			Quaternion randomRotate = Quaternion.AngleAxis(randomAngle, Vector3.up);
			Vector3 newTargetVector = randomRotate * dir;
			
			RaycastHit hit;
			
			if(Physics.Raycast(pos,newTargetVector,out hit,mMaxDistance,mWallLayer))
			{
				//! if the goal is against the wall
				data.mTarget = hit.point + (hit.normal * 2.0f);
			}
			else
			{
				data.mTarget = pos + newTargetVector * mMaxDistance;
			}
			
			data.mPauseTimer = 0.0f;
		}
		
		if(rad < 1.0f)
		{
			rad = 1.0f;
		}
		
		float sqrDist = (data.mTarget - pos).sqrMagnitude;
		
		Collider[] colliders = Physics.OverlapSphere(pos,rad,mAllyLayer);
		
		//! reach destination
		if(sqrDist < mMinDistanceSqr || colliders.Length > 1)
		{
			//Debug.Log("LOGHA");
			data.mTarget = Vector3.zero;
			return data.mTarget;
		}
		
		Vector3 result = Vector3.zero;
		
		//Debug.DrawRay(pos,(data.mTarget - pos),Color.blue);
		
		if(data.mTarget != Vector3.zero)
		{
			result = data.mTarget - pos;
		}
		return result;
	}
}
