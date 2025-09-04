using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsUI;

    private void Awake()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    public void StartGame()
    {
        // Settings UI를 비활성화합니다.
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }

        // "Game" 씬으로 이동
        SceneManager.LoadScene("Game");
    }

    // Settings 버튼 클릭 시 호출될 함수
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // Back 버튼 클릭 시 호출될 함수
    public void CloseSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    // Exit 버튼 클릭 시 호출될 함수
    public void ExitGame()
    {
        Application.Quit();
    }
}