using Skiples.Picross;
using UnityEditor;
using UnityEngine;

namespace Skiples.Picross
{
    [CustomEditor(typeof(PicrossData))]
    public class PicrossDataEditor : Editor
    {
        const int max = 30;
        const int min = 1;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PicrossData data = (PicrossData)target;
            if (data.inputTexture != null && GUILayout.Button("Generate Picross Data"))
            {
                data.GetPicrossData();
                EditorUtility.SetDirty(data); // 데이터 저장 상태 업데이트

            }

            data.gridSize = Mathf.Clamp(data.gridSize, min, max);
            data.gridSize = EditorGUILayout.IntField("Grid Size", data.gridSize);


            data.editMode = EditorGUILayout.Toggle("EditMode", data.editMode);

            int gridSize = data.gridSize * data.gridSize;

            // 그리드 초기화
            if (data.answer == null || data.answer.Length != data.gridSize)
            {
                Undo.RecordObject(data, "Grid Initialization");
                //data.answer = new int[data.gridSize];
                //data.grid = new bool[gridSize];
                data.CreateNewArr();
                EditorUtility.SetDirty(data); // 변경 사항 저장
                AssetDatabase.SaveAssets();
            }
            //if (GUILayout.Button("Preview Grid"))
            {
                for (int col = 0; col < data.gridSize; col++)
                    for (int row = 0; row < data.gridSize; row++)
                        data.grid[col * data.gridSize + row] = data.Check(row, col);
            }
            if (!data.editMode)
                GUI.enabled = data.editMode;

            // 체크박스 그리드 그리기
            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            for (int y = 0; y < data.gridSize; y++)
            {
                EditorGUILayout.BeginHorizontal(); // 줄 단위로 그리기
                for (int x = 0; x < data.gridSize; x++)
                {
                    //Debug.Log("answer = " + data.answer.Length + "\ngridSize = " + data.gridSize + "\nx, y = " + x + ", " + y);
                    int index = y * data.gridSize + x;
                    data.grid[index] = EditorGUILayout.Toggle(data.grid[index], GUILayout.Width(11), GUILayout.Height(11));

                    if ((x + 1) % 10 == 0) GUILayout.Space(2);
                    else if ((x + 1) % 5 == 0) GUILayout.Space(1);
                    data.AnswerSet(x, y, data.grid[index]);
                }
                EditorGUILayout.EndHorizontal();
                if ((y + 1) % 10 == 0) GUILayout.Space(3);
                else if ((y + 1) % 5 == 0) GUILayout.Space(2);
            }

            // 변경된 데이터 저장
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
        public bool Check(int row, int col)
        {
            PicrossData data = (PicrossData)target;

            //Debug.Log($"[{col}, {row}]answer len = {answer.Length} / gridSize = {gridSize}");
            return (data.answer == null || data.gridSize == 0) ? false : (data.answer[col] & (1 << (data.gridSize - 1 - row))) != 0;
        }
        public void CreateNewArr()
        {
            PicrossData data = (PicrossData)target;
            if (data.answer == null || data.answer.Length == 0)
                data.answer = new int[32];

            bool[] newGrid = new bool[data.gridSize * data.gridSize];
            int[] newArr = new int[data.gridSize];
            for (int col = 0; col < data.gridSize; col++)
            {
                int index = 0;
                for (int row = 0; row < data.gridSize; row++)
                {
                    if (Check(row, col))
                        index |= 1 << (data.gridSize - 1 - row);
                    newGrid[col * data.gridSize + row] = Check(row, col);
                }
                newArr[col] = index;
            }
            data.grid = newGrid;
            data.answer = newArr;
        }
    }
}