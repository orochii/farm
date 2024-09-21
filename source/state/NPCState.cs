using System;
using System.Collections.Generic;
using Godot;

[System.Serializable]
public class NPCState {
    private class RouteNode {
        public string map;
        public Vector2 originPos;
        public Vector2 targetPos;
    }
    public string ID;
    public string CurrentMap;
    public int CurrentPosX;
    public int CurrentPosY;
    public string TargetMap;
    public int TargetPosX;
    public int TargetPosY;
    // TODO: Planned Route
    private List<NPCRoutine> _currentAvailables = null;
    private NPCRoutine _lastRoutine = null;
    private RouteNode[] _route;
    public void Init(NPCData n) {
        ID = n.GetId();
        CurrentMap = n.StartingMapID;
        CurrentPosX = n.StartingPosition.X;
        CurrentPosY = n.StartingPosition.Y;
    }
    public void Refresh(NPCData n)
    {
        _currentAvailables = n.GetAvailableRoutines();
    }
    public bool Process(double delta, NPCData n)
    {
        bool _mapChange = false;
        if (_currentAvailables == null) return _mapChange;
        var currRoutine = NPCData.GetCurrentRoutine(_currentAvailables);
        if (currRoutine != _lastRoutine) {
            _lastRoutine = currRoutine;
            RecalculateRoute();
        }
        // TODO: Advance through route.
        
        return _mapChange;
    }
    private void RecalculateRoute() {
        if (_lastRoutine == null) {
            _route = null;
            return;
        }
        // Setup state variables.
        TargetMap = _lastRoutine.Activity.MapId;
        TargetPosX= _lastRoutine.Activity.Position.X;
        TargetPosY= _lastRoutine.Activity.Position.Y;
        // Trace route. Do dijkstra here.
        var startPosition = new Vector2(CurrentPosX, CurrentPosY);
        _route = FindRoute(startPosition, CurrentMap);
    }
    private RouteNode[] FindRoute(Vector2 prevPos, string map, int depth=0) {
        if (map == TargetMap) {
            var list = new RouteNode[depth+1];
            var node = new RouteNode();
            node.map = TargetMap;
            node.originPos = prevPos;
            node.targetPos = new Vector2(TargetPosX, TargetPosY);
            list[depth] = node;
            return list;
        } 
        else {
            var connections = Database.Get().MapGraph.GetMapConnections(map);
            foreach (var connection in connections) {
                var other = connection.GetOther(map);
                var list = FindRoute(other.Position, other.MapID, depth+1);
                if (list != null) {
                    var node = new RouteNode();
                    node.map = map;
                    node.originPos = prevPos;
                    node.targetPos = connection.Get(map).Position;
                    list[depth] = node;
                    return list;
                }
            }
        }
        //
        return null;
    }
}
