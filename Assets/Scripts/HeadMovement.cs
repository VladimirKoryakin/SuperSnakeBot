using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Block(Clone)")
        {
            //Debug.Log("Collision with a Block!(((");
        }
        //Debug.Log("Game Over (((");
        //Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
