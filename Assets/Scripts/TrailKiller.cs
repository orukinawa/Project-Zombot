using UnityEngine;
using System.Collections;

public class TrailKiller : MonoBehaviour
{
	TrailRenderer mTrailRenderer;
	float mTime = 0.0f;
	bool isJustOn = false;
	
	void Awake()
	{
		mTrailRenderer = GetComponent<TrailRenderer>();
		mTime = mTrailRenderer.time;
	}
	
	void Update()
	{
		if(isJustOn) return;
		isJustOn = true;
		mTrailRenderer.time = mTime;
	}
	
	void OnEnable()
	{
//		Debug.Log("On Enable!");
		isJustOn = false;
	}
	
	void OnDisable()
	{
		mTrailRenderer.time = 0.0f;
	}
}
