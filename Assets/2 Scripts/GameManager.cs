using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

// ������ �������� ���¿� �ٽ� ���(���� ����/����, UI, ���� ���� ��)�� �����ϴ� ��ũ��Ʈ
public class GameManager : MonoBehaviour
{
    // === ���� ���� ���� ���� ===
    [Header("Key Spawn Settings")]
    public GameObject keyPrefab;    // ������ ���� ������
    public Transform[] keySpawnPoints; // ���谡 ������ ��ġ �迭
    public int KEYS_TO_SPAWN = 5;      // ������ ������ �� ����

    // === UI ���� ���� ===
    [Header("UI")]
    public GameObject victoryUI;        // ���� �¸� �� ǥ�õ� UI
    public GameObject deadUI;           // ���� ���� �� ǥ�õ� UI
    public GameObject pauseUI;          // �Ͻ����� �޴� UI
    public GameObject inGameSettingsUI; // �ΰ��� ���� �޴� UI
    public GameObject videoUI;          // ���� ���� �޴� UI
    public GameObject soundUI;          // ���� ���� �޴� UI

    // === ���� ���� ���� ===
    [Header("Game State")]
    public static bool isGameOver = false; // ���� ���� ���� ����
    public int keysCollected = 0;          // ȹ���� ���� ����

    private bool isPaused = false; // �Ͻ����� ���� ����
    public static GameManager Instance; // �� ��ũ��Ʈ�� �̱��� �ν��Ͻ�

    // === �ʱ�ȭ ===
    private void Awake()
    {
        // �̱��� ���� ����
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
        isPaused = false;
        Time.timeScale = 1f; // ���� �ð��� ���� �ӵ��� ����

        // === ��� UI�� ���� ���� �� ��Ȱ��ȭ ===
        if (victoryUI != null) victoryUI.SetActive(false);
        if (deadUI != null) deadUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
        if (inGameSettingsUI != null) inGameSettingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundUI != null) soundUI.SetActive(false);

        // �� �̸��� "Game"�� ��쿡�� ���� ����
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SpawnKeys();
        }
    }

    // === �� �����Ӹ��� ȣ�� ===
    private void Update()
    {
        // Escape Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� Ȱ��ȭ�� UI�� ���� ������ '�ڷΰ���' �Ǵ� '�Ͻ�����' ��� ����
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

    // === ���� ���� ���� �Լ� ===
    // ���� ���� �Լ�
    private void SpawnKeys()
    {
        // ������ �ִ� ������� ��� �ı�
        GameObject[] existingKeys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in existingKeys)
        {
            Destroy(key);
        }

        // ���� ��ġ�� �������� ����
        Transform[] shuffledSpawnPoints = keySpawnPoints.OrderBy(x => Random.value).ToArray();

        // ������ ������ŭ ���踦 ����
        for (int i = 0; i < KEYS_TO_SPAWN && i < shuffledSpawnPoints.Length; i++)
        {
            if (keyPrefab != null)
            {
                Instantiate(keyPrefab, shuffledSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    // ���� ȹ�� �� ȣ��
    public void IncrementKeysCollected()
    {
        keysCollected++;
        Debug.Log("���� ȹ��! ���� ����: " + keysCollected);

        // ��� ���踦 ȹ���ϸ� ���� �¸�
        if (keysCollected >= KEYS_TO_SPAWN)
        {
            WinGame();
        }
    }

    // �÷��̾ �׾��� �� ȣ��
    public void Die()
    {
        if (isGameOver) return; // �̹� ���� ���� ���¸� �ߺ� ȣ�� ����

        isGameOver = true;
        Time.timeScale = 0f;          // ���� �ð� ����
        deadUI.SetActive(true);       // ���� ���� UI Ȱ��ȭ
        AudioManager.Instance.PauseAllAudio();
        Debug.Log("���� ����! ���Ϳ��� �������ϴ�.");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� ���̰� �ϰ� ���� ����
    }

    // ���� �¸� �� ȣ��
    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        victoryUI.SetActive(true);
        AudioManager.Instance.PauseAllAudio();
        Debug.Log("�����մϴ�! ���踦 ��� ��� ���ӿ��� �¸��߽��ϴ�!");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // === UI ���� �Լ� ===
    // �Ͻ����� ���¸� ���
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);

            // AudioManager�� ������� �Ͻ������ϵ��� ���
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

            // AudioManager�� ������� �ٽ� ����ϵ��� ���
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAllAudio();
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // �簳 ��ư Ŭ�� �� ȣ��
    public void OnResumeButton()
    {
        TogglePause();
    }

    // ���� ��ư Ŭ�� �� ȣ��
    public void OnSettingsButton()
    {
        pauseUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    // �ΰ��� ���� �޴����� �ڷΰ��� ��ư Ŭ�� �� ȣ��
    public void OnBackButton()
    {
        inGameSettingsUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��
    public void OnVideoSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        videoUI.SetActive(true);
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��
    public void OnSoundSettingsButton()
    {
        inGameSettingsUI.SetActive(false);
        soundUI.SetActive(true);
    }

    // ����/���� ���� �޴����� �ڷΰ��� ��ư Ŭ�� �� ȣ��
    public void OnSettingsSubMenuBackButton()
    {
        videoUI.SetActive(false);
        soundUI.SetActive(false);
        inGameSettingsUI.SetActive(true);
    }

    // ���� �޴��� ���ư��� ��ư Ŭ�� �� ȣ��
    public void OnReturnToMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ����� ��ư Ŭ�� �� ȣ��
    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // === �ػ� ���� ��� ===
    // 2560x1440 �ػ󵵷� ����
    public void SetResolution2560x1440()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreen);
    }

    // 1920x1080 �ػ󵵷� ����
    public void SetResolution1920x1080()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    // 1280x720 �ػ󵵷� ����
    public void SetResolution1280x720()
    {
        Screen.SetResolution(1280, 720, Screen.fullScreen);
    }
    // === ��üȭ��/â��� ��� ��� ===
    public void ToggleFullscreen()

    {

        Screen.fullScreen = !Screen.fullScreen;

    }
}
    