using UnityEngine;

public class Pawn : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;

    // Auto generate getter and setter
    public bool HasMoved { get; set; }


    public Pawn(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
        HasMoved = false;
    }

    
    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Pawn;
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



}
