using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;
using System;

public class BiscuitMatrix : MonoBehaviour
{
    public Dictionary<float, Dictionary<float, GameObject>> biscuitMatrix;

    public void SetBiscuitOnPosition(GameObject biscuit)
    {
        float x = biscuit.transform.position.x;
        float y = biscuit.transform.position.y;

        if (biscuitMatrix == null)
        {
            biscuitMatrix = new Dictionary<float, Dictionary<float, GameObject>>();
        }

        try
        {
            biscuitMatrix[x].Add(y, biscuit);
        }
        catch (KeyNotFoundException)
        {
            Dictionary<float, GameObject> row = new Dictionary<float, GameObject>();
            biscuitMatrix.Add(x, row);
            biscuitMatrix[x].Add(y, biscuit);
        }
        
    }

    public GameObject GetBiscuitOnPosition(float x, float y)
    {
        return biscuitMatrix[x][y];
    }

    public void SetLine(float x, float y, int direction)
    {
        if(GetBiscuitOnPosition(x, y).GetComponent<Biscuit>().isSideClosed(direction))
            throw new System.ArgumentException(); // if there is already a line, throw an exception;
        try
        {
            biscuitMatrix[x][y].GetComponent<Biscuit>().SetLine(direction);
        }
        catch(KeyNotFoundException e)
        {
            throw e;
        }
    }

    public bool CheckClosedSquare(float x, float y)
    {
        bool isClosed = false;
        try
        {
            isClosed = biscuitMatrix[x][y].GetComponent<Biscuit>().IsClosed();
        }
        catch(KeyNotFoundException)
        {
            return false;
        }
        return isClosed;
    }

}
