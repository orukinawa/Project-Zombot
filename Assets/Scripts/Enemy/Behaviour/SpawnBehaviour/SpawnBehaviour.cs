using UnityEngine;
using System.Collections;

public class SpawnBehaviour : BehaviourBase {

	public GameObject SpawnPuffParticle;
	
	public override void Init (EnemyBase enemyBase)
	{
		// spawn particle
		Instantiate(SpawnPuffParticle,enemyBase.transform.position,Quaternion.identity);
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		RaycastHit hit;
		Physics.Raycast(enemyBase.transform.position,Vector3.down,out hit,Mathf.Infinity);
		
		Vector3 dir = hit.point - enemyBase.transform.position;
		if(dir.sqrMagnitude <= 1.21f + enemyBase.charController.radius)
		{
			ExecuteTransition(enemyBase);	
		}
		
		return Vector3.zero;
	}
	
}
