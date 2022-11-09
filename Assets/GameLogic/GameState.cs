public struct GameState {

    public bool WhiteCastleQueenSide { get; set; }
    public bool WhiteCastleKingSide { get; set; }
    public bool BlackCastleQueenSide { get; set; }
    public bool BlackCastleKingSide { get; set; }
    public int EnPassantSquare { get; set; }
    public int CapturedPiece { get; set; }
    public int WhiteKingSquare { get; set; }
    public int BlackKingSquare { get; set; }
    public static GameState Initialize() {
        return new GameState {
            WhiteCastleQueenSide = true,
            WhiteCastleKingSide = true,
            BlackCastleQueenSide = true,
            BlackCastleKingSide = true,
            EnPassantSquare = -1,
            CapturedPiece = Piece.None,
            WhiteKingSquare = BoardUtils.E1,
            BlackKingSquare = BoardUtils.E8,
        };
    }

    public GameState Clone() {
        bool newWhiteQueenCastle = WhiteCastleQueenSide;
        bool newWhiteKingCastle = WhiteCastleKingSide;
        bool newBlackQueenCastle = BlackCastleQueenSide;
        bool newBlackKingCastle = BlackCastleQueenSide;
        int newEp = EnPassantSquare;
        int newCapture = CapturedPiece;
        int newWhiteKingPos = WhiteKingSquare;
        int newBlackKingPos = BlackKingSquare;

        return new GameState {
            WhiteCastleQueenSide = newWhiteQueenCastle,
            WhiteCastleKingSide = newWhiteKingCastle,
            BlackCastleQueenSide = newBlackQueenCastle,
            BlackCastleKingSide = newBlackKingCastle,
            EnPassantSquare = newEp,
            CapturedPiece = newCapture,
            WhiteKingSquare = newWhiteKingPos,
            BlackKingSquare = newBlackKingPos,
        };

    }

    public override string ToString() {
        return $"White Queen Side Castle: {WhiteCastleQueenSide}\n" +
            $"White King Side Castle: {WhiteCastleKingSide}\n" +
            $"Black Queen Side Castle: {BlackCastleQueenSide}\n" +
            $"Black King Side Castle: {BlackCastleKingSide}\n" +
            $"En-Passant Square: {EnPassantSquare}\n" +
            $"Captured Piece: {CapturedPiece}";
    }
}
