using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int _boardSize;
    [SerializeField] private string _levelDifficult;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(ClickedButton);
    }

    private void ClickedButton()
    {
        SoundManager.PlaySFX(SoundType.Click);

        GameManager.Instance.BoardSize = _boardSize;
        GameManager.Instance.Difficult = _levelDifficult;
        GameManager.Instance.IsContinue = false;
        DataManager.Instance.DeleteGameProgress();
        //GameManager.Instance.GoToGameplayScene();
        GameManager.Instance.LoadScene("Gameplay");
        SoundManager.PlayBGM(SoundType.IGMusic);
    }
}
