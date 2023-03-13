using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AppleHandler : MonoBehaviour
{
    public GameObject blockObject;
    public GameObject bodyObject;
    void GenerateRandomPosition()
    {
        int numberOfTries = 1;
        Vector3 newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
        while (!IsValidPosition(newPosition) && numberOfTries < 1000)
        {
            newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
            ++numberOfTries;
        }

        transform.position = newPosition;
    }

    void GenerateRandomBlock()
    {
        int numberOfTries = 1;
        Vector3 newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
        while (!IsValidPosition(newPosition) && numberOfTries < 1000)
        {
            newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
            ++numberOfTries;
        }
        
        Instantiate(blockObject, newPosition, Quaternion.Euler(0, 0, 0));
    }
    
    private bool IsValidPosition(Vector3 pos)
    {
        // var direction = Camera.main.transform.position - pos;
        if (Physics2D.Raycast(pos, new Vector2(1, 0), 0.1f).collider != null)
        {
            Debug.Log("The new position is not free!" + pos.ToString());
            return false;
        }

        return true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomPosition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Head")
        {
            Debug.Log("An apple is eaten!!!");
            // SnakeMover.body.Add(Instantiate(bodyObject, SnakeMover.tailPosition, Quaternion.Euler(0, 0, 0)));
            GenerateRandomPosition();
            GenerateRandomBlock();
        }
    }
}
