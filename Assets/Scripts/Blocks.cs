using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Blocks : MonoBehaviour
{
    public GameObject obj;
    public static GameObject blockObject;
    public static HashSet<Vector3> blocks = new HashSet<Vector3>();
    
    void Start()
    {
        blockObject = obj;
        for (int i = -10; i < 11; i++)
        {
            foreach (var j in new List<int>(){-5, 5})
            {
                Instantiate(blockObject, new Vector3(i, j, 0), Quaternion.Euler(0, 0, 0));
                blocks.Add(new Vector3(i, j, 0));
            }
        }
        
        foreach (var i in new List<int>(){-10, 10})
        {
            for (int j = -4; j < 5; j++)
            {
                Instantiate(blockObject, new Vector3(i, j, 0), Quaternion.Euler(0, 0, 0));
                blocks.Add(new Vector3(i, j, 0));
            }
        }
    }
    
    public static void GenerateRandomBlock()
    {
        int numberOfTries = 1;
        Vector3 newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
        while ((!AppleHandler.IsValidPosition(newPosition) ||
                !BotSnakeMover.SearchForMoves(BotSnakeMover.headPosition,
                    BotSnakeMover.body[0].GetComponent<Transform>().position, 1, BotSnakeMover.bodyQueue)) 
               && numberOfTries < 1000)
        {
            newPosition = new Vector3(Random.Range(-9, 10), Random.Range(-4, 5), 0);
            ++numberOfTries;
        }
        
        Instantiate(blockObject, newPosition, Quaternion.Euler(0, 0, 0));
        blocks.Add(newPosition);
    }

    public static bool IsPositionBlocked(Vector3 pos)
    {
        return blocks.Contains(pos);
    }
}
