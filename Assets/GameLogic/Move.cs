using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move {
    public Move(BoardPosition fromPos, BoardPosition toPos) {
        from = fromPos;
        to = toPos;
    }

    public BoardPosition from { get; }
    public BoardPosition to { get; }

    public override string ToString() {
        return $"{from} --> {to}";
    }


}