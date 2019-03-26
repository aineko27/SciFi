using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunction
{
    public static int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }

    public static int[] RandomIndexArray(int arrayCount)
    {
        int[] randomIndexArray = new int[arrayCount];
        List<int> indexList = new List<int>();
        for(int i = 0; i< arrayCount; i++)
        {
            indexList.Add(i);
        }

        for(int i = 0; i < arrayCount; i++)
        {
            int randomNumber = Random.Range(0, indexList.Count);
            randomIndexArray[i] = indexList[randomNumber];

            indexList.RemoveAt(randomNumber);
        }

        return randomIndexArray;
    }

    public static int LimitRangeInt(int min, int med, int max)
    {
        return Mathf.Min(Mathf.Max(min, med), max);
    }

    public static float LimitRangeFloat(float min, float med, float max)
    {
        return Mathf.Min(Mathf.Max(min, med), max);
    }

    public static Color ToColor(this string self)
    {
        var color = default(Color);
        if (!ColorUtility.TryParseHtmlString(self, out color))
        {
            Debug.LogWarning("Unknown color code... " + self);
        }
        return color;
    }
}
