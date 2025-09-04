using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

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

    // === 게임 상태 변수 ===
    [Header("Game State")]
    public static bool isGameOver = false;
    public int keysCollected = 0;

    private bool isPaused = false;

    // 이 스크립트의 싱글톤 인스턴스
    public static GameManager Instance;

    private void Awake()
    {
        // GameManager의 싱글톤 인스턴스 설정
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
        Time.timeScale = 1f;
        victoryUI.SetActive(false);
        deadUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);

        // GameManager는 메인 메뉴에서도 사용되므로 게임 씬에서만 열쇠를 스폰
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SpawnKeys();
        }
    }

    // ESC 키 입력을 확인하는 Update 함수
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameOver) return;
            TogglePause();
        }
    }

    public void StartGame()
    {
        SpawnKeys();
    }

    private void SpawnKeys()
    {
        List<Transform> spawnPointsList = keySpawnPoints.ToList();
        List<Transform> selectedPoints = new List<Transform>();

        for (int i = 0; i < KEYS_TO_SPAWN; i++)
        {
            if (spawnPointsList.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPointsList.Count);
                selectedPoints.Add(spawnPointsList[randomIndex]);
                spawnPointsList.RemoveAt(randomIndex);
            }
        }

        foreach (Transform point in selectedPoints)
        {
            Instantiate(keyPrefab, point.position, Quaternion.identity);
        }
    }

    public void IncrementKeysCollected()
    {
        if (isGameOver) return;

        keysCollected++;
        Debug.Log($"열쇠 획득! 현재 개수: {keysCollected} / {KEYS_TO_SPAWN}");

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
        Debug.Log("게임 오버! AI에게 잡혔습니다.");

        // 마우스 커서를 보이게 하고 고정을 해제합니다.
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

        // 마우스 커서를 보이게 하고 고정을 해제합니다.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === 일시정지 관련 함수 ===
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);

            
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

            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAllAudio();
            }

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
        Debug.Log("설정 UI로 이동");
    }

    public void OnReturnButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}