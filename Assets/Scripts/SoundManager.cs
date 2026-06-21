using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("All sound volume")]
    public AudioClip clickClip; 

    private AudioSource audioSource;
    
    public float sfxVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip, sfxVolume);
        }
    }

    public void ChangeSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}
