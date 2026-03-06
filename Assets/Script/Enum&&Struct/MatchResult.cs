using System.Collections.Generic;

namespace Script.Enum__Struct
{
    public struct MatchResult
    {
        public List<Crush> ConnectedCrushes;
        public MatchDirection Direction;
    }

    public enum MatchDirection
    {
        None,
        Vertical,
        Horizontal,
    }
}