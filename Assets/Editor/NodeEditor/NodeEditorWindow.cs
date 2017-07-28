using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public enum NodeType
{
    InputNode,
    OutputNode,
    CompNode,
    CalcNode,
}
public class NodeEditorWindow : EditorWindow
{

    private List<BaseNode> _nodes = new List<BaseNode>();
    private Event _curevent;
    private GenericMenu _createNodeMenu;
    private GenericMenu _nodeMenu;
    private bool isLineMode = false;   //是否是画线模式
    private InputNode selectNode = null;

    [MenuItem("Window/Calac")]
    public static void OpenWindow()
    {
        GetWindow<NodeEditorWindow>("Calac");
    }

    public NodeEditorWindow()
    {
        _createNodeMenu = new GenericMenu();
        _createNodeMenu.AddItem(new GUIContent("Add Input"), false, CreateNode, NodeType.InputNode);
        _createNodeMenu.AddItem(new GUIContent("Add Output"), false, CreateNode, NodeType.OutputNode);
        _createNodeMenu.AddItem(new GUIContent("Add Calc"), false, CreateNode, NodeType.CalcNode);
        _createNodeMenu.AddItem(new GUIContent("Add Comp"), false, CreateNode, NodeType.CompNode);

        _nodeMenu = new GenericMenu();
        _nodeMenu.AddItem(new GUIContent("Delete Node"), false, NodeCallBack, "Delete");

    }

    //开始绘制  
    private void OnGUI()
    {
        _curevent = Event.current;

        //点击鼠标右键的时候.
        if (_curevent.isMouse && _curevent.type == EventType.mouseUp)
        {
            if (_curevent.button == 1 &&  !isLineMode)
            {
                int index = CheckMouseInNodes();
                if (index != -1)
                {
                    selectNode = _nodes[index] as InputNode;
                    
                    if(selectNode != null)
                        _nodeMenu.AddItem(new GUIContent("Start Line"), false, NodeCallBack, "Line");
                    else
                        _nodeMenu.AddDisabledItem(new GUIContent("Start Line"));

                    _nodeMenu.ShowAsContext();
                    _curevent.Use();
                }
                else
                {
                    _createNodeMenu.ShowAsContext();
                    _curevent.Use();
                }
            }
            if(isLineMode)
            {
                int index = CheckMouseInNodes();
                if(index != -1)
                {
					_nodes [index].HandleLine (_curevent.mousePosition, selectNode);
                }
                isLineMode = false;
            }
        }
        

        if (isLineMode)
        {
            DrawBezier(selectNode.nodeRect.center, _curevent.mousePosition);
            Repaint();
        }

        //维护画线功能  
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].DrawBezier();
        }

        BeginWindows(); //开始绘制弹出窗口  

        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].nodeRect = GUI.Window(i, _nodes[i].nodeRect, DrawChildNode, _nodes[i].nodeName);
        }

        EndWindows();//结束绘制弹出窗口


    }

    void DrawChildNode(int id)
    {
        _nodes[id].DrawNode();
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    void CreateNode(object key)
    {
        BaseNode node = null;
        switch((NodeType)key)
        {
            case NodeType.InputNode:
                node = new InputNode(this, _curevent.mousePosition);
                break;
            case NodeType.OutputNode:
                node = new OutputNode(this, _curevent.mousePosition);
                break;
			case NodeType.CalcNode:
				node = new CalcNode(this, _curevent.mousePosition);
				break;
            case NodeType.CompNode:
                node = new CompNode(this, _curevent.mousePosition);
                break;
        }
        if(node != null)
        {
            _nodes.Add(node);
        }
    }

    void NodeCallBack(object key)
    {
        switch(key.ToString())
        {
			case "Delete":
				for (int i = 0; i < _nodes.Count; i++) {
					_nodes [i].DeleteNode (selectNode);
				}
				_nodes.Remove (selectNode);
                break;
            case "Line":
                isLineMode = true;
                break;
        }
    }

    /*
     * 判断是否在节点中右键
     */
    int CheckMouseInNodes()
    {
        for (int i = 0; i < _nodes.Count; i++)
            if (_nodes[i].nodeRect.Contains(_curevent.mousePosition))
                return i;
        return -1;
    }

    /*
     * 绘制贝塞尔曲线
     */
    public void DrawBezier(Vector3 start, Vector3 end)
    {
        Vector3 startTan = start + Vector3.right * 50;
        Vector3 endTan = end + Vector3.left * 50;
        Color shadow = new Color(0, 0, 0, 0.7f);

        for (int i = 0; i < 2; i++)
        {
            Handles.DrawBezier(start, end, startTan, endTan, shadow, null, 1 + (i * 2));
        }
        Handles.DrawBezier(start, end, startTan, endTan, Color.black, null, 1);
    }

}
