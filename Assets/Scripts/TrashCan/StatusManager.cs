//using UnityEngine;
//using System.Collections;
//
//public class StatusManager : MonoBehaviour
//{
//	StatusBase[] statusArray;
//	
//	void OnEnable()
//	{
//		//Subscribe to the event
//		TickManager.onTick += Tick;
//	}
//	
//	void OnDisable()
//	{
//		//Unsubscribe
//		TickManager.onTick -= Tick;
//	}
//	
//	void Tick()
//	{
//		foreach(StatusBase stats in statusArray)
//		{
//			stats.updateStatus();
//		}
//	}
//	
//	void Awake()
//	{
//		statusArray = GetComponents<StatusBase>();
//		foreach(StatusBase status in statusArray)
//		{
//			status.InitializeStatus();
//		}
//	}
//}
