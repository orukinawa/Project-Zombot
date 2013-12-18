using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class BulletCollider : BulletBase
{
	protected float distanceTravelled = 0.0f;	
	protected Vector3 previousPosition = Vector3.zero;
	
	protected Vector3 direction = Vector3.zero;
	
	protected int layerInt = 0;
	
	protected Transform mTransform;
	protected BoxCollider mBoxCollider;
	
	public override void InitializeBullet (float speed, float range, float damage, EffectBase effect, StatTracker stat)
	{
		base.InitializeBullet (speed, range, damage, effect, stat);
		distanceTravelled = 0.0f;
		mBoxCollider = GetComponent<BoxCollider>();
		mTransform = transform;	
		direction = mTransform.TransformDirection(Vector3.forward);
		previousPosition = mTransform.position;
		mBoxCollider.size = new Vector3(0.5f, 0.5f, speed * Time.fixedDeltaTime);
		mBoxCollider.center = new Vector3(0.0f, 0.0f, -0.5f * (speed * Time.fixedDeltaTime));
		layerInt = ~(1 << LayerMask.NameToLayer("Bullet"));
	}

	void FixedUpdate ()
	{
		_Update ();
	}
	
	void OnTriggerEnter(Collider col)
	{
		//Debug.Log("Went in: " + col.name);
		_OnTriggerEnter(col);
	}
	
	public override void _Update ()
	{
		previousPosition = mTransform.position;
		mTransform.position += direction * bulletSpeed * Time.fixedDeltaTime;
		distanceTravelled += Time.fixedDeltaTime * bulletSpeed;
		if(distanceTravelled > bulletRange)
		{
			SelfDestruct();
		}
		//HACK to destroy bullet spawned while respawning gun
		if(bulletSpeed == 0.0f)
		{
			SelfDestruct();
		}
	}
	
	public override void SelfDestruct()
	{
		PoolManager.pools["Bullet Pool"].DeSpawn(gameObject);
	}
	
	public virtual void _OnTriggerEnter(Collider col)
	{
		if(mEffect == null)
		{
			SelfDestruct();
			return;
		}
		if(col.gameObject.layer == LayerMask.NameToLayer("Enemy") || col.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{			
			//effectPrefab.GetComponent<EffectBase>().ApplyEffect(col,gameObject, transform.position);
			mEffect.ApplyEffect(col,gameObject,transform.position, bulletDamage);
			SelfDestruct();
		}
	}
	
	public virtual void SetDirection()
	{
		
	}
}
