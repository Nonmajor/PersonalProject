using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // === 열쇠 스폰 관련 변수 ===
    [Header("Key Spawn Settings")]
    public GameObject keyPrefab;
    public Transform[] keySpawnPoints;
    public int KEYS_TO_SPAWN = 5;

    // === UI 관련 변수 ===
    [Header("UI")]
    public GameObject victoryUI;
    public GameObject deadUI;
    public GameObject pauseUI;
    public GameObject inGameSettingsUI;
    public GameObject videoUI;
    public GameObject soundUI;

    // === 게임 상태 변수 ===
    [Header("Game State")]
    public static bool isGameOver = false;
    public int keysCollected = 0;

    private bool isPaused = false;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;

        // === 모든 UI를 게임 시작 시 비활성화 ===
        if (victoryUI != null) victoryUI.SetActive(false);
        if (deadUI != null) deadUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
        if (inGameSettingsUI != null) inGameSettingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundUI != null) soundUI.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Game")
        {
            SpawnKeys();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
    private void SpawnKeys()
    {
        GameObject[] existingKeys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in existingKeys)
        {
            Destroy(key);
        }

        Transform[] shuffledSpawnPoints = keySpawnPoints.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < KEYS_TO_SPAWN && i < shuffledSpawnPoints.Length; i++)
        {
            if (keyPrefab != null)
            {
                Instantiate(keyPrefab, shuffledSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    public void IncrementKeysCollected()
    {
        keysCollected++;
        Debug.Log("열쇠 획득! 현재 개수: " + keysCollected);

        if (keysCollected >= KEYS_TO_SPAWN)
        {
            WinGame();
        }
    }

    public void Die()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        deadUI.SetActive(true);
        Debug.Log("게임 오버! 몬스터에게 잡혔습니다.");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        victoryUI.SetActive(true);
        Debug.Log("축하합니다! 열쇠를 모두 모아 게임에서 승리했습니다!");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === UI 관련 함수 ===
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            pauseUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnResumeButton()
    {
        TogglePause();
    }

    public void OnSettingsButton()
    {
        pauseUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    public void OnBackButton()
    {
        inGameSettingsUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    public void OnVideoSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        videoUI.SetActive(true);
    }

    public void OnSoundSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        soundUI.SetActive(true);
    }

    public void OnSettingsSubMenuBackButton()
    {
        videoUI.SetActive(false);
        soundUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    public void OnReturnToMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // === 해상도 변경 기능 ===
    public void SetResolution2560x1440()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreen);
    }

    public void SetResolution1920x1080()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

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