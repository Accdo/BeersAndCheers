using UnityEngine;
using UnityEditor;
using System.IO;

// EditorWindow를 상속받는 클래스를 정의합니다.
public class QuestDataCreator : EditorWindow
{
    // 스크롤 위치를 저장할 변수
    private Vector2 scrollPosition;

    // 편집 중인 QuestData를 임시로 담을 ScriptableObject
    private QuestData tempQuestData;
    private Editor tempQuestEditor;

    [MenuItem("Window/Quest Creator/Quest Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<QuestDataCreator>("Quest Creator");
        window.minSize = new Vector2(500, 800); // 윈도우 최소 크기 설정
    }

    // 창이 활성화될 때 임시 데이터 생성
    private void OnEnable()
    {
        tempQuestData = CreateInstance<QuestData>();
        tempQuestData.questID = "QUEST_" + System.DateTime.Now.Ticks; // 기본 ID 생성
        tempQuestData.questTitle = "새로운 퀘스트";
        tempQuestEditor = Editor.CreateEditor(tempQuestData);
    }

    // 창이 비활성화될 때 임시 데이터 파괴
    private void OnDisable()
    {
        DestroyImmediate(tempQuestEditor);
        DestroyImmediate(tempQuestData);
    }

    // 창의 GUI를 그리는 메서드입니다.
    private void OnGUI()
    {
        GUILayout.Label("퀘스트 데이터 생성기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 스크롤 뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 기본 인스펙터 UI를 그대로 사용
        if (tempQuestEditor != null)
        {
            tempQuestEditor.OnInspectorGUI();
        }

        EditorGUILayout.EndScrollView();
        // 스크롤 뷰 종료

        GUILayout.FlexibleSpace(); // 버튼을 맨 아래에 위치시키기 위한 공간

        // 생성 버튼
        if (GUILayout.Button("Create Quest Data Asset", GUILayout.Height(30)))
        {
            CreateQuestDataAsset();
        }
        GUILayout.Space(5);
    }

    private void CreateQuestDataAsset()
    {
        if (string.IsNullOrEmpty(tempQuestData.questID))
        {
            EditorUtility.DisplayDialog("Error", "Quest ID는 비어있을 수 없습니다.", "OK");
            return;
        }
        if (string.IsNullOrEmpty(tempQuestData.questTitle))
        {
            EditorUtility.DisplayDialog("Error", "Quest Title은 비어있을 수 없습니다.", "OK");
            return;
        }

        // 1. 저장 경로 설정
        string path = "Assets/Resources/Customer/QuestData";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + tempQuestData.questID + ".asset");

        // 2. 새로운 인스턴스를 만들어 데이터를 복사 (에셋 생성용)
        QuestData newQuest = Instantiate(tempQuestData);
        newQuest.name = tempQuestData.questID;

        // 3. 에셋 파일 생성
        AssetDatabase.CreateAsset(newQuest, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // 4. 생성된 에셋 선택
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newQuest;

        EditorUtility.DisplayDialog("Success", "Quest Data가 성공적으로 생성되었습니다.\n" + assetPathAndName, "OK");
        this.Close();
    }
}