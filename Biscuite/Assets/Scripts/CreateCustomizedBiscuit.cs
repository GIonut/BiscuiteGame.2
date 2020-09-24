using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class CreateCustomizedBiscuit : MonoBehaviour
{
    // parameters
    public GameObject biscuit;
    public GameObject deactivatedCircle;
    public GameObject biscuitParent;
    private BiscuitMatrix biscuitMatrix;

    private SmartFox sfs;
    private GameObject biscuitContainer;

    public List<GameObject> References;

    private int heightLevel = 0;
    private int maxHeightLevel = 0;

    public BiscuitMatrix GetBiscuitMatrix()
    {
        return biscuitMatrix;
    }

    private void OnExtensionResponse(BaseEvent e)
    {
    }

    public GameObject CreateBiscuit(int height, SmartFox sfsReference)
    {
        if (sfsReference != null)
        {
            sfs = sfsReference;
            sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        }

        float x = 10f;// the middle of the screen on the x axis
        float y = 8 + height/2; // 16 is the highest point on the y axis of the camera, so the biscuit matrix will start from the middle of it( 16/2 =8) with an offset of the given height/2
        heightLevel = 0;
        maxHeightLevel = height;

        References = new List<GameObject>();

        biscuitContainer = Instantiate(biscuitParent) as GameObject;
        biscuitContainer.transform.position = new Vector3(10f, 8f, 0);// in the middle of the screen
        biscuitMatrix = biscuitContainer.GetComponent<BiscuitMatrix>();
        References.Add(biscuitContainer);

        while (heightLevel <= maxHeightLevel / 2)
        {
            PlaceBiscuit(x, y);
            heightLevel++;
        }

        return biscuitContainer;

    }

    private void PlaceBiscuit(float x, float y)
    {
        float biscuitY = y - heightLevel;
        float symmetricBiscuitY = y - maxHeightLevel + heightLevel;

        if (heightLevel > maxHeightLevel / 2)
            heightLevel = maxHeightLevel - heightLevel;

        int objectsOnRow = heightLevel * 2 + 1;


        for (int i = 0; i <= objectsOnRow / 2; i++)
        {
            float biscuitX = x + i - heightLevel;
            float symmetricBiscuitX = x + objectsOnRow / 2 - i;

            GameObject cookie = InstantiateBiscuit(biscuitX, biscuitY);
            AddBiscuitToMatrix(i, heightLevel, cookie);
            InstantiateCircle(biscuitX - 0.5f, biscuitY + 0.5f);
            

            if (biscuitY != symmetricBiscuitY)
            {
                cookie = InstantiateBiscuit(biscuitX, symmetricBiscuitY);
                AddBiscuitToMatrix(i, maxHeightLevel - heightLevel, cookie);

            }
            InstantiateCircle(biscuitX - 0.5f, symmetricBiscuitY - 0.5f);
            
            if (biscuitX != symmetricBiscuitX)
            {
                cookie = InstantiateBiscuit(symmetricBiscuitX, biscuitY);
                AddBiscuitToMatrix(objectsOnRow - 1 - i, heightLevel, cookie);
                InstantiateCircle(symmetricBiscuitX - 0.5f, biscuitY + 0.5f);
                
                if (biscuitY != symmetricBiscuitY)
                {
                    cookie = InstantiateBiscuit(symmetricBiscuitX, symmetricBiscuitY);
                    AddBiscuitToMatrix(objectsOnRow - i - 1, maxHeightLevel - heightLevel, cookie);

                }
                InstantiateCircle(symmetricBiscuitX - 0.5f, symmetricBiscuitY - 0.5f);
                
            }

            if (i == 0)
            {
                InstantiateCircle(symmetricBiscuitX + 0.5f, biscuitY + 0.5f);
                InstantiateCircle(symmetricBiscuitX + 0.5f, symmetricBiscuitY - 0.5f);
            }
        }
    }

    private GameObject InstantiateBiscuit(float xPosition, float yPosition)
    {
        GameObject cookie = Instantiate(biscuit) as GameObject;
        cookie.transform.position = new Vector3(xPosition, yPosition);
        cookie.transform.SetParent(biscuitContainer.transform);
        return cookie;
    }

    private void InstantiateCircle(float xPosition, float yPosition)
    {
        GameObject circle = Instantiate(deactivatedCircle) as GameObject;
        circle.transform.position = new Vector3(xPosition, yPosition);
        circle.transform.SetParent(biscuitContainer.transform);
    }

    private void AddBiscuitToMatrix(int x, int y, GameObject cookie)
    {
        CompleteClosedSides(x, y, cookie);

        // if not in multiplayer mode
        if(sfs == null)
            biscuitMatrix.SetBiscuitOnPosition(cookie);
    }

    private void CompleteClosedSides(int x, int y, GameObject cookie)
    {
        int top = 0;
        int right = 1;
        int bottom = 2;
        int left = 3;
        int direction = -1;

        int middle = maxHeightLevel / 2;
        int lastBiscuitOnNorthRow = y * 2;// last biscuit if the row is in the upper part of the matrix
        int lastBiscuitOnSouthRow = (maxHeightLevel - y) * 2;// last biscuit if the row is in the lower side of the matrix

        Biscuit biscuitScript = cookie.GetComponent<Biscuit>();

        if (y <= middle) // biscuits on the upper side of the matrix
        {
            if (x == 0 || x == y * 2) // biscuits on the upper and extreme sides of the matrix
            {
                biscuitScript.CloseSideOnDirection(top);
                if (sfs != null)
                {
                    // if on multiplayer mode, the gameBoard will be on the server side
                    SendRequest(x, y, top);
                }
            }
        }

        if (y >= middle)// biscuits on the lower side of the matrix
        {
            if (x == 0 || x == lastBiscuitOnSouthRow) // biscuits on the extreme sides of the matrix
            {

                biscuitScript.CloseSideOnDirection(bottom); // biscuits on the lower and extreme sides of the matrix
                if (sfs != null)
                {
                    SendRequest(x, y, bottom);
                }

            }
        }

        if (x == 0) // biscuits on the left side
        {
            biscuitScript.CloseSideOnDirection(left);
            if (sfs != null)
            {
                SendRequest(x, y, left);
            }
        }

        if (x == lastBiscuitOnSouthRow || x == lastBiscuitOnNorthRow) // biscuits on the right side
        {
            biscuitScript.CloseSideOnDirection(right);
            if (sfs != null)
            {
                SendRequest(x, y, right);
            }
        }
        
    }

    private void SendRequest(int x, int y, int direction)
    {
        ISFSObject objIn = new SFSObject();
        objIn.PutFloat("x", x);
        objIn.PutFloat("y", y);
        objIn.PutInt("dir", direction);
        sfs.Send(new ExtensionRequest("place", objIn, sfs.LastJoinedRoom));
    }
}
