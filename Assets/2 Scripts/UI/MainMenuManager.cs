using UnityEngine;
using UnityEngine.SceneManagement;

// ���� �޴� ���� UI�� ��ɵ��� �����ϴ� ��ũ��Ʈ
public class MainMenuManager : MonoBehaviour
{
    
    // ����Ƽ �����Ϳ��� UI�� �Ҵ��� �� �ֵ��� public���� ����
    [Header("UI")]
    public GameObject settingsUI; // ���� �޴� �г�
    public GameObject videoUI;    // ���� ���� �޴� �г�
    public GameObject soundsUI;   // ���� ���� �޴� �г�

    
    private void Awake()
    {
        // ���� ���۵� �� ��� UI �г��� ��Ȱ��ȭ
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
    }

    // === ���� ���� ��ư ===
    public void StartGame()
    {
        // ��� UI�� ��Ȱ��ȭ�ϰ� "Game" ������ �̵�
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);

        SceneManager.LoadScene("Game");
    }

    // === UI ���� �Լ� ===
    // '����' ��ư Ŭ�� �� ȣ��
    public void OpenSettingsUI()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
    }

    // '�ݱ�' ��ư Ŭ�� �� ȣ��
    public void CloseSettingsUI()
    {
        // ���� �޴��� �ݰ� ��� ����޴�(����, ����)�� �Բ� ����
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

    // '���� ����' ��ư Ŭ�� �� ȣ��
    public void OpenVideoUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (videoUI != null) videoUI.SetActive(true);
    }

    // '���� ����' ��ư Ŭ�� �� ȣ��
    public void OpenSoundUI()
    {
        if (settingsUI != null) settingsUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(true);
    }

    // '�ڷΰ���' ��ư Ŭ�� �� ȣ�� (����/���� �޴�����)
    public void CloseSubMenu()
    {
        // ���� �����ִ� ����޴��� �ݰ� ���� ���� �޴��� ���ư�
        if (videoUI != null) videoUI.SetActive(false);
        if (soundsUI != null) soundsUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(true);
    }

    // === ���� ���� ��ư ===
    
    public void ExitGame()
    {
        // �����Ϳ��� ���� ���� ���
#if UNITY_EDITOR
        // ������ �÷��� ��带 ����
        UnityEditor.EditorApplication.isPlaying = false;
        // ����� ������ ���
#else
        // ���ø����̼��� ������ ����
        Application.Quit();
#endif
    }

    // === �ػ� ���� �Լ� ===
    public void SetResolution2560x1440()
    {
        // 2560x1440 �ػ󵵷� ���� (���� ��üȭ�� ��� ����)
        Screen.SetResolution(2560, 1440, Screen.fullScreen);
    }

    public void SetResolution1920x1080()
    {
        // 1920x1080 �ػ󵵷� ���� (���� ��üȭ�� ��� ����)
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    public void SetResolution1280x720()
    {
        // 1280x720 �ػ󵵷� ���� (���� ��üȭ�� ��� ����)
        Screen.SetResolution(1280, 720, Screen.fullScreen);
    }

    // === ��üȭ�� ��� ===
    public void ToggleFullscreen()
    {
        // ��üȭ�� ���¸� ���� (��üȭ�� <-> â���)
        Screen.fullScreen = !Screen.fullScreen;
    }
}