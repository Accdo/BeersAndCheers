using UnityEngine;

public enum PortalDestination { Dungeon, Hometown, Farm }
public static class PlaceXYZ
{
    public static readonly Vector3 DUNGEON_XYZ = new Vector3(0, 0, 0);
    public static readonly Vector3 HOMETOWN_XYZ = new Vector3(0, 0, 0);
    public static readonly Vector3 FARM_XYZ = new Vector3(0, 0, 0);
}
public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private PortalDestination destination;
    public string GetCursorType() => "Portal";
    public string GetInteractionID() => "Portal";

    public InteractionType GetInteractionType() => InteractionType.Instant;

    public void Interact()
    {
        // 로딩 및 플레이어 워프
        switch (destination)
        {
            case PortalDestination.Dungeon:
                if (DungeonManager.Instance.IsInDungeon)
                {
                    DungeonManager.Instance.ChangeFloor(DungeonManager.Instance.CurrentFloor + 1);
                    // 던전 시작지점으로 워프....?
                    Destroy(this);
                }
                else
                {
                    DungeonManager.Instance.ChangeFloor(1);
                    DungeonManager.Instance.IsInDungeon = true;
                    // 던전 지점으로 워프
                }
                break;
            case PortalDestination.Hometown:
                if (DungeonManager.Instance.IsInDungeon)
                {
                    DungeonManager.Instance.IsInDungeon = false;
                }
                // 마을로 워프
                break;
            case PortalDestination.Farm:
                // 농장으로 워프
                break;
        }
    }

}
