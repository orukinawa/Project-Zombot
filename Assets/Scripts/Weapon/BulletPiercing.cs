using UnityEngine;
using System.Collections;

public class BulletPiercing : BulletCollider
{
	int environmentLayerInt = 0;
	
	void FixedUpdate()
	{
		_Update();
	}
	
	void OnTriggerEnter(Collider col)
	{
		_OnTriggerEnter(col);
	}	
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);
		environmentLayerInt = (1 << LayerMask.NameToLayer("Environment"));
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
			float rayDistance = bulletSpeed * Time.fixedDeltaTime;
			if (Physics.Linecast(previousPosition, mTransform.position, layerInt))
			{
				if(Physics.Linecast(previousPosition, mTransform.position, out hit, environmentLayerInt))
				{
					rayDistance = hit.distance;
				}
				RaycastHit[] hits = Physics.RaycastAll(previousPosition, transform.forward, rayDistance, layerInt);
				foreach (RaycastHit mHit in hits)
				{
					effectPrefab.GetComponent<EffectBase>().ApplyEffect(mHit.collider,gameObject, mHit.point);
				}				
			}			
		}
		else if(col.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			SelfDestruct();
		}
	}
}
