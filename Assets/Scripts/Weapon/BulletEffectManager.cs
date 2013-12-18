using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Global effect manager so that the player can change bullet effect with bullets still flying
public class BulletEffectManager : MonoBehaviour
{
	//! Make this a singleton
	static BulletEffectManager sInstance;
	public static BulletEffectManager Instance
	{ get{ return sInstance; } }
	
	Dictionary<string,EffectBase> effectDict = new Dictionary<string, EffectBase>();
		
	void Awake()
	{
		sInstance = this;
	}
	
	void Start()
	{
		//Initialize the Dict
		foreach(Transform child in transform)
		{
			effectDict.Add(child.name, child.GetComponent<EffectBase>());
		}
		
		foreach(KeyValuePair<string,EffectBase> blah in effectDict)
		{
			Debug.Log(blah.Key);
		}
	}
	
	public EffectBase getEffect(string effectName)
	{
		#if UNITY_EDITOR
		if(!checkName(effectName))
		{
			Debug.LogError(effectName + " not found in effectDict!");
			return null;
		}
		#endif		
		return effectDict[effectName];
	}	
	
	#if UNITY_EDITOR
	bool checkName(string name)
	{
		return effectDict.ContainsKey(name);
	}
	#endif
}
