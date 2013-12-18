using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//! This class catters for 3D, play once, SFX.
//! Draw back, if a clip is called before the current one ended, the current one gets cut.
//! TODO: Check if this is a draw back according to the design or if it's fine.
//! Another way would be to use AudioSource.PlayOneShot() and manually calculate the volume according to the distance to the listener.

public class SFXPlayer : MonoBehaviour
{
	//! Array of SFXClips to enable editing via inspector.
	public AudioClip[] SFXList;
	
	//! Dictionary of SFXClips with a string key.
	private Dictionary<string , AudioClip> SFXDict = new Dictionary<string, AudioClip>();
	
	//! Reference to AudioSource
	public AudioSource mSFXAudioSource;
	
	//! Called on load.
	void Awake()
	{
		_setDictionaries();
	}
	
	//! Helper method to push SFXList into SFXDict.
	void _setDictionaries()
	{
		foreach(AudioClip sfxClip in SFXList)
		{
			SFXDict.Add(sfxClip.name, sfxClip);
		}
	}
	
	//! Plays a SFX Once
	public void SFXPlay(string SFXName)
	{
		#if UNITY_EDITOR
		if(!_checkSFXName(SFXName)) return;
		#endif
		
		mSFXAudioSource.clip = SFXDict[SFXName];
		mSFXAudioSource.Play();
	}
	
	#if UNITY_EDITOR
	//! Helper method that calls a LogWarning if an invalid SFX name is passed.
	//! Only checks in Editor Mode.
	bool _checkSFXName(string name)
	{		
		for(int i = 0; i < SFXList.Length; ++i)
		{
			if(SFXList[i].name == name) return true;
		}
		Debug.LogWarning("No SFX called " + name + " found!");
		return false;
	}
	#endif
}
