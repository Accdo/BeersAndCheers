using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

public class DungeonManager : MonoBehaviour
{
    #region 상수
    private const string ENEMYDATA_LABEL = "EnemyData";
    #endregion
    private static DungeonManager instance;
    public static DungeonManager Instance => instance;
    private int currentFloor; // 현재 층수
    public int CurrentFloor => currentFloor;
    [SerializeField] private List<EnemySpawner> spawnZones;
    private Dictionary<string, EnemyData> enemyDataDic;

    public static void CreateManager()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("DungeonManager");
            instance = obj.AddComponent<DungeonManager>();
            DontDestroyOnLoad(obj);
            instance.Init();
        }
    }

    public static void RemoveManager()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    private async void Init()
    {
        await LoadAllEnemyData();

        var spawnable = enemyDataDic.Values
        .Where(enemy => enemy.SpawnZone == currentFloor)
        .ToList();

        await SpawnEnemies(spawnable); // 생성시에도 1층 몹 소환
    }

    private async Task LoadAllEnemyData()
    {
        // var handle = Addressables.LoadAssetsAsync<EnemyData>(ENEMYDATA_LABEL, null);
        // var list = await handle.Task;

        // if (handle.Status != AsyncOperationStatus.Succeeded)
        // {
        //     Debug.Log("EnemyData 로딩 실패");
        //     return;
        // }

        // enemyDataDic = list.ToDictionary(enemy => enemy.EnemyName);
    }

    public void ChangeFloor(int floor)
    {
        currentFloor = floor;

        Init();
    }

    private async Task SpawnEnemies(List<EnemyData> datas)
    {
        // foreach (var zone in spawnZones)
        // {
        //     foreach (var data in datas)
        //     {
        //         string key = $"Enemy_{data.EnemyName}";
        //         var handle = Addressables.LoadAssetAsync<GameObject>(key);
        //         var prefab = await handle.Task;

        //         if (handle.Status != AsyncOperationStatus.Succeeded) continue;

        //         var pos = zone.GetRandomSpawnPosition();
        //         var obj = Instantiate(prefab, pos, Quaternion.identity);
        //         obj.GetComponent<Enemy>().Init(data);
        //     }
        // }
    }


}
