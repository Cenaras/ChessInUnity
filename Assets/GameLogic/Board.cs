
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;
using System.Text;
using UnityEditor;


// Give the board a history object which can track if pieces moved and castle information and stuff of the sorts
// TODO: Implement a way to detect possible positions to compare with engines - and for the love of god clean up the code when the test framework is there...

public class Board {
    private readonly Piece[,] pieces;
    public GameConstants.GameColor colorToMove;
    public MoveGenerator moveGen;
    private BoardUI boardUI;
    public King WhiteKing;
    public King BlackKing;

    public Pawn enPassantPawn = null;


    public Board(Piece[,] pieces, BoardUI boardUI) {
        this.pieces = pieces;
        colorToMove = GameConstants.GameColor.White;
        moveGen = new MoveGenerator();
        this.boardUI = boardUI;
        WhiteKing = pieces[4, 0] as King;
        BlackKing = pieces[4, 7] as King;
    }

    public King GetKingOfColor(GameConstants.GameColor kingColor) {
        if (kingColor == GameConstants.GameColor.White)
            return WhiteKing;
        else
            return BlackKing;
    }


    private void SwapTurn() {
        if (colorToMove == GameConstants.GameColor.White) {
            colorToMove = GameConstants.GameColor.Black;
        } else {
            colorToMove = GameConstants.GameColor.White;
        }
    }

    
    public void MakeMove(Piece piece, Move move) {
        MovePiece(piece, move.From, move.To);

        // Check the en passant pawn and remove it if capture
        if (move.Type == Move.MoveType.EnPassantCapture) {
            Debug.Log("En passant!");
            ClearPieceAt(enPassantPawn.GetPosition());
        }


        // Set en-passant pawn - and reset to null after a non-double move
        if (move.Type == Move.MoveType.PawnDoubleMove) {
            Pawn pawn = piece as Pawn;
            enPassantPawn = pawn;
        } else {
            enPassantPawn = null;
        }

        piece.SetHasMoved(true);


        // Check if move was a castle, and if it was, move the rook...
        if (move.Type == Move.MoveType.QueenCastle || move.Type == Move.MoveType.KingCastle) {
            Debug.Log("Move was castle");
            MoveRookForCastle(piece as King, move.Type);
        }

        SwapTurn();
        //Debug.Log("move made, current players turn is " + colorToMove);
        boardUI.UpdatePosition(this);
    }

    /* Makes moves without updating the HasMoved information - used for scanning ahead without ruining the game state */
    public void MakeSilentMove(Piece piece, Move move) {
        MovePiece(piece, move.From, move.To);
        // Check if move was a castle, and if it was, move the rook...
        if (move.Type == Move.MoveType.QueenCastle || move.Type == Move.MoveType.KingCastle) {
            MoveRookForCastle(piece as King, move.Type);
        }
        SwapTurn();
    }


    /* Unmakes a move played - used for undoing the scan ahead search to restore the board state */
    public void UnmakeMove(Piece piece, Move moveToUndo, Piece capturedPiece) {
        MovePiece(piece, moveToUndo.To, moveToUndo.From);
        // Undo capture, by placing captured piece at the moves To location
        PlacePieceAt(moveToUndo.To, capturedPiece);
        if (moveToUndo.Type == Move.MoveType.QueenCastle || moveToUndo.Type == Move.MoveType.KingCastle) {
            UndoCastleMove(piece as King, moveToUndo.Type);
        }

        SwapTurn();
    }

    private void UndoCastleMove(King king, Move.MoveType castleType) {
        switch (king.PieceColor()) {
            case GameConstants.GameColor.White: {
                    if (castleType == Move.MoveType.QueenCastle) {
                        MoveRookForCastle(new BoardPosition(3,0), GameConstants.WhiteQueenRookStartPos);
                    } else {
                        MoveRookForCastle(new BoardPosition(5, 0), GameConstants.WhiteKingRookStartPos);

                    }
                    break;
                }
            case GameConstants.GameColor.Black: {
                    if (castleType == Move.MoveType.QueenCastle) {
                        MoveRookForCastle(new BoardPosition(3, 7), GameConstants.BlackQueenRookStartPos);
                    } else {
                        MoveRookForCastle(new BoardPosition(5, 7), GameConstants.BlackKingRookStartPos);
                    }
                    break;
                }
        }
    }

    private void MoveRookForCastle(King king, Move.MoveType castleType) {
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
        piece.SetPosition(to);
        PlacePieceAt(to, piece);
        ClearPieceAt(from);

    }


    public Piece PieceAt(BoardPosition position) {
        return pieces[position.file, position.rank];
    }

    private void PlacePieceAt(BoardPosition position, Piece piece) {
        pieces[position.file, position.rank] = piece;
    }

    private void ClearPieceAt(BoardPosition position) {
        pieces[position.file, position.rank] = null;
    }


    public static Piece[,] ParseFen(string fen) {
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
        string[] fenRanks = fen.Split('/');
        for (int i = 0; i < 8; i++) {
            int file = 0;
            string fenRank = fenRanks[i];
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
        return pieces;
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
