using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Lock,
    Concreate,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x, y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Objects Stuff")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;
    [Header("Board Dimentional")]
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject breakableTilePrefarb;
    public GameObject tilePrefarb;
    public GameObject lockTilePrefab;
    public GameObject concreateTilePrefab;

    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTies;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreateTiles;

    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if(world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.pause;
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTies = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreateTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        //allTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    public void GenerateBlankSpace()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        for(int i =0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefarb, tempPosition, Quaternion.identity);
                breakableTies[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateLockTiles() //
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateConcreateTiles() //
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Concreate)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreateTilePrefab, tempPosition, Quaternion.identity);
                concreateTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void SetUp()
    {
        GenerateBlankSpace();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreateTiles();
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreateTiles[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefarb, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                        //Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
        if (IsDeadlocked())
        {
            Debug.Log("���������� ��� ����������");
            ShuffleBoard();
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column>1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <=1 || row <=1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null) {
            foreach (GameObject currentPiece in findMatches.currentMatches)
        {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
            {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
                }
            }
        return (numberVertical == 5 || numberHorizontal == 5);
            }

    private void CheckToMakeBombs()
            {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
            findMatches.CheckBombs();
            }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
    {
            if (ColumnOrRow())
        {
                if(currentDot != null)
            {
                    if (currentDot.isMatched)
                {
                        if (!currentDot.isColorBomb)
                        {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                }
                    }
                else
                {
                        if(currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                        {
                                if (!otherDot.isColorBomb)
                                {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            }
                }
            }
            else
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
            {
                        if (!currentDot.isAdjacentBomb)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                }
                    }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeAdjacentBomb();
                        }
                    }
                }
            }
            }
        }
    }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if (findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            if (breakableTies[column,row]!= null)
            {
                breakableTies[column, row].TakeDamage(1);
                if (breakableTies[column, row].hitPoints <= 0)
                {
                    breakableTies[column, row] = null;
                }
            }
            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            DamageConcreate(column, row);
            if (goalManager != null )
            {
                goalManager.CompareGoals(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if(soundManager != null)
            {
                soundManager.PlayRandomDestroyNoice();
            }
            GameObject particle =Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j=0; j< height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

    private void DamageConcreate(int column, int row) //
    {
        if(column > 0)
        {
            if (concreateTiles[column - 1, row])
            {
                concreateTiles[column - 1, row].TakeDamage(1);
                if (concreateTiles[column - 1, row].hitPoints <= 0)
                {
                    concreateTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreateTiles[column + 1, row])
            {
                concreateTiles[column + 1, row].TakeDamage(1);
                if (concreateTiles[column + 1, row].hitPoints <= 0)
                {
                    concreateTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreateTiles[column, row - 1])
            {
                concreateTiles[column, row - 1].TakeDamage(1);
                if (concreateTiles[column, row - 1].hitPoints <= 0)
                {
                    concreateTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreateTiles[column, row + 1])
            {
                concreateTiles[column, row + 1].TakeDamage(1);
                if (concreateTiles[column, row + 1].hitPoints <= 0)
                {
                    concreateTiles[column, row + 1] = null;
                }
            }
        }
    }

    private IEnumerator DecreaseRowCo2()
    {
        for(int i =0; i < width; i++)
        {
            for(int j =0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreateTiles[i, j])
                {
                    for(int k = j + 1; k < height; k++)
                    {
                        if (allDots[i,k] != null)
                        {
                            allDots[i,k].GetComponent<Dot>().row = j;
                            allDots[i,k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0;i < width; i++)
        {
            for (int j=0; j< height; j++)
            {
                if (allDots[i,j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for(int i =0; i < width; i++)
        {
            for(int j =0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreateTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations <100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i,j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            streakValue ++;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        if (IsDeadlocked())
        {
            Debug.Log("�����");
            ShuffleBoard();
        }
        else
        {
            //Debug.Log("�� ���");
        }
        if(currentState != GameState.pause)
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches()
    {
        for(int i = 0; i< width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (i < width - 2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    } 

    private bool IsDeadlocked()
    {
        for(int i =0; i<width; i++)
        {
            for(int j =0; j<height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if(i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for(int i =0;i< width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        for(int i =0; i < width; i++)
        {
            for (int j = 0;j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreateTiles[i, j]  )
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    allDots[i,j] =newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }



}