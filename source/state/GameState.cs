using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Compression;
using Godot;
using Newtonsoft.Json;

public partial class GameState {
    public class Container {
        public List<PlaceableObject.State> Items;
        public int MaxItems = 8;
    }
    [System.Serializable]
    public class State {
        public int Money = 100;
        public int DaysElapsed = 0;
        public int Stamina = 100;
        public int Hour = 6;
        public int Minute = 0;
        public float Second = 0;
        public string CurrentMapName = "";
        public PlaceableObject.State ItemInHands;
        public Dictionary<string,Container> Containers = new Dictionary<string, Container>();
        public Dictionary<string,Map.State> MapStates = new Dictionary<string, Map.State>();
    }
    private State state;
    private Map currentMap;
    public Map Map => currentMap;
    public string SaveFileName = "savegame";
    public string SaveFilePath => "user://"+SaveFileName+".sav";
    const string ENCRYPTION_KEY = "12345678901234567890123456789012";
    public GameState() {
        state = new State();
        AddContainer("backpack", 2);
    }
    public Map.State GetMapState(string name) {
        if (state.MapStates.TryGetValue(name, out var map)) {
            return map;
        }
        return null;
    }
    public Map.State GetCurrentMapState() {
        return GetMapState(state.CurrentMapName);
    }
    public void AddContainer(string containerId, int maxItems) {
        if (!state.Containers.TryGetValue(containerId, out var v)) {
            var container = new Container();
            container.Items = new List<PlaceableObject.State>();
            container.MaxItems = maxItems;
            state.Containers.Add(containerId, container);
        }
    }
    public bool AddItemToContainer(string containerId, PlaceableObject.State item) {
        if (state.Containers.TryGetValue(containerId, out var c)) {
            if (c.Items.Count >= c.MaxItems) return false;
            c.Items.Add(item);
        }
        return false;
    }
    public string PopMapName() {
        var mn = state.CurrentMapName;
        state.CurrentMapName = "";
        return mn;
    }
    public async void ChangeMap(string newMapName) {
        if (newMapName == state.CurrentMapName) return;
        // Hide screen.
        Main.Instance.Loader.LoadScene(newMapName);
        await Main.Instance.Loader.ToSignal(Main.Instance.Loader, Loader.SignalName.OnShowFinished);
        // Clear current map.
        if (currentMap != null) {
            // Save map state.
            state.MapStates[state.CurrentMapName] = currentMap.Serialize();
            // Deinstantiate current map.
            currentMap.QueueFree();
            currentMap = null;
        }
        // Instantiate new map.
        var awaiter = await Main.Instance.Loader.ToSignal(Main.Instance.Loader, Loader.SignalName.OnLoadFinished);
        var newMapScene = awaiter[1].As<PackedScene>();
        //var newMapScene = ResourceLoader.Load<PackedScene>(newMapName);
        if (newMapScene != null) {
            state.CurrentMapName = newMapName;
            currentMap = newMapScene.Instantiate<Map>();
            Main.Instance.WorldRoot.AddChild(currentMap);
        }
        // Wait for a bit.
        var t = Main.Instance.GetTree().CreateTimer(0.1);
        await Main.Instance.ToSignal(t, Timer.SignalName.Timeout);
        // Show screen.
        Main.Instance.Loader.HideLoader();
    }
    public void Save() {
        // Serialize current map just in case.
        state.MapStates[state.CurrentMapName] = currentMap.Serialize();
        // Open file.
        var ek = ENCRYPTION_KEY.ToUtf8Buffer();
        var saveFile = FileAccess.OpenEncrypted(SaveFilePath, FileAccess.ModeFlags.Write, ek);
        // Json the crap of the dude.
        string json = JsonConvert.SerializeObject(state);
        GD.Print(json);
        // Then compress!
        var gzip = new StreamPeerGZip();
        gzip.StartCompression();
        gzip.PutData(json.ToUtf8Buffer());
        gzip.Finish();
        var compressed = gzip.GetData(gzip.GetAvailableBytes())[1].AsByteArray();
        saveFile.StoreBuffer(compressed);
        saveFile.Close();
    }
    public void Load() {
        // Open file.
        var ek = ENCRYPTION_KEY.ToUtf8Buffer();
        var saveFile = FileAccess.OpenEncrypted(SaveFilePath, FileAccess.ModeFlags.Read, ek);
        if (saveFile == null) return;
        // Decompress
        var data = saveFile.GetBuffer((long)saveFile.GetLength());
        var gzip = new StreamPeerGZip();
        gzip.StartDecompression();
        gzip.PutData(data);
        //gzip.Finish();
        string json = gzip.GetUtf8String(gzip.GetAvailableBytes());
        GD.Print(json);
        // Deserialize.
        state = JsonConvert.DeserializeObject<State>(json);
        saveFile.Close();
        GD.Print("Loading finished!");
    }

    public void SetHeldItem(PlaceableObject.State item)
    {
        state.ItemInHands = item;
    }
    public PlaceableObject.State GetHeldItem() {
        return state.ItemInHands;
    }
    //
}