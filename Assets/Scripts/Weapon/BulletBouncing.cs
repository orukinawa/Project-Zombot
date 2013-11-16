using UnityEngine;
using System.Collections.Generic;

public class BulletBouncing : BulletCollider
{
	public int bounceAmount;
	public float bounceDetectionRadius;
	protected int bounceCounter = 0;
	protected List<Collider> alreadyHitList = new List<Collider>();
	int enemyLayerMask;
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);
		bounceCounter = 0;
		alreadyHitList.Clear();
		enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
	}
	
	void FixedUpdate()
	{
		_Update();
	}
	
	void OnTriggerEnter(Collider col)
	{
		_OnTriggerEnter(col);
	}
	
	public override void _Update ()
	{
		base._Update ();
	}
	
	public override void _OnTriggerEnter (Collider col)
	{		
		if(effectPrefab == null)
		{
			SelfDestruct();
			return;
		}
		if(col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			RaycastHit hit;
			if (Physics.Linecast(previousPosition, mTransform.position, out hit, layerInt))
			{
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				{
					if(alreadyHitList.Contains(hit.collider))
					{
						return;
					}
					effectPrefab.GetComponent<EffectBase>().ApplyEffect(hit.collider,gameObject, hit.point);
					++bounceCounter;
					if(bounceCounter >= bounceAmount)
					{
						SelfDestruct();
					}
					else
					{
						alreadyHitList.Add(hit.collider);
						//aim to next
						AimTo(getClosestTarget());
					}
				}
			}			
		}
		else if(col.gameObject.layer == LayerMask.NameToLayer("Environment") || col.gameObject.layer == LayerMask.NameToLayer("EnemyShield") )
		{
			SelfDestruct();
		}
	}
	
	Transform getClosestTarget()
	{
		float closestDistance = Mathf.Infinity;
		Transform tranformToReturn = null;
		float tempSqrtDistance = 0.0f;
		foreach(Collider col in Physics.OverlapSphere(transform.position, bounceDetectionRadius, enemyLayerMask))
		{
			if(!alreadyHitList.Contains(col))
			{
				tempSqrtDistance = (transform.position - col.transform.position).sqrMagnitude;
				if(tempSqrtDistance < closestDistance)
				{
					tranformToReturn = col.transform;
					//Debug.Log(tranformToReturn.name);
					closestDistance = tempSqrtDistance;
				}
			}
		}		
		return tranformToReturn;
	}
	
	void AimTo(Transform target)
	{
		if(target == null)
		{
			//Debug.Log("Target is null");
			SelfDestruct();
			return;
		}
		transform.rotation = Quaternion.LookRotation(target.position - transform.position);
		direction = transform.TransformDirection(Vector3.forward);
		//base.SetVelocity();
	}
}
