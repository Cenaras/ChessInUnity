/* TODO: Probably make this as easy in memory as possible to reduce memory during search */

public struct Move {

    public Move(int startSquare, int targetSquare) {
        StartSquare = startSquare;
        TargetSquare = targetSquare;
    
    }

    public int StartSquare { get; }
    public int TargetSquare { get; }

}