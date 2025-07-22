using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private const string SaveDataDifficult = "DataDifficult";
    private const string firstTimeToOpenGame = "FirstTime";

    public DifficultListData CurrentData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            LoadGameProgress();
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        int firstTime = PlayerPrefs.GetInt(firstTimeToOpenGame, 0);

        if (firstTime == 0) // nếu chưa mở game bao giờ
        {
            CurrentData = new DifficultListData()
            {
                difficultDatas = new List<DifficultData>()
            {
                new DifficultData { difficult = "Easy", timer = 0f },
                new DifficultData { difficult = "Medium", timer = 0f},
                new DifficultData { difficult = "Challenging", timer = 0f},
                new DifficultData { difficult = "Hard", timer = 0f},
                new DifficultData { difficult = "Very Hard", timer = 0f},
                new DifficultData { difficult = "Extreme", timer = 0f},
            }
            };

            PlayerPrefs.SetInt(firstTimeToOpenGame, 1);
            PlayerPrefs.Save();
            Debug.Log("First time opening game - data initialized.");

            SaveData();
        }
        else
        {
            string json = PlayerPrefs.GetString(SaveDataDifficult, "");
            if (!string.IsNullOrEmpty(json))
            {
                CurrentData = JsonUtility.FromJson<DifficultListData>(json);
                Debug.Log($"Data loaded: {json}");
            }
        }
    }


    public void SaveData()
    {
        string json = JsonUtility.ToJson(CurrentData);
        PlayerPrefs.SetString(SaveDataDifficult, json);
        PlayerPrefs.Save();
        Debug.Log($"Data Saved: {json}");
    }

    public void UpdateCurrentData(string difficultLevel, float timer)
    {
        var item = CurrentData.difficultDatas.Find(d => d.difficult == difficultLevel);

        if (item != null)
        {
            if (item.timer == 0f || timer < item.timer)
            {
                float rounded = Mathf.Round(timer * 100f) / 100f;

                Debug.Log($"Update timer for '{difficultLevel}': {item.timer} -> {rounded}");

                item.timer = rounded;
                SaveData();
            }
        }
    }

    public float GetTimer(string difficultLevel)
    {
        var item = CurrentData.difficultDatas.Find(d => d.difficult == difficultLevel);
        if (item != null)
        {
            return item.timer;
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy dữ liệu cho độ khó: {difficultLevel}");
            return 0f;
        }
    }

    #region PROGRESS_PLAYER

    private const string SaveKeyGameProgress = "GameProgressData";

    public void SaveGameProgress(SaveData data)
    {
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(SaveKeyGameProgress, json);
        PlayerPrefs.Save();
    }

    public SaveData LoadGameProgress()
    {
        string json = PlayerPrefs.GetString(SaveKeyGameProgress);

        if (!string.IsNullOrEmpty(json))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            return null;
        }
    }

    public void DeleteGameProgress()
    {
        PlayerPrefs.DeleteKey(SaveKeyGameProgress);
        PlayerPrefs.Save();
    }

    #endregion
}
