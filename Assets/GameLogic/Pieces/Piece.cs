using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Piece 
{
    public enum PieceType {
        Pawn, 
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public PieceType GetPieceType();
    public GameColor PieceColor();

}
