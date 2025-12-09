using UnityEngine;
using UnityEngine.UIElements;
using Unity.Entities;
using Unity.NetCode;

public sealed class SpawnButton : MonoBehaviour
{
    World GetClientWorld()
    {
        foreach (var world in World.All)
            if (world.IsClient()) return world;
        return null;
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("spawn-button").clicked += OnSpawnButtonPressed;
    }

    void OnSpawnButtonPressed()
    {
        var manager = GetClientWorld().EntityManager;
        manager.CreateEntity(typeof(PlayerSpawnRequest));
    }
}
