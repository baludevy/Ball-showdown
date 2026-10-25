using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EntityInterpolationManager : MonoBehaviour
{
    public static EntityInterpolationManager Instance;

    private readonly Dictionary<ushort, SnapshotBuffer> buffers = new();

    private bool hasClock;
    private uint firstTick;
    private uint latestTick;
    private float clockOffset;

    private const float worldUpdateInterval = 0.02f;
    private const int interpolationTicks = 1;
    private const float interpolationSeconds = interpolationTicks * worldUpdateInterval;

    private const int maxSnapshots = 32;
    private const float clockCorrectionSpeed = 0.01f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!hasClock)
            return;

        float renderTime = GetRenderTime();

        foreach (SnapshotBuffer buffer in buffers.Values)
        {
            if (buffer == null)
                continue;

            buffer.UpdatePosition(renderTime);
        }
    }

    private float GetRenderTime()
    {
        float currentTime = Time.time - clockOffset;
        return currentTime - interpolationSeconds;
    }

    public void AddSnapshot(Entity entity, uint tick, EntitySnapshot snapshot)
    {
        if (entity == null || snapshot == null)
            return;
        
        if (!hasClock)
            InitializeClock(tick);

        snapshot.tick = tick;
        snapshot.time = CalculateSnapshotTime(tick);
    }

    private void InitializeClock(uint tick)
    {
        hasClock = true;
        firstTick = tick;
        latestTick = tick;
        clockOffset = Time.time;
    }

    private float CalculateSnapshotTime(uint tick)
    {
        return (tick - firstTick) * worldUpdateInterval;
    }

    private void SynchronizeClock(uint tick, float snapshotTime)
    {
        latestTick = tick;

        float expectedOffset = Time.time - snapshotTime;

        clockOffset = Mathf.Lerp(
            clockOffset,
            expectedOffset,
            clockCorrectionSpeed
        );
    }

    private SnapshotBuffer GetOrCreateBuffer(Entity entity)
    {
        if (buffers.TryGetValue(entity.entityId, out SnapshotBuffer buffer))
            return buffer;

        buffer = new SnapshotBuffer(entity, maxSnapshots);
        buffers.Add(entity.entityId, buffer);

        return buffer;
    }

    public void RemoveEntity(ushort entityId)
    {
        if (!buffers.ContainsKey(entityId))
            return;

        buffers.Remove(entityId);
    }

    public void ClearSnapshots()
    {
        buffers.Clear();
    }

    private void ResetClock()
    {
        hasClock= false;
        firstTick = 0;
        latestTick = 0;
        clockOffset = 0f;
    }

    private void ResetInterpolation()
    {
        ClearSnapshots();
        ResetClock();
    }

    private void OnDestroy()
    {
        ResetInterpolation();

        if (Instance == this)
            Instance = null;
    }

    private class SnapshotBuffer
    {
        private readonly Entity entity;
        private readonly List<EntitySnapshot> snapshots = new();

        private readonly int capacity;

        public SnapshotBuffer(Entity entity, int capacity)
        {
            this.entity = entity;
            this.capacity = capacity;
        }

        public void AddSnapshot(EntitySnapshot snapshot)
        {
            if (snapshot == null)
                return;

            if (!IsValidSnapshot(snapshot))
                return;
        }

        private bool IsValidSnapshot(EntitySnapshot snapshot)
        {
            if (snapshots.Count == 0)
                return true;

            EntitySnapshot previous = snapshots[snapshots.Count - 1];

            if (snapshot.tick <= previous.tick)
                return false;

            return true;
        }

        private void TrimSnapshots()
        {
            while (snapshots.Count > capacity)
                snapshots.RemoveAt(0);
        }

        public void UpdatePosition(float renderTime)
        {
            if (entity == null)
                return;
            
            if (snapshots.Count == 0)
                return;
            
            if (snapshots.Count == 1)
            {
                ApplySingleSnapshot();
                return;
            }

            RemoveOldSnapshots(renderTime);

            InterpolateSnapshots(renderTime);
        }

        private void RemoveOldSnapshots(float renderTime)
        {
            while (snapshots.Count > 2)
            {
                if (snapshots[1].time > renderTime)
                    break;

                snapshots.RemoveAt(0);
            }
        }

        private void InterpolateSnapshots(float renderTime)
        {
            EntitySnapshot previous = snapshots[0];
            EntitySnapshot next = snapshots[1];

            float duration = next.time - previous.time;

            if (duration <= 0f)
                return;

            float elapsed = renderTime - previous.time;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 targetPosition = Vector3.Lerp(
                previous.position,
                next.position,
                t
            );

            ApplyInterpolatedState(previous, next, targetPosition, t);
        }

        private void ApplyInterpolatedState(
            EntitySnapshot previous,
            EntitySnapshot next,
            Vector3 position,
            float t)
        {
            entity.transform.position = position;

            entity.ApplySnapshot(previous, next, t);
        }
        
        private void ApplySingleSnapshot()
        {
            EntitySnapshot snapshot = snapshots[0];

            entity.transform.position = snapshot.position;
            entity.ApplySnapshot(snapshot, snapshot, 1f);
        }
    }
}

public class EntitySnapshot
{
    public uint tick;
    public float time;
    public Vector3 position;
}