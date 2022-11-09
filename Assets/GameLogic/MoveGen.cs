using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// Idea: Keep an array here of the squares a player targets.
// We can do this while iterating over the valid moves for the current player and maybe update this list as we go?
// Worst case do a 2nd scan. Depends on what data we need (e.g. relvant data is squares opponent controls, not ourselves so maybe save the data from prev search?)
// But this has to work if we also do deeper searches and it might not, think about it...
public class MoveGen {
    // {1, -1, 8, -8} for rook movement. {7, -7, 9, -9} for bishop movements
    private static readonly int[] rookDirections = new int[] { -1, 1, -8, 8 };
    private static readonly int[] bishopDirections = new int[] { 7, -7, 9, -9 };
    private static readonly int[] queenDirections = new int[] { -1, 1, -8, 8, 7, -7, 9, -9 };
    private static readonly int[] knightDirections = new int[] { 17, -17, 15, -15, 10, -10, 6, -6 };
    private static readonly int[] kingDirections = new int[] { -1, 1, -8, 8, -7, 7, -9, 9};
    // TODO: Pawns also?

    // TODO: Maybe make the map from piece to moves into a 2D array for memory efficiency?
    private static readonly Dictionary<int, int[]> directionsForPieceType = new() {
        { Piece.Rook, rookDirections},
        { Piece.Bishop, bishopDirections},
        { Piece.Queen, queenDirections},
        { Piece.King, kingDirections},
        { Piece.Knight, knightDirections},
    };


    // TODO: Precompute distance from square to out of board for each square and direction so we can avoid having to check if target square is valid.
    // Could be int[,] with first entry being square and second being direction.

    /* Generate legal moves for a position */
    public List<Move> LegalMoves(Board board) {
        // TODO: Figure out if MakeMove or UnmakeMove or w/e is wrong and fix...

        // For now, naïve implementation - iterate over all pseudo legal moves. Generate all responses, check if a response can capture king and if not, add move
        /*List<Move> legalMoves = new();
        List<Move> pseudoLegal = PseudoLegalMoves(board);

        // I THINK THE ISSUE IS: We play the move, AND THEN look at valid moves - e.g. white plays 1.e4 and after that we look at white moves.

        Debug.Log("Initial moves: " + pseudoLegal.Count);
        //foreach (Move m in pseudoLegal) Debug.Log(m);
        
        
        foreach (Move pseudoMove in pseudoLegal) {
            board.MakeMove(pseudoMove);
            List<Move> responses = PseudoLegalMoves(board);

            // King was not capturable
            if (!responses.Any(r => r.TargetSquare == board.KingSquareForColor(Piece.OppositeColor(board.ColorToMove)))) legalMoves.Add(pseudoMove);
            board.UnmakeMove(pseudoMove);
        }

        return legalMoves; */
        return PseudoLegalMoves(board);
    }


    /* Generate valid moves for a position, ignoring checks and pins. */
    public List<Move> PseudoLegalMoves(Board board) {
        List<Move> pseudoLegal = new List<Move>();

        // Iterate over the entire board to find moves for each piece. 
        // Idea for optimization: Only recompute for moved piece and pieces who could see the from and to pos of moved piece.
        for (int square = 0; square < 64; square++) {
            int piece = board.PieceAt(square);
            if (Piece.IsColor(piece, board.ColorToMove)) {
                pseudoLegal.AddRange(PseudoLegalMovesForPiece(piece, square, board));
            }
        }
        return pseudoLegal;
    }

    private List<Move> PseudoLegalMovesForPiece(int piece, int startSquare, Board board) {
        int pieceType = Piece.Type(piece);

        // Since pawns only move forward, we give them special treatment
        if (pieceType == Piece.Pawn) {
            return GeneratePawnMoves(piece, startSquare, board);
        } else {
            // Compute the directions for the given piece type and handle computations of valid moves.
            int[] directions = directionsForPieceType[pieceType];
            if (pieceType == Piece.King) {
                return GenerateKingMoves(piece, directions, startSquare, board);
            } else if (pieceType == Piece.Knight) {
                return GenerateKnightMoves(piece, directions, startSquare, board);
            } else {
                return GenerateSlidingPieceMoves(piece, directions, startSquare, board);
            }
        }
    }

