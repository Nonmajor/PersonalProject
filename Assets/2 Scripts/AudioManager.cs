using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // 각 사운드에 대한 오디오 소스 변수
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    // === 추가: 효과음(SFX)을 위한 오디오 소스 ===
    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    // === 발소리용 AudioSource 변수 추가 ===
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

        // 원래 볼륨 값 저장
        ingameMusicOriginalVolume = ingameMusicSource.volume;
        chaseMusicOriginalVolume = chaseMusicSource.volume;
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
    }

    // === 사운드 재생 및 정지 함수 ===

    public void PlayMainMusic()
    {
        if (ingameMusicSource != null) ingameMusicSource.Stop();
        if (chaseMusicSource != null) chaseMusicSource.Stop();
        if (mainMusicSource != null && !mainMusicSource.isPlaying)
        {
            mainMusicSource.Play();
        }
    }

    public void PlayIngameMusic()
    {
        if (mainMusicSource != null) mainMusicSource.Stop();
        if (ingameMusicSource != null && !ingameMusicSource.isPlaying)
        {
            ingameMusicSource.Play();
        }
    }

    public void PlayChaseMusic()
    {
        if (ingameMusicSource != null) ingameMusicSource.Stop();
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

    
    public void PlayFootstepSound()
    {
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
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
        audioSource.volume = startVolume; // 볼륨을 원래대로 되돌려 다음 재생에 대비
    }
}