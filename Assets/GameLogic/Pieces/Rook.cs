public class Rook : Piece {

    private GameColor color;

    public Rook(GameColor pieceColor) {
        color = pieceColor;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Rook;
    }

    public GameColor PieceColor() {
        return color;
    }

}
