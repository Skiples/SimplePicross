using UnityEngine;

namespace Skiples.Picross
{
    [CreateAssetMenu(fileName = "PicrossData", menuName = "Skiples/Picross/PicrossData")]
    public class PicrossData : ScriptableObject
    {
        [HideInInspector] public int gridSize = 25;
        [HideInInspector] public int[] answer = new int[25];
        public bool Check(int row, int col) =>
            (answer == null || answer.Length <= col) ? false : (answer[col] & (1 << (gridSize - 1 - row))) != 0;
#if UNITY_EDITOR
        [HideInInspector] public bool editMode;
        [HideInInspector] public bool[] grid = new bool[25];
        public Texture2D inputTexture; // Unity Editor에서 이미지 참조
        public void GetPicrossData()
        {
            if (inputTexture == null) return;
            // 이미지 크기 기준으로 배열 생성
            bool[] picrossData = new bool[gridSize * gridSize];
            float cellWidth = (float)inputTexture.width / gridSize;
            float cellHeight = (float)inputTexture.height / gridSize;

            // 픽셀 읽기
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int pixelX = Mathf.FloorToInt((x + 0.5f) * cellWidth);
                    int pixelY = Mathf.FloorToInt((y + 0.5f) * cellHeight);
                    // Get pixel color at (x, y)
                    Color pixelColor = inputTexture.GetPixel(pixelX, pixelY);

                    // 흑백 기준: 밝기(Luminance) 사용
                    float brightness = pixelColor.grayscale; // 0.0 (검정) ~ 1.0 (흰색)
                    bool check = brightness < 0.5f;
                    picrossData[y * gridSize + x] = check; // true = 검정, false = 흰색
                    AnswerSet(x, y, check);
                }
            }

            grid = picrossData;
        }
        public void CreateNewArr()
        {/*
            answer = new int[25] {
            33360255,17120577,24481885,24499805,24436573,17154113,33379711,25600,
                27021871,30083546,315380,2200821,27143406,31020920,7175788,3779236,
                28056060,85786,33309012,17126686,24489463,24433391,24411818,17131606,33397431 };*/
            bool[] newGrid = new bool[gridSize * gridSize];
            int[] newArr = new int[gridSize];
            int shift = gridSize - answer.Length;
            for (int col = 0; col < gridSize; col++)
            {
                if(answer.Length > col)
                    answer[col] = shift > 0 ? answer[col] << shift : answer[col] >> -shift;
                
                int index = 0;
                for (int row = 0; row < gridSize; row++)
                {
                    if (Check(row, col))
                        index |= 1 << (gridSize - 1 - row);
                    newGrid[col * gridSize + row] = Check(row, col);
                }
                newArr[col] = index;
            }
            grid = newGrid;
            answer = newArr;
        }
        public void AnswerSet(int row, int col, bool check)
        {
            if (check)
                answer[col] |= 1 << (gridSize - 1 - row);
            else
                answer[col] &= ~(1 << (gridSize - 1 - row));
        }
#endif
    }

}
