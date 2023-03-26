using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class HumanSnakeMover : MonoBehaviour, ISnakeMover
{
    public GameObject trophy;
    public GameObject loosingFace;
    public bool isFinished;
    public bool isPaused;
    public bool isActiveSnakeMover;
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
        if (!isActiveSnakeMover)
        {
            return;
        }
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
        if (!isActiveSnakeMover)
        {
            return;
        }

        AppleHandler.isBotPlaying = false;
        isFinished = false;
        isPaused = false;
        _isAppleEaten = false;
        _alreadyMoved = false;
        _renderer = GetComponent<SpriteRenderer>();
        BotSnakeMover.headPosition = new Vector3(0, 0, 0);
        BotSnakeMover.body.Add(Instantiate(bodyObject, new Vector3(0, -1, 0), Quaternion.Euler(0, 0, 0)));
        BotSnakeMover.bodyQueue.Enqueue(new Vector3(0, -1, 0));
        body.Add(BotSnakeMover.body[0]);
        StartMoving();
    }

    public void IncreaseBody()
    {
        body.Add(Instantiate(bodyObject, tailPosition, Quaternion.Euler(0, 0, 0)));
        BotSnakeMover.body.Add(body[body.Count - 1]);
        BotSnakeMover.bodyQueue.Enqueue(tailPosition);
    }
    
    private void Rotate(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }

    public IEnumerator MoveRoutine()
    {
        Vector3 temp;
        Vector3 lastPosition;
        
        while (true)
        {
            
            lastPosition = transform.position;
            transform.position += transform.right * _renderer.bounds.size.x;
            BotSnakeMover.headPosition = transform.position;
            BotSnakeMover.bodyQueue.Enqueue(BotSnakeMover.headPosition);
            BotSnakeMover.bodyQueue.Dequeue();
            
            if (_isAppleEaten)
            {
                if (body.Count == 30)
                {
                    isFinished = true;
                    Debug.Log("Epic win!!! Congratulations!");
                    Stop();
                    trophy.GetComponent<SpriteRenderer>().enabled = true;
                }
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
        if (!isActiveSnakeMover)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isFinished)
        {
            if (isPaused)
            {
                StartMoving();
                isPaused = false;
            }
            else
            {
                Stop();
                isPaused = true;
            }
        }
        
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
        if (!isActiveSnakeMover)
        {
            return;
        }
        if (other.gameObject.name == "Apple")
        {
            //Debug.Log("Snake ate an apple!");
            _isAppleEaten = true;
            IncreaseBody();
        }
        else
        {
            Stop();
            loosingFace.GetComponent<SpriteRenderer>().enabled = true;
            //Application.Quit();
            // UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}