using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

    private bool hasMoved;
    private GameConstants.GameColor color;
    private BoardPosition position;
    private static List<BoardPosition> knightMoves = new List<BoardPosition> { 
        new BoardPosition(-2, -1), 
        new BoardPosition(-2, 1), 
        new BoardPosition(2, -1), 
        new BoardPosition(2, 1), 
        new BoardPosition(-1, -2), 
        new BoardPosition(-1, 2), 
        new BoardPosition(1, -2), 
        new BoardPosition(1, 2), 
    };
    public Knight(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
        hasMoved = false;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Knight;
    }

    public GameConstants.GameColor PieceColor() {
        return color;
    }
    public BoardPosition GetPosition() {
        return position;
    }

    public void SetPosition(BoardPosition position) {
        this.position = position;
    }

    public List<BoardPosition> CandidateSquares() {

        bool isWhite = color == GameConstants.GameColor.White;
        List<BoardPosition> candidatePositions = new List<BoardPosition>();
        foreach (BoardPosition candMove in knightMoves) {
            BoardPosition targetSquare = BoardPosition.Add(position, candMove);
            Piece.AddPositionIfValid(candidatePositions, targetSquare);
        }

        return candidatePositions;
    }

    public bool HasMoved() {
        return hasMoved;
    }

    public void SetHasMoved(bool value) {
        hasMoved = value;
    }
}
