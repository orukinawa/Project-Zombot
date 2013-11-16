using UnityEngine;
using System.Collections;

public class MotorPlayerInverted : MotorPlayer
{
	public override void Rotate (Vector3 facingDirection)
	{
		facingDirection *= -1.0f;
		base.Rotate (facingDirection);
	}
	
	public override void Dash (Vector3 dashDirection)
	{
		dashDirection *= -1.0f;
		base.Dash (dashDirection);
	}
}
