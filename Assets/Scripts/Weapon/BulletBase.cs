using UnityEngine;
using System.Collections;

public class BulletBase : MonoBehaviour
{
	//Attributes
	protected float bulletSpeed = 0.0f;
	protected float bulletRange = 0.0f;
	protected float bulletDamage = 0.0f;
	//protected GameObject effectPrefab = null;
	protected EffectBase mEffect;
	protected StatTracker mStat;
	
	public float damageModifierPercent = 100.0f;
	public float rangeModifierPercent = 100.0f;
	public float speedModifierPercent = 100.0f;
	
	public virtual void InitializeBullet(float speed, float range, float damage, EffectBase effect, StatTracker stat)
	{
		bulletSpeed = speed;
		bulletRange = range;
		bulletDamage = damage;
		mEffect = effect;
		mStat = stat;
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
