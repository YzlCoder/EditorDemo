using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Class01
/// </summary>
[CustomEditor(typeof(RectAndTexture))]
public class RectAndTextureEditor : Editor {

	public override void OnInspectorGUI ()
	{
		RectAndTexture rt = (RectAndTexture)target;
		rt.mRectValue = EditorGUILayout.RectField ("窗口坐标", rt.mRectValue);
		rt.texture = EditorGUILayout.ObjectField ("贴图", 
			rt.texture, typeof(Texture), true) as Texture;
		rt.go = EditorGUILayout.ObjectField ("物体", 
			rt.go, typeof(GameObject), true) as GameObject;
	}

}
