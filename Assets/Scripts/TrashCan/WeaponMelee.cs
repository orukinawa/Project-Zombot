//using UnityEngine;
//using System.Collections.Generic;
//
//public class WeaponMelee : WeaponBase
//{
//	public float angle;
//	public float range;
//	public int damage;
//	private float cosHalfAngle;
//	
//	//Animation	
//	public float animationLength;
//	private Animation swingAnimation;
//	
//	//Debug
//	Vector3 forward = Vector3.zero;
//	float f = 0.0f;
//	float l = 0.0f;
//	
//	
//	
//	void Start ()
//	{
//		InitializeValues();
//		swingAnimation = GetComponentInChildren<Animation>();
//		swingAnimation["Swing"].speed = swingAnimation["Swing"].clip.length/animationLength;
//	}
//	
//	void Update ()
//	{
//		base.UpdateFireRate();
//		
//		//Debug
//		f = range*Mathf.Cos((angle/2.0f)*Mathf.Deg2Rad);
//		l = range*Mathf.Sin((angle/2.0f)*Mathf.Deg2Rad);
//		
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward));
//		Debug.DrawLine(transform.position, transform.position + forward*range, Color.red);
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward*f + Vector3.left*l));
//		Debug.DrawLine(transform.position, transform.position + forward*range, Color.blue);
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward*f + Vector3.right*l));
//		Debug.DrawLine(transform.position, transform.position + forward*range, Color.blue);		
//	}
//	
//	public override void PrimaryFire()
//	{
//		if(canShoot)
//		{
//			//Animation
//			swingAnimation.Stop();
//			swingAnimation.Play("Swing",PlayMode.StopAll);
//			
//			Vector2 localForward = convertToVector2(transform.TransformDirection(Vector3.forward));
//			foreach(Collider col in Physics.OverlapSphere(transform.position, range))
//			{
//				if(col.GetComponent<StatsEnemy>() != null)
//				{
//					Vector3 test = col.transform.position - transform.position;
//					Vector2 test2D = convertToVector2(test);
//					if(Vector2.Dot(localForward.normalized,test2D.normalized) > cosHalfAngle)
//					{
//						col.GetComponent<StatsEnemy>().ApplyDamage(damage);
//						Debug.Log(col.bounds.size);
//					}
//				}
//			}
//			base.AfterShooting();
//		}
//	}
//	
//	Vector2 convertToVector2(Vector3 v)
//	{
//		return new Vector2(v.x,v.z);
//	}
//	
//	void InitializeValues()
//	{
//		cosHalfAngle = Mathf.Cos((angle/2.0f)*Mathf.Deg2Rad);
//	}
//}