    // Then also look for captures
    // Handle EP (+ castle for King) after base is done...

    private List<Move> GeneratePawnMoves(int pawn, int startSquare, Board board) {
        List<Move> moves = new List<Move>();
        // Compute the forward direction of the pawn
        int pawnDirection = Piece.IsColor(pawn, Piece.White) ? 8 : -8;

        /* MOVEMENT FOR PAWN */

        int infront = startSquare + pawnDirection;
        int colorOfPawn = Piece.GetColor(pawn);
        //bool validInfront = infront >= 0 && infront <= 64;

        if (infront < 0 || infront > 64) {
            Debug.Log($"Infront is {infront} for piece at start square {startSquare} of type {Piece.Type(pawn)}");
        }

        // If square infront of pawn is empty
        if (board.PieceAt(infront) == Piece.None) {

            // TODO: Check if startsquare is one from promotion since this should be handled (adding flag to move struct)
            if (BoardUtils.IsPromotionRank(pawn, colorOfPawn)) Debug.Log("Handle Promotion");
            
            
            moves.Add(new Move(startSquare, infront));

            // Check if the square two in front is also empty and the pawn square is the starting
            int twoInFront = startSquare + 2 * pawnDirection;
            int rankOfPawn = BoardUtils.RankOfSquare(startSquare);
            bool atStartingRank = rankOfPawn == BoardUtils.PawnStartRank(colorOfPawn);

            if (board.PieceAt(twoInFront) == Piece.None && atStartingRank) {
                moves.Add(new Move(startSquare, twoInFront, Move.Type.PawnDouble));
            }
        }


        /* CAPTURE FOR PAWN */
        
        // Compute the legal capture squares for the pawn
        int[] captureSquares = BoardUtils.CaptureSquaresForPawn(startSquare, colorOfPawn);
        for (int i = 0; i < captureSquares.Length; i++) {

            // Regular captures - an enemy piece is present at the target position
            int targetSquare = startSquare + captureSquares[i];
            int targetPiece = board.PieceAt(targetSquare);
            
            // Piece at target square is enemy piece
            if (Piece.IsColor(targetPiece, Piece.OppositeColor(colorOfPawn))) {
                moves.Add(new Move(startSquare, targetSquare));
            }

            // For en passant: We store the ep square in the board history thing and check if target square is that.
            int epSquare = board.currentState.EnPassantSquare;
            if (targetSquare == epSquare) {
                moves.Add(new Move(startSquare, targetSquare, Move.Type.EnPassant));
            }

        }

        return moves;
    }

