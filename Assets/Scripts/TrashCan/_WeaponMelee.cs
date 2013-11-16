///*---------------------
//- Tried to code an alternative to use colliding Mesh
//- Not sure whether it is less expensive or not
//- The list of colliders can be iterated through a lot depending on the no of raycasts and targets
//- Raycasts do not return true if the source is inside collider
//	- A solution to that is to raycast twice from both edges towards the centre
//	- This this double the amount of raycasts and iteration throught the list
//---------------------*/
//
//
//using UnityEngine;
//using System.Collections.Generic;
//
//public class _WeaponMelee : WeaponBase
//{
//	//Melee Hit Area
//	public float startWidth;
//	public float endWidth;
//	public float length;
//	public int raycastNumber;
//	
//	// Time between each swing
//	public float fireRate = 0.0f;
//	private float fireRateTimer = 0.0f;
//	private bool canShoot = true;
//	
//	public float animationLength;
//	public int damage;
//	
//	private List<StatsBase> hitList = new List<StatsBase>();
//	private Vector3 localForward;
//	private Vector3 localRight;
//	private float diagonalLength = 0.0f;
//	private sbyte negativeModifier;
//	public bool showLines = false;
//	public bool showBlue = false;
//	public bool showRed = false;
//	
//	private Animation animation;
//	
//	void Start ()
//	{
//		InitializeRaycastArea();
//		animation = GetComponentInChildren<Animation>();
//		animation["Swing"].speed = animation["Swing"].clip.length/animationLength;
//	}
//	
//	void Update ()
//	{
//		if(!canShoot)
//		{
//			if(fireRateTimer < fireRate)
//			{
//				fireRateTimer += Time.deltaTime;
//			}
//			else
//			{
//				canShoot = true;
//			}
//		}
//		
//		if(showLines)
//		{
//			InitializeRaycastArea();
//			localRight = Vector3.Normalize(transform.TransformDirection(Vector3.right));
//			localForward = Vector3.Normalize(transform.position + Vector3.Normalize(transform.TransformDirection(Vector3.forward))*length - localRight*endWidth/2.0f - transform.position + localRight*startWidth/2.0f);
//			for (int i=0; i<raycastNumber; ++i)
//			{
//				Vector3 startPos = transform.position -localRight*startWidth/2.0f + localForward*diagonalLength/((raycastNumber-1)*1.0f)*i;
//				//float diag = ((diagonalLength/((raycastNumber-1)*1.0f))*i);
//				//float vert = ((length/((raycastNumber-1)*1.0f))*i);
//				float distance = negativeModifier*Mathf.Sqrt((((diagonalLength/((raycastNumber-1)*1.0f))*i)*((diagonalLength/((raycastNumber-1)*1.0f))*i)) - (((length/((raycastNumber-1)*1.0f))*i)*((length/((raycastNumber-1)*1.0f))*i)))*2.0f + startWidth;
//				if(showBlue)
//				Debug.DrawLine(startPos,startPos+localRight*distance,Color.blue);				
//				startPos = startPos + localRight*distance;
//				if(showRed)
//				Debug.DrawLine(startPos,startPos-localRight*distance,Color.red);
//			}
//		}
//	}
//	
//	public override void PrimaryFire ()
//	{
//		if(canShoot)
//		{			
//			animation.Stop();
//			animation.Play("Swing",PlayMode.StopAll);
//			
//			//! Method 1 - Twice less calls but fails if raycast source is inside collider
////			hitList.Clear();
////			right = Vector3.Normalize(transform.TransformDirection(Vector3.right));
////			//Vector3 baseVector = transform.position + right*startWidth/2.0f;
////			//Vector3 topVector = transform.position + Vector3.Normalize(transform.TransformDirection(Vector3.forward))*length - right*endWidth/2.0f;
////			forward = Vector3.Normalize(transform.position + Vector3.Normalize(transform.TransformDirection(Vector3.forward))*length - right*endWidth/2.0f - transform.position + right*startWidth/2.0f);
////			for (int i=0; i<raycastNumber; ++i)
////			{
////				Vector3 startPos = transform.position -right*startWidth/2.0f + forward*diagonalLength/((raycastNumber-1)*1.0f)*i;
////				//float diag = ((diagonalLength/((raycastNumber-1)*1.0f))*i);
////				//float vert = ((length/((raycastNumber-1)*1.0f))*i);
////				float distance = negativeModifier*Mathf.Sqrt((((diagonalLength/((raycastNumber-1)*1.0f))*i)*((diagonalLength/((raycastNumber-1)*1.0f))*i)) - (((length/((raycastNumber-1)*1.0f))*i)*((length/((raycastNumber-1)*1.0f))*i)))*2 + startWidth;
////				if(Physics.Raycast(startPos,right,distance))
////				{
////					foreach (RaycastHit hit in Physics.RaycastAll(startPos, right, distance))
////					{
////						bool isInList = false;
////						if(hit.collider.GetComponent<StatsBase>() != null)
////						{
////							StatsBase tempStatsBase = hit.collider.GetComponent<StatsBase>();
////							foreach(StatsBase sBase in hitList)
////							{
////								if(tempStatsBase == sBase)
////								{
////									isInList = true;
////									break;
////								}
////							}
////							
////							if(!isInList)
////							{
////								hitList.Add(hit.collider.GetComponent<StatsBase>());
////							}
////						}						
////					}
////				}
////			}
////			if(hitList.Count > 0)
////			{
////				foreach(StatsBase sBase in hitList)
////				{
////					sBase.ApplyDamage(damage);
////				}
////			}
////
////			fireRateTimer = 0.0f;
////			canShoot = false;
//			
//			//Method 2 - Raycasts from both ends
//			hitList.Clear();
//			localRight = Vector3.Normalize(transform.TransformDirection(Vector3.right));
//			//Vector3 baseVector = transform.position + localRight*startWidth/2.0f;
//			//Vector3 topVector = transform.position + Vector3.Normalize(transform.TransformDirection(Vector3.forward))*length - localRight*endWidth/2.0f;
//			localForward = Vector3.Normalize(transform.position + Vector3.Normalize(transform.TransformDirection(Vector3.forward))*length - localRight*endWidth/2.0f - transform.position + localRight*startWidth/2.0f);
//			for (int i=0; i<raycastNumber; ++i)
//			{
//				Vector3 startPos = transform.position -localRight*startWidth/2.0f + localForward*diagonalLength/((raycastNumber-1)*1.0f)*i;
//				//float diag = ((diagonalLength/((raycastNumber-1)*1.0f))*i);
//				//float vert = ((length/((raycastNumber-1)*1.0f))*i);
//				float distance = negativeModifier*Mathf.Sqrt((((diagonalLength/((raycastNumber-1)*1.0f))*i)*((diagonalLength/((raycastNumber-1)*1.0f))*i)) - (((length/((raycastNumber-1)*1.0f))*i)*((length/((raycastNumber-1)*1.0f))*i)))*2.0f + startWidth;
//				if(Physics.Raycast(startPos,localRight,distance))
//				{
//					foreach (RaycastHit hit in Physics.RaycastAll(startPos, localRight, distance))
//					{
//						bool isInList = false;
//						if(hit.collider.GetComponent<StatsBase>() != null)
//						{
//							StatsBase tempStatsBase = hit.collider.GetComponent<StatsBase>();
//							foreach(StatsBase sBase in hitList)
//							{
//								if(tempStatsBase == sBase)
//								{
//									isInList = true;
//									break;
//								}
//							}
//							
//							if(!isInList)
//							{
//								hitList.Add(hit.collider.GetComponent<StatsBase>());
//							}
//						}						
//					}
//				}
//				startPos = startPos + localRight*distance;
//				if(Physics.Raycast(startPos,-localRight,distance))
//				{
//					foreach (RaycastHit hit in Physics.RaycastAll(startPos, -localRight, distance))
//					{
//						bool isInList = false;
//						if(hit.collider.GetComponent<StatsBase>() != null)
//						{
//							StatsBase tempStatsBase = hit.collider.GetComponent<StatsBase>();
//							foreach(StatsBase sBase in hitList)
//							{
//								if(tempStatsBase == sBase)
//								{
//									isInList = true;
//									break;
//								}
//							}
//							
//							if(!isInList)
//							{
//								hitList.Add(hit.collider.GetComponent<StatsBase>());
//							}
//						}						
//					}
//				}
//			}
//			if(hitList.Count > 0)
//			{
//				foreach(StatsBase sBase in hitList)
//				{
//					sBase.ApplyDamage(damage);
//				}
//			}
//
//			fireRateTimer = 0.0f;
//			canShoot = false;
//		}
//	}
//	
//	void InitializeRaycastArea()
//	{
//		diagonalLength = Mathf.Sqrt((length*length) + (((endWidth-startWidth)/2.0f)*((endWidth-startWidth)/2.0f)));
//		if(startWidth < endWidth)
//		{
//			negativeModifier = 1;
//		}
//		else
//		{
//			negativeModifier = -1;
//		}
//	}	
//}
