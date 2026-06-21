using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Screen")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Sound in game")]
    public Slider volumeSlider;      
    public Slider sfxPauseSlider;    
    public AudioSource inGameMusicSource;

    [Header("Result Screen")]
    public Image resultBannerImage;
    public Sprite winSprite;
    public Sprite loseSprite;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (inGameMusicSource == null) inGameMusicSource = GetComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        if (inGameMusicSource != null)
        {
            inGameMusicSource.volume = savedVolume;
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        if (sfxPauseSlider != null)
        {
            float savedSFX = (SoundManager.Instance != null) ? SoundManager.Instance.GetSFXVolume() : PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            sfxPauseSlider.value = savedSFX;
            
            if (SoundManager.Instance != null)
            {
                sfxPauseSlider.onValueChanged.AddListener(SoundManager.Instance.ChangeSFXVolume);
            }
        }
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausePanel.activeSelf) ResumeGame();
            else OpenPause();
        }
    }

    public void OpenPause()
    {
        PlayClickSound();
        pausePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        PlayClickSound();
        pausePanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void ChangeVolume(float value)
    {
        if (inGameMusicSource != null)
        {
            inGameMusicSource.volume = value;
        }
        PlayerPrefs.SetFloat("GameVolume", value);
    }

    public void ExitToMainMenu()
    {
        PlayClickSound();
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }

    public void ShowMatchResult(bool isWin)
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 

        if (isWin) resultBannerImage.sprite = winSprite;
        else resultBannerImage.sprite = loseSprite;
        
        resultBannerImage.SetNativeSize(); 
    }

    public void PlayClickSound()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
    }
}
