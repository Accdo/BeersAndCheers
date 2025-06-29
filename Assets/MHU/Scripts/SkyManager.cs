using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkyManager : MonoBehaviour
{
    private static SkyManager instance; // �̱��� �ν��Ͻ�
    [SerializeField] private Light directionalLight; // ���⼺ ����Ʈ ������Ʈ

    private float morningTemperature = 20000f; // ��ħ ���µ� (�̺�)
    private float nightTemperature = 1500f; // �� ���µ� (�̺�)

    public static SkyManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ������Ʈ ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }

    private void Update()
    {
        if (directionalLight != null)
        {
            //// ���� �ð��� sleepTime�� ������� lerp ���� ���
            //float t = Mathf.Clamp01(TavernManager.instance.gameTime.Timer / TavernManager.instance.gameTime.sleepTime);
            //// ��ħ ���µ�(20000K)���� �� ���µ�(1500K)�� lerp
            //float currentTemperature = Mathf.Lerp(morningTemperature, nightTemperature, t);
            //directionalLight.colorTemperature = currentTemperature;

            if(TavernManager.instance.gameTime.Timer==0)
            {
                SetMorning();
            }
            else if (TavernManager.instance.gameTime.Timer >= TavernManager.instance.gameTime.nightTime)
            {
                SetNight();
            }
        }
    }

    // Ư�� �ð��� ���� �޼���
    public void SetMorning()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = morningTemperature;
        }
    }

    public void SetAfternoon()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = 5000f;
        }
    }

    public void SetNight()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = nightTemperature;
        }
    }
}
