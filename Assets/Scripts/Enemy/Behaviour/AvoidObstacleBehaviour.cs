using UnityEngine;
using System.Collections;

public class AvoidObstacleBehaviour : BehaviourBase
{
	public LayerMask mWallLayer;
	public float mDistToWall = 0.5f;
	float mColliderRad;
	
	public override void Init (EnemyBase enemyBase)
	{
		mColliderRad = enemyBase.charController.radius;
	}
	
	bool RayHitWall(Vector3 middle,Vector3 dir, float distance)
	{
//		Debug.DrawRay(middle,dir * distance, Color.red);
//		Debug.DrawRay(right,dir * distance, Color.red);
//		Debug.DrawRay(left,dir * distance, Color.red);
		
		if(Physics.Raycast(middle,dir,mDistToWall,mWallLayer))
		{
			//Debug.Log("YOLLLLLLLLLLLLOL");
			return true;
		}
//		if(Physics.Raycast(right,dir,mDistToWall,mWallLayer))
//		{
//			return true;
//		}
//		if(Physics.Raycast(left,dir,mDistToWall,mWallLayer))
//		{
//			return true;
//		}
		return false;	
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		Vector3 pos = enemyBase.transform.position;
		//Vector3 rightDir = enemyBase.transform.right;
		//Vector3 leftSide = enemyBase.transform.position + rightDir * colliderRad;
		//Vector3 rightSide = enemyBase.transform.position - rightDir * colliderRad;
		Vector3 dir = enemyBase.transform.forward;
		
		Collider[] colliders = Physics.OverlapSphere(pos,mColliderRad + mDistToWall,mWallLayer);
		
		//! check if there is a nearby wall
		if(colliders.Length > 0)
		{
			//! checks whether it hits the wall based on the front direction
			if(RayHitWall(pos,dir,mColliderRad + mDistToWall))
			{
				ExecuteTransition(enemyBase);
			}
		}
		
		return Vector3.zero;
	}
}
