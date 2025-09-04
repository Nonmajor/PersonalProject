using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // 추가: UI 요소를 사용하기 위해 필요

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

    // === 추가: 볼륨 조절 슬라이더 변수 ===
    [Header("Volume Sliders")]
    public Slider mainVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;


    // 원래 볼륨을 저장할 변수들
    private float mainMusicOriginalVolume;
    private float ingameMusicOriginalVolume;
    private float chaseMusicOriginalVolume;
    private float flashlightOriginalVolume;
    private float footstepOriginalVolume;

    // 현재 볼륨 상태를 저장할 변수
    private float mainVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float effectVolume = 1.0f;

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

        // Awake()에서 모든 AudioSource의 볼륨을 1로 강제 설정 후, 원래 볼륨 값 저장
        if (mainMusicSource != null)
        {
            mainMusicSource.volume = 1.0f;
            mainMusicOriginalVolume = mainMusicSource.volume;
        }
        if (ingameMusicSource != null)
        {
            ingameMusicSource.volume = 1.0f;
            ingameMusicOriginalVolume = ingameMusicSource.volume;
        }
        if (chaseMusicSource != null)
        {
            chaseMusicSource.volume = 1.0f;
            chaseMusicOriginalVolume = chaseMusicSource.volume;
        }
        if (flashlightSoundSource != null)
        {
            flashlightSoundSource.volume = 1.0f;
            flashlightOriginalVolume = flashlightSoundSource.volume;
        }
        if (footstepSoundSource != null)
        {
            footstepSoundSource.volume = 1.0f;
            footstepOriginalVolume = footstepSoundSource.volume;
        }
    }


    private void Update()
    {
        UpdateAllVolumes();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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

        UpdateAllVolumes();
    }

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

    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        UpdateAllVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateAllVolumes();
    }

    public void SetEffectVolume(float volume)
    {
        effectVolume = volume;
        UpdateAllVolumes();
    }

    private void UpdateAllVolumes()
    {
        // 슬라이더가 연결되어 있으면 값을 가져오고, 아니면 기본값(1.0f) 사용
        float currentMainVolume = (mainVolumeSlider != null) ? mainVolumeSlider.value : 1.0f;
        float currentMusicVolume = (musicVolumeSlider != null) ? musicVolumeSlider.value : 1.0f;
        float currentEffectVolume = (effectVolumeSlider != null) ? effectVolumeSlider.value : 1.0f;

        if (mainMusicSource != null) mainMusicSource.volume = mainMusicOriginalVolume * currentMainVolume * currentMusicVolume;
        if (ingameMusicSource != null) ingameMusicSource.volume = ingameMusicOriginalVolume * currentMainVolume * currentMusicVolume;
        if (chaseMusicSource != null) chaseMusicSource.volume = chaseMusicOriginalVolume * currentMainVolume * currentMusicVolume;

        if (flashlightSoundSource != null) flashlightSoundSource.volume = flashlightOriginalVolume * currentMainVolume * currentEffectVolume;
        if (footstepSoundSource != null) footstepSoundSource.volume = footstepOriginalVolume * footstepOriginalVolume * currentMainVolume * currentEffectVolume;
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