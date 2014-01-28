using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {
	
	public bool endGame = false;
	void OnTriggerEnter(Collider other)
	{
		StatsCharacter statChar = other.GetComponent<StatsCharacter>();
		if(statChar)
		{
			endGame = true;
		}
	}
	
	void OnGUI()
	{
		if(endGame)
		{
			if(GUI.Button(new Rect(Screen.width * 0.5f,Screen.height * 0.5f,400,100),"QUIT GAME!"))
			{
				Application.Quit();
			}
		}
	}
}
