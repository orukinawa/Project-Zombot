using UnityEngine;
using System.Collections;

class FleeBehaviourData
{
	public Vector3 mTarget;
}

public class FleeBehaviour : BehaviourBase
{
	public float mFleeingRange;
	public LayerMask mLayerName;
	
	public override void Init (EnemyBase enemyBase)
	{
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			FleeBehaviourData data = new FleeBehaviourData();
			data.mTarget = Vector3.zero;
			enemyBase.mCustomData[this] = data;
		}
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		FleeBehaviourData data = (FleeBehaviourData)enemyBase.mCustomData[this];
		GameObject gameObj = enemyBase.gameObject;
		
		Collider[] colliders = Physics.OverlapSphere(gameObj.transform.position,mFleeingRange,mLayerName);
		foreach(Collider collider in colliders)
		{
			data.mTarget = collider.transform.position;
			return enemyBase.gameObject.transform.position - data.mTarget;
		}
		return Vector3.zero;
	}
}
