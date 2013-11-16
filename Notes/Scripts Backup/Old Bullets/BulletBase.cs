using UnityEngine;
using System.Collections;

public class BulletBase : PoolableObject
{
	//Attributes
	protected float bulletSpeed = 0.0f;
	protected float bulletRange = 0.0f;
	protected GameObject effectPrefab = null;
	
	public virtual void InitializeBullet(int damage, float speed, float range, GameObject effect)
	{
		bulletSpeed = speed;
		bulletRange = range;
		effectPrefab = effect;
		effectPrefab.GetComponent<EffectBase>().Initialize(damage);
	}
	
	public virtual void _Update()
	{
	}
	
	public virtual void SelfDestruct()
	{
	}
	
	public virtual void ApplyEffect()
	{
	}
}
