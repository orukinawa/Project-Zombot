using UnityEngine;
using System.Collections;

public class InRangeRatio : Transition {

	public enum CONDITION
	{
		MORE_THAN,
		LESS_THAN,
	}
	
	public enum RADIUS_TYPE
	{
		ATTACK,
		DETECTION
	}
	
	//! 0 to 1
	public float mRatioRange;
	public CONDITION mCondition;
	public RADIUS_TYPE mRadiusType;
	public LayerMask mTargetLayer;
	
	float GetRatio01(float baseValue, float currentValue)
	{
		float ratio = currentValue / (baseValue * baseValue);
		ratio = Mathf.Clamp01(ratio);
		return ratio;
	}
	
	public override bool VerifyTransition (StateManager context)
	{
		float range = 0.0f;
		Vector3 pos = context.gameObject.GetComponent<EnemyBase>().transform.position;
		if(mRadiusType == RADIUS_TYPE.ATTACK)
		{
			range = context.gameObject.GetComponent<EnemyBase>().mAttackRadius;
		}
		else if(mRadiusType == RADIUS_TYPE.DETECTION)
		{
			range = context.gameObject.GetComponent<EnemyBase>().mDetectionRadius;
		}
		
		//! get the range of the first target only
		Collider[] colliders = Physics.OverlapSphere(context.transform.position,range,mTargetLayer);
		
		if(colliders.Length <= 0)return false;
		
		Vector3 dir = colliders[0].transform.position - pos;
		float ratio = GetRatio01(range, dir.sqrMagnitude);
		
		if(mCondition == CONDITION.MORE_THAN)
		{
			if(ratio > mRatioRange)
			{
				if(mRadiusType == RADIUS_TYPE.ATTACK)
				{
					RaycastHit hit;
					if(Physics.Linecast(context.transform.position, colliders[0].transform.position,out hit))
					{
						if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
						{
							return true;
						}
					}
					return false;
				}
				
				return true;
			}
		}
		else if(mCondition == CONDITION.LESS_THAN)
		{
			if(ratio < mRatioRange)
			{
				return true;
			}
		}
		
		return false;
	}
}
