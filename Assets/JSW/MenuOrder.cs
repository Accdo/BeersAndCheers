using UnityEngine;
using UnityEngine.UI; // UI 이미지 사용을 위해 필요

public class MenuOrder : MonoBehaviour
{
    public Image menuImage; // 주문 UI의 이미지 오브젝트 (에디터에서 할당)
    public Sprite[] menuSprites; // 메뉴별 스프라이트 배열 (에디터에서 할당)

    // 메뉴 목록 나중에 받아서 쓸 부분
    public enum MenuType
    {
        Beer,
        Chicken,
        Pizza,
    }

    private MenuType currentMenu;

    // 메뉴 주문(이미지 변경 하기)
    public void OrderMenu(MenuType menu)
    {
        currentMenu = menu;
        int idx = (int)menu;
        if (menuSprites != null && idx >= 0 && idx < menuSprites.Length)
        {
            menuImage.sprite = menuSprites[idx];
            menuImage.SetNativeSize(); // 필요시 이미지 크기 자동 조정
        }
        else
        {
            Debug.LogWarning("MenuOrder: 메뉴 스프라이트가 올바르게 할당되지 않았습니다.");
        }
    }

    // 현재 주문한 메뉴 반환
    public MenuType GetCurrentMenu()
    {
        return currentMenu;
    }
}