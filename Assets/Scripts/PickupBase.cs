using UnityEngine;
using System.Collections;

public class PickupBase : MonoBehaviour
{
	public enum PICKUP_TYPE
	{
		WEAPON,
		ITEM,
		STATS
	}
	
	public PICKUP_TYPE pickupType;
	
	public bool isOnGround = true;
	
	public virtual void PickupEffect(StatsBase statsBase)
	{		
	}
}
