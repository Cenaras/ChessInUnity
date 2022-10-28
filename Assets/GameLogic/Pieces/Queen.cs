public class Queen : Piece {

    private GameColor color;

    public Queen(GameColor pieceColor) {
        color = pieceColor;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Queen;
    }

    public GameColor PieceColor() {
        return color;
    }

}
