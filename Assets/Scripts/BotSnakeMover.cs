using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotSnakeMover : MonoBehaviour, ISnakeMover
{
    public GameObject trophy;
    public GameObject loosingFace;
    public bool isFinished;
    public bool isPaused;
    public bool isActiveSnakeMover;
    public static List<GameObject> body = new List<GameObject>();
    public GameObject bodyObject;
    private float _delay = 0.05f;
    private Coroutine _moveRoutine;
    private SpriteRenderer _renderer;
    private bool _alreadyMoved;
    private bool _isAppleEaten;
    public static Vector3 tailPosition;
    public static int depth = 200;
    public static Queue<Vector3> bodyQueue = new Queue<Vector3>();
    public static Vector3 headPosition;

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

        AppleHandler.isBotPlaying = true;
        isFinished = false;
        isPaused = false;
        _isAppleEaten = false;
        _alreadyMoved = false;
        _renderer = GetComponent<SpriteRenderer>();
        headPosition = new Vector3(0, 0, 0);
        body.Add(Instantiate(bodyObject, new Vector3(0, -1, 0), Quaternion.Euler(0, 0, 0)));
        bodyQueue.Enqueue(new Vector3(0, -1, 0));
        StartMoving();
    }

    public void IncreaseBody()
    {
        body.Add(Instantiate(bodyObject, tailPosition, Quaternion.Euler(0, 0, 0)));
        bodyQueue.Enqueue(tailPosition);
    }
    
    private void Rotate(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }

    public static bool IsPositionFree(Vector3 pos)
    {
        if (Physics2D.OverlapPoint(pos) != null)
        {
            return false;
        }
        //Debug.Log(pos.ToString() + " is free!!!");
        return true;
    }

    public static List<Vector3> FindPossibleMoves(Vector3 pos, Vector3 previousPos, ref Queue<Vector3> curBody)
    {
        List<Vector3> possibleMoves = new List<Vector3>();
        if ((!Blocks.IsPositionBlocked(pos + Vector3.down) && !curBody.Contains(pos + Vector3.down)) 
            || pos + Vector3.down == AppleHandler.currentPosition)
        {
            possibleMoves.Add(pos + Vector3.down);
        }
        if ((!Blocks.IsPositionBlocked(pos + Vector3.up) && !curBody.Contains(pos + Vector3.up)) 
            || pos + Vector3.up == AppleHandler.currentPosition)
        {
            possibleMoves.Add(pos + Vector3.up);
        }
        if (!Blocks.IsPositionBlocked(pos + Vector3.left) && !curBody.Contains(pos + Vector3.left) 
            || pos + Vector3.left == AppleHandler.currentPosition)
        {
            possibleMoves.Add(pos + Vector3.left);
        }
        if (!Blocks.IsPositionBlocked(pos + Vector3.right) && !curBody.Contains(pos + Vector3.right) 
            || pos + Vector3.right == AppleHandler.currentPosition)
        {
            possibleMoves.Add(pos + Vector3.right);
        }

        possibleMoves.Remove(previousPos);
        
        return possibleMoves;
    }
    
    public static bool SearchForApple(Vector3 pos, Vector3 previousPos, int move, Queue<Vector3> curBody)
    {
        bool res = false;
        foreach (var newPos in FindPossibleMoves(pos, previousPos, ref curBody))
        {
            Queue<Vector3> newBody = new Queue<Vector3>(curBody);
            newBody.Dequeue();
            newBody.Enqueue(newPos);
            if (pos == AppleHandler.currentPosition)
            {
                //Debug.Log("SearchForMoves(" + pos + ", " + previousPos + ", " + move);
                return SearchForMoves(newPos, pos, 1, newBody);
            }

            if (move == 10)
            {
                return false;
            }
            
            res = res || SearchForApple(newPos, pos, move + 1, newBody);
        }
        
        return res;
    }
    public static bool SearchForMoves(Vector3 pos, Vector3 previousPos, int move, Queue<Vector3> curBody)
    {
        bool res = false;
        foreach (var newPos in FindPossibleMoves(pos, previousPos, ref curBody))
        {
            if (depth == move)
            {
                //Debug.Log("SearchForMoves(" + pos + ", " + previousPos + ", " + move);
                return true;
            }

            Queue<Vector3> newBody = new Queue<Vector3>(curBody);
            newBody.Dequeue();
            newBody.Enqueue(newPos);
            res = res || SearchForMoves(newPos, pos, move + 1, newBody);
        }
        
        return res;
    }
    
    public IEnumerator MoveRoutine()
    {
        Vector3 temp;
        Vector3 lastPosition;
        List<Vector3> possibleMoves = new List<Vector3>();
        int randomIndex;
        HashSet<Vector3> visited = new HashSet<Vector3>();;
        while (true)
        {
            if (_isAppleEaten)
            {
                visited.Clear();
                //IncreaseBody();
                if (body.Count == 30)
                {
                    isFinished = true;
                    Debug.Log("Epic WIN!!! Congratulations)");
                    Stop();
                    trophy.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            
            possibleMoves = FindPossibleMoves(transform.position, body[0].GetComponent<Transform>().position, ref bodyQueue);

            randomIndex = Random.Range(0, possibleMoves.Count);
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                if (possibleMoves[i] == AppleHandler.currentPosition)
                {
                    randomIndex = i;
                    break;
                }
                if (!visited.Contains(possibleMoves[i]))
                {
                    randomIndex = i;
                }
            }
            
            int numberOfAttempts = 1;
            
            while (!SearchForMoves(possibleMoves[randomIndex], transform.position, 1, bodyQueue))
            {
                if (numberOfAttempts > 6)
                {
                    Debug.Log("GAME OVER!!!");
                    foreach (var e in bodyQueue)
                    {
                        Debug.Log(e);
                    }
                    throw new UnityException("No possible moves!");
                    break;
                }
                randomIndex = (randomIndex + 1) % possibleMoves.Count;
                numberOfAttempts++;
            }

            visited.Add(possibleMoves[randomIndex]);
            
            lastPosition = transform.position;
            headPosition = possibleMoves[randomIndex];
            transform.position = possibleMoves[randomIndex];
            bodyQueue.Enqueue(possibleMoves[randomIndex]);
            bodyQueue.Dequeue();
            
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
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveSnakeMover)
        {
            return;
        }
        if (other.gameObject.name == "Apple")
        {
            Debug.Log("Snake ate an apple! " + body.Count);
            _isAppleEaten = true;
            IncreaseBody();
        }
    }
}
