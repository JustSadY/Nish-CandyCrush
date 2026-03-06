using System.Collections;
using System.Collections.Generic;
using Script.Enum__Struct;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridBoard : MonoBehaviour
{
    public static GridBoard Instance { get; private set; }

    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float spacing = 1.0f;
    [SerializeField] private GameObject[] crushPrefabs;

    private Node[,] _grid;
    private Crush _selectedCrush;
    private bool _bIsProcessingMove;
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        _inputActions.Player.Interact.performed += Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (_bIsProcessingMove || Camera.main == null || Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            Crush clickedCrush = hit.collider.GetComponent<Crush>();
            if (clickedCrush != null)
            {
                HandleSelection(clickedCrush);
            }
        }
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        _grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateCrushAt(x, y);
            }
        }

        while (CheckForMatchesOnBoard())
        {
            ClearAndReinitialize();
        }
    }

    private void CreateCrushAt(int x, int y)
    {
        Vector2 worldPos = CalculatePosition(x, y);
        int randomIndex = Random.Range(0, crushPrefabs.Length);
        GameObject newObj = Instantiate(crushPrefabs[randomIndex], worldPos, Quaternion.identity, transform);

        Crush crush = newObj.GetComponent<Crush>();
        crush.InitializeCrush(x, y);

        Node node = newObj.AddComponent<Node>(node => node.InitializeNode(true, newObj));
        _grid[x, y] = node;
    }

    private Vector2 CalculatePosition(int x, int y)
    {
        float offsetX = (width - 1) * spacing / 2f;
        float offsetY = (height - 1) * spacing / 2f;
        return new Vector2(x * spacing - offsetX, y * spacing - offsetY);
    }

    private void HandleSelection(Crush clickedCrush)
    {
        SoundManager.Instance.PlaySfx(SoundType.ClickEffect);
        if (_selectedCrush == null)
        {
            _selectedCrush = clickedCrush;
        }
        else if (_selectedCrush == clickedCrush)
        {
            _selectedCrush = null;
        }
        else
        {
            if (IsAdjacent(_selectedCrush, clickedCrush))
            {
                StartCoroutine(SwapAndProcess(_selectedCrush, clickedCrush));
            }

            _selectedCrush = null;
        }
    }

    private IEnumerator SwapAndProcess(Crush a, Crush b)
    {
        _bIsProcessingMove = true;

        ExecuteSwapLogic(a, b);
        yield return new WaitForSeconds(0.3f);
        if (CheckForMatchesOnBoard()) DestroyMatchedCrushes();
        else ExecuteSwapLogic(a, b);

        _bIsProcessingMove = false;
    }

    private void ExecuteSwapLogic(Crush a, Crush b)
    {
        int xA = a.xIndex;
        int yA = a.yIndex;
        int xB = b.xIndex;
        int yB = b.yIndex;

        (_grid[xA, yA], _grid[xB, yB]) = (_grid[xB, yB], _grid[xA, yA]);

        a.xIndex = xB;
        a.yIndex = yB;
        b.xIndex = xA;
        b.yIndex = yA;

        a.MoveToTarget(CalculatePosition(xB, yB));
        b.MoveToTarget(CalculatePosition(xA, yA));
    }

    private bool CheckForMatchesOnBoard()
    {
        bool bHasFoundAnyMatch = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node currentNode = _grid[x, y];
                if (currentNode == null || !currentNode.isUsable) continue;

                Crush currentCrush = currentNode.GetComponent<Crush>();
                if (currentCrush == null || currentCrush.IsMatched()) continue;

                MatchResult result = IsConnected(currentCrush);

                if (result.ConnectedCrushes.Count >= 3)
                {
                    foreach (Crush c in result.ConnectedCrushes)
                    {
                        c.SetMatched(true);
                    }

                    bHasFoundAnyMatch = true;
                }
            }
        }

        return bHasFoundAnyMatch;
    }

    private MatchResult IsConnected(Crush originCrush)
    {
        List<Crush> horizontalMatches = new List<Crush> { originCrush };
        FindMatchesInDirection(originCrush, new Vector2Int(1, 0), horizontalMatches);
        FindMatchesInDirection(originCrush, new Vector2Int(-1, 0), horizontalMatches);

        if (horizontalMatches.Count >= 3)
        {
            return new MatchResult { ConnectedCrushes = horizontalMatches, Direction = MatchDirection.Horizontal };
        }

        List<Crush> verticalMatches = new List<Crush> { originCrush };
        FindMatchesInDirection(originCrush, new Vector2Int(0, 1), verticalMatches);
        FindMatchesInDirection(originCrush, new Vector2Int(0, -1), verticalMatches);

        if (verticalMatches.Count >= 3)
        {
            return new MatchResult { ConnectedCrushes = verticalMatches, Direction = MatchDirection.Vertical };
        }

        return new MatchResult { ConnectedCrushes = new List<Crush>(), Direction = MatchDirection.None };
    }

    private void FindMatchesInDirection(Crush startCrush, Vector2Int direction, List<Crush> matchList)
    {
        CrushType targetType = startCrush.GetCrushType();
        int nextX = startCrush.xIndex + direction.x;
        int nextY = startCrush.yIndex + direction.y;

        while (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
        {
            Node neighborNode = _grid[nextX, nextY];
            if (neighborNode != null && neighborNode.isUsable)
            {
                Crush neighborCrush = neighborNode.GetComponent<Crush>();
                if (neighborCrush != null && neighborCrush.GetCrushType() == targetType)
                {
                    matchList.Add(neighborCrush);
                    nextX += direction.x;
                    nextY += direction.y;
                }
                else break;
            }
            else break;
        }
    }

    private bool IsAdjacent(Crush a, Crush b)
    {
        return Mathf.Abs(a.xIndex - b.xIndex) + Mathf.Abs(a.yIndex - b.yIndex) == 1;
    }

    private void ClearAndReinitialize()
    {
        foreach (Node n in _grid)
            if (n != null)
                Destroy(n.gameObject);
        InitializeBoard();
    }

    private void DestroyMatchedCrushes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node currentNode = _grid[x, y];
                if (currentNode != null)
                {
                    Crush crush = currentNode.GetComponent<Crush>();
                    if (crush != null && crush.IsMatched())
                    {
                        _grid[x, y] = null;
                        Destroy(crush.gameObject);
                        SoundManager.Instance.PlaySfx(SoundType.Crushes);
                    }
                }
            }
        }

        StartCoroutine(RefillBoard());
    }

    private IEnumerator RefillBoard()
    {
        yield return new WaitForSeconds(0.2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_grid[x, y] == null)
                {
                    for (int nextY = y + 1; nextY < height; nextY++)
                    {
                        if (_grid[x, nextY] != null)
                        {
                            _grid[x, y] = _grid[x, nextY];
                            _grid[x, nextY] = null;

                            Crush movedCrush = _grid[x, y].GetComponent<Crush>();
                            movedCrush.yIndex = y;
                            movedCrush.MoveToTarget(CalculatePosition(x, y));
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        FillTopEmptySpaces();
    }

    private void FillTopEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_grid[x, y] == null)
                {
                    CreateCrushAt(x, y);
                }
            }
        }

        if (CheckForMatchesOnBoard())
        {
            DestroyMatchedCrushes();
        }
    }
}