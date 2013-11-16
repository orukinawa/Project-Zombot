using UnityEngine;
using System.Collections;
using System;

public class TestMovementScript : MonoBehaviour
{
	StatsBase mStatsBase;
	CharacterController mCharController;
	int direction = 1;
	
	void Start()
	{
		mStatsBase = gameObject.GetComponent<StatsBase>();
		mCharController = gameObject.GetComponent<CharacterController>();
	}
	
	void Update()
	{
		if(Physics.Raycast(transform.position, Vector3.right*direction, 5.0f))
		{
			direction *= -1;
		}
		//mCharController.SimpleMove(Vector3.right*mStatsBase.currentMoveSpeed*direction);
	}
}
