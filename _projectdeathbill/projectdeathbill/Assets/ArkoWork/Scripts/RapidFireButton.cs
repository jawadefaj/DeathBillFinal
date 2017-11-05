using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RapidFireButton : MonoBehaviour , IPointerExitHandler, IPointerDownHandler , IPointerUpHandler
{
    //public static bool 
    public bool pressedOn;
    public static RapidFireButton instance;
    void Awake()
    {
        instance = this;
        pressedOn = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        pressedOn = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        pressedOn = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        pressedOn = false;
    }
}