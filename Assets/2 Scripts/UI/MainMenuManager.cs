using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsUI;
    public GameObject videoUI;
    public GameObject soundsUI;

    private void Awake()
    {
        // ���� ���� �� ��� ���� UI�� �ʱ⿡ ��Ȱ��ȭ
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
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

    // Settings ��ư Ŭ�� �� ȣ��� �Լ� (���� �޴����� ȣ��)
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // Settings UI���� Video ��ư Ŭ�� �� ȣ��
    public void OpenVideoUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(true);
    }

    // Settings UI���� Sound ��ư Ŭ�� �� ȣ��
    public void OpenSoundsUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(true);
    }

    // ���� UI(Video/Sound)���� Back ��ư Ŭ�� �� ȣ��
    public void BackToSettingsUI()
    {
        // ���� Ȱ��ȭ�� ���� UI�� ��Ȱ��ȭ�ϰ� Settings UI�� Ȱ��ȭ
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);

        if (settingsUI != null) settingsUI.SetActive(true);
    }

    // Settings UI���� Back ��ư Ŭ�� �� ȣ�� (���� �޴��� ���ư�)
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