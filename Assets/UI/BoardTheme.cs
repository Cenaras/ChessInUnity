using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Theme/Board")]
public class BoardTheme : ScriptableObject
{
    public Color whiteSquareColor;
    public Color blackSquareColor;
    public Color highlightSquareColor;
    public Color selectedSquareColor;



    public Color ColorForSquare(BoardPosition position) {
        return position.IsWhiteSquare() ? whiteSquareColor : blackSquareColor;
    }


    /*[System.Serializable]
    public struct PieceColor {
        public Color normal;
        public Color highlighted;
        public Color lastMove;
    }*/
}
