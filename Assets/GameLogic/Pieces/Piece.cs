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
    public GameConstants.GameColor PieceColor();
    public BoardPosition GetPosition();
    public void SetPosition(BoardPosition position);
    public Sprite sprite() {
        return GameConstants.GetPieceSprite(this);
    }
    // Maybe let the pieces compute their movement directions - only let the MoveGen check if they're valid?
    public List<BoardPosition> CandidateSquares();
    public static void AddIfValid(List<BoardPosition> listOfPos, BoardPosition position) {
        if (position.IsValidPosition())
            listOfPos.Add(position);
    }

}
