using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class GamePadManager : MonoBehaviour
{
	//! Make this a singleton
	static GamePadManager sInstance;
	public static GamePadManager Instance
	{ get{ return sInstance; } }	
	
	void Awake()
	{
		//Destroy new instances
		if (sInstance != null)
		{
			Debug.LogWarning("GamePadManager instantiated more than once! Destroying extras!");
			DestroyImmediate(gameObject);
			return;
		}
		sInstance = this;
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void Update()
	{
		
	}
}
