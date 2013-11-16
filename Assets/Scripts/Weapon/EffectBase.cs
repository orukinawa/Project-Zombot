using UnityEngine;
using System.Collections;

public class EffectBase : MonoBehaviour
{
	//Attributes
	protected float bulletDamage = 0;
	
	public float damageModifierPercent = 100.0f;
	public float rangeModifierPercent = 100.0f;
	public float speedModifierPercent = 100.0f;
	
	public GameObject hitVisualPrefab;
	
	public virtual void Initialize(float damage)
	{
		bulletDamage = damage;
		//hitVisualPrefab.GetComponent<TimedScript>().poolName = "Visual Pool";
	}
	
	public virtual void ApplyEffect(Collider col, GameObject bullet, Vector3 hitPos)
	{
		
	}
	
	protected void SpawnHitVisual(Vector3 pos)
	{
		PoolManager.pools["HitEffect Pool"].Spawn(hitVisualPrefab,pos,transform.rotation);
	}
	
//	public GameObject SpawnHitVisual(Vector3 pos)
//	{
//		return PoolManager.pools["Visual Pool"].Spawn(hitVisualPrefab,pos,transform.rotation);
//	}
	
	
}
