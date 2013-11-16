using UnityEngine;
using System.Collections;

public class InRangeEnemyStats : Transition
{
	public enum CONDITION
	{
		IN_RANGE,
		NOT_IN_RANGE
	}
	
	public enum RADIUS_TYPE
	{
		//! range define by the enemy detection radius (see enemybase)
		DETECTION,
		//! range define by the enemy attack radius (see enemybase)
		ATTACK,
	}
	
	public CONDITION mCondition;
	public RADIUS_TYPE mRadiusType;
	public LayerMask mLayerTarget;
	

	public override bool VerifyTransition (StateManager context)
	{	
		float detectionRadius = 0.0f;
		float detectionRad = context.gameObject.GetComponent<EnemyBase>().mDetectionRadius;
		float attackRad = context.gameObject.GetComponent<EnemyBase>().mAttackRadius;
		//Debug.Log("Transition disable: " + mDisable);
		if(mRadiusType == RADIUS_TYPE.ATTACK)
		{
			detectionRadius = attackRad;
		}
		else if(mRadiusType == RADIUS_TYPE.DETECTION)
		{
			detectionRadius = detectionRad;
		}
		
		
		//Debug.Log("range: " + detectionRadius);
		Collider[] colliders = Physics.OverlapSphere(context.transform.position, detectionRadius,mLayerTarget);
		
		
		if(colliders.Length > 0)
		{
			RaycastHit hit;
			int layer = ~(1 << LayerMask.NameToLayer("Enemy"));
			if(mCondition == CONDITION.IN_RANGE)
			{
				//Debug.DrawLine(context.transform.position,colliders[0].transform.position,Color.yellow);
				//! check if he's not block by obstacle
				if(Physics.Linecast(context.transform.position, colliders[0].transform.position,out hit,layer))
				{
					//! checks on player
					StatsCharacter stat = hit.collider.GetComponent<StatsCharacter>();
					if(stat)
					{
						return true;
					}
					return false;
				}
			}
//			else	
//			{
//				if(Physics.Linecast(context.transform.position, colliders[0].transform.position,out hit,layer))
//				{
//					//! checks on player
//					StatsCharacter stat = hit.collider.GetComponent<StatsCharacter>();
//					if(stat)
//					{
//						return false;
//					}
//					return true;
//				}
//			}
		}
		else
		{
			if(mCondition == CONDITION.NOT_IN_RANGE)
			{
				return true;
			}
		}
		return false;
	}
}
