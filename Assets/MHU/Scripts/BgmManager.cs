using UnityEngine;

public class BgmManager : MonoBehaviour
{
    private void Update()
    {
        // WoodenSign과 DungeonManager의 상태에 따라 BGM을 재생하기 위한 switch 문
        switch (true)
        {
            // 던전에 있는 경우
            case bool _ when DungeonManager.Instance.IsInDungeon:
                SoundManager.Instance.Stop("NormalBGM");
                SoundManager.Instance.Stop("WorkingBGM");

                SoundManager.Instance.PlayLoop("DungeonBGM");
                break;
            // WoodenSign이 열려있는 경우
            case bool _ when WoodenSign.instance.isOpen:
                SoundManager.Instance.Stop("NormalBGM");
                SoundManager.Instance.Stop("DungeonBGM");

                SoundManager.Instance.PlayLoop("WorkingBGM");
                break;

            // WoodenSign이 닫혀있는 경우
            case bool _ when !WoodenSign.instance.isOpen:
                SoundManager.Instance.Stop("WorkingBGM");
                SoundManager.Instance.Stop("DungeonBGM");

                SoundManager.Instance.PlayLoop("NormalBGM");
                break;

            
        }
    }
}