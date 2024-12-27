using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Skiples.Picross
{
    public enum TileState
    {
        DEFAULT,
        FILL,
        X
    }
    public class TileScript : MonoBehaviour
    {
        [SerializeField] TileData data;
        GameManager manager;
        SpriteRenderer renderer;
        int row, col;
        TileState state = TileState.DEFAULT;

        void Start()
        {
            if (!TryGetComponent(out renderer))
                Debug.LogError("No SpriteRenderer " + name);
            RendererSet();
        }
        bool IsActive() => state == TileState.FILL;

        void RendererSet()
        {
            renderer.color = data.GetColor(state);
            renderer.sprite = data.GetSprite(state);
        }

        public void SetTile(int r, int c,GameManager target) 
        {
            row = r;
            col = c;
            manager = target;
        }

        public void MissEffect()
        {

        }


        public void OnClickTile(bool leftClick)
        {
            TileState preState = state;
            if (leftClick)
            {
                if (state == TileState.DEFAULT)
                    state = TileState.FILL;
                else if (state == TileState.FILL)
                    state = TileState.DEFAULT;
            }
            else
            {
                if (state == TileState.DEFAULT)
                    state = TileState.X;
                else if (state == TileState.X)
                    state = TileState.DEFAULT;
            }

            if (preState == state) return;

            RendererSet();
            manager.OnTileClicked(row, col, IsActive());
        }
    }
}
