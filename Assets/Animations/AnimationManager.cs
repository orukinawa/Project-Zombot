using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
	public Animator mAnimator;
	
	AnimatorStateInfo currentBaseState;
	
	int idleState = Animator.StringToHash("Movement.Idle");	
	int walkState = Animator.StringToHash("Movement.Walk");	
	int runState = Animator.StringToHash("Movement.Run");
	int deadState = Animator.StringToHash("Movement.Dead");
	
	
	int nothingState = Animator.StringToHash("UpperBody.Nothing");
	int shootingState = Animator.StringToHash("UpperBody.Shooting");
	int lowerArmState = Animator.StringToHash("UpperBody.LowerArm");
	
	void Update()
	{
		currentBaseState = mAnimator.GetCurrentAnimatorStateInfo(0);
		
		if(currentBaseState.nameHash == deadState)
		{
			mAnimator.SetBool("isDead",false);
		}
		
		if(currentBaseState.nameHash == idleState)
		{
			mAnimator.SetBool("isRevive",false);
		}
	}
}
