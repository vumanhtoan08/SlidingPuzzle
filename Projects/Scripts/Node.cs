using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private int _nodeValue = 0; // giá trị của node
    private int _nodePosition = 0; // vị trí của node đang đứng

    public int NodeValue => _nodeValue;
    public int NodePosition => _nodePosition;

    private Node _topNode;
    private Node _bottomNode;
    private Node _leftNode;
    private Node _rightNode;

    public bool IsNullNode = false;

    public void SetValue(int value)
    {
        _nodeValue = value;
    }

    public void SetPosition(int pos)
    {
        _nodePosition = pos;
    }

    public void AssignNeighbors(Node top, Node bottom, Node left, Node right)
    {
        _topNode = top;
        _bottomNode = bottom;
        _leftNode = left;
        _rightNode = right;
    }

    public List<Node> GetNeighbors()
    {
        return new List<Node>()
        {
            _topNode,
            _bottomNode,
            _leftNode,
            _rightNode
        };
    }
}
