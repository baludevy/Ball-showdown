using System;
using System.Collections.Generic;
using UnityEngine;

public class BallInterpolationManager : MonoBehaviour
{
    public static BallInterpolationManager Instance;
    
    private const double WorldUpdateInterval = 0.02;
    private const int MaxSnapshots = 32;

    [SerializeField, Min(1)] private int interpolationTicks = 3;

    private readonly Dictionary<ushort, SnapshotBuffer> buffers =
        new Dictionary<ushort, SnapshotBuffer>();

    private bool hasClock;
    private uint latestTick;
    private double latestSnapshotTime;
    private double renderTime;
    private double lastLocalTime;

    private double InterpolationDelay =>
        Math.Max(1, interpolationTicks) * WorldUpdateInterval;

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

        double now = Time.unscaledTimeAsDouble;
        double deltaTime = Math.Max(0.0, now - lastLocalTime);
        lastLocalTime = now;

        double delay = InterpolationDelay;
        double bufferedTime = latestSnapshotTime - renderTime;

        if (bufferedTime > Math.Max(0.5, delay * 4.0))
        {
            renderTime = latestSnapshotTime - delay;
            bufferedTime = delay;
        }
        
        double tolerance = WorldUpdateInterval * 0.5;
        double playbackSpeed = 1.0;

        if (bufferedTime > delay + tolerance)
            playbackSpeed = 1.05;
        else if (bufferedTime < delay - tolerance)
            playbackSpeed = 0.95;

        renderTime = Math.Min(
            renderTime + deltaTime * playbackSpeed,
            latestSnapshotTime);

        foreach (SnapshotBuffer buffer in buffers.Values)
            buffer.UpdatePosition(renderTime);
    }

    public void AddSnapshot(Ball interpolatedBall, uint tick, BallSnapshot snapshot)
    {
        if (interpolatedBall == null || snapshot == null)
            return;

        double snapshotTime;

        if (!hasClock)
        {
            hasClock = true;
            latestTick = tick;
            latestSnapshotTime = 0.0;
            renderTime = -InterpolationDelay;
            lastLocalTime = Time.unscaledTimeAsDouble;
            snapshotTime = 0.0;
        }
        else
        {
            int tickDelta = unchecked((int)(tick - latestTick));
            snapshotTime = latestSnapshotTime + tickDelta * WorldUpdateInterval;

            if (tickDelta > 0)
            {
                latestTick = tick;
                latestSnapshotTime = snapshotTime;
            }
        }

        ushort id = interpolatedBall.owner.id;

        if (!buffers.TryGetValue(id, out SnapshotBuffer buffer) ||
            !buffer.IsFor(interpolatedBall))
        {
            buffer = new SnapshotBuffer(interpolatedBall);
            buffers[id] = buffer;
        }
        
        buffer.AddSnapshot(new BallSnapshot
        {
            tick = tick,
            time = snapshotTime,
            position = snapshot.position
        });
    }

    public void RemoveBall(ushort ballId)
    {
        buffers.Remove(ballId);
    }
    
    public void ClearSnapshots()
    {
        buffers.Clear();
        hasClock = false;
        latestTick = 0;
        latestSnapshotTime = 0.0;
        renderTime = 0.0;
        lastLocalTime = 0.0;
    }

    private void OnDestroy()
    {
        ClearSnapshots();

        if (Instance == this)
            Instance = null;
    }

    private sealed class SnapshotBuffer
    {
        private readonly Ball ball;
        private readonly List<BallSnapshot> snapshots =
            new List<BallSnapshot>(MaxSnapshots);

        private double lastAppliedTime = double.NegativeInfinity;

        public SnapshotBuffer(Ball ball)
        {
            this.ball = ball;
        }

        public bool IsFor(Ball candidate) => ball == candidate;

        public void AddSnapshot(BallSnapshot snapshot)
        {
            if (snapshot.time <= lastAppliedTime)
                return;

            int index = snapshots.Count;
            while (index > 0 && snapshots[index - 1].time > snapshot.time)
                index--;

            if (index > 0 && snapshots[index - 1].tick == snapshot.tick)
                return;

            snapshots.Insert(index, snapshot);

            if (snapshots.Count > MaxSnapshots)
                snapshots.RemoveAt(0);
        }

        public void UpdatePosition(double time)
        {
            if (ball == null || snapshots.Count == 0)
                return;

            time = Math.Max(time, lastAppliedTime);

            while (snapshots.Count > 2 && snapshots[1].time <= time)
                snapshots.RemoveAt(0);

            BallSnapshot previous = snapshots[0];

            if (snapshots.Count == 1 || time <= previous.time)
            {
                Apply(previous, previous, 1f);
                lastAppliedTime = previous.time;
                return;
            }

            BallSnapshot next = snapshots[1];
            double duration = next.time - previous.time;
            float t = duration > 0.0
                ? Mathf.Clamp01((float)((time - previous.time) / duration))
                : 1f;

            Apply(previous, next, t);
            lastAppliedTime = Math.Min(time, next.time);
        }

        private void Apply(BallSnapshot previous, BallSnapshot next, float t)
        {
            ball.transform.position = Vector3.Lerp(
                previous.position, next.position, t);

            ball.ApplySnapshot(previous, next, t);
        }
    }
}

public class BallSnapshot
{
    public uint tick;
    public double time;
    public Vector3 position;
}
