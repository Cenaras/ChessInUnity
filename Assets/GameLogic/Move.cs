/* TODO: Probably make this as easy in memory as possible to reduce memory during search */

public struct Move {

    public enum Type {
        Normal,
        KingCastle,
        QueenCastle,
        EnPassant,
        PawnDouble,
    }

    public Move(int startSquare, int targetSquare) {
        StartSquare = startSquare;
        TargetSquare = targetSquare;
        MoveType = Type.Normal;
    }

    public Move(int startSquare, int targetSquare, Type moveType) {
        StartSquare = startSquare;
        TargetSquare = targetSquare;
        MoveType = moveType;
    }


    public int StartSquare { get; }
    public int TargetSquare { get; }
    public Type MoveType { get; }

}
