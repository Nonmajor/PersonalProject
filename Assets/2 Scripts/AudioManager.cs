using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // �� ���忡 ���� ����� �ҽ� ����
    [Header("Music")]
    public AudioSource mainMusicSource;
    public AudioSource ingameMusicSource;
    public AudioSource chaseMusicSource;

    // === �߰�: ȿ����(SFX)�� ���� ����� �ҽ� ===
    [Header("SFX")]
    public AudioSource flashlightSoundSource;
    // === �߼Ҹ��� AudioSource ���� �߰� ===
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

        // ���� ���� �� ����
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

    // === ���� ��� �� ���� �Լ� ===

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

    
    public void PlayFootstepSound()
    {
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
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
        audioSource.volume = startVolume; // ������ ������� �ǵ��� ���� ����� ���
    }
}