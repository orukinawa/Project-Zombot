using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour 
{
	//! the initial aniamtion prefix
	Animation mAnimation;
	public AnimationClip mCurrentClip;
	
	public delegate bool BoolDelegate(EnemyBase enemyBase);
	BoolDelegate mIsAnimationComplete;
	// reference to the enemyBase
	EnemyBase mEnemyBase;
	
	public Dictionary<AnimationClip,BoolDelegate> mAnimationCallbacks = new Dictionary<AnimationClip, BoolDelegate>();
	
	void Awake()
	{
		Init ();
	}
	
	public void Init()
	{
		mAnimation = GetComponentInChildren<Animation>();
		if(mAnimation == null)
		{
			Debug.LogError("[AnimationContoller] Animation component not found at: " + gameObject.name);
		}
		mEnemyBase = GetComponent<EnemyBase>();
		if(mEnemyBase == null)
		{
			Debug.LogError("[AnimationContoller] Enemy base not found at: " + gameObject.name);
		}
	}
	
	public void Reset()
	{
		mAnimationCallbacks.Clear();
	}
	
	public void CrossFade(AnimationClip animationClip, WrapMode wrapMode)
	{
		if(animationClip == null)
		{
			Debug.LogError("assigning crossfade animation with null animation clip!!");
		}
		
		if(mCurrentClip != animationClip)
		{
			string animationName = animationClip.name;
			mAnimation.CrossFade(animationName);
			mAnimation.wrapMode = wrapMode;
			mCurrentClip = animationClip;
		}
	}
	
	public void CrossFade(AnimationClip animationClip, WrapMode wrapMode, float speed)
	{
		if(animationClip == null)
		{
			Debug.LogError("assigning crossfade animation with null animation clip!!");
		}
		
		if(mCurrentClip != animationClip)
		{
			string animationName = animationClip.name;
			mAnimation.CrossFade(animationName);
			mAnimation.wrapMode = wrapMode;
			mAnimation[animationName].speed = speed;
			mCurrentClip = animationClip;
		}
	}
	
	public void AniMix(string animationA, string animationB, WrapMode wrapMode)
	{
		
	}
	
	public void AniAdd(string animationA, string animationB, WrapMode wrapMode)
	{
		
	}
	
	public void IsComplete(AnimationClip clip, BoolDelegate callback)
	{
		if(mAnimationCallbacks.ContainsKey(clip)){
			//Debug.Log("Callback has been assigned");
			return;
		}
		mIsAnimationComplete = new BoolDelegate(callback);
		
		mAnimationCallbacks[clip] = mIsAnimationComplete;
		//Debug.Log("Callback list count: " + mAnimationCallbacks.Count);
	}
	
	void UpdateCallback()
	{
		for(int i = 0; i < mAnimationCallbacks.Count; i++)
		{
			if(mCurrentClip == null){
				//Debug.Log("MyCurrent Clip is null");
				return;	
			}
			// check the call back for the current clip
			if(mAnimationCallbacks.ContainsKey(mCurrentClip))
			{
				//Debug.Log("Evaluating an animation callback! " + mAnimation.isPlaying);
				if(!mAnimation.isPlaying && mAnimation.wrapMode != WrapMode.Loop)
				{
					//Debug.Log("CALLBACK!");
//					// callback
					mAnimationCallbacks[mCurrentClip](mEnemyBase);
				}
			}
		}
	}
	
	void Update()
	{
		UpdateCallback();
	}
}
