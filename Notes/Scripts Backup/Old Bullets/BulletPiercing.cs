using UnityEngine;
using System.Collections;

public class BulletPiercing : BulletNormal
{
	public override void InitializeBullet (int damage, float speed, float range, GameObject effect)
	{
		base.InitializeBullet (damage, speed, range, effect);
	}
	
	void Update()
	{
		_Update();
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
		}		
		
		previousPosition = transform.position;
	}
}
