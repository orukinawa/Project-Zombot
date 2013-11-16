using UnityEngine;
using System.Collections;

public class SpitterGoo : BulletMortar
{
	// Update is called once per frame
	void Update ()
	{
		base._Update();
	}
	
	//! logic to do when object destory
	public override void SelfDestruct ()
	{
		Debug.Log("YOLO");
	}
	
	//! apply dmg and effect here
	public override void _OnTriggerEnter (Collider col)
	{
		Debug.Log("YOLO HITS SOMEONE");
	}
}
