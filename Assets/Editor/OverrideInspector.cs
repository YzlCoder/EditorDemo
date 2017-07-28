
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Class07
/// </summary>
[CustomEditor(typeof(DefaultAsset))]
public class CustomInspector : Editor
{
    Data data;
    Data selectData;
    void OnEnable()
    {
        if (Directory.Exists(AssetDatabase.GetAssetPath(target)))
        {
            data = new Data();
            LoadFiles(data, AssetDatabase.GetAssetPath(Selection.activeObject));
        }

    }
    public override void OnInspectorGUI()
    {

        if (Directory.Exists(AssetDatabase.GetAssetPath(target)))
        {
            GUI.enabled = true;
            EditorGUIUtility.SetIconSize(Vector2.one * 16);
            DrawData(data);
        }

    }

    void LoadFiles(Data data, string currentPath, int indent = 0)
    {
        GUIContent content = GetGUIContent(currentPath);

        if (content != null)
        {
            data.indent = indent;
            data.content = content;
            data.assetPath = currentPath;

        }

        foreach (var path in Directory.GetFiles(currentPath))
        {
            content = GetGUIContent(path);
            if (content != null)
            {
                Data child = new Data();
                child.indent = indent + 1;
                child.content = content;
                child.assetPath = path;
                data.childs.Add(child);
            }
        }


        foreach (var path in Directory.GetDirectories(currentPath))
        {
            Data childDir = new Data();
            data.childs.Add(childDir);
            LoadFiles(childDir, path, indent + 1);
        }
    }



    void DrawData(Data data)
    {
        if (data.content != null)
        {
            EditorGUI.indentLevel = data.indent;
            DrawGUIData(data);

        }
        for (int i = 0; i < data.childs.Count; i++)
        {
            Data child = data.childs[i];
            if (child.content != null)
            {
                EditorGUI.indentLevel = child.indent;
                if (child.childs.Count > 0)
                    DrawData(child);
                else
                    DrawGUIData(child);
            }
        }
    }




    void DrawGUIData(Data data)
    {
        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(data.content, style);
        if (data.isSelected)
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }

        rt.x += (16 * EditorGUI.indentLevel);
        if (GUI.Button(rt, data.content, style))
        {
            if (selectData != null)
            {
                selectData.isSelected = false;
            }
            data.isSelected = true;
            selectData = data;
            Debug.Log(data.assetPath);
        }
    }

    GUIContent GetGUIContent(string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        if (asset)
        {
            return new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
        }
        return null;
    }

    class Data
    {
        public bool isSelected = false;
        public int indent = 0;
        public GUIContent content;
        public string assetPath;
        public List<Data> childs = new List<Data>();
    }
}

[CustomEditor(typeof(SceneAsset))]
public class CustomSceneInspector : Editor
{
    List<Data> datas = new List<Data>();

    public override void OnInspectorGUI()
    {
        Event e = Event.current;
        string path = AssetDatabase.GetAssetPath(target);

        GUI.enabled = true;
        if (path.EndsWith (".unity")) 
        {
            Draw();
            if (e.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                e.Use();
            }
            if (e.type == EventType.DragPerform)
            {

                Object o1 = DragAndDrop.objectReferences[0];
                if (o1 is GameObject)
                {
                    datas.Add(new Data() { go = o1 as GameObject });
                }
            }
            
        }
    }

    Vector2 scrollPos = Vector2.zero;
    void Draw()
    {

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (Data data in datas)
        {
            var editor = Editor.CreateEditor(data.go);
            data.fold = EditorGUILayout.InspectorTitlebar(data.fold, data.go);


            if (data.fold)
            {
                editor.OnInspectorGUI();
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                foreach (Component c in data.go.GetComponents<Component>())
                {
                    if (!data.editors.ContainsKey(c))
                        data.editors.Add(c, Editor.CreateEditor(c));
                }
                foreach (Component c in data.go.GetComponents<Component>())
                {
                    if (data.editors.ContainsKey(c))
                    {
                        data.foldouts[c] = EditorGUILayout.InspectorTitlebar(data.foldouts.ContainsKey(c) ? data.foldouts[c] : true, c);
                        if (data.foldouts[c])
                            data.editors[c].OnInspectorGUI();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

        }
        EditorGUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    class Data
    {
        public GameObject go;
        public bool fold = true;
        public Dictionary<Object, Editor> editors = new Dictionary<Object, Editor>();
        public Dictionary<Object, bool> foldouts = new Dictionary<Object, bool>();
    }
}