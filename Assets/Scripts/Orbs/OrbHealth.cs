using UnityEngine;
using System.Collections;

public class OrbHealth : OrbBase
{
	public float healAmount = 0.0f;
	
	void Start()
	{
		InitializeOrb();
	}
	
	void Update()
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
	
	public override Transform FindTarget ()
	{
		Transform closestTarget = null;
		float distance;
		Collider[] colliders = Physics.OverlapSphere(mTransform.position, detectionRadius, targetlayer);
		if(colliders.Length > 0)
		{
			float closestDistance = Mathf.Infinity;
			foreach(Collider col in colliders)
			{
				if(col.GetComponent<StatsCharacter>().isFullHealth())
				{
					continue;	
				}
				distance = (mTransform.position - col.transform.position).sqrMagnitude;
				if(distance < closestDistance)
				{
					closestDistance = distance;
					closestTarget = col.transform;
				}
			}
		}
		return closestTarget;
	}
	
	public override void OrbEffect (StatsCharacter statsChar)
	{
		statsChar.ApplyDamage(healAmount);
	}
}
