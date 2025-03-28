using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class TextUtilities
{
    public static string ConvertAmmountToText(this int ammount)
    {
        if (ammount < 0)
        {
            return "0";
        }
        else if (ammount < 1000)
        {
            return ammount.ToString();
        }
        else if (ammount < 1000000)
        {
            var value = ammount / 1000;
            return $"{value}.{(ammount - value * 1000)/100}k";
        }
        else
        {
            var value = ammount / 1000000;
            return $"{value}.{(ammount - value * 1000000) / 100000}m";
        }
    }
}
