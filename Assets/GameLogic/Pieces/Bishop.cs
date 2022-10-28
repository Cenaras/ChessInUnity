public class Bishop : Piece {

    private GameColor color;

    public Bishop(GameColor pieceColor) {
        color = pieceColor;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Bishop;
    }

    public GameColor PieceColor() {
        return color;
    }

}
