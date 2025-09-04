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
        // 게임 시작 시 모든 설정 UI를 초기에 비활성화
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
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

    // Settings 버튼 클릭 시 호출될 함수 (메인 메뉴에서 호출)
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // Settings UI에서 Video 버튼 클릭 시 호출
    public void OpenVideoUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(true);
    }

    // Settings UI에서 Sound 버튼 클릭 시 호출
    public void OpenSoundsUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(true);
    }

    // 서브 UI(Video/Sound)에서 Back 버튼 클릭 시 호출
    public void BackToSettingsUI()
    {
        // 현재 활성화된 서브 UI를 비활성화하고 Settings UI를 활성화
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);

        if (settingsUI != null) settingsUI.SetActive(true);
    }

    // Settings UI에서 Back 버튼 클릭 시 호출 (메인 메뉴로 돌아감)
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