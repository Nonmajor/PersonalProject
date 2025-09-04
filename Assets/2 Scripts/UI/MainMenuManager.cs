using UnityEngine;
using UnityEngine.SceneManagement;

// 메인 메뉴 씬의 UI와 기능들을 관리하는 스크립트
public class MainMenuManager : MonoBehaviour
{
    
    // 유니티 에디터에서 UI를 할당할 수 있도록 public으로 선언
    [Header("UI")]
    public GameObject settingsUI; // 설정 메뉴 패널
    public GameObject videoUI;    // 비디오 설정 메뉴 패널
    public GameObject soundsUI;   // 사운드 설정 메뉴 패널

    
    private void Awake()
    {
        // 씬이 시작될 때 모든 UI 패널을 비활성화
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
    }

    // === 게임 시작 버튼 ===
    public void StartGame()
    {
        // 모든 UI를 비활성화하고 "Game" 씬으로 이동
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);

        SceneManager.LoadScene("Game");
    }

    // === UI 관리 함수 ===
    // '설정' 버튼 클릭 시 호출
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // '닫기' 버튼 클릭 시 호출
    public void CloseSettingsUI()
    {
        // 설정 메뉴를 닫고 모든 서브메뉴(비디오, 사운드)도 함께 닫음
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

    // '비디오 설정' 버튼 클릭 시 호출
    public void OpenVideoUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(true);
    }

    // '사운드 설정' 버튼 클릭 시 호출
    public void OpenSoundUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(true);
    }

    // '뒤로가기' 버튼 클릭 시 호출 (비디오/사운드 메뉴에서)
    public void CloseSubMenu()
    {
        // 현재 열려있는 서브메뉴를 닫고 상위 설정 메뉴로 돌아감
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(true);
    }

    // === 게임 종료 버튼 ===
    
    public void ExitGame()
    {
        // 에디터에서 실행 중일 경우
#if UNITY_EDITOR
        // 에디터 플레이 모드를 중지
        UnityEditor.EditorApplication.isPlaying = false;
        // 빌드된 게임일 경우
#else
        // 어플리케이션을 완전히 종료
        Application.Quit();
#endif
    }

    // === 해상도 변경 함수 ===
    public void SetResolution2560x1440()
    {
        // 2560x1440 해상도로 변경 (현재 전체화면 모드 유지)
        Screen.SetResolution(2560, 1440, Screen.fullScreen);
    }

    public void SetResolution1920x1080()
    {
        // 1920x1080 해상도로 변경 (현재 전체화면 모드 유지)
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    public void SetResolution1280x720()
    {
        // 1280x720 해상도로 변경 (현재 전체화면 모드 유지)
        Screen.SetResolution(1280, 720, Screen.fullScreen);
    }

    // === 전체화면 토글 ===
    public void ToggleFullscreen()
    {
        // 전체화면 상태를 반전 (전체화면 <-> 창모드)
        Screen.fullScreen = !Screen.fullScreen;
    }
}