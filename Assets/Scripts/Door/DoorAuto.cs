using UnityEngine;
using System.Collections.Generic;

public class DoorAuto : DoorBase
{
	public List<Collider> objectInRange = new List<Collider>();
	
	OPENING_TYPE mOpenType;
	
	void OnTriggerEnter(Collider other)	
	{
		objectInRange.Add(other);
		if(objectInRange.Count > 0 && mLockState != DoorBase.LOCK_STATE.LOCK)
		{
			if(mDoorState != DoorBase.DOOR_STATE.OPEN)
			{
				OPENING_TYPE type = GetOpeningType(other.transform);
				OpenDoor(type);
				mOpenType = type;
				mDoorState = DoorBase.DOOR_STATE.OPEN;
			}
		}
	}
	
	void OnTriggerExit(Collider other)	
	{
		objectInRange.Remove(other);
		if(objectInRange.Count <= 0 && mLockState != DoorBase.LOCK_STATE.LOCK)
		{
			OPENING_TYPE type = GetOpeningType(other.transform);
			CloseDoor(mOpenType);
			mDoorState = DoorBase.DOOR_STATE.CLOSE;
		}
	}
}