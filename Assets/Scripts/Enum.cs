using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficLightState
{
   Red = 0, 
   Yellow = 1, 
   Green = 2,
}

public enum Tag
{
    Goal = 0,
    Vehicle = 1,
    Intersection4 = 2,
    Intersection3 = 3,
    Road = 4,
    Queue = 5,
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
