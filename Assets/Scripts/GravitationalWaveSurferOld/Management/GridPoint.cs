using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPoint 
{
    public int row;
    public int col;
    public GameObject obj;
   
    public GridPoint(int row, int col, GameObject obj) 
    {
        this.row = row;
        this.col = col;
        this.obj = obj;
    }

    public void Describe()
    {
        Debug.Log((col, row, obj, obj.transform.position));
    }
}
