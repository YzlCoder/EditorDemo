using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;

public class InputHandle
{
	public Rect area;
	public InputNode node = null;
}
public abstract class BaseNode
{
	
    protected NodeEditorWindow _parent;
	protected List<InputHandle> _handle = new List<InputHandle>();

    public string nodeName;
    public Rect nodeRect;

    public BaseNode(NodeEditorWindow window)
    {
        _parent = window;
    }
		
	public virtual bool HandleLine(Vector2 position, InputNode node)
	{
		position = position - nodeRect.position;
		for (int i = 0; i < _handle.Count; i++) {
			if (_handle [i].area.Contains (position)) {
				_handle [i].node = node;
				return true;
			}
		}
		return false;
	}

	public virtual void DeleteNode (BaseNode node)
	{
		for (int i = 0; i < _handle.Count; i++) {
			if (_handle [i].node == node) {
				_handle [i].node = null;
			}
		}
	}

	public abstract void DrawNode();
	public abstract void DrawBezier();
}

public class InputNode : BaseNode
{
    protected float value;
    public InputNode(NodeEditorWindow window, Vector2 position):base(window)
    {
        nodeName = "InputNode";
        nodeRect = new Rect(position, new Vector2(220, 80));
        value = 0;
    }

    public virtual float GetValue()
    {
        return value;
    }

    public override void DrawNode()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        value = EditorGUILayout.FloatField("值: ",value);
    }

    public override void DrawBezier()
    {
        
    }
}

public class OutputNode : BaseNode
{
    private string result;

    public OutputNode(NodeEditorWindow window,Vector2 position) : base(window)
    {
		_handle.Add (new InputHandle ());
        nodeName = "OutputNode";
        nodeRect = new Rect(position, new Vector2(220, 80));
        result = "";
    }

    public void SetInputNode(InputNode innode)
    {
		_handle[0].node = innode;
    }

    public override void DrawNode()
    {
		if (_handle[0].node != null)
			result = _handle[0].node.GetValue().ToString();
        else
            result = "";
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("结果: ", result);
        if (Event.current.type == EventType.Repaint)
        {
			_handle[0].area = GUILayoutUtility.GetLastRect();
        }
    }

    public override void DrawBezier()
    {
		if(_handle[0].node != null)
        {
			Vector3 end = new Vector2 (_handle [0].area.xMin - 15, _handle [0].area.center.y) + nodeRect.position;
			Vector2 start = _handle [0].node.nodeRect.center;
			_parent.DrawBezier(start, end);
			Handles.color = Color.white;
			end.z = -100;
			Handles.ConeCap(1,end,Quaternion.FromToRotation (Vector3.back, Vector3.left),18);
        }
    }
}

public enum CalcType
{
    Add = '+',       // +
    Sub = '-',       // -
    Mul = '*',       // *
    Dev = '/',       // /
}

public class CalcNode : InputNode
{
    private readonly float ESP = 0.000001f;

	private CalcType calcType = CalcType.Add;
	private float result = 0;
    
	public CalcNode(NodeEditorWindow window,Vector2 position) : base(window,position)
    {
		_handle.Add (new InputHandle ());
		_handle.Add (new InputHandle ());
        nodeName = "CalcNode";
        nodeRect = new Rect(position, new Vector2(240, 130));
    }

    public override float GetValue()
    {
		if (_handle[0].node == null || _handle[1].node == null)
            return 0;
        UpdateResult();
        return result;
    }

