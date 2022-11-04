using System.Collections.Generic;
using UnityEngine.UIElements;

public class Rook : Piece {

    private GameConstants.GameColor color;
    private BoardPosition position;
    private bool hasMoved;

    private static List<BoardPosition> directionOffsets = new List<BoardPosition> {
        new BoardPosition(0, -1),
        new BoardPosition(0, 1),
        new BoardPosition(1, 0),
        new BoardPosition(-1, 0),
    };

    public Rook(GameConstants.GameColor pieceColor, BoardPosition startingPosition) {
        color = pieceColor;
        position = startingPosition;
        hasMoved = false;
    }


    public Piece.PieceType GetPieceType() {
        return Piece.PieceType.Rook;
    }

    public GameConstants.GameColor PieceColor() {
        return color;
    }

    public BoardPosition GetPosition() {
        return position;
    }

    public void SetPosition(BoardPosition position) {
        this.position = position;
    }

    public List<BoardPosition> CandidateSquares() {
        List<BoardPosition> candidatePositions = new List<BoardPosition>();
        for (int distance = 1; distance < 8; distance++) {
            foreach (BoardPosition direction in directionOffsets) {
                BoardPosition candidatePosition = BoardPosition.Add(position, BoardPosition.ScalarMult(direction, distance));
                Piece.AddPositionIfValid(candidatePositions, candidatePosition);
            }
        }
        return candidatePositions;
    }

    bool Piece.HasMoved() {
        return hasMoved;
    }

    public void SetHasMoved(bool value) {
        hasMoved = value;
    }
}
