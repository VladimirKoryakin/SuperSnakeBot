using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AppleHandler : MonoBehaviour
{
    public GameObject blockObject;
    public GameObject bodyObject;
    public static Vector3 currentPosition;
    public static bool isBotPlaying = false;
    void GenerateRandomPosition()
    {
        int numberOfTries = 1;
        Vector3 newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
        currentPosition = newPosition;

        if (isBotPlaying)
        {
            while ((!IsValidPosition(newPosition) ||
                    (!BotSnakeMover.SearchForApple(BotSnakeMover.headPosition,
                        BotSnakeMover.body[0].GetComponent<Transform>().position, 1, BotSnakeMover.bodyQueue)))
                   && numberOfTries < 1000)
            {
                newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
                currentPosition = newPosition;
                ++numberOfTries;
            }
        }
        else
        {
            while ((!IsValidPosition(newPosition) || CountFreeTilesAround(newPosition) < 3) && numberOfTries < 1000)
            {
                newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
                currentPosition = newPosition;
                ++numberOfTries;
            }
        }

        transform.position = newPosition;
        currentPosition = newPosition;
        enabled = true;
    }

    private int CountFreeTilesAround(Vector3 pos)
    {
        int res = 0;
        if (BotSnakeMover.IsPositionFree(pos + Vector3.down))
        {
            res++;
        }
        if (BotSnakeMover.IsPositionFree(pos + Vector3.up))
        {
            res++;
        }
        if (BotSnakeMover.IsPositionFree(pos + Vector3.left))
        {
            res++;
        }
        if (BotSnakeMover.IsPositionFree(pos + Vector3.right))
        {
            res++;
        }

        return res;
    }
    
    public static bool IsValidPosition(Vector3 pos)
    {
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
            //Debug.Log("An apple is eaten!!!");
            // HumanSnakeMover.body.Add(Instantiate(bodyObject, SnakeMover.tailPosition, Quaternion.Euler(0, 0, 0)));
            enabled = false;
            Blocks.GenerateRandomBlock();
            GenerateRandomPosition();
        }
    }
}
