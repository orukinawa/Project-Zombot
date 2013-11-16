using UnityEngine;
using System.Collections;

public class PlayerSightTransition : Transition
{
	public enum LOOK_MODE
	{
		LOOK_AT,
		LOOK_AWAY
	}
	
	public enum TYPE_RADIUS
	{
		DETECTION,
		ATTACK,
	}
	
	public LOOK_MODE mLookMode;
	public TYPE_RADIUS mTypeRadius;
	public float mAngleRange;
	public LayerMask mPlayerLayer;
	
	public bool IsPlayerLooking(Transform self, Transform other)
	{
		//! TODO check if player's transform forward is certain degree
		//! the angle that consider if the player is looking
		float incidentAngle = (360.0f - mAngleRange) * 0.5f;
		float angle = Vector3.Angle(self.transform.forward,other.transform.forward);
		bool flag = false;
		//Debug.Log("incidentAngle: " + incidentAngle);
		//Debug.Log("angle: " + angle);
		
		if(angle > incidentAngle)
		{
			if(mLookMode == LOOK_MODE.LOOK_AT)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else if(angle < incidentAngle)
		{
			if(mLookMode == LOOK_MODE.LOOK_AWAY)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		
		return flag;
	}
	
	public override bool VerifyTransition (StateManager context)
	{
		//PlayerSightTransitionData data = (PlayerSightTransitionData)context.mCustomData[this];
		Transform trans = context.gameObject.transform;
		float detectionRadius = 0.0f;
		
		if(mTypeRadius == TYPE_RADIUS.DETECTION)
		{
			detectionRadius = context.gameObject.GetComponent<EnemyBase>().mDetectionRadius;
		}
		else if(mTypeRadius == TYPE_RADIUS.ATTACK)
		{
			detectionRadius = context.gameObject.GetComponent<EnemyBase>().mAttackRadius;
		}
		
		Collider[] colliders = Physics.OverlapSphere(trans.position,detectionRadius,mPlayerLayer);
		bool IsSeen = false;
		foreach(Collider collider in colliders)
		{
			IsSeen = IsPlayerLooking(trans, collider.transform);
			if(IsSeen)
			{
				//! if seen by any player consider true
				break;
			}
		}
		
		return IsSeen;
	}
}
