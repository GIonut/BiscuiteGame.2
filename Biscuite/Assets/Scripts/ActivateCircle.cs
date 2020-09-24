using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCircle : MonoBehaviour
{
    // references
    public Sprite activatedCircle;
    public Sprite deactivatedCircle;

    GameController gameController;
    MultiplayerGameController multiplayerGameController;

    //parameters
    bool isActivated = false;
    

    private void Start()
    {
        // make all circles invisible. They will be visible only on hover
        TurnInvisible();
        gameController = FindObjectOfType<GameController>();
        if(gameController == null)
            multiplayerGameController = FindObjectOfType<MultiplayerGameController>();
        
    }

    private void OnMouseOver()
    {
        this.gameObject.GetComponent<Renderer>().enabled = true;
    }

    private void OnMouseExit()
    {
        if (!isActivated)
        {
            // if exit hover, make circles invisible again
            Invoke("TurnInvisible", 0.1f);
        }
    }

    public void OnMouseDown()
    {
        
        if (isActivated)
        {
            if ((gameController != null && gameController.CanBeDeactivated(this.gameObject)) || 
                (multiplayerGameController != null && multiplayerGameController.CanBeDeactivated(this.gameObject)))
            {
                // if the circle can be deactivated, deactivate it
                this.gameObject.GetComponent<SpriteRenderer>().sprite = deactivatedCircle;
                isActivated = false;
            }
        }
        else
        {
            // on click, if the circle is deactivated, check if it can be activated
            // ( is neighbour with the starting circle or is the starting circle
            if ((gameController != null && gameController.CanBeActivated(this.gameObject)) ||
                (multiplayerGameController != null && multiplayerGameController.CanBeActivated(this.gameObject)))
            {
                // if so, activate the circle( make it turn green)
                
                Activate();
            }
        }
    }
    public void TurnInvisible()
    {
        this.gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void Deactivate()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = deactivatedCircle;
        Invoke("TurnInvisible", 0f);
        isActivated = false;
    }


    public void Activate()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = activatedCircle;
        isActivated = true;
    }
}
