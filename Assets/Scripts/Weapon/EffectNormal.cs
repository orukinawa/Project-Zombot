using UnityEngine;
using System.Collections;

public class EffectNormal : EffectBase
{
	public override void Initialize(float damage)
	{
		base.Initialize(damage);
	}
	
	public override void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos, float damage)
	{
		SpawnHitVisual(hitPos);
		//visual.transform.parent = col.transform;
		if(col.GetComponent<StatsEnemy>() != null)
		{
			col.GetComponent<StatsEnemy>().ApplyDamage(-damage);		
		}
	}
}
