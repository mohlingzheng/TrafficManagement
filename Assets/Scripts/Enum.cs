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
    Remove = 2,
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
    public static readonly string Transition = "Transition";
    public static readonly string End_Small = "End_Small";
    public static readonly string End_Large = "End_Large";
    public static readonly string Bump = "Bump";
    public static readonly string Transition_Wall = "Transition_Wall";
    public static readonly string Untagged = "Untagged";

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
    public static readonly string Large = "Large";
    public static readonly string Small = "Small";

    public static readonly string High = "High";
    public static readonly string Low = "Low";

    public static readonly string Transition_2_4 = "Transition_2_4";
    public static readonly string Transition_4_2 = "Transition_4_2";


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

public enum GraphMode
{
    Original = 0,
    Modified = 1,
}

public enum VehicleType
{
    Light = 0,
    Heavy = 1,
}

public static class AnchorType
{
    public static readonly string anchor_north = "Anchor North";
    public static readonly string anchor_east = "Anchor East";
    public static readonly string anchor_south = "Anchor South";
    public static readonly string anchor_west = "Anchor West";
}

public static class TimeScale
{
    public static readonly float stop = 0f;
    public static readonly float normal = 1.0f;
    public static readonly float fast = 2.0f;
    public static readonly float very_fast = 5.0f;
}

public static class WeightConstant
{
    public static readonly float TrafficDensityMin = 0f;
    public static readonly float TrafficDensityMax = 0.089f;
    public static readonly float TrafficDensityCoefficient = 102.498f;
}

public static class FixedWaitedTime
{
    public static readonly List<double> WaitedTime = new()
    {
        0.106608245f,
        10.9014924f,
        36.18398388f,
        55.28241246f,
        87.79218312f,
        144.5410181f,
        191.77911f,
        231.6899636f,
        325.1652985f,
        427.0959541f,
        479.145313f,
        536.5426905f,
        611.9491346f,
        656.8358437f,
        675.0748294f,
        682.9953039f,
        671.9399782f,
        666.9417229f,
        676.7217201f,
        687.1219404f,
        685.2956278f,
        673.4734966f,
        670.6585677f,
        645.43039f,
        617.7233957f,
        607.4294434f,
        604.3465966f,
        573.9005115f,
        556.4151366f,
        544.3526063f,
        533.5279742f,
        494.0914408f,
        464.3620349f,
        417.9878491f,
        396.4358706f,
        379.1565585f,
        341.5532327f,
        329.5819528f,
        328.6682493f,
        321.238547f,
        319.0635228f,
        339.5439418f,
        345.6344994f,
        340.3745556f,
        357.0114359f,
        366.5468077f,
        361.7452576f,
        358.6849396f,
        370.9345083f,
        370.1813933f,
        363.8845796f,
        364.3541411f,
        372.6266443f,
        365.8192873f,
        356.2589731f,
        365.3936084f,
        363.6989978f,
        348.2745932f,
        342.7318721f,
        227
    };
}

public static class FixedQueuePoints
{
    public static readonly List<Vector3> QueuePoints = new List<Vector3>
    {
        new Vector3(165.5f, 0f, 30.3f),
        new Vector3(82.8f, 0f, 174.0f),
        new Vector3(6.6f, 0f, 233.0f),
        new Vector3(4.4f, 0f, 380.1f),
        new Vector3(5.5f, 0f, 545.1f),
        new Vector3(366f, 0f, 706.1f),
        new Vector3(425.8f, 0f, 693.8f),
        new Vector3(529.9f, 0f, 706.5f),
        new Vector3(605.1f, 0f, 703.9f),
        new Vector3(740.3f, 0f, 700.8f),
        new Vector3(841f, 0f, 704.8f),
        new Vector3(974f, 0f, 699.8f),
        new Vector3(1317f, 0f, 704.4f),
        new Vector3(1435.5f, 0f, 704.1f),
        new Vector3(1477.6f, 0f, 372f),
        new Vector3(1516.9f, 0f, 336.2f),
        new Vector3(1502.2f, 0f, 244f),
        new Vector3(1483.7f, 0f, 30.6f),
        new Vector3(1207.8f, 0f, 12f),
        new Vector3(1086.6f, 0f, 7.8f),
        new Vector3(906f, 0f, 6.5f),
        new Vector3(866.2f, 0f, 7.3f),
        new Vector3(654.8f, 0f, 10.4f),
        new Vector3(506f, 0f, 9.9f),
        new Vector3(303.6f, 0f, 11.6f),
        new Vector3(225.7f, 0f, 10.5f),
    };
}