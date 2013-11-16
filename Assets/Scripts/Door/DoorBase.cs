using UnityEngine;
using System.Collections;

public class DoorBase : MonoBehaviour 
{
	public enum LOCK_STATE
	{
		UNLOCK,
		LOCK,
	}
	
	public enum DOOR_STATE
	{
		CLOSE,
		OPEN,
	}
	
	public enum OPENING_TYPE
	{
		IN,
		OUT,
	}
	
	public LOCK_STATE mLockState;
	public DOOR_STATE mDoorState;
	
	//! to tell whether this is a start door or not
	public bool mStartDoor = false;
	
	Animation mAnimation;
	
	void Start()
	{
		mAnimation = GetComponent<Animation>();
		foreach(AnimationState ani in mAnimation)
		{
			ani.wrapMode = WrapMode.ClampForever;
		}
	}
	
	//! how the door will be open
	protected OPENING_TYPE GetOpeningType(Transform trans)
	{
		OPENING_TYPE type = OPENING_TYPE.IN;
		Vector3 targetDir = trans.position - transform.position;
		float dotValue = Vector3.Dot(transform.forward,targetDir);
		//! target facing the door transform forward
		if(dotValue > 0)
		{
			type = OPENING_TYPE.OUT;
			return type;
		}
		return type;
	}
	
	//! play open door animation
    protected virtual void OpenDoor(OPENING_TYPE type)
	{
		if(type == OPENING_TYPE.IN)
		{
			mAnimation.CrossFade("in-open-slowly");
		}
		else
		{
			mAnimation.CrossFade("out-open-slowly");
		}
	}
	
	//! play close door animation
	protected virtual void CloseDoor(OPENING_TYPE type)
	{
		if(type == OPENING_TYPE.IN)
		{
			mAnimation.CrossFade("in-close");
		}
		else
		{
			mAnimation.CrossFade("out-close");
		}
	}
}
