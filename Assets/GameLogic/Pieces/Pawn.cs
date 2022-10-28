public class Pawn : Piece {

    private GameColor color;
    
    public Pawn(GameColor pieceColor) {
        color = pieceColor;
    }

    
    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Pawn;
    }

    public GameColor PieceColor() {
        return color;
    }

}
