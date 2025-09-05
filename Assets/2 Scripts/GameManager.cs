using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

// 게임의 전반적인 상태와 핵심 기능(게임 시작/종료, UI, 열쇠 스폰 등)을 관리하는 스크립트
public class GameManager : MonoBehaviour
{
    // === 열쇠 스폰 관련 변수 ===
    [Header("Key Spawn Settings")]
    public GameObject keyPrefab;    // 스폰할 열쇠 프리팹
    public Transform[] keySpawnPoints; // 열쇠가 스폰될 위치 배열
    public int KEYS_TO_SPAWN = 5;      // 스폰할 열쇠의 총 개수

    // === UI 관련 변수 ===
    [Header("UI")]
    public GameObject victoryUI;        // 게임 승리 시 표시될 UI
    public GameObject deadUI;           // 게임 오버 시 표시될 UI
    public GameObject pauseUI;          // 일시정지 메뉴 UI
    public GameObject inGameSettingsUI; // 인게임 설정 메뉴 UI
    public GameObject videoUI;          // 비디오 설정 메뉴 UI
    public GameObject soundUI;          // 사운드 설정 메뉴 UI

    // === 게임 상태 변수 ===
    [Header("Game State")]
    public static bool isGameOver = false; // 게임 오버 상태 여부
    public int keysCollected = 0;          // 획득한 열쇠 개수

    private bool isPaused = false; // 일시정지 상태 여부
    public static GameManager Instance; // 이 스크립트의 싱글톤 인스턴스

    // === 초기화 ===
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 게임 시작 시 초기 상태 설정
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f; // 게임 시간을 정상 속도로 설정

        // === 모든 UI를 게임 시작 시 비활성화 ===
        if (victoryUI != null) victoryUI.SetActive(false);
        if (deadUI != null) deadUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
        if (inGameSettingsUI != null) inGameSettingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundUI != null) soundUI.SetActive(false);

        // 씬 이름이 "Game"일 경우에만 열쇠 스폰
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SpawnKeys();
        }
    }

    // === 매 프레임마다 호출 ===
    private void Update()
    {
        // Escape 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 현재 활성화된 UI에 따라 적절한 '뒤로가기' 또는 '일시정지' 기능 실행
            if (inGameSettingsUI != null && inGameSettingsUI.activeSelf)
            {
                OnBackButton();
            }
            else if (videoUI != null && videoUI.activeSelf)
            {
                OnSettingsSubMenuBackButton();
            }
            else if (soundUI != null && soundUI.activeSelf)
            {
                OnSettingsSubMenuBackButton();
            }
            else if (pauseUI != null && pauseUI.activeSelf)
            {
                TogglePause();
            }
            else
            {
                TogglePause();
            }
        }
    }

    // === 게임 로직 관련 함수 ===
    // 열쇠 스폰 함수
    private void SpawnKeys()
    {
        // 기존에 있던 열쇠들을 모두 파괴
        GameObject[] existingKeys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in existingKeys)
        {
            Destroy(key);
        }

        // 스폰 위치를 무작위로 섞음
        Transform[] shuffledSpawnPoints = keySpawnPoints.OrderBy(x => Random.value).ToArray();

        // 설정된 개수만큼 열쇠를 스폰
        for (int i = 0; i < KEYS_TO_SPAWN && i < shuffledSpawnPoints.Length; i++)
        {
            if (keyPrefab != null)
            {
                Instantiate(keyPrefab, shuffledSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    // 열쇠 획득 시 호출
    public void IncrementKeysCollected()
    {
        keysCollected++;
        Debug.Log("열쇠 획득! 현재 개수: " + keysCollected);

        // 모든 열쇠를 획득하면 게임 승리
        if (keysCollected >= KEYS_TO_SPAWN)
        {
            WinGame();
        }
    }

    // 플레이어가 죽었을 때 호출
    public void Die()
    {
        if (isGameOver) return; // 이미 게임 오버 상태면 중복 호출 방지

        isGameOver = true;
        Time.timeScale = 0f;          // 게임 시간 정지
        deadUI.SetActive(true);       // 게임 오버 UI 활성화
        AudioManager.Instance.PauseAllAudio();
        Debug.Log("게임 오버! 몬스터에게 잡혔습니다.");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 보이게 하고 고정 해제
    }

    // 게임 승리 시 호출
    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        victoryUI.SetActive(true);
        AudioManager.Instance.PauseAllAudio();
        Debug.Log("축하합니다! 열쇠를 모두 모아 게임에서 승리했습니다!");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === UI 관련 함수 ===
    // 일시정지 상태를 토글
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);

            // AudioManager에 오디오를 일시정지하도록 명령
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseAllAudio();
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            pauseUI.SetActive(false);

            // AudioManager에 오디오를 다시 재생하도록 명령
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAllAudio();
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // 재개 버튼 클릭 시 호출
    public void OnResumeButton()
    {
        TogglePause();
    }

    // 설정 버튼 클릭 시 호출
    public void OnSettingsButton()
    {
        pauseUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    // 인게임 설정 메뉴에서 뒤로가기 버튼 클릭 시 호출
    public void OnBackButton()
    {
        inGameSettingsUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    // 비디오 설정 버튼 클릭 시 호출
    public void OnVideoSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        videoUI.SetActive(true);
    }

    // 사운드 설정 버튼 클릭 시 호출
    public void OnSoundSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        soundUI.SetActive(true);
    }

    // 비디오/사운드 설정 메뉴에서 뒤로가기 버튼 클릭 시 호출
    public void OnSettingsSubMenuBackButton()
    {
        videoUI.SetActive(false);
        soundUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    // 메인 메뉴로 돌아가기 버튼 클릭 시 호출
    public void OnReturnToMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // 재시작 버튼 클릭 시 호출
    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // === 해상도 변경 기능 ===
    // 2560x1440 해상도로 변경
    public void SetResolution2560x1440()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreen);
    }

    // 1920x1080 해상도로 변경
    public void SetResolution1920x1080()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    // 1280x720 해상도로 변경
    public void SetResolution1280x720()
    {
        Screen.SetResolution(1280, 720, Screen.fullScreen);
    }
    // === 전체화면/창모드 토글 기능 ===
    public void ToggleFullscreen()

    {

        Screen.fullScreen = !Screen.fullScreen;

    }
}
    