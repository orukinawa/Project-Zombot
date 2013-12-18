using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//! This class catters for 3D, looping, SFX. (E.g: Fire noise)
//! Draw back, cannot pause and can play only 1 looping sfx per object (Unless multiple SFXLoopingPlayers are attached)
//! TODO: Check if this is a draw back according to the design or if it's fine.
//! NOTE: usually looping sfx will be constant so just add an audio source, put in the clip and loop + play on awake.

public class SFXLoopingPlayer : MonoBehaviour
{
	//! Array of SFXClips to enable editing via inspector.
	public AudioClip[] SFXLoopingList;
	
	//! Dictionary of SFXClips with a string key.
	private Dictionary<string , AudioClip> SFXLoopingDict = new Dictionary<string, AudioClip>();
	
	//! Reference to AudioSource (should be looping).
	public AudioSource mSFXLoopingAudioSource;
	
	//! Called on load.
	void Awake()
	{
		_setDictionaries();
	}
	
	//! Helper method to push SFXList into SFXDict.
	void _setDictionaries()
	{
		foreach(AudioClip sfxClip in SFXLoopingList)
		{
			SFXLoopingDict.Add(sfxClip.name, sfxClip);
		}
	}
	
	//! Plays a Looping SFX
	public void SFXLoopingPlay(string SFXName)
	{
		#if UNITY_EDITOR
		if(!_checkSFXName(SFXName)) return;
		#endif
		
		if(mSFXLoopingAudioSource.clip != null)
		{
			if(mSFXLoopingAudioSource.clip.name == SFXName) return;
		}
		mSFXLoopingAudioSource.clip = SFXLoopingDict[SFXName];
		mSFXLoopingAudioSource.Play();
	}
	
	//! Stops a Looping SFX
	public void SFXLoopingStop()
	{
		if(mSFXLoopingAudioSource.isPlaying) mSFXLoopingAudioSource.Stop();
		mSFXLoopingAudioSource.clip = null;
	}
	
	#if UNITY_EDITOR
	//! Helper method that calls a LogWarning if an invalid SFX name is passed.
	//! Only checks in Editor Mode.
	bool _checkSFXName(string name)
	{		
		for(int i = 0; i < SFXLoopingList.Length; ++i)
		{
			if(SFXLoopingList[i].name == name) return true;
		}
		Debug.LogWarning("No SFX called " + name + " found!");
		return false;
	}
	#endif
}
