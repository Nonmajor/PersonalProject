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
    public GameObject pauseUI; // 추가: 일시정지 UI

    // === 게임 상태 변수 ===
    [Header("Game State")]
    public static bool isGameOver = false;
    public int keysCollected = 0;

    // 추가: 일시정지 상태를 확인하는 변수
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
        isPaused = false; // 추가
        Time.timeScale = 1f;
        victoryUI.SetActive(false);
        deadUI.SetActive(false);
        if (pauseUI != null) // 추가: 일시정지 UI도 비활성화
        {
            pauseUI.SetActive(false);
        }

        StartGame();
    }

    // 추가: ESC 키 입력을 확인하는 Update 함수
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

        // 추가: 마우스 커서를 보이게 하고 고정을 해제합니다.
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

        // 추가: 마우스 커서를 보이게 하고 고정을 해제합니다.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === 추가: 일시정지 관련 함수 ===
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);
            // 마우스 커서를 보이게 합니다.
            Cursor.visible = true;
            // 마우스 커서를 게임 뷰 밖으로 나가지 못하게 고정하지 않습니다.
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            pauseUI.SetActive(false);
            // 마우스 커서를 숨깁니다.
            Cursor.visible = false;
            // 마우스 커서를 게임 뷰 중앙에 고정합니다.
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

    // === 기존 함수 ===
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