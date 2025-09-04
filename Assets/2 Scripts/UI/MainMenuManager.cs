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
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
    }

    public void StartGame()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);

        SceneManager.LoadScene("Game");
    }

    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    public void CloseSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        if (videoUI != null)
        {
            videoUI.SetActive(false);
        }
        if (soundsUI != null)
        {
            soundsUI.SetActive(false);
        }
    }

    public void OpenVideoUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(true);
    }

    public void OpenSoundUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(true);
    }

    public void CloseSubMenu()
    {
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(true);
    }

    // 수정된 ExitGame() 함수
    public void ExitGame()
    {
        // 에디터에서 실행 중일 경우
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // 빌드된 게임일 경우
#else
            Application.Quit();
#endif
    }

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

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}