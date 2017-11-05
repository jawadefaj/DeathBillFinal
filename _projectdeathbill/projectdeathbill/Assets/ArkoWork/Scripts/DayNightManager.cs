using UnityEngine;
using System.Collections;

public class DayNightManager : MonoBehaviour {

	public Light directionalLight;
	public GameObject pointLightGroup;

	//Day settings
	public Material dayMaterial;
	public Color dayColor;
	public float dayLightIntensity;

	//Night Settings
	public Material nightMaterial;
	public Color nightColor;
	public float nightLightIntensity;

	public void ChangeToDay()
	{
		RenderSettings.skybox = dayMaterial;
		directionalLight.color = dayColor;
		directionalLight.intensity = dayLightIntensity;

		pointLightGroup.SetActive(false);
	}

	public void ChangeToNight()
	{
		RenderSettings.skybox = nightMaterial;
		directionalLight.color = nightColor;
		directionalLight.intensity = nightLightIntensity;

		pointLightGroup.SetActive(true);
	}

	// Use this for initialization
	/*void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Q))
		{
			ChangeToDay();
		}
		if(Input.GetKeyDown(KeyCode.W))
		{
			ChangeToNight();
		}
	
	}*/
}
