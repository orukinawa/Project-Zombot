using UnityEngine;
using System.Collections.Generic;

public class DoorManual : DoorBase
{
	public List<Collider> statsCharList = new List<Collider>();
	
	OPENING_TYPE mOpenType;
	
	void AccessDoor(Transform trans)
	{
		OPENING_TYPE type = GetOpeningType(trans);
		if(mDoorState == DoorBase.DOOR_STATE.CLOSE)
		{
			OpenDoor(type);
			mOpenType = type;
			mDoorState = DoorBase.DOOR_STATE.OPEN;
		}
		else
		{
			CloseDoor(mOpenType);
			mDoorState = DoorBase.DOOR_STATE.CLOSE;
		}
	}
	
	void Update()
	{
		if(statsCharList.Count > 0)
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				if(mLockState != DoorBase.LOCK_STATE.LOCK)
				{
					AccessDoor(statsCharList[0].transform);
					if(mStartDoor && !EventManager.mStartGame)
					{
						EventManager.mStartGame = true;
					}
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)	
	{
		StatsCharacter stats = other.GetComponent<StatsCharacter>();
		if(stats)
		{
			statsCharList.Add(other);
		}
	}
	
	void OnTriggerExit(Collider other)	
	{
		StatsCharacter stats = other.GetComponent<StatsCharacter>();
		if(stats)
		{
			statsCharList.Remove(other);
		}
	}
}
