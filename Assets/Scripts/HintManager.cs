using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySecond;
    public GameObject hintParticle;
    public GameObject currentHint;
    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySecond = hintDelay;
    }

    void Update()
    {
        hintDelaySecond -= Time.deltaTime;
        if(hintDelaySecond <=0 && currentHint == null)
        {
            MarkHint();
            hintDelaySecond = hintDelay;
        }
    }

    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    GameObject PickOneRandom()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int PieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[PieceToUse];
        }
        return null;
    }

    private void MarkHint()
    {
        GameObject move = PickOneRandom();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint()
    {
       if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySecond = hintDelay;
        }
    }

}
