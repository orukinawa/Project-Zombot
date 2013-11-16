using UnityEngine;
using System.Collections;

public class SeparationBehaviour : BehaviourBase
{
	public float mRadius;
	public LayerMask mTargetLayer;
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		Vector3 result = Vector3.zero;
		//int targetLayer = 1 << LayerMask.NameToLayer("Enemy");
		Collider[]colliderList = Physics.OverlapSphere(enemyBase.transform.position, mRadius, mTargetLayer);
		
		foreach(Collider collider in colliderList)
		{
			//! don check itself
			if(collider == this.gameObject.collider)
			{
				continue;
			}
			else
			{
				//! subtract vector from other colliding units
				//dirVector = collider.transform - controller.transform
				result -= (collider.transform.position - enemyBase.transform.position);
			}
		}
		//! preventing enemy from going btm
		result.y = 0;
		result.Normalize();
		
		return result;
	}
}
