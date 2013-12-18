using UnityEngine;
using System.Collections;

public class BulletPiercing : BulletCollider
{
	int environmentLayerInt = 0;
	
	void FixedUpdate()
	{
		_Update();
	}
	
	void OnTriggerStay(Collider col)
	{
		//Debug.Log("Stay " + col.name);
		_OnTriggerEnter(col);
		//Debug.Log(col.name + " has been hit!");
	}
	
	public override void InitializeBullet (float speed, float range, float damage, EffectBase effect, StatTracker stat)
	{
		base.InitializeBullet (speed, range, damage, effect, stat);
		environmentLayerInt = (1 << LayerMask.NameToLayer("Environment"));
	}
	
	public override void _Update ()
	{
		base._Update ();
	}
	
	public override void _OnTriggerEnter (Collider col)
	{
		if(mEffect == null)
		{
			SelfDestruct();
			return;
		}
		if(col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			RaycastHit hit;
			float rayDistance = bulletSpeed * Time.fixedDeltaTime;
			if (Physics.Linecast(previousPosition - (mTransform.position - previousPosition), mTransform.position, layerInt))
			{
				if(Physics.Linecast(previousPosition - (mTransform.position - previousPosition), mTransform.position, out hit, environmentLayerInt))
				{
					rayDistance = hit.distance;
				}
				RaycastHit[] hits = Physics.RaycastAll(previousPosition - (mTransform.position - previousPosition), mTransform.forward, rayDistance, layerInt);
				foreach (RaycastHit mHit in hits)
				{
					mEffect.ApplyEffect(mHit.collider,gameObject, mHit.point, bulletDamage);
				}				
			}			
		}
		else if(col.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			mEffect.ApplyEffect(col,gameObject, transform.position, bulletDamage);
			SelfDestruct();
		}
	}
}
