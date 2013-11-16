using UnityEngine;
using System.Collections;

public class InRangeCustom : Transition
{
	public enum CONDITION
	{
		IN_RANGE,
		NOT_IN_RANGE
	}
	
	public float mCustomRange;
	public CONDITION mCondition;
	public LayerMask mTargetLayer;
	
	public override bool VerifyTransition (StateManager context)
	{
		Collider[] colliders = Physics.OverlapSphere(context.transform.position, mCustomRange,mTargetLayer);
		
		if(colliders.Length > 0)
		{
			if(mCondition == CONDITION.IN_RANGE)
			{
				return true;
			}
			else
			{
				return false;
			}
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
