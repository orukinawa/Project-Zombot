using UnityEngine;
using System.Collections;

public class DamageEnemy : MonoBehaviour {

	public KeyCode mButton;
	public float mDamage;
	EnemyBase enemyBase;
	
	void Start()
	{
		enemyBase = GetComponent<EnemyBase>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(mButton))
		{
			enemyBase.mStatEnemy.currentHealth -= mDamage;
		}
	}
}
