using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBar : BarBase
{
    public override void SetValue(float value, float maxValue) 
    {
        base.SetValue(value, maxValue);

        transform.localScale = new Vector3(
                                           transform.localScale.x * value / maxValue,
                                           transform.localScale.y,
                                           transform.localScale.z
                                           );
    }
}
