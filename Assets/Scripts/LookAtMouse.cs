using UnityEngine;
using System.Collections;

public class LookAtMouse : MonoBehaviour {
	
	public float turnSpeed = 1.0f;
	Vector3 targetPoint;
	Vector3 mousePos;
	
	public FollowPlayer test;
	
	//Determines where the camera is centred between Player and Cursor
	public float focusRatio;
	
	// Use this for initialization
	void Start () 
	{
		test = FindObjectOfType(typeof(FollowPlayer)) as FollowPlayer;
		test.SetLookAtMouseScript(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		mousePos = Input.mousePosition;
		
		if(mousePos.x < 0.0f)
			mousePos.x = 0.0f;
		
		if(mousePos.x > Screen.width)
			mousePos.x = Screen.width;
		
		if(mousePos.y < 0.0f)
			mousePos.y = 0.0f;
		
		if(mousePos.y > Screen.height)
			mousePos.y = Screen.height;		
		
		//Disable camera movement when cursor out of screen
		//if(mousePos.x > 0.0f && mousePos.x < Screen.width && mousePos.y > 0.0f && mousePos.y < Screen.height)
		//{
			// Generate a plane that intersects the transform's position with an upwards normal.
	        Plane playerPlane = new Plane(Vector3.up, transform.position);
	   
	        // Generate a ray from the cursor position
	        Ray ray = Camera.main.ScreenPointToRay (mousePos);
	   
	        // Determine the point where the cursor ray intersects the plane.
	        // This will be the point that the object must look towards to be looking at the mouse.
	        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
	        //   then find the point along that ray that meets that distance.  This will be the point
	        //   to look at.
	        float hitdist = 0.0f;
	        // If the ray is parallel to the plane, Raycast will return false.
	        if (playerPlane.Raycast (ray, out hitdist))
	        {
	            // Get the point along the ray that hits the calculated distance.
	            targetPoint = ray.GetPoint(hitdist);
	       
	            //Determine the target rotation.  This is the rotation if the transform looks at the target point.
	            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
	       
	            // Smoothly rotate towards the target point.
	            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.time);
				
				//transform.position = targetPoint;
	        }
		//}
	}
	
	public Vector3 GetMidPoint()
	{
		return (transform.position);// + targetPoint*focusRatio);
	}
}
