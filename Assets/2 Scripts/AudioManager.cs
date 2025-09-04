using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // 각 사운드에 대한 오디오 소스 변수
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    // === 효과음(SFX)을 위한 오디오 소스 ===
    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    public AudioSource footstepSoundSource;

    // 각 오디오 소스의 원래 볼륨을 저장할 변수
    private float ingameMusicOriginalVolume;
    private float chaseMusicOriginalVolume;

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

        if (ingameMusicSource != null)
        {
            ingameMusicOriginalVolume = ingameMusicSource.volume;
        }
        if (chaseMusicSource != null)
        {
            chaseMusicOriginalVolume = chaseMusicSource.volume;
        }
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
    }

    // 모든 음악을 정지
    private void StopAllMusic()
    {
        if (mainMusicSource != null) mainMusicSource.Stop();
        if (ingameMusicSource != null) ingameMusicSource.Stop();
        if (chaseMusicSource != null) chaseMusicSource.Stop();
    }

    // 메인 메뉴 음악 재생
    public void PlayMainMusic()
    {
        StopAllMusic();
        if (mainMusicSource != null)
        {
            mainMusicSource.Play();
        }
    }

    // 인게임 음악 재생
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
        // 추격 음악을 5초에 걸쳐 페이드아웃
        StartCoroutine(StopChaseMusicWithFade(5f, 10f));
    }

    public void PlayFlashlightSound()
    {
        if (flashlightSoundSource != null)
        {
            flashlightSoundSource.Play();
        }
    }

    // === 발소리 재생 및 정지 함수 ===
    public void PlayFootstepSound()
    {
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
    }

    // === 모든 음악 및 효과음을 일시 정지하는 함수 ===
    public void PauseAllAudio()
    {
        if (mainMusicSource != null && mainMusicSource.isPlaying) mainMusicSource.Pause();
        if (ingameMusicSource != null && ingameMusicSource.isPlaying) ingameMusicSource.Pause();
        if (chaseMusicSource != null && chaseMusicSource.isPlaying) chaseMusicSource.Pause();
        if (flashlightSoundSource != null && flashlightSoundSource.isPlaying) flashlightSoundSource.Pause();
        if (footstepSoundSource != null && footstepSoundSource.isPlaying) footstepSoundSource.Pause();
    }

    // === 일시 정지된 모든 음악 및 효과음을 다시 재생하는 함수 ===
    public void ResumeAllAudio()
    {
        if (mainMusicSource != null) mainMusicSource.UnPause();
        if (ingameMusicSource != null) ingameMusicSource.UnPause();
        if (chaseMusicSource != null) chaseMusicSource.UnPause();
        if (flashlightSoundSource != null) flashlightSoundSource.UnPause();
        if (footstepSoundSource != null) footstepSoundSource.UnPause();
    }

    // === 코루틴 ===
    private IEnumerator StopChaseMusicWithFade(float fadeOutDuration, float delayDuration)
    {
        // 추격 음악을 5초 동안 페이드아웃
        yield return StartCoroutine(FadeOut(chaseMusicSource, fadeOutDuration));

        // 10초 동안 대기
        yield return new WaitForSeconds(delayDuration);

        // 게임 내 음악 페이드인
        PlayIngameMusic();
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume; // 원래 볼륨으로 되돌림
    }
}