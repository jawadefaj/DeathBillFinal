using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIKnoobScroller : MonoBehaviour {

	public Image knob1;
	public Image knob2;
	public Image knob3;

	public bool horizontalKnob = true;

	void Start()
	{
		SwitchToK1();
	}

	public void OnScrolled(Vector2 value)
	{
		float v = horizontalKnob==true?value.x:value.y;

		if(v<0.33f)
		{
			if(horizontalKnob)
				SwitchToK1();
			else
				SwitchToK3();
		}
		else if (v<0.67f)
		{
			SwitchToK2();
		}
		else
		{
			if(horizontalKnob)
				SwitchToK3();
			else
				SwitchToK1();
		}
	}

	private void SwitchToK1()
	{
		knob1.color += new Color(0,0,0,1);

		knob2.color *= new Color(1,1,1,0);
		knob2.color += new Color(0,0,0,0.274f);

		knob3.color *= new Color(1,1,1,0);
		knob3.color += new Color(0,0,0,0.274f);
	}

	private void SwitchToK2()
	{
		knob2.color += new Color(0,0,0,1);

		knob1.color *= new Color(1,1,1,0);
		knob1.color += new Color(0,0,0,0.274f);

		knob3.color *= new Color(1,1,1,0);
		knob3.color += new Color(0,0,0,0.274f);
	}

	private void SwitchToK3()
	{
		knob3.color += new Color(0,0,0,1);

		knob2.color *= new Color(1,1,1,0);
		knob2.color += new Color(0,0,0,0.274f);

		knob1.color *= new Color(1,1,1,0);
		knob1.color += new Color(0,0,0,0.274f);
	}
}
