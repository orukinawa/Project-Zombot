using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour
{
	GamePadInput mGamePadInput;
	
	void Start()
	{
		mGamePadInput = GetComponent<GamePadInput>();
	}
	
	void Update()
	{
		if(mGamePadInput.GetButtonDown(GamePadInput.ButtonType.START))
		{
			Application.LoadLevel("PrototypeLevel");
		}
	}
}
