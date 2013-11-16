using UnityEngine;
using System.Collections;

public class EffectSlowMovement : EffectBase
{
	public int ticks;
	public float slowMultiplier;
	
	public override void Initialize(float damage)
	{
		base.Initialize(damage);
	}
	
	public override void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos)
	{
		SpawnHitVisual(hitPos);
		if(col.GetComponent<StatsEnemy>() != null)
		{
			col.GetComponent<StatsEnemy>().ApplyDamage(-bulletDamage);			
		}
		if(col.GetComponent<StatusSpeedModifier>() != null)
		{
			col.GetComponent<StatusSpeedModifier>().InitiateSlow(slowMultiplier,ticks);
		}		
	}
}