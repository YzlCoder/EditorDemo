using UnityEngine;
using System.Collections;

/// <summary>
/// Class01
/// </summary>
public class RectAndTexture : MonoBehaviour {

	public Rect mRectValue;
	public Texture texture;
	public GameObject go;

	void Start()
	{
		Debug.Log (mRectValue);
		Debug.Log (texture);
		Debug.Log (go);
	}



}
