
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;
using System.Text;

// Magic for records to work
namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit { }
}

public record BoardPosition(int file, int rank) {

    public bool IsValidPosition() {
        return (rank >= 0 && rank <= 7 && file >= 0 && file <= 7);
    }

    public override string ToString() {
        return $"[{file}, {rank}]";
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

    // Use this to determine if after a piece moves, it attacks the enemy king.
    public Piece LastMovedPiece{ get; set; }

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

                    BoardPosition posOnBoard = new BoardPosition(file, 7 - i);

                    Piece piece = pieceType switch {
                        Piece.PieceType.Pawn => new Pawn(pieceColor, posOnBoard),
                        Piece.PieceType.Knight => new Knight(pieceColor, posOnBoard),
                        Piece.PieceType.Bishop => new Bishop(pieceColor, posOnBoard),
                        Piece.PieceType.Rook => new Rook(pieceColor, posOnBoard),
                        Piece.PieceType.Queen => new Queen(pieceColor, posOnBoard),
                        Piece.PieceType.King => new King(pieceColor, posOnBoard),
                        _ => throw new Exception("Impossible")
                    };

                    pieces[file, 7 - i] = piece;

                    file++;
                }
            }
        }
        return new Board(pieces);
    }


    /** Returns true if the move was made */
    public bool TryMakeMove(GameConstants.GameColor playerColor, Move move, List<Move> validMoves) {
        Piece movingPiece = PieceAt(move.From);

        if (movingPiece == null) {
            Debug.Log("Moving piece is null");
        }

        if (movingPiece.GetPieceType() == Piece.PieceType.King) {
            Debug.Log("Holding King");
            Debug.Log("Move is: " + move.From + ", " + move.To + ": " + move.Type);
        }

        // Only move the piece if it exists and is from the right player
        if (movingPiece == null || movingPiece.PieceColor() != playerColor) {
            //Debug.Log("Cannot move - wrong color or no piece");
            return false;
        }

        // Generate valid moves for current position and check if the move we tried to make is valid.
        if (!validMoves.Contains(move)) {
            return false;
        }

        MovePiece(movingPiece, move.From, move.To);
        movingPiece.SetPosition(move.To);
        // If pawn move, mark as HasMoved.
        if (movingPiece.GetPieceType() == Piece.PieceType.Pawn) {
            (movingPiece as Pawn).HasMoved = true;
        }
        if (movingPiece.GetPieceType() == Piece.PieceType.King) {
            (movingPiece as King).HasMoved = true;
        }
        if (movingPiece.GetPieceType() == Piece.PieceType.Rook) {
            (movingPiece as Rook).HasMoved = true;
        }

        // Check if move was a castle, and if it was, move the rook...
        if (move.Type == Move.MoveType.QueenCastle || move.Type == Move.MoveType.KingCastle) {
            MoveRookForCastle(movingPiece as King, move.Type);
        }


        SwapTurn();
        LastMovedPiece = movingPiece;
        return true;
    }

    private void MoveRookForCastle(King king, Move.MoveType castleType) {
        BoardPosition kingPos = king.GetPosition();
        switch (king.PieceColor()) {
            case GameConstants.GameColor.White: {
                    if (castleType == Move.MoveType.QueenCastle) {
                        MoveRookForCastle(GameConstants.WhiteQueenRookStartPos, new BoardPosition(3, 0));
                    } else {
                        MoveRookForCastle(GameConstants.WhiteKingRookStartPos, new BoardPosition(5, 0));
                    }
                    break;
                }
            case GameConstants.GameColor.Black: {
                    if (castleType == Move.MoveType.QueenCastle) {
                        MoveRookForCastle(GameConstants.BlackQueenRookStartPos, new BoardPosition(3, 7));
                    } else {
                        MoveRookForCastle(GameConstants.BlackKingRookStartPos, new BoardPosition(5, 7));
                    }
                    break;
                }
        }
    }

    private void MoveRookForCastle(BoardPosition rookStartPos, BoardPosition targetPos) {
        Piece rook = PieceAt(rookStartPos);
        MovePiece(rook, rookStartPos, targetPos);
    }


    private void MovePiece(Piece piece, BoardPosition from, BoardPosition to) {
        ClearPositionAt(from);
        PlacePieceAt(to, piece);
    }


    public Piece PieceAt(BoardPosition position) {
        return pieces[position.file, position.rank];
    }

    private void PlacePieceAt(BoardPosition position, Piece piece) {
        pieces[position.file, position.rank] = piece;
    }

    private void ClearPositionAt(BoardPosition position) {
        pieces[position.file, position.rank] = null;
    }

    public void PrintBoard() {
        StringBuilder builder = new StringBuilder();
        for (int rank = 7; rank >= 0; rank--) {
            for (int file = 0; file < 8; file++) {
                Piece piece = PieceAt(new BoardPosition(file, rank));
                if (piece != null) {
                    builder.Append(piece.GetPieceType()).Append(" ");
                } else {
                    builder.Append(" X ");
                }
            }
            builder.AppendLine("");
        }

        Debug.Log(builder.ToString());
    }
}
