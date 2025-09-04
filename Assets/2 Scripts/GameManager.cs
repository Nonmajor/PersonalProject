using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // === ���� ���� ���� ���� ===
    [Header("Key Spawn Settings")]
    public GameObject keyPrefab;
    public Transform[] keySpawnPoints;
    public int KEYS_TO_SPAWN = 5;

    // === UI ���� ���� ===
    [Header("UI")]
    public GameObject victoryUI;
    public GameObject deadUI;
    public GameObject pauseUI; // �߰�: �Ͻ����� UI

    // === ���� ���� ���� ===
    [Header("Game State")]
    public static bool isGameOver = false;
    public int keysCollected = 0;

    // �߰�: �Ͻ����� ���¸� Ȯ���ϴ� ����
    private bool isPaused = false;

    // �� ��ũ��Ʈ�� �̱��� �ν��Ͻ�
    public static GameManager Instance;

    private void Awake()
    {
        // GameManager�� �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // ���� ���� �� �ʱ� ���� ����
        isGameOver = false;
        isPaused = false; // �߰�
        Time.timeScale = 1f;
        victoryUI.SetActive(false);
        deadUI.SetActive(false);
        if (pauseUI != null) // �߰�: �Ͻ����� UI�� ��Ȱ��ȭ
        {
            pauseUI.SetActive(false);
        }

        StartGame();
    }

    // �߰�: ESC Ű �Է��� Ȯ���ϴ� Update �Լ�
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
        Debug.Log($"���� ȹ��! ���� ����: {keysCollected} / {KEYS_TO_SPAWN}");

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
        Debug.Log("���� ����! AI���� �������ϴ�.");

        // �߰�: ���콺 Ŀ���� ���̰� �ϰ� ������ �����մϴ�.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        victoryUI.SetActive(true);
        Debug.Log("�����մϴ�! ���踦 ��� ��� ���ӿ��� �¸��߽��ϴ�!");

        // �߰�: ���콺 Ŀ���� ���̰� �ϰ� ������ �����մϴ�.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === �߰�: �Ͻ����� ���� �Լ� ===
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);
            // ���콺 Ŀ���� ���̰� �մϴ�.
            Cursor.visible = true;
            // ���콺 Ŀ���� ���� �� ������ ������ ���ϰ� �������� �ʽ��ϴ�.
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            pauseUI.SetActive(false);
            // ���콺 Ŀ���� ����ϴ�.
            Cursor.visible = false;
            // ���콺 Ŀ���� ���� �� �߾ӿ� �����մϴ�.
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnResumeButton()
    {
        TogglePause();
    }

    public void OnSettingsButton()
    {

        Debug.Log("���� UI�� �̵�");
    }

    // === ���� �Լ� ===
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