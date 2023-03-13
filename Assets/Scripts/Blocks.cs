using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    public GameObject obj;
    public List<GameObject> objects;
    private void Awake()
    {
        objects = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = -10; i < 11; i++)
        {
            foreach (var j in new List<int>(){-5, 5})
            {
                Instantiate(obj, new Vector3(i, j, 0), Quaternion.Euler(0, 0, 0));
            }
        }
        
        foreach (var i in new List<int>(){-10, 10})
        {
            for (int j = -4; j < 5; j++)
            {
                Instantiate(obj, new Vector3(i, j, 0), Quaternion.Euler(0, 0, 0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision with a block!");
    }
    */
}
