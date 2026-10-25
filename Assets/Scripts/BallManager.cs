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
}