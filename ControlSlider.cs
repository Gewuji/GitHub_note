using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlSlider : MonoBehaviour
{
    public Slider slider;

    public void ResetValue()
    {
        slider.value = 0;
    }

    public void ChangeValue(float value)
    {
        slider.value += value;
        //Debug.Log("进度条参数:" + slider.value);
    }

}
