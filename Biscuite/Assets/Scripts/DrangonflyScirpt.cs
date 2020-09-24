using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DrangonflyScirpt : MonoBehaviour
{
    bool goDown = false;
    void Update()
    {

        if (!goDown)
        {
            gameObject.transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y + 0.01f, 0f);
            if(transform.position.y >= 6f)
            {
                goDown = true;
            }
        }
        else
        {
            gameObject.transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y - 0.01f, 0f);
            if (transform.position.y <= 5f)
            {
                goDown = false;
            }
        }
        
    }
}