    /* When optimizing this, compute visible squares by enemy and only allow move if that square is not in the list. */
    private List<Move> GenerateKingMoves(int king, int[] directions, int startSquare, Board board) {
        List<Move> moves = new List<Move>();

        int friendlyColor = Piece.GetColor(king);

        for (int i = 0; i < directions.Length; i++) {
            int targetSquare = startSquare + directions[i];
            // Valid square on the board
            if (targetSquare >= 0 && targetSquare <= 63) {

                int pieceAtTarget = board.PieceAt(targetSquare);
                
                // If empty or enemy present, allow the move - TODO: Only allow if square is not controlled by enemy
                if (!Piece.IsColor(pieceAtTarget, friendlyColor)) moves.Add(new Move(startSquare, targetSquare));
            }
        }

        /* Handle Castling separetly to distingush between normal and special moves. */

        // Small optimization - get currentState for castle rights and only enter loop of one of them is true.

        // Castling directions
        for (int i = -2; i <= 2; i += 4) {

            int targetSquare = startSquare + i;
            
            // TODO: Find a cleaner way instead of checking on this condition each time perhaps?
            if (targetSquare >= 0 && targetSquare <= 63) {
                int pieceAtTarget = board.PieceAt(targetSquare);

                /* KingSide Castling */
                bool canCastleKingSide = friendlyColor == Piece.White ? board.currentState.WhiteCastleKingSide : board.currentState.BlackCastleKingSide;
                if (BoardUtils.IsKingSideCastleSquare(targetSquare, friendlyColor) && canCastleKingSide) {
                    // Castle if no pieces are between king and rook, and not castling through check
                    int rightOfKing = board.PieceAt(targetSquare - 1);
                    bool clearPath = rightOfKing == Piece.None && pieceAtTarget == Piece.None;
                    // TODO: Also ensure not castling through check
                    if (clearPath) {
                        moves.Add(new Move(startSquare, targetSquare, Move.Type.KingCastle));
                    }
                }

                /* QueenSide Castling */
                bool canCastleQueenSide = friendlyColor == Piece.White ? board.currentState.WhiteCastleQueenSide : board.currentState.BlackCastleQueenSide;
                if (BoardUtils.IsQueenSideCastleSquare(targetSquare, friendlyColor) && canCastleQueenSide) {
                    int leftOfKing = board.PieceAt(targetSquare + 1);
                    int rightOfRook = board.PieceAt(targetSquare - 1);
                    bool clearPath = pieceAtTarget == Piece.None && leftOfKing == Piece.None && rightOfRook == Piece.None;
                    // TODO: Also ensure not castling through check
                    if (clearPath) moves.Add(new Move(startSquare, targetSquare, Move.Type.QueenCastle));
                }
            }
        }


        return moves;
    }

    private List<Move> GenerateKnightMoves(int knight, int[] directions, int startSquare, Board board) {
        List<Move> moves = new List<Move>();

        for (int i = 0; i < directions.Length; i++) {
            // Knights can jump over pieces so we only need to add the valid positions

            int targetSquare = startSquare + directions[i];

            // Ensure knight moved max 2 on x or y (to avoid wrapping around the board)
            int startRank = startSquare / 8;
            int startFile = startSquare - startRank * 8;

            int endRank = targetSquare / 8;
            int endFile = targetSquare - endRank * 8;
            int maxDist = Math.Max(Math.Abs(endRank - startRank), Math.Abs(endFile - startFile));

            // For now, ensure position is on board like this. TODO: Generalize this.
            if (maxDist == 2 && targetSquare >= 0 && targetSquare <= 63) {
                int targetPiece = board.PieceAt(targetSquare);

                // If piece is same color, not a valid move, else add it
                if (!Piece.IsColor(knight, Piece.GetColor(targetPiece))) moves.Add(new Move(startSquare, targetSquare));
            }
        }

        return moves;

    }

    private List<Move> GenerateSlidingPieceMoves(int piece, int[] directions, int startSquare, Board board) {
        List<Move> moves = new List<Move>();

        // Loop over all possible directions for our piece
        for (int i = 0; i < directions.Length; i++) {
            // Worst case, we have a distance of 7 to the edge of the board for the direction. TODO: Precompute this for each square and direction to safe memory.
            for (int j = 1; j < 7; j++) {

                int targetSquare = startSquare + directions[i] * j;

                if (targetSquare >= 0 && targetSquare <= 63) {
                    int pieceOnTarget = board.PieceAt(targetSquare);

                    // If there is a friendly piece, stop the search since we're blocked.
                    if (Piece.IsColor(pieceOnTarget, Piece.GetColor(piece))) break;

                    moves.Add(new Move(startSquare, targetSquare));

                    // If enemy piece at target square, blocked again so stop searching
                    if (Piece.IsColor(pieceOnTarget, Piece.OppositeColor(piece))) break;
                }

            }
        }
        return moves;
    }


}
