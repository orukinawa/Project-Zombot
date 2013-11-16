using UnityEngine;
using System.Collections;

public class BulletNormal : BulletBase
{
	protected Vector3 previousPosition = Vector3.zero;
	protected Vector3 direction = Vector3.zero;
	
	protected float distanceTravelled = 0.0f;
	protected float raycastDistance = 0.0f;
	
	protected RaycastHit[] hits;
	protected RaycastHit closestHit;
	protected float closestHitDistance = Mathf.Infinity;
	
	public override void InitializeBullet (int damage, float speed, float range, GameObject effect)
	{
		base.InitializeBullet (damage, speed, range, effect);
		previousPosition = transform.position;
		direction = transform.TransformDirection(Vector3.forward);
		distanceTravelled = 0.0f;
		raycastDistance = 0.0f;
	}
	
	void Update ()
	{
		_Update ();
	}
	
	public override void _Update ()
	{
		transform.position += direction * bulletSpeed * Time.deltaTime;
		raycastDistance = Vector3.Distance(transform.position, previousPosition);
		distanceTravelled += raycastDistance;
		if(distanceTravelled > bulletRange)
		{
			effectPrefab.GetComponent<EffectBase>().SelfDestruct();
			SelfDestruct();
		}
		//Debug.DrawLine(transform.position, previousPosition, Color.red);
		
		if (Physics.Raycast (previousPosition, direction, raycastDistance))
		{
			hits = Physics.RaycastAll(previousPosition, direction, raycastDistance);
			closestHitDistance = Mathf.Infinity;
			foreach (RaycastHit hit in hits)
			{
				if(hit.distance < closestHitDistance)
				{
					closestHitDistance = hit.distance;
					closestHit = hit;
				}
			}
			effectPrefab.GetComponent<EffectBase>().ApplyEffect(closestHit.collider);
			effectPrefab.GetComponent<EffectBase>().SelfDestruct();
			SelfDestruct();
		}		
		
		previousPosition = transform.position;
	}
	
	public override void SelfDestruct()
	{
		Debug.Log(gameObject.name + " selfdestructed!");
		PoolManager.pools["Bullet Pool"].DeSpawn(gameObject);
	}
}
