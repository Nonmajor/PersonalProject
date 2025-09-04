using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // 각 사운드에 대한 오디오 소스 변수
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    public AudioSource footstepSoundSource;

    // 이 변수들은 이제 값을 저장하는 역할만 합니다.
    private float mainVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float effectVolume = 1.0f;

    // 원래 볼륨 저장 변수들은 유지
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

        // Awake에서 원래 볼륨을 저장합니다.
        // 이 값들은 OnSceneLoaded에서 다시 설정되지 않습니다.
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

        // 게임 시작 시 저장된 볼륨 설정 불러오기
        LoadVolumeSettings();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 새로운 슬라이더를 찾아서 볼륨을 설정합니다.
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
        // 씬의 모든 Slider 컴포넌트를 찾습니다.
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

    // PlayerPrefs에 볼륨 설정을 저장하는 함수
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);
        PlayerPrefs.Save();
    }

    // PlayerPrefs에서 볼륨 설정을 불러오는 함수
    private void LoadVolumeSettings()
    {
        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f); // 기본값 1.0f
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        effectVolume = PlayerPrefs.GetFloat("EffectVolume", 1.0f);

        // 불러온 볼륨 값으로 모든 사운드 볼륨을 즉시 업데이트
        UpdateAllVolumes();
    }

    // 각 슬라이더의 onValueChanged 이벤트에 연결할 함수들
    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings(); // 볼륨 변경 시 저장
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

    // === 기존 함수들 ===
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