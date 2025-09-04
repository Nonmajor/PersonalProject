using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // �� ���忡 ���� ����� �ҽ� ����
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    public AudioSource footstepSoundSource;

    // �� �������� ���� ���� �����ϴ� ���Ҹ� �մϴ�.
    private float mainVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float effectVolume = 1.0f;

    // ���� ���� ���� �������� ����
    private float mainMusicOriginalVolume;
    private float ingameMusicOriginalVolume;
    private float chaseMusicOriginalVolume;
    private float flashlightOriginalVolume;
    private float footstepOriginalVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        // Awake���� ���� ������ �����մϴ�.
        // �� ������ OnSceneLoaded���� �ٽ� �������� �ʽ��ϴ�.
        if (mainMusicSource != null)
        {
            mainMusicOriginalVolume = mainMusicSource.volume;
        }
        if (ingameMusicSource != null)
        {
            ingameMusicOriginalVolume = ingameMusicSource.volume;
        }
        if (chaseMusicSource != null)
        {
            chaseMusicOriginalVolume = chaseMusicSource.volume;
        }
        if (flashlightSoundSource != null)
        {
            flashlightOriginalVolume = flashlightSoundSource.volume;
        }
        if (footstepSoundSource != null)
        {
            footstepOriginalVolume = footstepSoundSource.volume;
        }

        // ���� ���� �� ����� ���� ���� �ҷ�����
        LoadVolumeSettings();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� ������ ���ο� �����̴��� ã�Ƽ� ������ �����մϴ�.
        FindAndSetupVolumeSliders();

        if (scene.name == "MainMenu")
        {
            PlayMainMusic();
        }
        else if (scene.name == "Game")
        {
            PlayIngameMusic();
        }
        else
        {
            StopAllMusic();
        }
    }

    private void FindAndSetupVolumeSliders()
    {
        // ���� ��� Slider ������Ʈ�� ã���ϴ�.
        Slider[] allSliders = FindObjectsOfType<Slider>(true);

        foreach (Slider slider in allSliders)
        {
            if (slider.gameObject.name == "MainVolumeSlider")
            {
                slider.value = mainVolume;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(SetMainVolume);
            }
            else if (slider.gameObject.name == "MusicVolumeSlider")
            {
                slider.value = musicVolume;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(SetMusicVolume);
            }
            else if (slider.gameObject.name == "EffectVolumeSlider")
            {
                slider.value = effectVolume;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(SetEffectVolume);
            }
        }
    }

    // PlayerPrefs�� ���� ������ �����ϴ� �Լ�
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);
        PlayerPrefs.Save();
    }

    // PlayerPrefs���� ���� ������ �ҷ����� �Լ�
    private void LoadVolumeSettings()
    {
        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f); // �⺻�� 1.0f
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        effectVolume = PlayerPrefs.GetFloat("EffectVolume", 1.0f);

        // �ҷ��� ���� ������ ��� ���� ������ ��� ������Ʈ
        UpdateAllVolumes();
    }

    // �� �����̴��� onValueChanged �̺�Ʈ�� ������ �Լ���
    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings(); // ���� ���� �� ����
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetEffectVolume(float volume)
    {
        effectVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    // === ���� �Լ��� ===
    private void StopAllMusic()
    {
        if (mainMusicSource != null) mainMusicSource.Stop();
        if (ingameMusicSource != null) ingameMusicSource.Stop();
        if (chaseMusicSource != null) chaseMusicSource.Stop();
    }

    public void PlayMainMusic()
    {
        StopAllMusic();
        if (mainMusicSource != null)
        {
            mainMusicSource.Play();
        }
    }

    public void PlayIngameMusic()
    {
        StopAllMusic();
        if (ingameMusicSource != null && !ingameMusicSource.isPlaying)
        {
            ingameMusicSource.Play();
        }
    }

    public void PlayChaseMusic()
    {
        if (ingameMusicSource != null && ingameMusicSource.isPlaying) ingameMusicSource.Stop();
        if (chaseMusicSource != null && !chaseMusicSource.isPlaying)
        {
            chaseMusicSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        StartCoroutine(StopChaseMusicWithFade(5f, 10f));
    }

    public void PlayFlashlightSound()
    {
        if (flashlightSoundSource != null)
        {
            flashlightSoundSource.Play();
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
    }

    public void PauseAllAudio()
    {
        if (mainMusicSource != null) mainMusicSource.Pause();
        if (ingameMusicSource != null) ingameMusicSource.Pause();
        if (chaseMusicSource != null) chaseMusicSource.Pause();
        if (flashlightSoundSource != null) flashlightSoundSource.Pause();
        if (footstepSoundSource != null) footstepSoundSource.Pause();
    }

    public void ResumeAllAudio()
    {
        if (mainMusicSource != null) mainMusicSource.UnPause();
        if (ingameMusicSource != null) ingameMusicSource.UnPause();
        if (chaseMusicSource != null) chaseMusicSource.UnPause();
        if (flashlightSoundSource != null) flashlightSoundSource.UnPause();
        if (footstepSoundSource != null) footstepSoundSource.UnPause();
    }

    private void UpdateAllVolumes()
    {
        if (mainMusicSource != null) mainMusicSource.volume = mainMusicOriginalVolume * mainVolume * musicVolume;
        if (ingameMusicSource != null) ingameMusicSource.volume = ingameMusicOriginalVolume * mainVolume * musicVolume;
        if (chaseMusicSource != null) chaseMusicSource.volume = chaseMusicOriginalVolume * mainVolume * musicVolume;
        if (flashlightSoundSource != null) flashlightSoundSource.volume = flashlightOriginalVolume * mainVolume * effectVolume;
        if (footstepSoundSource != null) footstepSoundSource.volume = footstepOriginalVolume * mainVolume * effectVolume;
    }

    private IEnumerator StopChaseMusicWithFade(float fadeOutDuration, float delayDuration)
    {
        float startVolume = chaseMusicSource.volume;
        while (chaseMusicSource.volume > 0)
        {
            chaseMusicSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }
        chaseMusicSource.Stop();
        chaseMusicSource.volume = chaseMusicOriginalVolume * mainVolume * musicVolume;
        yield return new WaitForSeconds(delayDuration);
        PlayIngameMusic();
    }
}