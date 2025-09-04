using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start 버튼 클릭 시 호출될 함수
    public void StartGame()
    {
        // "Game" 씬으로 이동
        SceneManager.LoadScene("Game");
    }

    // Exit 버튼 클릭 시 호출될 함수
    public void ExitGame()
    {
        // 애플리케이션 종료
        Application.Quit();

        
    }
}