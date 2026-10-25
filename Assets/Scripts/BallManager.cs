using System;
using System.Collections.Generic;
using UnityEngine;



public class BallManager : MonoBehaviour
{
    public List<Ball> spawnedBalls = new List<Ball>();
    
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
        GameObject prefab = BallCatalog.Instance.GetBall(ballType).prefab;

        Ball ball = Instantiate(prefab).GetComponent<Ball>();

        ball.owner = player;
        ball.type = ballType;
        
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
            List<BallSnapshot> ballSnapshots = new List<BallSnapshot>();

            foreach (Ball ball in spawnedBalls)
            {
                ballSnapshots.Add(new BallSnapshot
                {
                    id = ball.owner.id,
                    position = ball.transform.position,
                });
            }
        
            ServerSend.BallPosition(ballSnapshots);
        }
    }

    public void ApplySnapshots(List<BallSnapshot> snapshots)
    {
        foreach (BallSnapshot snapshot in snapshots)
        {
            Ball ball = spawnedBalls.Find(x => x.owner.id == snapshot.id);
            
            ball.transform.position = snapshot.position;
        }
    }

    public void Clear()
    {
        foreach (Ball ball in spawnedBalls)
        {
            Destroy(ball.gameObject);
        }
        
        spawnedBalls.Clear();
    }
}