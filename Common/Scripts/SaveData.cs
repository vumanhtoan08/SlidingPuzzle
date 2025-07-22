using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int boardSize; // kích thước của board 
    public int[] board; // vị trí của từng con số
    public int moveCount; // các bước đã đi
    public float timer; // thời gian chơi
    public string difficult; // độ khó
}

[System.Serializable]
public class DifficultListData
{
    public List<DifficultData> difficultDatas;
}

[System.Serializable]
public class DifficultData
{
    public string difficult;
    public float timer; 
}
