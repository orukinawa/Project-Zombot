using UnityEngine;
using System.Collections;

public class BulletMortar : BulletCollider
{
	public float height;
	float distance;
	//public float distanceFixHack;
	
	float heightDistanceRatio;
	
	float offsetHori;
	float previousOffsetVerti;
	float offsetVerti;
	float defaultBulletSpeed;
	float offsetAngle;
	float previousOffsetAngle;
	
	//Vector3 nextPosition = Vector3.zero;
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);
		offsetHori = 0.0f;
		offsetVerti = 0.0f;
		offsetAngle = 0.0f;
		previousOffsetVerti = 0.0f;
		previousOffsetAngle = 0.0f;
		Plane groundPlane = new Plane (transform.up, Vector3.zero);
		Vector3 cursorWorldPos = CursorManager.ScreenPointToWorldPointOnPlane (Input.mousePosition, groundPlane, Camera.main);
		distance = Vector3.Distance(cursorWorldPos,transform.position);
		height = Mathf.Sqrt(distance) * 50.0f;
		heightDistanceRatio = (height/distance)*4.0f;
		defaultBulletSpeed = bulletSpeed;
	}
	
	public void FixedUpdate()
	{		
		_Update();		
	}
	
	public void OnTriggerEnter(Collider col)
	{
		//Debug.Log(" HIT " + col.name);
		_OnTriggerEnter(col);
	}
	
	public override void _Update ()
	{
		offsetHori += defaultBulletSpeed * Time.deltaTime;
		offsetVerti = heightDistanceRatio*(offsetHori-((offsetHori*offsetHori)/distance));
		offsetAngle = Mathf.Atan((offsetVerti - previousOffsetVerti)/defaultBulletSpeed) * Mathf.Rad2Deg;
		//transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.Rotate(-offsetAngle + previousOffsetAngle ,0,0);
		previousOffsetAngle = offsetAngle;
		direction = transform.TransformDirection(Vector3.forward);
		//base.SetVelocity();
		bulletSpeed = Mathf.Sqrt(defaultBulletSpeed*defaultBulletSpeed + (offsetVerti - previousOffsetVerti)*(offsetVerti - previousOffsetVerti));
		previousOffsetVerti = offsetVerti;
		base._Update ();
	}
	
	public override void _OnTriggerEnter (Collider col)
	{
		base._OnTriggerEnter (col);
	}
}
