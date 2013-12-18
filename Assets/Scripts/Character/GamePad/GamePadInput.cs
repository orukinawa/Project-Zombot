using UnityEngine;
using System;
using System.Collections;
using XInputDotNetPure;

public class GamePadInput : MonoBehaviour
{
	public enum ButtonType
	{
		A,
		B,
		X,
		Y,
		LEFT_BUMPER,
		RIGHT_BUMBER,
		START,
		BACK,
		LEFT_STICK,
		RIGHT_STICK
	}
	
	public enum AxisType
	{
		LEFT_STICK_X,
		LEFT_STICK_Y,
		RIGHT_STICK_X,
		RIGHT_STICK_Y,
		LEFT_TRIGGER,
		RIGHT_TRIGGER
	}
	
	public PlayerIndex mPlayerIndex;
	float vibrationPower = 0.5f;
	
	GamePadState mGamePadState;
	
    ButtonState[] currentButtonStates = new ButtonState[10];
    ButtonState[] previousButtonStates = new ButtonState[10];
	
	float[] currentAxisValues = new float[6];
	
	bool isVibratingConstant = false;
	
	bool isVibratingOnce = false;
	float vibrationDuration = 0.1f;
	float vibrationTimer = 0.0f;
	
	void Update()
	{
		mGamePadState = GamePad.GetState(mPlayerIndex);
		_updateAxes();
		_updateButtons();
		_updateVibration();
	}
	
	void _updateAxes()
	{
		currentAxisValues[0] = mGamePadState.ThumbSticks.Left.X;
		currentAxisValues[1] = mGamePadState.ThumbSticks.Left.Y;
		currentAxisValues[2] = mGamePadState.ThumbSticks.Right.X;
		currentAxisValues[3] = mGamePadState.ThumbSticks.Right.Y;
		currentAxisValues[4] = mGamePadState.Triggers.Left;
		currentAxisValues[5] = mGamePadState.Triggers.Right;
	}
	
	void _updateButtons()
	{
		Array.Copy(currentButtonStates,previousButtonStates,10);
		currentButtonStates[0] = mGamePadState.Buttons.A;
		currentButtonStates[1] = mGamePadState.Buttons.B;
		currentButtonStates[2] = mGamePadState.Buttons.X;
		currentButtonStates[3] = mGamePadState.Buttons.Y;
		currentButtonStates[4] = mGamePadState.Buttons.LeftShoulder;
		currentButtonStates[5] = mGamePadState.Buttons.RightShoulder;
		currentButtonStates[6] = mGamePadState.Buttons.Start;
		currentButtonStates[7] = mGamePadState.Buttons.Back;
		currentButtonStates[8] = mGamePadState.Buttons.LeftStick;
		currentButtonStates[9] = mGamePadState.Buttons.RightStick;
	}
	
	void _updateVibration()
	{
		if(!isVibratingOnce) return;
		if(vibrationTimer > vibrationDuration)
		{
			isVibratingOnce = false;
			if(isVibratingConstant) return;
			GamePad.SetVibration(mPlayerIndex, 0.0f,0.0f);
			return;
		}
		vibrationTimer += Time.deltaTime;
	}
	
	public float GetAxis(AxisType type)
	{
		return currentAxisValues[(int)type];
	}
	
	public bool GetButton(ButtonType type)
	{
		if(currentButtonStates[(int)type] == ButtonState.Pressed) return true;
		return false;
	}
	
	public bool GetButtonDown(ButtonType type)
	{
		int index = (int)type;
		if(currentButtonStates[index] == ButtonState.Released) return false;
		if(previousButtonStates[index] == ButtonState.Released) return true;
		return false;
	}
	
	public bool GetButtonUp(ButtonType type)
	{
		int index = (int)type;
		if(currentButtonStates[index] == ButtonState.Pressed) return false;
		if(previousButtonStates[index] == ButtonState.Pressed) return true;
		return false;
	}
	
	public void VibrateOnce()
	{
		isVibratingOnce = true;
		vibrationTimer = 0.0f;
		GamePad.SetVibration(mPlayerIndex, vibrationPower,vibrationPower);	
	}
	
	public void VibrateStart()
	{
		if(isVibratingConstant) return;
		isVibratingConstant = true;
		GamePad.SetVibration(mPlayerIndex, vibrationPower,vibrationPower);
	}
	
	public void VibrateStop()
	{
		if(!isVibratingConstant) return;
		isVibratingConstant = false;
		GamePad.SetVibration(mPlayerIndex, 0.0f,0.0f);
	}
	
//	void OnGUI()
//	{
//		GUILayout.BeginVertical();
//		GUILayout.Label("A: " + currentButtonStates[0]);
//		GUILayout.Label("B: " + currentButtonStates[1]);
//		GUILayout.Label("X: " + currentButtonStates[2]);
//		GUILayout.Label("Y: " + currentButtonStates[3]);
//		GUILayout.Label("LB: " + currentButtonStates[4]);
//		GUILayout.Label("RB: " + currentButtonStates[5]);
//		GUILayout.Label("Start: " + currentButtonStates[6]);
//		GUILayout.Label("Back: " + currentButtonStates[7]);
//		GUILayout.Label("LS: " + currentButtonStates[8]);
//		GUILayout.Label("RS: " + currentButtonStates[9]);
//		GUILayout.Label("");
//		GUILayout.Label("LeftX: " + currentAxisValues[0]);
//		GUILayout.Label("LeftY: " + currentAxisValues[1]);
//		GUILayout.Label("RightX: " + currentAxisValues[2]);
//		GUILayout.Label("RightY: " + currentAxisValues[3]);
//		GUILayout.Label("LT: " + currentAxisValues[4]);
//		GUILayout.Label("RT: " + currentAxisValues[5]);
//		GUILayout.Label("");
//		GUILayout.Label("A: " + GetButton(ButtonType.A));
//		GUILayout.Label("A Down: " + GetButtonDown(ButtonType.A));
//		GUILayout.Label("A Up: " + GetButtonUp(ButtonType.A));
//	}
}
