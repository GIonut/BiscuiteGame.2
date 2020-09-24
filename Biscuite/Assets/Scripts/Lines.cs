using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lines : MonoBehaviour
{
    public GameObject line;

    public void DrawLine(float xPosition, float yPosition, bool lineIsRotated)
    {
        GameObject lineObject = Instantiate(line) as GameObject;
        lineObject.transform.position = new UnityEngine.Vector3(xPosition, yPosition, 0);

        if (lineIsRotated)
        {
            //  rotate the line 90 degrees
            lineObject.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 90);
        }

    }

    public void DrawLine(UnityEngine.Vector3 position, bool isRotated, int direction)
    {
        int top = 0;
        int right = 1;
        int bottom = 2;
        int left = 3;

        if (direction == top)
        {
            DrawLine(position.x, position.y + 0.5f, isRotated);
        }
        else if (direction == right)
        {
            DrawLine(position.x + 0.5f, position.y, isRotated);
        }
        else if (direction == bottom)
        {
            DrawLine(position.x, position.y - 0.5f, isRotated);
        }
        else if (direction == left)
        {
            DrawLine(position.x - 0.5f, position.y, isRotated);
        }

    }
}
