using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Class04
/// </summary>
public class GetSetValue : MonoBehaviour
{
    [SerializeField,SetProperty("Data")]
    private float data;
    public float Data
    {
        get
        {
            return data;
        }
        set
        {
            Debug.Log("new data :" + value);
            data = value;
        }
    }
}