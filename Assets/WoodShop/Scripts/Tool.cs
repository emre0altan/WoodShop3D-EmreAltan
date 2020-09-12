using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public ReShape reShape;
    public InputHandler inputHandler;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wood"))
        {
            reShape.CutWood();
        }
        else if (collision.gameObject.CompareTag("TargetWood"))
        {
            inputHandler.Failed(-1);
        }        
    }
}
