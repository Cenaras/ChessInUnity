using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using UnityEngine;

public static class BoardUtils
{

    public static string FenStartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    private static Dictionary<char, int> pieceFromSymbol = new Dictionary<char, int>() {
            {'r', Piece.Rook},
            {'n', Piece.Knight},
            {'b', Piece.Bishop},
            {'q', Piece.Queen},
            {'k', Piece.King},
            {'p', Piece.Pawn},
        };

    private static Dictionary<int, char> symbolFromPiece = new Dictionary<int, char>() {
            {Piece.Rook, 'R'},
            {Piece.Knight, 'N'},
            {Piece.Bishop, 'B'},
            {Piece.Queen, 'Q'},
            {Piece.King, 'K'},
            {Piece.Pawn, 'P'},
        };


    private static char SymbolFromPiece(int piece) {
        return symbolFromPiece[Piece.Type(piece)];
    }


    public static Board ParseFenString(string fenString) {

        int[] Squares = new int[64];
        string[] fenRanks = fenString.Split('/');

        for (int rank = 7; rank >= 0; rank--) {
            int file = 0;
            string fenRank = fenRanks[7-rank];

            foreach (char c in fenRank) {
                if (char.IsDigit(c)) file += (int) char.GetNumericValue(c);
                else {
                    // A piece is defined as the type OR color as a 5 bit number.
                    int type = pieceFromSymbol[char.ToLower(c)];
                    int color = char.IsUpper(c) ? Piece.White : Piece.Black;
                    int piece = type | color;

                    Squares[rank * 8 + file] = piece;
                }
                file++;
            }

        }

        return new Board(Squares);
    }

    public static void PrintBoard(Board board) {

        string rankSeperation = "\n+---+---+---+---+---+---+---+---+\n";

        StringBuilder builder = new StringBuilder(rankSeperation);
        for (int rank = 7; rank >= 0; rank--) {
            for (int file = 0; file < 8; file++) {
                builder.Append(" | ");

                int square = rank * 8 + file;

                int piece = board.PieceAt(square);
                int pieceType = Piece.Type(piece);

                if (pieceType == 0) builder.Append(" X ");
                else {
                    // Capital letter for White, Lower case for Black
                    bool isWhite = Piece.IsColor(piece, Piece.White);
                    char symbol = symbolFromPiece[pieceType];

                    char coloredSymbol = isWhite ? symbol : char.ToLower(symbol);
                    builder.Append(" ").Append(coloredSymbol).Append(" ");
                }
            }
            builder.Append(" |   ").Append(1 + rank).Append(rankSeperation);
        }

        builder.Append("     a     b     c     d     e     f     g     h     ");

        Debug.Log(builder.ToString());

    }


    public static int SquareFrom(int file, int rank) {
        return rank * 8 + file;
    }

    public static int SquareFrom(BoardPosition position) {
        return SquareFrom(position.File, position.Rank);
    }


    public static Sprite SpriteForPiece(int piece) {
        if (piece == 0) return null; // Return null for Piece.None

        bool isWhite = Piece.IsColor(piece, Piece.White);
        string color = isWhite ? "w" : "b";
        char id = SymbolFromPiece(piece);
        string spritePath = $"Sprites/{color}{id}";

        return Resources.Load<Sprite>(spritePath);


    }


}
