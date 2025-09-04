using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// 게임의 모든 오디오(음악, 효과음)를 관리하는 싱글톤 스크립트
// 씬이 변경되어도 파괴되지 않고 유지됩니다.
public class AudioManager : MonoBehaviour
{
    // 이 스크립트의 싱글톤 인스턴스
    public static AudioManager Instance;

    // 각 사운드에 대한 오디오 소스 변수
    // 유니티 에디터에서 AudioSource 컴포넌트를 할당해야 합니다.
    [Header("Music")]
    public AudioSource mainMusicSource;   // 메인 메뉴 음악
    public AudioSource ingameMusicSource; // 인게임 배경 음악
    public AudioSource chaseMusicSource;  // 몬스터 추격 음악

    [Header("SFX")]
    public AudioSource flashlightSoundSource; // 손전등 효과음
    public AudioSource footstepSoundSource;   // 발소리 효과음

    // 슬라이더 값에 따라 최종 볼륨을 계산하기 위한 변수
    private float mainVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float effectVolume = 1.0f;

    // 각 AudioSource의 원래 볼륨을 저장하는 변수
    // 이 값들은 볼륨 조절 시 기준이 됩니다.
    private float mainMusicOriginalVolume;
    private float ingameMusicOriginalVolume;
    private float chaseMusicOriginalVolume;
    private float flashlightOriginalVolume;
    private float footstepOriginalVolume;

    // === 초기화 ===
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // 씬이 변경되어도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
            // 씬이 로드될 때마다 OnSceneLoaded 함수를 호출하도록 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // 이미 인스턴스가 존재하면 현재 게임 오브젝트 파괴
            Destroy(gameObject);
        }

        // 게임 시작 시 각 AudioSource의 원래 볼륨 값 저장
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

    // 오브젝트가 파괴될 때 이벤트 등록 해제
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출되는 이벤트 핸들러
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 시 슬라이더를 다시 찾고 연결
        FindAndSetupVolumeSliders();

        // 현재 씬에 따라 적절한 음악을 재생
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

    // 씬에서 볼륨 슬라이더를 찾아 연결하는 함수
    private void FindAndSetupVolumeSliders()
    {
        // 씬에 있는 모든 슬라이더 컴포넌트를 찾음
        Slider[] allSliders = FindObjectsOfType<Slider>(true);

        foreach (Slider slider in allSliders)
        {
            // 슬라이더의 이름에 따라 각 볼륨 설정 함수에 연결
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
        // 저장된 볼륨 값이 없으면 기본값 1.0f 사용
        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        effectVolume = PlayerPrefs.GetFloat("EffectVolume", 1.0f);

        // 불러온 볼륨 값으로 모든 사운드 볼륨을 즉시 업데이트
        UpdateAllVolumes();
    }

    // === 슬라이더 값 변경에 따른 볼륨 설정 함수 ===
    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings();
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

    // === 음악 재생 및 정지 함수 ===
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
        // 인게임 음악을 멈추고 추격 음악 재생
        if (ingameMusicSource != null && ingameMusicSource.isPlaying) ingameMusicSource.Stop();
        if (chaseMusicSource != null && !chaseMusicSource.isPlaying)
        {
            chaseMusicSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        // 추격 음악을 코루틴을 이용해 페이드아웃하며 정지
        StartCoroutine(StopChaseMusicWithFade(5f, 10f));
    }

    // === 효과음 재생 함수 ===
    public void PlayFlashlightSound()
    {
        if (flashlightSoundSource != null)
        {
            flashlightSoundSource.Play();
        }
    }

    public void PlayFootstepSound()
    {
        // 한 번만 재생되는 발소리 효과음
        if (footstepSoundSource != null && footstepSoundSource.clip != null)
        {
            footstepSoundSource.PlayOneShot(footstepSoundSource.clip);
        }
    }

    // === 오디오 일시정지 및 재개 ===
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

    // === 볼륨 일괄 업데이트 ===
    // 각 AudioSource의 최종 볼륨을 계산하여 적용
    private void UpdateAllVolumes()
    {
        if (mainMusicSource != null) mainMusicSource.volume = mainMusicOriginalVolume * mainVolume * musicVolume;
        if (ingameMusicSource != null) ingameMusicSource.volume = ingameMusicOriginalVolume * mainVolume * musicVolume;
        if (chaseMusicSource != null) chaseMusicSource.volume = chaseMusicOriginalVolume * mainVolume * musicVolume;
        if (flashlightSoundSource != null) flashlightSoundSource.volume = flashlightOriginalVolume * mainVolume * effectVolume;
        if (footstepSoundSource != null) footstepSoundSource.volume = footstepOriginalVolume * mainVolume * effectVolume;
    }

    // === 코루틴 ===
    // 추격 음악을 페이드아웃하고 인게임 음악을 다시 재생
    private IEnumerator StopChaseMusicWithFade(float fadeOutDuration, float delayDuration)
    {
        float startVolume = chaseMusicSource.volume;
        // 볼륨이 0에 도달할 때까지 서서히 줄임
        while (chaseMusicSource.volume > 0)
        {
            chaseMusicSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }
        chaseMusicSource.Stop();
        // 볼륨을 원래 상태로 되돌림
        chaseMusicSource.volume = chaseMusicOriginalVolume * mainVolume * musicVolume;
        // 일정 시간 대기 후 인게임 음악 재생
        yield return new WaitForSeconds(delayDuration);
        PlayIngameMusic();
    }
}