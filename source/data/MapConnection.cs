using Godot;

[GlobalClass]
public partial class MapConnection : Resource {
    [Export] public MapConnectionSide Left;
    [Export] public MapConnectionSide Right;
    [Export] public int Cost;
    public MapConnectionSide Get(string id) {
        bool isLeft = Left.MapID.CompareTo(id)==0;
        bool isRight= Right.MapID.CompareTo(id)==0;
        if (isLeft) return Left;
        if (isRight) return Right;
        return null;
    }
    public MapConnectionSide GetOther(string id) {
        bool isLeft = Left.MapID.CompareTo(id)==0;
        bool isRight= Right.MapID.CompareTo(id)==0;
        if (isLeft) return Right;
        if (isRight) return Left;
        return null;
    }
    
}