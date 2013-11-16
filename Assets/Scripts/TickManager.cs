using UnityEngine;
using System.Collections;

public class TickManager : MonoBehaviour
{
	public float tickDuration = 0.5f;
	private float tickDurationTimer = 0.0f;
	
	public delegate void TickHandler();
	public static event TickHandler onTick;
	
	void FixedUpdate()
	{
		if(tickDurationTimer >= tickDuration)
		{
			if(onTick != null)
			{
				onTick();
			}
			tickDurationTimer = 0.0f;
		}
		else
		{
			tickDurationTimer += Time.deltaTime;
		}
	}
}
