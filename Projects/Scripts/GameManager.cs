using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    #region GAME_VARIABLE

    private int boardSize;
    private string difficult;
    public bool IsContinue = false;

    public int BoardSize
    {
        get { return boardSize; }
        set { boardSize = value; }
    }

    public string Difficult
    {
        get { return difficult; }
        set { difficult = value; }
    }

    #endregion

    #region START METHODS
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    #endregion

    #region SAVE_LOAD_METHODS

    public void SaveGame(int boardSize, int[] board, int moveCount, float timer)
    {
        SaveData data = new SaveData()
        {
            boardSize = boardSize,
            board = board,
            moveCount = moveCount,
            timer = timer
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveGame", json);
        PlayerPrefs.Save();

        Debug.Log("Game Save Successed");
    }

    public SaveData LoadGameData()
    {
        if (PlayerPrefs.HasKey("SaveGame"))
        {
            string json = PlayerPrefs.GetString("SaveGame");
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game Loaded");
            return data;
        }
        else
        {
            Debug.Log("Not Founded Data");
            return null;
        }
    }

    #endregion

    #region SCENES_MANAGER

    [SerializeField] private Image _darkImage;
    [SerializeField] private float duration = 0.5f;

    public void LoadScene(string sceneName)
    {
        _darkImage.raycastTarget = true;
        _darkImage.DOFade(1f, duration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _darkImage.DOFade(0f, duration).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            _darkImage.raycastTarget = false;
        });
    }

    #endregion
}