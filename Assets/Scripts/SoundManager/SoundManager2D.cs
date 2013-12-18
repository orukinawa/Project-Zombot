using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//! This class is a singleton that catters for all 2D Sounds.
//! E.g: BGM and UI SFX.

public class SoundManager2D : MonoBehaviour
{
	//Make this a singleton
	static SoundManager2D sInstance;
	public static SoundManager2D Instance
	{ get{ return sInstance; } }
	
	[System.Serializable]
	//! Class that wraps BGM Clips with their playbackTime(for resume).
	public class BGMClip
	{
		public AudioClip audioClip;
		[HideInInspector]public float playbackTime = 0.0f;
	}
	
	//! Modes to Launch a BGM Clip
	public enum StartMode
	{
		RESTART = 0,
		RESUME = 1
	}
	
	[SerializeField]
	//! Array of BGMClips to enable editing via inspector.
	private BGMClip[] BGMList;
	
	[SerializeField]
	//! Array of SFXClips to enable editing via inspector.
	private AudioClip[] SFXList;
	
	//! Dictionary of BGMClips with a string key.
	//! BGMClipList is pushed into this at runtime.
	//! OPTIMIZABLE: Serialize instead (if got time!)
	private Dictionary<string , BGMClip> BGMDict = new Dictionary<string, BGMClip>();
	
	//! Dictionary of SFXClips with a string key.
	private Dictionary<string , AudioClip> SFXDict = new Dictionary<string, AudioClip>();
	
	//! Reference to the current clip.
	private BGMClip currentBGMClip;
	
	//! Reference to audio source for playback.
	private AudioSource mBGMAudioSource;
	
	//! Need a seperate audio source for SFX so that they don't stop when BGMStop is called.
	private AudioSource mSFXAudioSource;
	
	//! Called on Load
	void Awake()
	{
		_setDictionaries();
		_setAudioSources();
		sInstance = this;
	}	
	
	//! STOPS any playing BGM and PLAYS a new one.
	public void BGMPlay(string BGMName)
	{		
		#if UNITY_EDITOR
		if(!_checkBGMName(BGMName)) return;
		#endif	
		
		if(mBGMAudioSource.isPlaying)
		{
			if(currentBGMClip.audioClip.name == BGMName) return;
			_stopBGM();
		}		
		_launchBGM(BGMDict[BGMName] , StartMode.RESTART);
	}
	
	//! STOPS the current BGM.
	public void BGMStop()
	{
		if(mBGMAudioSource.isPlaying)
		{
			_stopBGM();
		}		
	}
	
	//! PAUSES the current BGM.
	public void BGMPause()
	{
		if(mBGMAudioSource.isPlaying)
		{
			_pauseBGM();
		}
	}
	
	//! STOPS any playing BGM and RESUMES a paused BGM.
	public void BGMResume(string BGMName)
	{
		#if UNITY_EDITOR
		if(!_checkBGMName(BGMName)) return;
		#endif
		
		if(mBGMAudioSource.isPlaying)_stopBGM();
		_launchBGM(BGMDict[BGMName] , StartMode.RESUME);
	}
	
	//! RESUMES the current paused BGM.
	public void BGMResume()
	{
		if(mBGMAudioSource.isPlaying) return;
		if(currentBGMClip == null) return;
		mBGMAudioSource.time = currentBGMClip.playbackTime;
		mBGMAudioSource.Play();
	}
	
	
	//! RESTARTS the current BGM.
	public void BGMRestart()
	{
		if(currentBGMClip == null) return;
		mBGMAudioSource.time = 0.0f;
		if(!mBGMAudioSource.isPlaying) mBGMAudioSource.Play();
	}
	
	//! Returns the current BGM name or null if nothing is playing/paused.
	public string getBGMName()
	{
		if(currentBGMClip == null) return null;
		return currentBGMClip.audioClip.name;
	}
	
	//! Plays a SFX Once
	public void SFXPlay(string SFXName)
	{
		#if UNITY_EDITOR
		if(!_checkSFXName(SFXName)) return;
		#endif
		
		// Need to pass back the AudioListener.volume (Unity 4.2 bug, fixed in 4.3)
		mSFXAudioSource.PlayOneShot(SFXDict[SFXName], AudioListener.volume);	
	}
	
	//! Helper method to push BGM/SFX Lists into Dicts.
	void _setDictionaries()
	{
		foreach(BGMClip bgmClip in BGMList)
		{
			BGMDict.Add(bgmClip.audioClip.name , bgmClip);
		}
		
		foreach(AudioClip sfxClip in SFXList)
		{
			SFXDict.Add(sfxClip.name, sfxClip);
		}
	}
	
	//! Helper method to set seperate audio sources for BGM and SFX
	void _setAudioSources()
	{
		AudioSource[] mAudioSources = GetComponents<AudioSource>();
		mBGMAudioSource = mAudioSources[0];
		mSFXAudioSource = mAudioSources[1];
	}
	
	//! Helper method to stop a BGM.
	void _stopBGM()
	{
		mBGMAudioSource.Stop();
		currentBGMClip.playbackTime = 0.0f;
		currentBGMClip = null;
	}
	
	//! Helper method to restart or resume a BGM.
	void _launchBGM(BGMClip clip , StartMode mode)
	{
		currentBGMClip = clip;
		mBGMAudioSource.clip = currentBGMClip.audioClip;
		mBGMAudioSource.time = (float)mode * currentBGMClip.playbackTime;
		mBGMAudioSource.Play();
	}
	
	//! Helper method to pause a BGM.
	void _pauseBGM()
	{
		currentBGMClip.playbackTime = mBGMAudioSource.time;
		mBGMAudioSource.Stop();
	}
	
	#if UNITY_EDITOR
	//! Helper method that calls a LogWarning if an invalid BGM name is passed.
	//! Only checks in Editor Mode.
	bool _checkBGMName(string name)
	{		
		for(int i = 0; i < BGMList.Length; ++i)
		{
			if(BGMList[i].audioClip.name == name) return true;
		}
		Debug.LogWarning("No BGM called " + name + " found!");
		return false;
	}
	
	//! Same thing with SFX names
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
