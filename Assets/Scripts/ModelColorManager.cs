using UnityEngine;
using System.Collections;

public class ModelColorManager : MonoBehaviour
{
	public Color hitColor;
	private Color originalColor;
	
	private bool isColorChange = false;
	public float colorChangeDuration = 0.0f;
	private float colorChangeTimer = 0.0f;
	
	void Start ()
	{
		originalColor = renderer.material.color;
	}
	
	void Update()
	{
		if(isColorChange)
		{
			colorChangeTimer += Time.deltaTime;
			if(colorChangeTimer > colorChangeDuration)
			{
				renderer.material.color = originalColor;
				colorChangeTimer = 0.0f;
				isColorChange = false;
			}
		}
	}
	
	public void ChangeColor()
	{
		isColorChange = true;
		renderer.material.color = hitColor;
		colorChangeTimer = 0.0f;
	}
	
	public Color GetOriginalColor()
	{
		return originalColor;
	}
	
	public void SetColor(Color newColor)
	{
		renderer.material.color = newColor;
	}
	
	public void ResetColor()
	{
		renderer.material.color = originalColor;
	}
}
