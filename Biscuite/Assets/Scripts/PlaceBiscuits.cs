using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceBiscuits : MonoBehaviour
{
    // references
    public GameObject biscuit15x15;
    public GameObject biscuit13x13;
    public GameObject biscuit11x11;

    public GameObject CreateBiscuit(int difficulty)
    {
        if(difficulty == 2)
            return CreateBiscuit15x15();
        else if(difficulty == 1)
            return CreateBiscuit13x13();
        else if(difficulty == 0)
            return CreateBiscuit11x11();
        return null;
    }

    private GameObject CreateBiscuit15x15()
    {
        return Instantiate(biscuit15x15, biscuit15x15.transform.position, biscuit15x15.transform.rotation);
    }



    private GameObject CreateBiscuit13x13()
    {
        return Instantiate(biscuit13x13, biscuit13x13.transform.position, biscuit13x13.transform.rotation);
    }


    private GameObject CreateBiscuit11x11()
    {
        return Instantiate(biscuit11x11, biscuit11x11.transform.position, biscuit11x11.transform.rotation);
    }

}
