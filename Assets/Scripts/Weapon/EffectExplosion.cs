using UnityEngine;
using System.Collections;

public class EffectExplosion : EffectBase
{
	public float explosionRadius;
	
	public override void Initialize(float damage)
	{
		base.Initialize(damage);
	}
	
	public override void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos, float damage)
	{
		SpawnHitVisual(hitPos);
		//visual.transform.parent = col.transform;
		foreach(Collider collider in Physics.OverlapSphere(col.transform.position, explosionRadius))
		{
			//Debug.Log("Collider: " + collider);
			if(collider.GetComponent<StatsEnemy>() != null)
			{
				collider.GetComponent<StatsEnemy>().ApplyDamage(-damage);
				//SpawnHitVisual(collider.transform.position);
			}
		}		
	}
}
