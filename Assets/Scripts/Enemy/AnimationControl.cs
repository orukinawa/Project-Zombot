using UnityEngine;
using System.Collections;

public class AnimationControl : MonoBehaviour 
{
	float mTimer = 0.0f;
	float mAnimationEndTime;
	public AnimationClip mTargetClip;
	public GameObject mVisualEffect;
	
	void Start()
	{
		mAnimationEndTime = mTargetClip.length;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mTimer += Time.deltaTime;
		if(mTimer >= mAnimationEndTime)
		{
			//! do despawn here
			PoolManager.pools["EnemyVisualPool"].DeSpawn(gameObject);
			//! spawn the death visual effect
			GameObject obj = (GameObject)Instantiate(mVisualEffect,transform.position,Quaternion.identity);
			obj.transform.localScale = transform.localScale;
			mTimer = 0.0f;
		}
	}
}
