using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour 
{
	//! the initial aniamtion prefix
	public string mAnimationPrefix;
	public string mStartAnimation;
	public WrapMode mStartWrapMode;
	Animation mAnimation;
	public AnimationClip mCurrentClip;
	
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
	}
	
	public void Reset()
	{
		if(mStartAnimation != null)
		{
			// reset to the start animation
			CrossFade(mStartAnimation,mStartWrapMode);
		}
	}
	
	public void SetSpeed(string animationName)
	{
		mAnimation[mAnimationPrefix + animationName].speed = 0.5f;
	}
	
	public void CrossFade(string animationName, WrapMode wrapMode)
	{
		mAnimation.CrossFade(mAnimationPrefix + animationName);
		mAnimation.wrapMode = wrapMode;
	}
	
	public void CrossFade(AnimationClip animationClip, WrapMode wrapMode)
	{
		if(mCurrentClip != animationClip){
			string animationName = animationClip.name;
			mAnimation.CrossFade(animationName);
			mAnimation.wrapMode = wrapMode;
			mCurrentClip = animationClip;
		}
	}
	
	public void AniMix(string animationA, string animationB, WrapMode wrapMode)
	{
		
	}
	
	public void AniAdd(string animationA, string animationB, WrapMode wrapMode)
	{
		
	}
}
