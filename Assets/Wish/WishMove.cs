using UnityEngine;
using System.Collections;

public class WishMove : MonoBehaviour
{
	Animator mAnimator;
	
	bool isDead = false;
	
	void Start()
	{
		mAnimator = GetComponent<Animator>();
		mAnimator.SetBool("isDead", true);
	}
	
	void Update()
	{
		mAnimator.SetBool("isShooting", Input.GetKey(KeyCode.Alpha0));		
		//if(Input.GetKeyDown(KeyCode.L)) isDead = true;
	}
}
