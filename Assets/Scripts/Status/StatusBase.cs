using UnityEngine;
using System.Collections;

public class StatusBase : MonoBehaviour
{
	protected int ticksLeft = 0;
	protected bool isSubscribed = false;
	
	public virtual void InitializeStatus()
	{
		ticksLeft = 0;
		isSubscribed = false;
	}
	
	public virtual void updateStatus()
	{
		if(ticksLeft < 1)
		{
			UnsubscribeFromTickEvent();
		}
	}
	
	public virtual void SubscribeToTickEvent()
	{
		if(isSubscribed)
		{
			return;
		}
		TickManager.onTick += updateStatus;
		isSubscribed = true;
	}
	
	public virtual void UnsubscribeFromTickEvent()
	{
		if(!isSubscribed)
		{
			return;
		}
		TickManager.onTick -= updateStatus;
		isSubscribed = false;
		ticksLeft = 0;
	}
	
	void OnDisable()
	{
		if(isSubscribed)
		{
			UnsubscribeFromTickEvent();
		}
	}
}
