using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Skiples.Picross
{
    public class GuideNumberController : MonoBehaviour
    {
        [SerializeField] GameObject numberPrefab;
        [Header("Text Color")]
        [SerializeField] Color defaultColor = Color.black;
        [SerializeField] Color completeColor = Color.gray;
        List<List<TMP_Text>> rowGuideTextBoxs, colGuideTextBoxs;
        int arrSize;
        float tileSize;
        Spancing spancing;
        public void GenerateGuideNumber(int _arrSize, float _tileSize, Spancing _spancing)
        {
            arrSize = _arrSize;
            tileSize = _tileSize + spancing.spancing;
            spancing  = _spancing;
            SpawnColGuides();
            SpawnRowGuides();
        }

        public void CompleteCheck(int row,int col)
        {
            CheckRow(col);
            CheckCol(row);
        }

        void CheckRow(int col)
        {
            int count = 0;
            bool check = true;
            int len = 0;
            GameManager gm = GameManager.Instance;
            for (int j = 0; j < arrSize; j++)
            {
                if (gm.TileAnswerCheck(j, col))
                {
                    count++;
                    if(!gm.TileCurrentCheck(j,col))
                        check = false;
                }
                else
                {
                    //Debug.Log($"row IsCheck {j}/count{count} = {check}");
                    if (count > 0)
                    {
                        //Debug.Log($"row IsCheck = {check}");
                        rowGuideTextBoxs[col][len].color = check?completeColor:defaultColor;
                        len++;
                        count = 0;
                    }
                    check = true;
                }
            }

            if (count > 0) rowGuideTextBoxs[col][len].color = check ? completeColor : defaultColor; // 마지막 블록 추가
        }
        void CheckCol(int row)
        {
            int count = 0;
            int len = 0;
            bool check = true;
            GameManager gm = GameManager.Instance;
            for (int j = 0; j < arrSize; j++)
            {
                if (gm.TileAnswerCheck(row,j))
                {
                    count++;
                    if (!gm.TileCurrentCheck(row, j)) 
                        check = false;

                }
                else
                {
                    //Debug.Log($"col IsCheck {j}/count{count} = {check}");
                    if (count > 0)
                    {
                        colGuideTextBoxs[row][len].color = check ? completeColor : defaultColor;
                        len++;
                        count = 0;
                    }
                    check = true;
                }
            }if (count > 0) colGuideTextBoxs[row][len].color = check ? completeColor : defaultColor; // 마지막 블록 추가
        }

        void SpawnRowGuides()
        {
            var colArr = GenerateColGuides();
            rowGuideTextBoxs = new List<List<TMP_Text>>(arrSize);
            for (int row = 0; row < arrSize; row++)
            {
                float offset = (row / 10 * spancing.divide10) + (row / 5 * spancing.divide5);
                int count = colArr[row].Count;
                List<TMP_Text> textArr = new List<TMP_Text>(count);
                for (int j = 0; j < count; j++)
                {
                    GameObject guide = Instantiate(numberPrefab,
                        new Vector3((count - j) * -tileSize, (row * -tileSize) - offset, 0), Quaternion.identity);
                    guide.transform.SetParent(transform);
                    guide.name = $"Col Guide ({row}, {j})";

                    guide.TryGetComponent(out TMP_Text textBox);
                    Span<char> buffer = stackalloc char[2];
                    colArr[row][j].TryFormat(buffer, out int charWritten);
                    textBox.text = new string(buffer.Slice(0, charWritten));
                    textArr.Add(textBox);
                }

                rowGuideTextBoxs.Add(textArr);
            }
        }
        void SpawnColGuides()
        {
            var rowArr = GenerateRowGuides();
            colGuideTextBoxs = new List<List<TMP_Text>>(arrSize);
            for (int row = 0; row < arrSize; row++)
            {
                float offset = (row / 10 * spancing.divide10) + (row / 5 * spancing.divide5);
                int count = rowArr[row].Count;
                List<TMP_Text> textArr = new List<TMP_Text>(count);
                for (int j = 0; j < count; j++)
                {
                    GameObject guide = Instantiate(numberPrefab,
                        new Vector3((row * tileSize)+offset, (count-j) * tileSize, 0), Quaternion.identity);
                    guide.transform.SetParent(transform);
                    guide.name = $"Row Guide ({row}, {j})";

                    guide.TryGetComponent(out TMP_Text textBox);
                    Span<char> buffer = stackalloc char[2];
                    rowArr[row][j].TryFormat(buffer, out int charWritten);
                    textBox.text = new string(buffer.Slice(0, charWritten));
                    textArr.Add(textBox);
                }

                colGuideTextBoxs.Add(textArr);
            }
        }



        // 가로 가이드 생성 함수
        List<List<int>> GenerateRowGuides()
        {
            List<List<int>> rowGuides = new List<List<int>>(arrSize);
            for (int i = 0; i < arrSize; i++)
            {
                List<int> guide = new List<int>();
                int count = 0;

                for (int j = 0; j < arrSize; j++)
                {
                    if (GameManager.Instance.TileAnswerCheck(i, j)) count++;
                    else
                        if (count > 0)
                    {
                        guide.Add(count);
                        count = 0;
                    }

                }

                if (count > 0) guide.Add(count); // 마지막 블록 추가
                if (guide.Count == 0) guide.Add(0); // 빈 줄 처리
                rowGuides.Add(guide);
            }

            return rowGuides;
        }

        // 세로 가이드 생성 함수
        List<List<int>> GenerateColGuides()
        {
            List<List<int>> colGuides = new List<List<int>>(arrSize);

            for (int j = 0; j < arrSize; j++)
            {
                List<int> guide = new List<int>();
                int count = 0;

                for (int i = 0; i < arrSize; i++)
                {
                    if (GameManager.Instance.TileAnswerCheck(i, j))
                    {
                        count++;
                    }
                    else
                    {
                        if (count > 0)
                        {
                            guide.Add(count);
                            count = 0;
                        }
                    }
                }

                if (count > 0) guide.Add(count); // 마지막 블록 추가
                if (guide.Count == 0) guide.Add(0); // 빈 줄 처리
                colGuides.Add(guide);
            }

            return colGuides;
        }
    }
}
