using UnityEngine;
using System.Collections;

[System.Serializable]

public struct PanRect
{
	public float left;
	public float right;
	public float top;
	public float bottom;

	public PanRect(float _left, float _right, float _top, float _bottom)
	{
		this.left = _left;
		this.right = _right;
		this.top = _top;
		this.bottom = _bottom;
	}

	public static PanRect GetSqRect(float halfArmLength)
	{
		return new PanRect(halfArmLength,halfArmLength,halfArmLength,halfArmLength);
	}
}
