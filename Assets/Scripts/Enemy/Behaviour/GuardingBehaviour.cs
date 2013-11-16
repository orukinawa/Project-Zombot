using UnityEngine;
using System.Collections;

class GuardingBehaviourData
{
	public GameObject mShieldRef;
}

public class GuardingBehaviour : BehaviourBase
{
	//! to spawn the sheild
	public GameObject mShieldPrefab;
	
	public override void Init (EnemyBase enemyBase)
	{
		GuardingBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new GuardingBehaviourData();
			data.mShieldRef = null;
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data =(GuardingBehaviourData)enemyBase.mCustomData[this];
			if(data.mShieldRef != null)
			{
				data.mShieldRef.SetActive(true);
			}
		}
		
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		GuardingBehaviourData data = data =(GuardingBehaviourData)enemyBase.mCustomData[this];
		float radius = enemyBase.charController.radius;
		Vector3 dir = enemyBase.transform.forward;
		
		if(data.mShieldRef == null)
		{
			GameObject gameObj = (GameObject) Instantiate(mShieldPrefab,Vector3.zero,Quaternion.identity);
			float angle = GetAngleHelper.GetAngle(dir, gameObj.transform.forward, Vector3.up);
			//Debug.Log("angle" + angle);
			gameObj.transform.Rotate(new Vector3(0.0f,angle,0.0f));
			gameObj.transform.parent = enemyBase.transform;
			Vector3 shieldLocalPos = Vector3.zero + new Vector3(0.0f,0.0f,radius + 0.3f);
			gameObj.transform.localPosition = shieldLocalPos;
			gameObj.layer = LayerMask.NameToLayer("EnemyShield");
			data.mShieldRef = gameObj;
		}
		Vector3 targetDir = Vector3.zero;
		if(enemyBase.mTargetPlayer == null)
		{
			SearchForNewTarget(enemyBase,enemyBase.mDetectionRadius,1 << LayerMask.NameToLayer("Player"));
		}
		else
		{
			targetDir = enemyBase.mTargetPlayer.transform.position - enemyBase.transform.position;
		}
		return targetDir;
	}
	
	public override void DeInit (EnemyBase enemyBase)
	{
		//Debug.Log("GuardingBehaviour deinit");
		GuardingBehaviourData data = (GuardingBehaviourData)enemyBase.mCustomData[this];
		data.mShieldRef.SetActive(false);
	}
}
