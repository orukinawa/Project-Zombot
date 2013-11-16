using UnityEngine;
using System.Collections;

class PushFromWallBehaviourData
{
	public float mMemoryTimer;
	public Vector3 mMemoryDir;
}

public class PushFromWallBehaviour : BehaviourBase
{
	public LayerMask mWallLayer;
	public float mDistToWall = 0.5f;
	public float mMemoryDuration = 0.5f;
	float mColliderRad;
	Ray mRay;
	
	public override void Init (EnemyBase enemyBase)
	{
		PushFromWallBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new PushFromWallBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data =(PushFromWallBehaviourData)enemyBase.mCustomData[this];
		}
		data.mMemoryTimer = mMemoryDuration;
		data.mMemoryDir = Vector3.zero;
		mColliderRad = enemyBase.charController.radius;
	}
	
	Vector3 GetVectorByAngle(float degreeAngle, Vector3 beginDirection)
	{
		float radian = degreeAngle * Mathf.Deg2Rad;
		float xValue = beginDirection.x * Mathf.Cos(radian) + beginDirection.z * Mathf.Sin(radian);
		float zValue = -(beginDirection.x * Mathf.Sin(radian)) + beginDirection.z * Mathf.Cos (radian);
		
		Vector3 newDir = new Vector3(xValue,0.0f,zValue);
		
		return newDir;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		PushFromWallBehaviourData data = (PushFromWallBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		Vector3 frontDir = enemyBase.transform.forward;
		Vector3 rightDir = enemyBase.transform.right;
		float distance = mColliderRad + mDistToWall;
		
		Collider[] colliders = Physics.OverlapSphere(enemyBase.transform.position,distance,mWallLayer);
		
		Vector3 targetDir = Vector3.zero;
		
		if(colliders.Length > 0 )
		{
			data.mMemoryTimer += Time.deltaTime;
			if(data.mMemoryTimer < mMemoryDuration)
			{
				return data.mMemoryDir;
			}
			else
			{
				RaycastHit hit;
				//! 1 means right 2 means left
				int hitflag = 0;
				mRay.origin = enemyBase.transform.position;
				
				//! cast right
				mRay.direction = rightDir;
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				//! cast front right
				mRay.direction = GetVectorByAngle(-45.0f,rightDir);
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				//! cast behind right
				mRay.direction = GetVectorByAngle(45.0f,rightDir);
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				//! cast left
				mRay.direction = GetVectorByAngle(45.0f,-rightDir);
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				//! cast front left
				mRay.direction = GetVectorByAngle(-45.0f,-rightDir);
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				//! cast btm right
				mRay.direction = GetVectorByAngle(45.0f,-rightDir);
				if(Physics.Raycast(mRay,out hit,distance,mWallLayer))
				{
					targetDir += pos - hit.point;
				}
				
				data.mMemoryDir = targetDir;
				data.mMemoryTimer = 0.0f;
			}
			
//			//! hit right
//			if(hitflag == 1)
//			{
//				//! go left
//				targetDir += -rightDir;
//			}
//			//! hit left
//			if(hitflag == 2)
//			{
//				//! go right
//				targetDir += rightDir;
//			}
		}
//		Debug.DrawRay(pos,targetDir * 3.0f,Color.cyan);
//		Debug.DrawRay(enemyBase.transform.position,GetVectorByAngle(45.0f,enemyBase.transform.right) * distance, Color.yellow);
//		Debug.DrawRay(enemyBase.transform.position,GetVectorByAngle(-45.0f,enemyBase.transform.right) * distance, Color.yellow);
//		Debug.DrawRay(enemyBase.transform.position,GetVectorByAngle(45.0f,-enemyBase.transform.right) * distance, Color.yellow);
//		Debug.DrawRay(enemyBase.transform.position,GetVectorByAngle(-45.0f,-enemyBase.transform.right) * distance, Color.yellow);
//		Debug.DrawRay(enemyBase.transform.position,enemyBase.transform.right * distance, Color.yellow);
//		Debug.DrawRay(enemyBase.transform.position,-enemyBase.transform.right * distance, Color.yellow);
		//Debug.Log(normalDir);
		return targetDir;
	}
}
