using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GameplayManager : Singleton<GameManager>
{
    #region VARIABLES

    private float timer;
    private int moveCount;

    private bool isGameFinished = false;

    public bool IsGameFinished
    {
        get { return isGameFinished; }
        set
        {
            isGameFinished = value;
            if (isGameFinished)
            {
                DataManager.Instance.UpdateCurrentData(GameManager.Instance.Difficult, this.timer);
                UpdateResultInfor();
                DarkPanelIntro(0.5f);
            }
        }
    }

    private void ResetVariables()
    {
        timer = 0;
        moveCount = 0;
    }

    #endregion

    private void Awake()
    {
        if (GameManager.Instance.IsContinue)
        {
            SetupInforGame(DataManager.Instance.LoadGameProgress());
            SpawnNode(DataManager.Instance.LoadGameProgress());
            return;
        }

        SetupInforGame();
        SpawnNode();
        RandomizeBoard(GameManager.Instance.BoardSize * GameManager.Instance.BoardSize);
    }

    #region SETUP_METHODS

    [SerializeField] private TMP_Text _levelDifficultText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _moveCountText;

    private void SetupInforGame()
    {
        ResetVariables();

        _levelDifficultText.text = $"{GameManager.Instance.Difficult}";
        _timerText.text = $"{0}{0}:{0}{0}";
        _moveCountText.text = $"{moveCount}";
    }

    private void SetupInforGame(SaveData data)
    {
        timer = data.timer;
        moveCount = data.moveCount;

        _levelDifficultText.text = $"{data.difficult}";

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(data.timer);
        string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        _timerText.text = formattedTime;

        _moveCountText.text = $"{data.moveCount}";
    }


    #endregion

    #region UPDATE_METHOD

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        _timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    private void UpdateMoveCountText()
    {
        moveCount++;
        _moveCountText.text = $"{moveCount}";
    }

    private void Update()
    {
        if (isGameFinished)
            return;

        timer += Time.deltaTime;

        UpdateTimerText();
    }

    [SerializeField] private TMP_Text _highScore;
    [SerializeField] private TMP_Text _currentScore;
    [SerializeField] private TMP_Text _currentDifficult;

    private void UpdateResultInfor()
    {
        _currentScore.text = _timerText.text;
        _currentDifficult.text = _levelDifficultText.text;

        string difficulty = _levelDifficultText.text;
        float timer = DataManager.Instance.GetTimer(difficulty);

        if (timer <= 0f)
        {
            _highScore.text = "--:--";
        }
        else
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);

            string formatted = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            _highScore.text = formatted;
        }
    }


    #endregion

    #region SPAWN_METHODS

    [SerializeField] private GameObject _nodeNumberPrefabs;
    [SerializeField] private GameObject _nodeNullPrefabs;
    [SerializeField] private GridLayoutGroup _boardGridLayoutGroup;

    [SerializeField] private List<Node> _nodes = new List<Node>();


    private void SpawnNode()
    {
        var boardSize = GameManager.Instance.BoardSize;
        var boardRectTransform = _boardGridLayoutGroup.GetComponent<RectTransform>();

        _boardGridLayoutGroup.cellSize = new Vector2(boardRectTransform.rect.width / boardSize,
                                                      boardRectTransform.rect.height / boardSize);

        for (int i = 0; i < boardSize * boardSize; i++)
        {
            if (i == boardSize * boardSize - 1)
            {
                var nodeNull = Instantiate(_nodeNullPrefabs, boardRectTransform).GetComponent<Node>();
                nodeNull.name = $"NodeNull";
                nodeNull.SetPosition(i);
                nodeNull.SetValue(i + 1);
                nodeNull.IsNullNode = true;
                _nodes.Add(nodeNull);
                break;
            }
            else
            {
                var node = Instantiate(_nodeNumberPrefabs, boardRectTransform).GetComponent<Node>();
                node.transform.Find("value").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                node.transform.Find("value").GetComponent<TextMeshProUGUI>().fontSize = boardRectTransform.rect.width / boardSize * 60 / 100;
                node.gameObject.name = $"Node {i + 1}";

                node.SetPosition(i);
                node.SetValue(i + 1);

                AddNodeEvent(node);
                _nodes.Add(node);
            }
        }

        SetNeighbors(_nodes, boardSize);
    }

    private void SpawnNode(SaveData data)
    {
        int boardSize = data.boardSize;
        var boardRectTransform = _boardGridLayoutGroup.GetComponent<RectTransform>();

        _boardGridLayoutGroup.cellSize = new Vector2(
            boardRectTransform.rect.width / boardSize,
            boardRectTransform.rect.height / boardSize
        );

        for (int i = 0; i < data.board.Length; i++)
        {
            int value = data.board[i];

            if (value == boardSize * boardSize)
            {
                var nodeNull = Instantiate(_nodeNullPrefabs, boardRectTransform).GetComponent<Node>();
                nodeNull.name = $"NodeNull";
                nodeNull.SetPosition(i);
                nodeNull.SetValue(value);
                nodeNull.IsNullNode = true;

                _nodes.Add(nodeNull);
            }
            else
            {
                var node = Instantiate(_nodeNumberPrefabs, boardRectTransform).GetComponent<Node>();

                var text = node.transform.Find("value").GetComponent<TextMeshProUGUI>();
                text.text = $"{value}";
                text.fontSize = boardRectTransform.rect.width / boardSize * 60 / 100;

                node.gameObject.name = $"Node {value}";

                node.SetPosition(i);
                node.SetValue(value);

                AddNodeEvent(node);
                _nodes.Add(node);
            }
        }

        SetNeighbors(_nodes, boardSize);
    }

    #endregion

    #region FUCTION_NODE

    private void AddNodeEvent(Node node)
    {
        var buttonUI = node.GetComponent<ButtonUI>();

        buttonUI.ClickFunc = () =>
        {
            SoundManager.PlaySFX(SoundType.Click);
            //Debug.Log($"Đã Click vào node {node.gameObject.name}");
            if (!isGameFinished)
                TrySwapWithNull(node);
        };
    }

    private void TrySwapWithNull(Node node)
    {
        var neighbors = node.GetNeighbors();

        Node nodeNull = null;

        foreach (Node neighbor in neighbors)
        {
            if (neighbor != null && neighbor.IsNullNode)
            {
                nodeNull = neighbor;
                break;
            }
        }

        if (nodeNull != null)
        {
            RectTransform nodeRect = node.GetComponent<RectTransform>();
            RectTransform nullRect = nodeNull.GetComponent<RectTransform>();

            Vector2 targetPos = nullRect.anchoredPosition;

            nodeRect.DOAnchorPos(targetPos, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                int nodeIndex = node.transform.GetSiblingIndex();
                int nullIndex = nodeNull.transform.GetSiblingIndex();
                node.transform.SetSiblingIndex(nullIndex);
                nodeNull.transform.SetSiblingIndex(nodeIndex);

                int tempPos = node.NodePosition;
                node.SetPosition(nodeNull.NodePosition);
                nodeNull.SetPosition(tempPos);

                int indexA = _nodes.IndexOf(node);
                int indexB = _nodes.IndexOf(nodeNull);
                _nodes[indexA] = nodeNull;
                _nodes[indexB] = node;

                nodeRect.anchoredPosition = nullRect.anchoredPosition;
                nullRect.anchoredPosition = targetPos;

                UpdateMoveCountText();
                SetNeighbors(_nodes, GameManager.Instance.BoardSize);
                CheckWin();
            });
        }
    }


    private void SetNeighbors(List<Node> nodes, int boardSize)
    {
        int width = boardSize;
        int height = boardSize;

        for (int i = 0; i < nodes.Count; i++)
        {
            int x = i % width;
            int y = i / width;

            Node node = nodes[i];

            Node top = (y - 1 >= 0) ? nodes[(y - 1) * width + x] : null;
            Node bottom = (y + 1 < height) ? nodes[(y + 1) * width + x] : null;
            Node left = (x - 1 >= 0) ? nodes[y * width + (x - 1)] : null;
            Node right = (x + 1 < width) ? nodes[y * width + (x + 1)] : null;

            node.AssignNeighbors(top, bottom, left, right);
        }
    }

    #endregion

    #region RANDOM_MAP

    private Node GetNullNode()
    {
        return _nodes.Find(n => n.IsNullNode);
    }

    private void RandomizeBoard(int moves)
    {
        Node lastMovedNode = null;

        for (int i = 0; i < moves; i++)
        {
            Node nullNode = GetNullNode();
            var neighbors = nullNode.GetNeighbors();

            // Bỏ các node null
            neighbors.RemoveAll(n => n == null);

            // Để tránh đảo ngược bước trước, nếu có hơn 1 neighbor thì bỏ node vừa swap
            if (lastMovedNode != null && neighbors.Count > 1)
            {
                neighbors.Remove(lastMovedNode);
            }

            Node randomNode = neighbors[Random.Range(0, neighbors.Count)];
            SwapNodes(randomNode, nullNode);

            lastMovedNode = randomNode;
        }
    }

    private void SwapNodes(Node node, Node nodeNull)
    {
        // Đổi vị trí trong RectTransform hierarchy
        int nodeIndex = node.transform.GetSiblingIndex();
        int nullIndex = nodeNull.transform.GetSiblingIndex();

        node.transform.SetSiblingIndex(nullIndex);
        nodeNull.transform.SetSiblingIndex(nodeIndex);

        // Đổi vị trí logic
        int tempPos = node.NodePosition;
        node.SetPosition(nodeNull.NodePosition);
        nodeNull.SetPosition(tempPos);

        // Đổi vị trí trong List<Node>
        int indexA = _nodes.IndexOf(node);
        int indexB = _nodes.IndexOf(nodeNull);

        _nodes[indexA] = nodeNull;
        _nodes[indexB] = node;

        // Gán lại neighbors sau swap
        SetNeighbors(_nodes, GameManager.Instance.BoardSize);
    }

    #endregion

    #region WIN_CONDITION

    private bool CheckWin()
    {
        foreach (Node node in _nodes)
        {
            if ((node.NodeValue - 1) != node.NodePosition)
            {
                return isGameFinished = false;
            }
        }
        Debug.Log("Win...");
        return IsGameFinished = true;
    }

    #endregion

    #region CLICK_FUNCTIONS

    public void ClickedBackToMainMenu()
    {
        if (IsGameFinished)
        {
            SoundManager.PlaySFX(SoundType.Click);

            DataManager.Instance.DeleteGameProgress();
            GameManager.Instance.IsContinue = false;
            GameManager.Instance.LoadScene("MainMenu");
            SoundManager.PlayBGM(SoundType.BGMusic);
            return;
        }

        SoundManager.PlaySFX(SoundType.Click);
        SaveData data = new SaveData()
        {
            boardSize = GameManager.Instance.BoardSize,
            board = _nodes.Select(n => n.NodeValue).ToArray(),
            difficult = GameManager.Instance.Difficult,
            moveCount = moveCount,
            timer = Mathf.Round(timer * 100f) / 100f
        };

        DataManager.Instance.SaveGameProgress(data);
        GameManager.Instance.LoadScene("MainMenu");
        SoundManager.PlayBGM(SoundType.BGMusic);
    }

    #endregion

    #region RESULT_PANEL

    [SerializeField] private RectTransform _resultPanel;
    [SerializeField] private Image _darkPanel;

    private void DarkPanelIntro(float duration)
    {
        _darkPanel.raycastTarget = true;
        _darkPanel.DOFade(0.98f, duration).OnComplete(() =>
        {
            ResutPanelIntro(duration * 0.6f);
        });
    }

    private void DarkPanelOutro(float duration)
    {
        _darkPanel.raycastTarget = false;
        _darkPanel.DOFade(0, duration).OnComplete(() =>
        {
            ResutPanelOutro(duration * 0.6f);
        });
    }

    private void ResutPanelIntro(float duration)
    {
        _resultPanel.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InCubic);
    }

    private void ResutPanelOutro(float duration)
    {
        _resultPanel.DOAnchorPos(new Vector2(0, 1900), duration).SetEase(Ease.InCubic);
    }

    #endregion
}
