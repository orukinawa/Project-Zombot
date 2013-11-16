using UnityEngine;
using System.Collections;

public class PlayerMovementController : MonoBehaviour {

	public float moveSpeed = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float transX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
		float transY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
		
		transform.Translate(transX,0,transY); 
	}
}
