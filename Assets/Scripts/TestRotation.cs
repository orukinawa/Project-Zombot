using UnityEngine;
using System.Collections;

public class TestRotation : MonoBehaviour
{
	public float speed;
	
	void Start()
	{
		transform.Rotate(new Vector3(45,0,0));
	}
	
	void Update ()
	{
		
		//transform.position += Vector3.forward * speed * Time.deltaTime;
	}
}
