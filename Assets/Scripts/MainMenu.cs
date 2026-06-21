using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Screen")]
    public GameObject settingsPanel;
    public GameObject playModePanel;
    public GameObject saveSlotsPanel;

    [Header("Background Sound")]
    public Slider volumeSlider;
    public AudioSource backgroundMusicSource;

    [Header("Sound FX")]
    public Slider sfxSlider;

    [Header("SaveText")]
    public TextMeshProUGUI[] slotRatingTexts; 

    private bool isNewGameMode = false;

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (playModePanel != null) playModePanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);

        if (backgroundMusicSource == null) backgroundMusicSource = GetComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = savedVolume;
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume); 
        }

        if (sfxSlider != null && SoundManager.Instance != null)
        {
            sfxSlider.value = SoundManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.ChangeSFXVolume);
        }
    }

    public void ChangeVolume(float value)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = value;
        }
        PlayerPrefs.SetFloat("GameVolume", value); 
    }

    public void PlayClickSound()
    {
        if (SoundManager.Instance != null) 
        {
            SoundManager.Instance.PlayClick();
        }
    }

    public void OnClickPlayTrigger()
    {
        PlayClickSound();
        playModePanel.SetActive(true); 
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void SelectNewGameMode()
    {
        PlayClickSound();
        isNewGameMode = true;
        playModePanel.SetActive(false);
        saveSlotsPanel.SetActive(true); 
        UpdateSlotsUI(); 
    }

    public void SelectLoadGameMode()
    {
        PlayClickSound();
        isNewGameMode = false;
        playModePanel.SetActive(false);
        saveSlotsPanel.SetActive(true); 
        UpdateSlotsUI(); 
    }

    void UpdateSlotsUI()
    {
        for (int i = 0; i < 4; i++)
        {
            int slotNum = i + 1;
            if (SaveSystem.DoesSlotExist(slotNum))
            {
                int rating = SaveSystem.LoadRating(slotNum);
                slotRatingTexts[i].text = $"Rank: {rating} PTS";
            }
            else
            {
                slotRatingTexts[i].text = "Empty Slot";
            }
        }
    }

    public void OnClickSaveSlot(int slotNumber)
    {
        PlayClickSound();
        SaveSystem.currentSaveSlot = slotNumber;

        if (isNewGameMode)
        {
            SaveSystem.DeleteSlot(slotNumber);
            SaveSystem.SaveRating(slotNumber, 100); 
        }

        SceneManager.LoadScene(2);
    }

    public void OpenTutorialScene() 
    { 
        PlayClickSound();
        SceneManager.LoadScene(3);
    }

    public void OpenSettings() { PlayClickSound(); if (settingsPanel != null) settingsPanel.SetActive(true); }
    public void CloseSettings() { PlayClickSound(); if (settingsPanel != null) settingsPanel.SetActive(false); }
}
