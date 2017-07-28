using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Class03
/// </summary>
//自定义Tset脚本
[CustomEditor(typeof(CubeCS))] 
//请继承Editor
public class CubeCSEditor : Editor 
{
	[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
	static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
	{   
		Handles.Label(transform.position, transform.gameObject.name);
	}

	void OnSceneGUI() 
	{
		
		//得到test脚本的对象
		CubeCS test = (CubeCS) target;

		//绘制文本框
		Handles.Label(test.transform.position + Vector3.up,
			test.transform.name +" : "+ test.transform.position.ToString() );

		//开始绘制GUI
		Handles.BeginGUI();

		//规定GUI显示区域
		GUILayout.BeginArea(new Rect(100, 100, 100, 100));

		//GUI绘制一个按钮
		if(GUILayout.Button("这是一个按钮!"))
		{
			Debug.Log("test");		
		}
		//GUI绘制文本框
		GUILayout.Label("我在编辑Scene视图");	

		GUILayout.EndArea();

		Handles.EndGUI();
	}

}

