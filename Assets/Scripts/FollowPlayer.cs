using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour 
{
	//public GameObject mPlayer;
	LookAtMouse mLookAtMouse;

	// Use this for initialization
	void Start () 
	{
		//mLookAtMouse = mPlayer.GetComponentInChildren<LookAtMouse>();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(mLookAtMouse != null)
		{
			//try spring damping
			transform.position = mLookAtMouse.GetMidPoint();
		}
	}
	
	public void SetLookAtMouseScript(LookAtMouse script)
	{
		mLookAtMouse = script;
	}
}
