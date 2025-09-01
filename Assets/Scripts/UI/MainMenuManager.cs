using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start ��ư Ŭ�� �� ȣ��� �Լ�
    public void StartGame()
    {
        // "Game" ������ �̵�
        SceneManager.LoadScene("Game");
    }

    // Exit ��ư Ŭ�� �� ȣ��� �Լ�
    public void ExitGame()
    {
        // ���ø����̼� ����
        Application.Quit();

        
    }
}