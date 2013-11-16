using UnityEngine;
using System.Collections;

public class BulletBase : MonoBehaviour
{
	//Attributes
	protected float bulletSpeed = 0.0f;
	protected float bulletRange = 0.0f;
	protected GameObject effectPrefab = null;
	
	public float damageModifierPercent = 100.0f;
	public float rangeModifierPercent = 100.0f;
	public float speedModifierPercent = 100.0f;
	
	public virtual void InitializeBullet(float speed, float range, GameObject effect)
	{
		bulletSpeed = speed;
		bulletRange = range;
		effectPrefab = effect;
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
