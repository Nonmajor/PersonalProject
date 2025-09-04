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

    // === 게임 상태 변수 ===
    [Header("Game State")]
    public static bool isGameOver = false;
    public int keysCollected = 0;

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
        Time.timeScale = 1f;
        victoryUI.SetActive(false);
        deadUI.SetActive(false);

        // 게임 시작 시 마우스 커서를 숨기고 잠금
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartGame();
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

        // 이 두 줄을 추가하여 게임 오버 시 마우스 커서를 보이게 합니다.
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

        // 승리 시 마우스 커서 보이게 하고 잠금 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnReturnButton()
    {
        Time.timeScale = 1f;

        // 씬 전환 시 마우스 커서 상태 재설정
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;

        // 게임 재시작 시 마우스 커서 상태 재설정 (다시 숨기기)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}