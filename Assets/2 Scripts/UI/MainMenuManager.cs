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
        // Settings UI�� ��Ȱ��ȭ�մϴ�.
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }

        // "Game" ������ �̵�
        SceneManager.LoadScene("Game");
    }

    // Settings ��ư Ŭ�� �� ȣ��� �Լ�
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // Back ��ư Ŭ�� �� ȣ��� �Լ�
    public void CloseSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }

    // Exit ��ư Ŭ�� �� ȣ��� �Լ�
    public void ExitGame()
    {
        Application.Quit();
    }
}