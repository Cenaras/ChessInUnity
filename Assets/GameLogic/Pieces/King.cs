public class King : Piece {

    private GameColor color;

    public King(GameColor pieceColor) {
        color = pieceColor;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.King;
    }

    public GameColor PieceColor() {
        return color;
    }

}
