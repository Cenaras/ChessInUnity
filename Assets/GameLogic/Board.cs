
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;

// Magic for records to work
namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit { }
}

public record BoardPosition(int file, int rank) {

    public bool IsValidPosition() {
        return (rank >= 0 && rank <= 7 && file >= 0 && file <= 7);
    }

    public override string ToString() {
        return $"[{rank}, {file}]";
    }

    public static BoardPosition Add(BoardPosition left, BoardPosition right) {
        return new BoardPosition(left.file + right.file, left.rank + right.rank);
    }

    public static BoardPosition ScalarAdd(BoardPosition pos, int scalar) {
        return new BoardPosition(pos.file + scalar, pos.rank + scalar);

    }

    public static BoardPosition ScalarMult(BoardPosition pos, int scalar) {
        return new BoardPosition(pos.file * scalar, pos.rank * scalar);
    }

    public static bool Equals(BoardPosition one, BoardPosition two) {
        return (one.file == two.file) && (one.rank == two.rank);
    }
    


}

public class Board {


    private readonly Piece[,] pieces;
    public GameConstants.GameColor colorToMove;
    public MoveGenerator moveGen;


    private void SwapTurn() {
        if (colorToMove == GameConstants.GameColor.White) {
            colorToMove = GameConstants.GameColor.Black;
        } else {
            colorToMove = GameConstants.GameColor.White;
        }
    }

    public Board(Piece[,] pieces) {
        this.pieces = pieces;
        colorToMove = GameConstants.GameColor.White;
        moveGen = new MoveGenerator();
    }

    public Piece[,] GetBoardState() {
        return pieces;
    }

    public static Board parseFen(String fen) {
        Dictionary<char, Piece.PieceType> pieceFromSymbol = new Dictionary<char, Piece.PieceType>()
        {
            {'r', Piece.PieceType.Rook},
            {'n', Piece.PieceType.Knight},
            {'b', Piece.PieceType.Bishop},
            {'q', Piece.PieceType.Queen},
            {'k', Piece.PieceType.King},
            {'p', Piece.PieceType.Pawn},
        };

        Piece[,] pieces = new Piece[8, 8];
        String[] fenRanks = fen.Split('/');
        for (int i = 0; i < 8; i++) {
            int file = 0;
            String fenRank = fenRanks[i];
            foreach (char c in fenRank) {
                if (Char.IsDigit(c)) file += c;
                else {
                    Piece.PieceType pieceType = pieceFromSymbol[char.ToLower(c)];
                    bool isWhite = char.IsUpper(c);
                    //Debug.Log($"Placing piece {pieceType} at position {i}, {file}, white: {isWhite}");
                    GameConstants.GameColor pieceColor = isWhite ? GameConstants.GameColor.White : GameConstants.GameColor.Black;


                    BoardPosition posOnBoard = new BoardPosition(7 - i, file);

                    Piece piece = pieceType switch {
                        Piece.PieceType.Pawn => new Pawn(pieceColor, posOnBoard),
                        Piece.PieceType.Knight => new Knight(pieceColor, posOnBoard),
                        Piece.PieceType.Bishop => new Bishop(pieceColor, posOnBoard),
                        Piece.PieceType.Rook => new Rook(pieceColor, posOnBoard),
                        Piece.PieceType.Queen => new Queen(pieceColor, posOnBoard),
                        Piece.PieceType.King => new King(pieceColor, posOnBoard),
                        _ => throw new Exception("Impossible")
                    };

                    pieces[7 - i, file] = piece;
                    file++;
                }
            }
        }
        return new Board(pieces);
    }


    /** Returns true if the move was made */
    public bool TryMakeMove(GameConstants.GameColor playerColor, Move move, List<Move> validMoves) {
        /*
        How a move works:
        We get the piece.
        We inspect the valid moves from the piece
        We ensure the target position is valid
        We place the piece
         */

        // For now; brute force piece to position

        // Moving a piece to its own location is not valid.
        //if (from.Equals(to)) return false;


        Piece movingPiece = PieceAt(move.from);

        // Only move the piece if it exists and is from the right player
        if (movingPiece == null || movingPiece.PieceColor() != playerColor) {
            Debug.Log("Cannot move - wrong color or no piece");
            return false;
        }

        // Generate valid moves for current position and check if the move we tried to make is valid.
        if (!validMoves.Contains(move)) {
            return false;
        }

        MovePiece(movingPiece, move.from, move.to);
        movingPiece.SetPosition(move.to);
        // If pawn move, mark as HasMoved.
        if (movingPiece.GetPieceType()  == Piece.PieceType.Pawn) {
            (movingPiece as Pawn).HasMoved = true;
        }
        SwapTurn();
        return true;
    }


    private void MovePiece(Piece piece, BoardPosition from, BoardPosition to) {
        ClearPositionAt(from);
        PlacePieceAt(to, piece);
    }


    public Piece PieceAt(BoardPosition position) {
        //Debug.Log("Position: " + position);
        return pieces[position.file, position.rank];
    }

    private void PlacePieceAt(BoardPosition position, Piece piece) {
        //Debug.Log("Placing piece at " + position);
        pieces[position.file, position.rank] = piece;
    }

    private void ClearPositionAt(BoardPosition position) {
        //Debug.Log("Clearing piece at position " + position);
        pieces[position.file, position.rank] = null;
    }

}
