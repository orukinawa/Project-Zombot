using UnityEngine;
using System.Collections;

public class RotationOverTime : MonoBehaviour
{
	Transform mTransform;
	public Vector3 rotationVector;
	public float rotationSpeed;
	
	void Start()
	{
		mTransform = transform;
		mTransform.Rotate(rotationVector * Random.Range(0.0f,90.0f));
		rotationSpeed = Random.Range(rotationSpeed/2,rotationSpeed);
	}	
	
	void Update()
	{
		mTransform.Rotate(rotationVector * Time.deltaTime * rotationSpeed);
	}
}
