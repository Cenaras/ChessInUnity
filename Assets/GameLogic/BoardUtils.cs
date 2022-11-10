using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using UnityEngine;

public static class BoardUtils {

    public static int North = 8;
    public static int South = -8;
    public static int East = 1;
    public static int West = -1;
    public static int NorthEast = 9;
    public static int NorthWest = 7;
    public static int SouthEast = -7;
    public static int SouthWest = -9;

    public static string FenStartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public static readonly int RANK_1 = 0;
    public static readonly int RANK_2 = 1;
    public static readonly int RANK_3 = 2;
    public static readonly int RANK_4 = 3;
    public static readonly int RANK_5 = 4;
    public static readonly int RANK_6 = 5;
    public static readonly int RANK_7 = 6;
    public static readonly int RANK_8 = 7;

    private static readonly int G1 = 6;
    private static readonly int G8 = 62;

    private static readonly int C1 = 2;
    private static readonly int C8 = 58;

    public static readonly int H1 = 7;
    public static readonly int H8 = 63;

    public static readonly int A1 = 0;
    public static readonly int A8 = 56;

    public static readonly int E1 = 4;
    public static readonly int E8 = 60;

    // First index is start square, second index is direction, result is number of squares to edge of board. Initialized in from GameManager
    public static int[][] DistanceToEdge;

    internal static int RookKingStartSquare(int friendlyColor) {
        if (friendlyColor == Piece.White) return H1;
        else return H8;
    }
    internal static int RookQueenStartSquare(int friendlyColor) {
        if (friendlyColor == Piece.White) return A1;
        else return A8;
    }

    // Naive implementation for simplicity
    public static int RankOfSquare(int square) {
        if (square < 8) return RANK_1;
        if (square < 16) return RANK_2;
        if (square < 24) return RANK_3;
        if (square < 32) return RANK_4;
        if (square < 40) return RANK_5;
        if (square < 48) return RANK_6;
        if (square < 56) return RANK_7;
        return RANK_8;
    }

    public static int FileOfSquare(int square) {
        int rank = RankOfSquare(square);
        return square - rank * 8;
    }

    public static bool IsQueenSideCastleSquare(int square, int color) {
        if (color == Piece.White) return square == C1;
        else return square == C8;
    }
    public static bool IsKingSideCastleSquare(int square, int color) {
        if (color == Piece.White) return square == G1;
        else return square == G8;
    }


    public static int PawnStartRank(int color) {
        if (color == Piece.White) return RANK_2;
        else return RANK_7;
    }

    public static bool IsPromotionRank(int square, int color) {
        if (color == Piece.White) return RankOfSquare(square) == RANK_8;
        else return RankOfSquare(square) == RANK_1;
    }

    internal static int[] CaptureSquaresForPawn(int startSquare, int colorOfPawn) {
        // A-File = {0, 8, 16, 24, 32, 40, 48, 56}
        // H-File = {7, 15, 23, 31, 39, 47, 55, 63}
        int[] aFile = new int[] { 0, 8, 16, 24, 32, 40, 48, 56 };
        int[] hFile = new int[] { 7, 15, 23, 31, 39, 47, 55, 63 };


        bool isWhite = Piece.GetColor(colorOfPawn) == Piece.White;

        // From the perspective of white
        int whiteRightOffset = 9;
        int whiteLeftOffset = 7;

        int blackRightOffset = -7;
        int blackLeftOffset = -9;

        // Check if the pawn is on the edge of the board to only allow captures inside the board
        if (aFile.Contains(startSquare)) {
            if (isWhite) return new int[] { whiteRightOffset };
            else return new int[] { blackRightOffset };

        } else if (hFile.Contains(startSquare)) {
            if (isWhite) return new int[] { whiteLeftOffset };
            else return new int[] { blackLeftOffset };

            // Not on edge of board. Both directions are valid captures.
        } else {
            if (isWhite) return new int[] { whiteLeftOffset, whiteRightOffset };
            else return new int[] { blackLeftOffset, blackRightOffset };

        }







        throw new NotImplementedException();
    }


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
            string fenRank = fenRanks[7 - rank];

            foreach (char c in fenRank) {
                if (char.IsDigit(c)) file += (int)char.GetNumericValue(c);
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
    internal static void PrecomputeDistanceToEdge() {
        // 64 possible from squares
        DistanceToEdge = new int[64][];

        for (int square = 0; square < 64; square++) {
            // 8 directions for each square
            DistanceToEdge[square] = new int[8];

            // Compute file and rank for each square and use to compute distance to edge of board
            int file = FileOfSquare(square);
            int rank = RankOfSquare(square);

            /* Distances to edge in the given direction */
            int distNorth = 7 - rank;
            int distSouth = rank;
            int distEast = 7 - file;
            int distWest = file;

            /* Populate the array for the given square with computed information. Order is N, S, E, W, NE, NW, SE, SW*/
            DistanceToEdge[square][0] = distNorth;
            DistanceToEdge[square][1] = distSouth;
            DistanceToEdge[square][2] = distEast;
            DistanceToEdge[square][3] = distWest;

            DistanceToEdge[square][4] = Math.Min(distNorth, distEast);
            DistanceToEdge[square][5] = Math.Min(distNorth, distWest);
            DistanceToEdge[square][6] = Math.Min(distSouth, distEast);
            DistanceToEdge[square][7] = Math.Min(distSouth, distWest);
        }
    }
}
