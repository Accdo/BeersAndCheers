using UnityEngine;

public enum PortalDestination { Dungeon, Hometown, Farm }
public static class PlaceXYZ
{
    public static readonly Vector3 DUNGEON_PLAYER_XYZ = new Vector3(713f, 9f, 11f);
    public static readonly Vector3 DUNGEON_TO_DUNGEON_XYZ = new Vector3(713f, 7f, 11f);
    public static readonly Vector3 HOMETOWN_XYZ = new Vector3(-70.0f, 7f, 18.1f);
    public static readonly Vector3 DUNGEON_TO_HOME_XYZ = new Vector3(718f, 7f, 3f);
    public static readonly Vector3 FARM_XYZ = new Vector3(0, 0, 0);
}
public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private PortalDestination destination;
    public string GetCursorType() => "Portal";
    public string GetInteractionID() => "Portal";

    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public void Interact()
    {
        // 로딩 및 플레이어 워프
        switch (destination)
        {
            case PortalDestination.Dungeon:
                if (DungeonManager.Instance.IsInDungeon)
                {
                    DungeonManager.Instance.ChangeFloor(DungeonManager.Instance.CurrentFloor + 1);
                    GH_GameManager.instance.player.transform.position = PlaceXYZ.DUNGEON_PLAYER_XYZ;
                    Destroy(gameObject);
                }
                else
                {
                    DungeonManager.Instance.IsInDungeon = true;
                    DungeonManager.Instance.ChangeFloor(1);
                    GH_GameManager.instance.player.transform.position = PlaceXYZ.DUNGEON_PLAYER_XYZ;
                }
                break;
            case PortalDestination.Hometown:
                if (DungeonManager.Instance.IsInDungeon)
                {
                    DungeonManager.Instance.IsInDungeon = false;
                }
                GH_GameManager.instance.player.transform.position = PlaceXYZ.HOMETOWN_XYZ;
                Destroy(gameObject);
                break;
            case PortalDestination.Farm:
                // 농장으로 워프
                break;
        }
    }

}
