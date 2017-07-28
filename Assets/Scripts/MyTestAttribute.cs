using UnityEngine;
using System.Collections;
/// <summary>
/// Class06
/// </summary>
public class MyTestAttribute : PropertyAttribute
{

    public int max;
    public int min;

    public MyTestAttribute(int a, int b)
    {
        min = a;
        max = b;
    }
}