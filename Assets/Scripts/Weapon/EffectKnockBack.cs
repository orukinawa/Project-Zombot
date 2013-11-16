using UnityEngine;
using System.Collections;

public class EffectKnockBack : EffectBase
{
	public float knockBackDistance;
	
	public override void Initialize(float damage)
	{
		base.Initialize(damage);
	}
	
	public override void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos)
	{
		//GameObject visual = SpawnHitVisual(hitPos);
		//visual.transform.parent = col.transform;
		if(col.GetComponent<StatsEnemy>() != null)
		{			
			Vector3 direction = bullet.transform.TransformDirection(Vector3.forward);
			col.GetComponent<StatsEnemy>().ApplyDamage(-bulletDamage);
			col.GetComponent<CharacterController>().SimpleMove(direction.normalized*knockBackDistance);
		}
	}
}
