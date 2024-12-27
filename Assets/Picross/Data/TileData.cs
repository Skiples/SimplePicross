using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skiples.Picross
{
    [CreateAssetMenu(fileName = "TileData", menuName = "Skiples/Picross/TileData")]
    public class TileData : ScriptableObject
    {
       [SerializeField] Sprite defaultSprite;
       [SerializeField] Sprite fillSprite;
       [SerializeField] Sprite xSprite;
       [SerializeField] Color defaultColor = Color.white;
       [SerializeField] Color fillColor = Color.black;
       [SerializeField] Color xColor = Color.white;

        public Color GetColor(TileState state)
        {
            switch (state)
            {
                default:
                case TileState.DEFAULT: 
                    return defaultColor;

                case TileState.FILL: 
                    return fillColor;

                case TileState.X:
                    return xColor;
            }
        }
        public Sprite GetSprite(TileState state)
        {
            switch (state)
            {
                default:
                case TileState.DEFAULT:
                    return defaultSprite;

                case TileState.FILL:
                    return fillSprite;

                case TileState.X:
                    return xSprite;
            }
        }

    }
}
