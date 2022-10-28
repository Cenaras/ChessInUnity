public class Knight : Piece {

    private GameColor color;

    public Knight(GameColor pieceColor) {
        color = pieceColor;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Knight;
    }

    public GameColor PieceColor() {
        return color;
    }

}
