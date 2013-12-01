using UnityEngine;
using System.Collections;

public class ATestScript : MonoBehaviour {
	
	public Vector3 VectorA;
	public Vector3 VectorB;
	public float MaxSpeed;
	
	// Update is called once per frame
	void Update () 
	{
		//Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//test1
		Vector3 dir = VectorA + VectorB;
		dir = dir.normalized * MaxSpeed;
		transform.Translate(dir * Time.deltaTime);
		
		//test 2
//		Vector3 dir = VectorA;
//		dir = dir.normalized * MaxSpeed;
//		transform.Translate(dir * Time.deltaTime);
//		Vector3 dir2 = VectorB;
//		dir2 = dir2.normalized * MaxSpeed;
//		transform.Translate(dir2 * Time.deltaTime);
	}
}
