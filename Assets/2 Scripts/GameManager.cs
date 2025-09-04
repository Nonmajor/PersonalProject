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

    // === ���� ���� ���� ===
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

        // ���� ���� �� ���콺 Ŀ���� ����� ���
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

        // �� �� ���� �߰��Ͽ� ���� ���� �� ���콺 Ŀ���� ���̰� �մϴ�.
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

        // �¸� �� ���콺 Ŀ�� ���̰� �ϰ� ��� ����
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnReturnButton()
    {
        Time.timeScale = 1f;

        // �� ��ȯ �� ���콺 Ŀ�� ���� �缳��
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;

        // ���� ����� �� ���콺 Ŀ�� ���� �缳�� (�ٽ� �����)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}