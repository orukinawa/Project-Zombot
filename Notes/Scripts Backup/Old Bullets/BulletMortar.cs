using UnityEngine;
using System.Collections;

public class BulletMortar : BulletNormal
{
	public float height;
	public float distance;
	
	float heightDistanceRatio;
	
	float offsetHori;
	float previousOffsetVerti;
	float offsetVerti;
	float defaultBulletSpeed;
	float offsetAngle;
	float previousOffsetAngle;
	
	Vector3 nextPosition = Vector3.zero;
	
	public override void InitializeBullet (int damage, float speed, float range, GameObject effect)
	{
		base.InitializeBullet (damage, speed, range, effect);
		offsetHori = 0.0f;
		offsetVerti = 0.0f;
		offsetAngle = 0.0f;
		previousOffsetVerti = 0.0f;
		previousOffsetAngle = 0.0f;
		heightDistanceRatio = (height/distance)*4.0f;
		defaultBulletSpeed = bulletSpeed;
	}
	
	void Update()
	{
		offsetHori += defaultBulletSpeed * Time.deltaTime;
		offsetVerti = heightDistanceRatio*(offsetHori-((offsetHori*offsetHori)/distance));
		offsetAngle = Mathf.Atan((offsetVerti - previousOffsetVerti)/defaultBulletSpeed) * Mathf.Rad2Deg;
		previousOffsetVerti = offsetVerti;
		//transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.Rotate(-offsetAngle + previousOffsetAngle ,0,0);
		previousOffsetAngle = offsetAngle;
		direction = transform.TransformDirection(Vector3.forward);
		_Update();		
	}
	
	public override void _Update ()
	{		
		base._Update ();
	}
}
