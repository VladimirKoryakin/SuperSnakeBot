using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMover : MonoBehaviour
{
    public static List<GameObject> body = new List<GameObject>();
    public GameObject bodyObject;
    private float _delay = 0.5f;
    private Coroutine _moveRoutine;
    private SpriteRenderer _renderer;
    private bool _alreadyMoved;
    private bool _isAppleEaten;
    public static Vector3 tailPosition;

    public void StartMoving()
    {
        _alreadyMoved = false;
        Stop();
        _moveRoutine = StartCoroutine(MoveRoutine());
    }

    public void Stop()
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);
    }
    
    private void Awake()
    {
        _isAppleEaten = false;
        _alreadyMoved = false;
        _renderer = GetComponent<SpriteRenderer>();
        body.Add(Instantiate(bodyObject, new Vector3(0, -1, 0), Quaternion.Euler(0, 0, 0)));
        StartMoving();
    }

    public void IncreaseBody()
    {
        body.Add(Instantiate(bodyObject, tailPosition, Quaternion.Euler(0, 0, 0)));
    }
    
    private void Rotate(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }

    private IEnumerator MoveRoutine()
    {
        Vector3 temp;
        Vector3 lastPosition;
        while (true)
        {
            
            lastPosition = transform.position;
            transform.position += transform.right * _renderer.bounds.size.x;

            if (_isAppleEaten)
            {
                IncreaseBody();
            }
            foreach (var bodyPart in body)
            {
                temp = bodyPart.GetComponent<Transform>().position;
                bodyPart.GetComponent<Transform>().position = lastPosition;
                lastPosition = temp;
            }

            tailPosition = lastPosition;
            _isAppleEaten = false;
            _alreadyMoved = false;
            yield return new WaitForSecondsRealtime(_delay);
        }
    }

    private void Update()
    {
        if (_alreadyMoved)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) && transform.rotation != Quaternion.Euler(0, 0, -90))
        {
            _alreadyMoved = true;
            Rotate(Quaternion.Euler(0, 0, 90));
        }
        else if (Input.GetKeyDown(KeyCode.S) && transform.rotation != Quaternion.Euler(0, 0, 90))
        {
            _alreadyMoved = true;
            Rotate(Quaternion.Euler(0, 0, -90));
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.rotation != Quaternion.Euler(0, 0, 180))
        {
            _alreadyMoved = true;
            Rotate(Quaternion.Euler(0, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            _alreadyMoved = true;
            Rotate(Quaternion.Euler(0, 0, -180));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Apple")
        {
            Debug.Log("Snake ate an apple!");
            _isAppleEaten = true;
        }
    }
}