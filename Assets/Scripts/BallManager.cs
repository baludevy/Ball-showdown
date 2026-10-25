using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BallManager : MonoBehaviour
{
    public List<Ball> spawnedBalls = new List<Ball>();

    public uint tick;

    public static BallManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnBall(ushort ownerId, BallType ballType)
    {
        Player player = GameManager.Instance.GetPlayer(ownerId);
        BallEntry ballEntry = BallCatalog.Instance.GetBall(ballType);

        if (player == null || ballEntry == null || ballEntry.prefab == null)
        {
            Debug.LogWarning($"Could not spawn {ballType} for player {ownerId}.");
            return;
        }

        if (GetBall(ownerId) != null)
            return;

        GameObject prefab = ballEntry.prefab;

        Ball ball = Instantiate(prefab).GetComponent<Ball>();

        ball.owner = player;
        ball.type = ballType;

        ball.Initialize();

        spawnedBalls.Add(ball);

        if (NetworkManager.Instance.isHost)
        {
            ServerSend.SpawnBall(ownerId, ballType);
        }
    }

    private void FixedUpdate()
    {
        if (NetworkManager.Instance.isHost)
        {
            List<BallUpdate> ballUpdates = new List<BallUpdate>();

            foreach (Ball ball in spawnedBalls)
            {
                ballUpdates.Add(new BallUpdate
                {
                    id = ball.owner.id,
                    scale = ball.transform.localScale,
                    position = ball.transform.position,
                });
            }

            ServerSend.BallPosition(tick, ballUpdates);
        }

        tick++;
    }

    public void ApplyUpdates(uint tick, List<BallUpdate> ballUpdates)
    {
        foreach (BallUpdate update in ballUpdates)
        {
            Ball ball = spawnedBalls.Find(x => x.owner.id == update.id);

            if (ball == null)
                continue;
            
            ball.transform.localScale = update.scale;

            BallSnapshot snapshot = new BallSnapshot
            {
                position = update.position
            };

            BallInterpolationManager.Instance.AddSnapshot(
                ball,
                tick,
                snapshot
            );
        }
    }

    public void DestroyBall(ushort id)
    {
        Ball ball = GetBall(id);

        BallInterpolationManager.Instance?.RemoveBall(id);

        if (ball == null)
            return;

        Destroy(ball.gameObject);
        spawnedBalls.Remove(ball);

        if (spawnedBalls.Count <= 1)
        {
            Ball winner = spawnedBalls.First();
            
            ServerSend.PlayerWon(winner.owner.id);

            if (NetworkManager.Instance.isHost)
            {
                UIManager.Instance.lobbyUI.PlayerWon(winner.owner.id);
            }
            
            GameManager.Instance.ChangeGameState(GameState.Lobby);
        }
    }

    public Ball GetBall(ushort id) => spawnedBalls.Find(x => x.owner.id == id);
    public Ball GetMyBall() => spawnedBalls.Find(x => x.owner.id == NetworkManager.Instance.myId);

    public void Clear()
    {
        foreach (Ball ball in spawnedBalls)
        {
            Destroy(ball.gameObject);
        }

        spawnedBalls.Clear();
        BallInterpolationManager.Instance?.ClearSnapshots();
        tick = 0;
    }
}