    public override void DrawNode()
    { 
        EditorGUILayout.Space();
		calcType = (CalcType)EditorGUILayout.EnumPopup("计算类型: ", calcType);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
		EditorGUILayout.TextField("左操作数: ", _handle[0].node == null ? "" : _handle[0].node.GetValue().ToString());
        if (Event.current.type == EventType.Repaint)
        {
			_handle[0].area = GUILayoutUtility.GetLastRect();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
		EditorGUILayout.TextField("右操作数: ", _handle[1].node == null ? "" : _handle[1].node.GetValue().ToString());
        if (Event.current.type == EventType.Repaint)
        {
			_handle[1].area = GUILayoutUtility.GetLastRect();
        }
    }
		
    public override void DrawBezier()
    {
		if (_handle[0].node != null)
        {
			_parent.DrawBezier(_handle[0].node.nodeRect.center, 
				new Vector2(_handle[0].area.xMin, _handle[0].area.center.y) + nodeRect.position);
        }
		if(_handle[1].node != null)
        {
			_parent.DrawBezier(_handle[1].node.nodeRect.center, 
				new Vector2(_handle[1].area.xMin, _handle[1].area.center.y) + nodeRect.position);
        }
    }
    
    void UpdateResult()
    {
		if (_handle[0].node == null || _handle[1].node == null)
            return;
		switch(calcType)
        {
		case CalcType.Add:
			result = _handle[0].node.GetValue () + _handle[1].node.GetValue ();
            break;
		case CalcType.Sub:
			result = _handle[0].node.GetValue () - _handle[1].node.GetValue ();
			break;
		case CalcType.Mul:
			result = _handle[0].node.GetValue () * _handle[1].node.GetValue ();
			break;
		case CalcType.Dev:
			result = _handle[0].node.GetValue () / _handle[1].node.GetValue ();
			break;
        }
    }

}

public enum CompType
{
	Greater,       //>
	Less,          //<
	Equal,         //=
	NoEquial,      //!=
}

public class CompNode : InputNode
{
	private readonly float ESP = 0.000001f;
	private CompType comType = CompType.Equal;
	private int result = 0;

	public CompNode(NodeEditorWindow window,Vector2 position) : base(window,position)
	{
		_handle.Add (new InputHandle ());
		_handle.Add (new InputHandle ());
		nodeName = "CompNode";
		nodeRect = new Rect(position, new Vector2(240, 130));
	}

	public override float GetValue()
	{
		if (_handle[0].node == null || _handle[1].node == null)
			return 0;
		UpdateResult();
		return result;
	}

	public override void DrawNode()
	{ 
		EditorGUILayout.Space();
		comType = (CompType)EditorGUILayout.EnumPopup("比较类型: ", comType);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.TextField("左操作数: ", _handle[0].node == null ? "" : _handle[0].node.GetValue().ToString());
		if (Event.current.type == EventType.Repaint)
		{
			_handle[0].area = GUILayoutUtility.GetLastRect();
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.TextField("右操作数: ", _handle[1].node == null ? "" : _handle[1].node.GetValue().ToString());
		if (Event.current.type == EventType.Repaint)
		{
			_handle[1].area = GUILayoutUtility.GetLastRect();
		}
	}
		
	public override void DrawBezier()
	{
		if (_handle[0].node != null)
		{
			_parent.DrawBezier(_handle[0].node.nodeRect.center, 
				new Vector2(_handle[0].area.xMin, _handle[0].area.center.y) + nodeRect.position);
		}
		if(_handle[1].node != null)
		{
			_parent.DrawBezier(_handle[1].node.nodeRect.center, 
				new Vector2(_handle[1].area.xMin, _handle[1].area.center.y) + nodeRect.position);
		}
	}

	void UpdateResult()
	{
		if (_handle[0].node == null || _handle[1].node == null)
			return;
		switch(comType)
		{
		case CompType.Equal:
			result = Mathf.Abs(_handle[0].node.GetValue() - _handle[1].node.GetValue()) <= ESP ? 1:0;
			break;
		case CompType.NoEquial:
			result = Mathf.Abs(_handle[0].node.GetValue() - _handle[1].node.GetValue()) > ESP ? 1 : 0;
			break;
		case CompType.Greater:
			result = _handle[0].node.GetValue() > _handle[1].node.GetValue() ? 1 : 0;
			break;
		case CompType.Less:
			result = _handle[0].node.GetValue() < _handle[1].node.GetValue() ? 1 : 0;
			break;
		}
	}

}