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
	
	//int RaycastCall = 0;
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);
		distanceTravelled = 0.0f;
		mBoxCollider = GetComponent<BoxCollider>();
		mTransform = transform;	
		direction = mTransform.TransformDirection(Vector3.forward);
		previousPosition = mTransform.position;
		mBoxCollider.size = new Vector3(0.1f, 0.1f, speed * Time.fixedDeltaTime);
		mBoxCollider.center = new Vector3(0.0f, 0.0f, -speed/2*Time.fixedDeltaTime);
		layerInt = ~(1 << LayerMask.NameToLayer("Bullet"));
		//Debug.Log("Collider size: " + mBoxCollider.size);
		//RaycastCall = 0;
	}

	void Update ()
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
		mTransform.position += direction * bulletSpeed * Time.deltaTime;
		distanceTravelled += Time.deltaTime * bulletSpeed;
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
		if(effectPrefab == null)
		{
			SelfDestruct();
			return;
		}
		if(col.gameObject.layer == LayerMask.NameToLayer("Enemy") || col.gameObject.layer == LayerMask.NameToLayer("Environment") || col.gameObject.layer == LayerMask.NameToLayer("EnemyShield"))
		{			
			effectPrefab.GetComponent<EffectBase>().ApplyEffect(col,gameObject, transform.position);
			SelfDestruct();
//			RaycastHit hit;
//			if (Physics.Linecast(previousPosition, mTransform.position + mTransform.forward * 0.01f, out hit, layerInt))
//			{
//				effectPrefab.GetComponent<EffectBase>().ApplyEffect(hit.collider,gameObject, hit.point);
//				SelfDestruct();
//			}
		}
	}
	
	public virtual void SetDirection()
	{
		
	}
}
