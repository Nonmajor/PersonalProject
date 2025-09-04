using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // �� ���忡 ���� ����� �ҽ� ����
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    // === ȿ����(SFX)�� ���� ����� �ҽ� ===
    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    public AudioSource footstepSoundSource;

    // �� ����� �ҽ��� ���� ������ ������ ����
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

    // ��� ������ ����
    private void StopAllMusic()
    {
        if (mainMusicSource != null) mainMusicSource.Stop();
        if (ingameMusicSource != null) ingameMusicSource.Stop();
        if (chaseMusicSource != null) chaseMusicSource.Stop();
    }

    // ���� �޴� ���� ���
    public void PlayMainMusic()
    {
        StopAllMusic();
        if (mainMusicSource != null)
        {
            mainMusicSource.Play();
        }
    }

    // �ΰ��� ���� ���
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
        // �߰� ������ 5�ʿ� ���� ���̵�ƿ�
        StartCoroutine(StopChaseMusicWithFade(5f, 10f));
    }

    public void PlayFlashlightSound()
    {
        if (flashlightSoundSource != null)
        {
            flashlightSoundSource.Play();
        }
    }

    // === �߼Ҹ� ��� �� ���� �Լ� ===
    public void PlayFootstepSound()
    {
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
    }

    // === ��� ���� �� ȿ������ �Ͻ� �����ϴ� �Լ� ===
    public void PauseAllAudio()
    {
        if (mainMusicSource != null && mainMusicSource.isPlaying) mainMusicSource.Pause();
        if (ingameMusicSource != null && ingameMusicSource.isPlaying) ingameMusicSource.Pause();
        if (chaseMusicSource != null && chaseMusicSource.isPlaying) chaseMusicSource.Pause();
        if (flashlightSoundSource != null && flashlightSoundSource.isPlaying) flashlightSoundSource.Pause();
        if (footstepSoundSource != null && footstepSoundSource.isPlaying) footstepSoundSource.Pause();
    }

    // === �Ͻ� ������ ��� ���� �� ȿ������ �ٽ� ����ϴ� �Լ� ===
    public void ResumeAllAudio()
    {
        if (mainMusicSource != null) mainMusicSource.UnPause();
        if (ingameMusicSource != null) ingameMusicSource.UnPause();
        if (chaseMusicSource != null) chaseMusicSource.UnPause();
        if (flashlightSoundSource != null) flashlightSoundSource.UnPause();
        if (footstepSoundSource != null) footstepSoundSource.UnPause();
    }

    // === �ڷ�ƾ ===
    private IEnumerator StopChaseMusicWithFade(float fadeOutDuration, float delayDuration)
    {
        // �߰� ������ 5�� ���� ���̵�ƿ�
        yield return StartCoroutine(FadeOut(chaseMusicSource, fadeOutDuration));

        // 10�� ���� ���
        yield return new WaitForSeconds(delayDuration);

        // ���� �� ���� ���̵���
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
        audioSource.volume = startVolume; // ���� �������� �ǵ���
    }
}