using System;
using TMPro;
using UnityEngine;
namespace Skiples.Picross{

    [Serializable]
    public struct Spancing
    {
        public float spancing;
        public float divide5;
        public float divide10;
    }
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;
        [SerializeField] GameObject tilePrefab;
        [SerializeField] GuideNumberController guideNumberController;
        [SerializeField] PicrossData data;
        [SerializeField] TMP_Text timerText;
        [SerializeField] Spancing spancing;
        [SerializeField] float missDelay;
        TileScript[,] tiles;
        Collider2D preHit;
        Camera cam;
        public int[] currentTile { get; private set; }
        float tileSize;
        float startTime;
        bool isDragging;
        bool canCheck = true;
        bool isPlaying = true;

        /*const int size = 25;    // 보드의 행 수
        readonly int[] answer = new int[size] {
            33360255,17120577,24481885,24499805,24436573,17154113,33379711,25600,
                27021871,30083546,315380,2200821,27143406,31020920,7175788,3779236,
                28056060,85786,33309012,17126686,24489463,24433391,24411818,17131606,33397431 };*/
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            cam = Camera.main;
        }
        void Start()
        {
            currentTile = new int[data.gridSize];
            startTime = Time.time;
            GenerateBoard();
            guideNumberController.GenerateGuideNumber(data.gridSize, tileSize, spancing);
        }
        private void Update()
        {
            if(timerText != null && isPlaying)
            {
                float elapsedTime = Time.time - startTime; 
                TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
                string formattedTime = timeSpan.ToString(@"mm\:ss");
                timerText.text = formattedTime;
            }

            if (!canCheck) return;

            if(!OnClick(true))
                OnClick(false);
        }

        bool OnClick(bool left)
        {
            int mouseNum = left ? 0 : 1;
            if (Input.GetMouseButtonDown(mouseNum))
            {
                isDragging = true;
                CheckTileAtMousePosition(left);
            }
            else if (isDragging && Input.GetMouseButton(mouseNum))
                CheckTileAtMousePosition(left);
            else if (Input.GetMouseButtonUp(mouseNum))
                isDragging = false;
            else 
                return false;

            return true;
        }
        void CheckTileAtMousePosition(bool left)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.Raycast(mousePos, Vector2.zero).collider;
            if (hit != null && hit != preHit && hit.TryGetComponent(out TileScript tile))
            {
                preHit = hit;
                tile.OnClickTile(left);
            }
        }

        // 게임 보드 생성
        void GenerateBoard()
        {
            tiles = new TileScript[data.gridSize, data.gridSize];
            tilePrefab.TryGetComponent(out Renderer render);
            tileSize = render.bounds.size.x + spancing.spancing;

            for (int col = 0; col < data.gridSize; col++)
            {
                float yDivide = (col / 10 * spancing.divide10) + (col / 5 * spancing.divide5);
                for (int row = 0; row < data.gridSize; row++)
                {
                    float xDivide = (row / 10 * spancing.divide10) + (row / 5 * spancing.divide5);
                    GameObject tile = Instantiate(tilePrefab,
                        new Vector3((row * tileSize)+ xDivide, (-col * tileSize)- yDivide, 0), Quaternion.identity);
                    tile.transform.SetParent(transform);
                    tile.name = $"Tile ({row}, {col})";

                    tile.TryGetComponent(out tiles[col, row]);
                    tiles[col, row].SetTile(row, col, this);
                    
                }
            }
        }
        public bool TileAnswerCheck(int row, int col) => data.Check(row, col);

        public bool TileCurrentCheck(int row, int col)
            => (currentTile[col] & (1 << (data.gridSize - 1 - row))) != 0;

        public void OnTileClicked(int row, int col, bool isActive)
        {
            if (isActive)
                currentTile[col] |= 1 << (data.gridSize - 1 - row);
            else
                currentTile[col] &= ~(1 << (data.gridSize - 1 - row));

            if (TileAnswerCheck(row, col) == isActive)
            {
                guideNumberController.CompleteCheck(row, col);
                CheckIfPuzzleComplete();
            }
            else if (isActive) CheckMiss();



        }


        void CheckMiss()
        {
            if (missDelay <= 0) return;
            canCheck = false;
            Invoke("EndMissDelay", missDelay);
        }
        void EndMissDelay() => canCheck = true;
        

        void CheckIfPuzzleComplete()
        {
            for (int col = 0; col < data.gridSize; col++)
                if(currentTile[col] != data.answer[col])
                    return;

            GameComplete();
        }

        void GameComplete()
        {
            isPlaying = false;
            Debug.Log("Puzzle Complete!");
        }




    }
}