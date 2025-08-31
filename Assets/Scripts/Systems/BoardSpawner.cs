using UnityEngine;
using System.Collections.Generic;

public class BoardSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject boardPrefab;      // /Prefabs/Board.prefab
    public Transform parent;            // e.g., an empty "BoardsRoot" under AppShell
    [Range(0, 3)] public int cpuCount = 2;

    [Header("Runtime (read-only)")]
    public List<GameObject> instances = new();

    void Start()
    {
        SpawnAll();
    }

    // Call this from UI if you want to change CPU count at runtime
    public void RespawnWithCpuCount(int newCpuCount)
    {
        cpuCount = Mathf.Clamp(newCpuCount, 0, 3);
        DespawnAll();
        SpawnAll();
    }

    void SpawnAll()
    {
        if (!boardPrefab) { Debug.LogError("BoardSpawner: boardPrefab not set."); return; }
        if (!parent) parent = transform;

        // Player board
        var player = Instantiate(boardPrefab, parent);
        player.name = "Board_Player";
        player.transform.position = new Vector3(0f, 0f, 0f);
        Register(player, "Player");
        instances.Add(player);

        // CPU boards
        for (int i = 1; i <= cpuCount; i++)
        {
            var cpu = Instantiate(boardPrefab, parent);
            float xStep = 80f;
            cpu.name = $"Board_CPU_{i}";
            cpu.transform.position = new Vector3(i * xStep, 0f, 0f);
            Register(cpu, $"CPU{i}");
            instances.Add(cpu);
        }

        // Show Player view by default
        if (ViewSwitcher.Instance != null)
            ViewSwitcher.Instance.Show("Player");

        var auto = FindFirstObjectByType<ViewButtonsAuto>();
        if (auto) auto.Rebuild();
    }

    void Register(GameObject go, string key)
    {
        // Ensure there's a BoardCameraRegister and give it a unique key
        var reg = go.GetComponent<BoardCameraRegister>();
        if (!reg) reg = go.AddComponent<BoardCameraRegister>();
        reg.Initialize(key);

        // Cameras start disabled; ViewSwitcher turns on the chosen one
        var cam = go.GetComponentInChildren<Camera>(true);
        if (cam) cam.enabled = false;

        // Avoid "Multiple Audio Listeners" warnings
        var listener = cam ? cam.GetComponent<AudioListener>() : null;
        if (listener) listener.enabled = false;
    }

    public void DespawnAll()
    {
        foreach (var go in instances)
            if (go) Destroy(go);
        instances.Clear();
    }

    // Optional helpers for UI wiring:
    public void UI_SetCpuCount(int n) => cpuCount = Mathf.Clamp(n, 0, 3);
    public void UI_Rebuild() { DespawnAll(); SpawnAll(); }
}