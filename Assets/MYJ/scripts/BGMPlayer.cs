using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.Play("NormalBGM");
    }
    
}
