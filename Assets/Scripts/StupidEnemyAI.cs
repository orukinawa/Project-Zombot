using UnityEngine;
using System.Collections;

public class StupidEnemyAI : MonoBehaviour
{
	public float detectionRange;
	public float attackRange;
	public float movespeed;
	Commando commando = null;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (commando == null) 
		{
			commando = FindObjectOfType (typeof(Commando)) as Commando;
		} 
		else 
		{
			if (Vector3.Distance (commando.transform.root.position, transform.position) < detectionRange) 
			{
				MoveTowardsPlayer ();
			}
			if(Vector3.Distance (commando.transform.root.position, transform.position) < attackRange)
			{
				commando.transform.root.position -= -commando.transform.root.transform.forward * 2.0f;
			}
		}
	}
	
	void MoveTowardsPlayer ()
	{
		transform.rotation = Quaternion.LookRotation(commando.transform.root.position-transform.position);
		transform.position += transform.forward * movespeed * Time.deltaTime;
	}
}
