using UnityEngine;
using System.Collections;

public class PickupManager : MonoBehaviour
{
	public float range;
	private float closestDistance;
	private PickupBase targetPickup;
	
	void Start ()
	{
		closestDistance = range*range;
	}
	
	void Update ()
	{
		RefreshTarget();
		if(Input.GetKeyDown(KeyCode.E))
		{
			if(targetPickup != null)
			{
				if(targetPickup.pickupType == PickupBase.PICKUP_TYPE.WEAPON)
				{
					gameObject.GetComponent<Commando>().PickupWeapon(targetPickup.transform.root.gameObject);
				}
				else if(targetPickup.pickupType == PickupBase.PICKUP_TYPE.STATS)
				{
					Debug.Log("Called from PickupManager");
					targetPickup.GetComponent<PickupBase>().PickupEffect(gameObject.GetComponent<StatsBase>());
				}
			}
		}
	}
	
	void RefreshTarget()
	{
		closestDistance = range*range;
		targetPickup = null;
		foreach(Collider col in Physics.OverlapSphere(transform.position, range))
		{
			if(col.transform.root.GetComponent<PickupBase>() != null)
			{
				if(col.transform.root.GetComponent<PickupBase>().isOnGround)
				{
					Vector3 difference = col.transform.position - transform.position;
					if(difference.sqrMagnitude < closestDistance)
					{
						closestDistance = difference.sqrMagnitude;
						targetPickup = col.transform.root.GetComponent<PickupBase>();
					}
				}
			}
		}
	}
}
