using UnityEngine;
using System.Collections;

public class DeathBehaviour : BehaviourBase {
	
	public AnimationClip DeathAnimation;
	public GameObject DeathParticle;
	
	public override void Init (EnemyBase enemyBase)
	{
		if(DeathAnimation != null)
		{
			enemyBase.Animator.CrossFade(DeathAnimation,WrapMode.Once);
			enemyBase.Animator.IsComplete(DeathAnimation, DeathAnimationComplete);
		}
	}
	
	bool DeathAnimationComplete(EnemyBase enemyBase)
	{
//		Debug.Log("I DIED!!!! " + enemyBase.name);
		enemyBase.mStatEnemy.SelfDestruct();
		Instantiate(DeathParticle,enemyBase.transform.position,Quaternion.identity);
		return true;
	}
	
}
