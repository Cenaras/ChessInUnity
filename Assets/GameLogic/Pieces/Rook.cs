using UnityEngine.UIElements;

public class Rook : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;

    public Rook(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Rook;
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
