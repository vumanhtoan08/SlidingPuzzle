using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _levelPanel;
    [SerializeField] private GameObject _continueButton;
    private SaveData data;

    private void Awake()
    {
        _titlePanel.SetActive(true);
        _levelPanel.SetActive(false);

        _onOffSoundButton.onClick.AddListener(ActiveSoundButton);
        _onOffMusicButton.onClick.AddListener(ActiveMusicButton);
    }

    private void Start()
    {
        data = DataManager.Instance.LoadGameProgress();
        _continueButton.SetActive(data != null);

        SoundData soundData = SoundManager.Instance.soundData;
        CheckUIWhenStarted(soundData.isSoundMute, soundData.isMusicMute);
    }

    public void ClickPlay()
    {
        SoundManager.PlaySFX(SoundType.Click);

        _titlePanel.SetActive(false);
        _levelPanel.SetActive(true);
    }

    public void ClickedBackToTitle()
    {
        SoundManager.PlaySFX(SoundType.Click);

        _titlePanel.SetActive(true);
        _levelPanel.SetActive(false);
    }

    public void ClickContinue()
    {
        SoundManager.PlaySFX(SoundType.Click);

        GameManager.Instance.IsContinue = true;
        GameManager.Instance.BoardSize = data.boardSize;
        GameManager.Instance.Difficult = data.difficult;
        GameManager.Instance.LoadScene("Gameplay");
        SoundManager.PlayBGM(SoundType.BGMusic);
    }

    #region SETTING_PANEL

    [SerializeField] private Image _settingPanel;
    [SerializeField] private RectTransform _settingBoard;

    public void SettingPanelIntro()
    {
        SoundManager.PlaySFX(SoundType.Click);
        _settingPanel.raycastTarget = true;
        _settingPanel.DOFade(0.98f, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            _settingBoard.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InCubic);
        });
    }

    public void SettingPanelOutro()
    {
        SoundManager.PlaySFX(SoundType.Click);
        _settingBoard.DOAnchorPos(new Vector2(0, 1900f), 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            _settingPanel.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
            _settingPanel.raycastTarget = false;
        });
    }

    [SerializeField] private RectTransform _onOffSound;
    [SerializeField] private Button _onOffSoundButton;
    [SerializeField] private RectTransform _onOffMusic;
    [SerializeField] private Button _onOffMusicButton;

    private void CheckUIWhenStarted(bool isSoundMute, bool isMusicMute)
    {
        RectTransform buttonSoundRect = _onOffSoundButton.GetComponent<RectTransform>();
        RectTransform textSoundRect = _onOffSound.transform.Find("text").GetComponent<RectTransform>();
        TMP_Text textSound = _onOffSound.transform.Find("text").GetComponent<TMP_Text>();
        Image onOffSoundBG = _onOffSound.GetComponent<Image>();

        if (isSoundMute)
        {
            buttonSoundRect.DOAnchorPos(new Vector2(-50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffSoundBG.color = new Color32(98, 98, 98, 255);
                textSound.text = "OFF";
                textSoundRect.anchoredPosition = new Vector2(20, 0);
                textSound.color = new Color32(196, 189, 181, 255);
                textSoundRect.gameObject.SetActive(true);
            });
        }
        else
        {
            buttonSoundRect.DOAnchorPos(new Vector2(50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffSoundBG.color = new Color32(241, 110, 80, 255);
                textSound.text = "ON";
                textSoundRect.anchoredPosition = new Vector2(-20, 0);
                textSound.color = Color.white;
                textSoundRect.gameObject.SetActive(true);
            });
        }

        RectTransform buttonMusicRect = _onOffMusicButton.GetComponent<RectTransform>();
        RectTransform textRect = _onOffMusic.transform.Find("text").GetComponent<RectTransform>();
        TMP_Text text = _onOffMusic.transform.Find("text").GetComponent<TMP_Text>();
        Image onOffMusicBG = _onOffMusic.GetComponent<Image>();

        if (isMusicMute)
        {
            buttonMusicRect.DOAnchorPos(new Vector2(-50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffMusicBG.color = new Color32(98, 98, 98, 255);
                text.text = "OFF";
                textRect.anchoredPosition = new Vector2(20, 0);
                text.color = new Color32(196, 189, 181, 255);
                textRect.gameObject.SetActive(true);
            });
        }
        else
        {
            buttonMusicRect.DOAnchorPos(new Vector2(50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffMusicBG.color = new Color32(241, 110, 80, 255);
                text.text = "ON";
                textRect.anchoredPosition = new Vector2(-20, 0);
                text.color = Color.white;
                textRect.gameObject.SetActive(true);
            });
        }
    }

    public void ActiveSoundButton()
    {
        SoundManager.PlaySFX(SoundType.Click);

        SoundManager.Instance.ToggleSound();

        bool isMuted = SoundManager.Instance.soundData.isSoundMute;

        RectTransform buttonSoundRect = _onOffSoundButton.GetComponent<RectTransform>();
        RectTransform textRect = _onOffSound.transform.Find("text").GetComponent<RectTransform>();
        TMP_Text text = _onOffSound.transform.Find("text").GetComponent<TMP_Text>();
        Image onOffSoundBG = _onOffSound.GetComponent<Image>();

        textRect.gameObject.SetActive(false);

        if (isMuted)
        {
            buttonSoundRect.DOAnchorPos(new Vector2(-50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffSoundBG.color = new Color32(98, 98, 98, 255);
                text.text = "OFF";
                textRect.anchoredPosition = new Vector2(20, 0);
                text.color = new Color32(196, 189, 181, 255);
                textRect.gameObject.SetActive(true);
            });
        }
        else
        {
            buttonSoundRect.DOAnchorPos(new Vector2(50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffSoundBG.color = new Color32(241, 110, 80, 255);
                text.text = "ON";
                textRect.anchoredPosition = new Vector2(-20, 0);
                text.color = Color.white;
                textRect.gameObject.SetActive(true);
            });
        }
    }

    public void ActiveMusicButton()
    {
        SoundManager.PlaySFX(SoundType.Click);

        SoundManager.Instance.ToggleMusic();

        bool isMuted = SoundManager.Instance.soundData.isMusicMute;

        RectTransform buttonMusicRect = _onOffMusicButton.GetComponent<RectTransform>();
        RectTransform textRect = _onOffMusic.transform.Find("text").GetComponent<RectTransform>();
        TMP_Text text = _onOffMusic.transform.Find("text").GetComponent<TMP_Text>();
        Image onOffMusicBG = _onOffMusic.GetComponent<Image>();

        textRect.gameObject.SetActive(false);

        if (isMuted)
        {
            buttonMusicRect.DOAnchorPos(new Vector2(-50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffMusicBG.color = new Color32(98, 98, 98, 255);
                text.text = "OFF";
                textRect.anchoredPosition = new Vector2(20, 0);
                text.color = new Color32(196, 189, 181, 255);
                textRect.gameObject.SetActive(true);
            });
        }
        else
        {
            buttonMusicRect.DOAnchorPos(new Vector2(50, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onOffMusicBG.color = new Color32(241, 110, 80, 255);
                text.text = "ON";
                textRect.anchoredPosition = new Vector2(-20, 0);
                text.color = Color.white;
                textRect.gameObject.SetActive(true);
            });
        }
    }


    #endregion
}
