using UnityEngine;

public class BgmManager : MonoBehaviour
{
    private void Update()
    {
        // WoodenSign�� DungeonManager�� ���¿� ���� BGM�� ����ϱ� ���� switch ��
        switch (true)
        {
            // ������ �ִ� ���
            case bool _ when DungeonManager.Instance.IsInDungeon:
                SoundManager.Instance.Stop("NormalBGM");
                SoundManager.Instance.Stop("WorkingBGM");

                SoundManager.Instance.PlayLoop("DungeonBGM");
                break;
            // WoodenSign�� �����ִ� ���
            case bool _ when WoodenSign.instance.isOpen:
                SoundManager.Instance.Stop("NormalBGM");
                SoundManager.Instance.Stop("DungeonBGM");

                SoundManager.Instance.PlayLoop("WorkingBGM");
                break;

            // WoodenSign�� �����ִ� ���
            case bool _ when !WoodenSign.instance.isOpen:
                SoundManager.Instance.Stop("WorkingBGM");
                SoundManager.Instance.Stop("DungeonBGM");

                SoundManager.Instance.PlayLoop("NormalBGM");
                break;

            
        }
    }
}