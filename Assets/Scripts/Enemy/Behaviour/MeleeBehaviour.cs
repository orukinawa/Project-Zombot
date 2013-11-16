using UnityEngine;
using System.Collections;

public class MeleeBehaviourData
{
	public float mAttackTimer;
}

////! it must be below of the other class, apparent it reads require component above the class only
////! BUGGY DON USE, causes the inspector to BUG!
//[RequireComponent(typeof(SeekBehaviour))]

public class MeleeBehaviour : BehaviourBase
{
	public float mAttackAngle;
	public float mDistanceAtk;
	public float mAtkDelay;
	
	public GameObject mHitEffect;
	
	public override void Init (EnemyBase enemyBase)
	{
		MeleeBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new MeleeBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data =(MeleeBehaviourData)enemyBase.mCustomData[this];
		}
		data.mAttackTimer = mAtkDelay;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		MeleeBehaviourData data = (MeleeBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		Vector3 dir = enemyBase.transform.forward;
		GameObject player = enemyBase.mTargetPlayer;
		
		if(player == null)
		{
			Debug.LogWarning("Melee behaviour didn't get the player's refence");
			return Vector3.zero;
		}
		
		Vector3 targetDir = player.transform.position - pos;
		float angle = Vector3.Angle(dir, targetDir);
		
		data.mAttackTimer += Time.deltaTime;
		
		if(data.mAttackTimer > mAtkDelay)
		{
			if(angle < mAttackAngle)
			{
				if(targetDir.sqrMagnitude < mDistanceAtk * mDistanceAtk)
				{
					//! do attack animation here
					//! do weird shyt to player here
					if(mHitEffect != null)
					{
						Instantiate(mHitEffect,player.transform.position,Quaternion.identity);
					}
					player.GetComponent<StatsCharacter>().currentHealth -= 10;
				}
			}
			data.mAttackTimer = 0.0f;
		}
		
		return Vector3.zero;
	}
}
