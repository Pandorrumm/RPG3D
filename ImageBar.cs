using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBar : BarBase
{
    [SerializeField]
    private Image image;

    public override void SetValue(float value, float maxValue) 
    {
        base.SetValue(value, maxValue);

        image.fillAmount = value / maxValue;

    }
}

