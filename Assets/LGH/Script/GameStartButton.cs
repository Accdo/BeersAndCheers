using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    void Update()
    {
        // 엔터 키 또는 마우스 클릭으로 게임 시작
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            // 씬 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
        }
    }
}
