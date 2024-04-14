using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficLightState
{
   Red = 0, 
   Yellow = 1, 
   Green = 2,
}

public enum InputMode
{
    Default = 0,
    Build = 1,
}

public enum BuildMode
{
    Preview = 0,
    Actual = 1,
}

public static class Tag
{
    public static readonly string Goal = "Goal";
    public static readonly string Vehicle = "Vehicle";
    public static readonly string Intersection_3_Small = "Intersection_3_Small";
    public static readonly string Intersection_4_Small = "Intersection_4_Small";
    public static readonly string Intersection_3_Large = "Intersection_3_Large";
    public static readonly string Intersection_4_Large = "Intersection_4_Large";
    public static readonly string Road_Small = "Road_Small";
    public static readonly string Road_Large = "Road_Large";
    public static readonly string Queue = "Queue";

    public static bool CompareTags(Transform target, params string[] tags)
    {
        foreach (string tag in tags)
        {
            if (target.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}

public static class OnLanes
{
    private static readonly Dictionary<string, float> values = new Dictionary<string, float>
    {
        {"Low", -6f},
        {"High", -2f}
    };

    public static float GetValue(string key)
    {
        if (values.ContainsKey(key))
        {
            return values[key];
        }
        else
        {
            throw new ArgumentException($"No value associated with key '{key}'.");
        }
    }
}
