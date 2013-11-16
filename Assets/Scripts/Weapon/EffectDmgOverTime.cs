using UnityEngine;
using System.Collections;

public class EffectDmgOverTime : EffectBase
{
	public int ticks;
	public int damagePerTick;
	
	public override void Initialize(float damage)
	{
		base.Initialize(damage);
	}
	
	public override void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos)
	{
		SpawnHitVisual(hitPos);
		//visual.transform.parent = col.transform;
		if(col.GetComponent<StatsEnemy>() != null)
		{
			col.GetComponent<StatsEnemy>().ApplyDamage(-bulletDamage);			
		}
		if(col.GetComponent<StatusHPModifier>() != null)
		{
			col.GetComponent<StatusHPModifier>().initiateDmg(damagePerTick,ticks);		
		}		
	}
}
