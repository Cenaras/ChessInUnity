public class Bishop : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;


    public Bishop(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Bishop;
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
