using UnityEngine;
using System.Collections;

public class ChargingBehaviourData
{
	public float mChargingTimer;
	public bool mIsCharging;
	public GameObject mPlayerObj;
	public GameObject mCapturedPlayer;
	public float mPlayerDefaultMoveSpd;
}

public class ChargingBehaviour : BehaviourBase
{
	public float mChargingSpd;
	
	//! the initial charge delay
	public float mChargingDelay;
	//! the angle to allow player capture
	public float mCaptureAngle;
	public LayerMask mTargetLayer;
	public LayerMask mEnvironmentLayer;
	float mCaptureDistSqr;
	public GameObject mCrashEffect;

	
	
	public override void Init (EnemyBase enemyBase)
	{
		ChargingBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new ChargingBehaviourData();
			data.mChargingTimer = 0.0f;
			data.mIsCharging = false;
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data =(ChargingBehaviourData)enemyBase.mCustomData[this];
		}
		data.mChargingTimer = 0.0f;
		data.mIsCharging = false;
		data.mPlayerObj = null;
		
		mCaptureDistSqr = enemyBase.charController.radius * enemyBase.charController.radius;
	}
	
//	//! to prevent ANY behaviour taking place
//	public void RemoveAllBehaviours(EnemyBase enemyBase)
//	{
//		BehaviourBase[] behaviours = enemyBase.mBehaviourList;
//		for(int i = 0; i < behaviours.Length; i++)
//		{
//			if(behaviours[i] == this)continue;
//			behaviours[i] = null;
//		}
//	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		ChargingBehaviourData data = (ChargingBehaviourData)enemyBase.mCustomData[this];
		float attackRad = enemyBase.mAttackRadius;
		Vector3 pos = enemyBase.transform.position;
		float colliderRadius = enemyBase.charController.radius;
		RaycastHit hit;
		
		//! get the nearest player object information
		if(data.mPlayerObj == null)
		{
			data.mPlayerObj = GetNearestToTarget(pos,attackRad,mTargetLayer);
		}	
		
		//! charging!!
		if(!data.mIsCharging && data.mPlayerObj != null)
		{
			Vector3 targetDirection = data.mPlayerObj.transform.position - pos;
			targetDirection.y = 0.0f;
			data.mChargingTimer += Time.deltaTime;
			//! rotate to aim the charging move
			float targetAngle = GetAngleHelper.GetAngle(targetDirection,enemyBase.transform.forward,enemyBase.transform.up);
			if(Mathf.Abs(targetAngle) > 3.0f)
			{
				//enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(targetDirection), enemyBase.mSteeringForce * Time.deltaTime);
				enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
			}
			//! play prepare to charge animation here
			
			if(data.mChargingTimer >= mChargingDelay)data.mIsCharging = true;
		}
		//! CHARGE!!!!
		else if(data.mIsCharging)
		{
			Vector3 velocity = enemyBase.transform.forward * enemyBase.mCurrSpeed * mChargingSpd;
			enemyBase.charController.SimpleMove(velocity);
			//Debug.Log("CHARGING");
			
			//! set player to immobolize and move along with enemy's forward
			if(data.mCapturedPlayer == null)
			{
				Collider[] colliders = Physics.OverlapSphere(pos,colliderRadius * 2,mTargetLayer);
				//Debug.Log("length: " + colliders.Length);
				foreach(Collider collider in colliders)
				{
					CaptureTarget(collider, enemyBase.gameObject, data);
					break;
				}
			}
			else
			{
				//! update the player position with enemy speed
				//data.mCapturedPlayer.transform.parent = enemyBase.transform;
				
				//! to make sure that the charging keeps player a certain distance from him
				float currDist = (data.mCapturedPlayer.transform.position - pos).sqrMagnitude;
				
				if(currDist < mCaptureDistSqr)
				{
					data.mCapturedPlayer.GetComponent<CharacterController>().SimpleMove(velocity);
				}
			}
			
			//! if hit against the wall
			if(Physics.Raycast(pos,enemyBase.transform.forward,out hit,4.0f,mEnvironmentLayer))
			{
				data.mPlayerObj = null;
				data.mChargingTimer = 0.0f;
				data.mIsCharging = false;
				
				//! reset after player movement capabilities after hitting the wall
				if(data.mCapturedPlayer != null)
				{
					Instantiate(mCrashEffect,data.mCapturedPlayer.transform.position,Quaternion.identity);
					//Vector3 reverseDir = (enemyBase.transform.position - data.mCapturedPlayer.transform.position).normalized;
					data.mCapturedPlayer.GetComponent<StatsCharacter>().currentHealth -= 20;
					//enemyBase.charController.Move(reverseDir);
					ReleaseTarget(data, enemyBase.gameObject);
				}
				//Debug.Log("END OF CHARGE");
				ExecuteTransition(enemyBase);
			}
		}
		return Vector3.zero;
	}
	
	public override void Death (EnemyBase enemyBase)
	{
		ChargingBehaviourData data = (ChargingBehaviourData)enemyBase.mCustomData[this];
		ReleaseTarget(data,enemyBase.gameObject);
	}
	
	//! setting to release the target if in case charger dies
	public void ReleaseTarget(ChargingBehaviourData data, GameObject self)
	{
		if(data.mCapturedPlayer != null)
		{
			Physics.IgnoreLayerCollision(data.mCapturedPlayer.layer,self.layer,false);
			data.mCapturedPlayer.GetComponent<StatsCharacter>().RestoreMoveSpeed();
			data.mCapturedPlayer = null;
		}
	}
	
	//! please use this player as target only
	public void CaptureTarget(Collider player, GameObject self, ChargingBehaviourData data)
	{
		Vector3 selfDir = self.transform.forward;
		Vector3 targetDir = Vector3.Normalize(player.transform.position - self.transform.position);
		//float dot = Vector3.Dot(targetDir,selfDir);
		float angle = Vector3.Angle(targetDir,selfDir);
		//! if enemy is in front
		if(angle < mCaptureAngle)
		{
			//! set player to immobolize
			player.GetComponent<StatsCharacter>().ApplySlow(0);
			data.mCapturedPlayer = player.gameObject;
			//! ignore incoming collision of player to enemy when captured
			Physics.IgnoreLayerCollision(player.gameObject.layer,self.layer);
		}
	}
}
