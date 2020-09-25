using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Biscuit : MonoBehaviour
{
    // parameters
    [SerializeField] bool[] sides = { false, false, false, false };

    // references
    public Lines lines;

    public void SetLine(int direction)
    {
        switch (direction)
        {
            case 0:
                sides[0] = true;
                break;
            case 1:
                sides[1] = true;
                break;
            case 2:
                sides[2] = true;
                break;
            case 3:
                sides[3] = true;
                break;
        }
        
    }

    public void CloseSideOnDirection(int direction)
    {
        // 0 - top, 1 - right, 2 - bottom, 3 - left
        int top = 0;
        int right = 1;
        int bottom = 2;
        int left = 3;

        UnityEngine.Vector3 position = this.gameObject.transform.position;
        
        switch (direction)
        {
            case 0:
                {
                    sides[0] = true;
                    lines.DrawLine(position, false, top);
                    break;
                }
            case 1:
                {
                    sides[1] = true;
                    lines.DrawLine(position, true, right);
                    break;
                }
            case 2:
                {
                    sides[2] = true;
                    lines.DrawLine(position, false, bottom);
                    break;
                }
            case 3:
                {
                    sides[3] = true;
                    lines.DrawLine(position, true, left);
                    break;
                }
        }
    }

    public bool isSideClosed(int direction)
    {
        return sides[direction];
    }

    public bool IsClosed()
    {
        foreach(bool side in sides)
        {
            if (side == false)
                return false;
        }
        return true;
    }
}
