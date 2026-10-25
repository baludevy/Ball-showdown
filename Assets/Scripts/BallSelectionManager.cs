using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSelectionManager : MonoBehaviour
{
    public List<BallEntry> ownSelectableBalls = new List<BallEntry>();

    private Dictionary<ushort, List<BallType>> selectableBallsByPlayer = new Dictionary<ushort, List<BallType>>();

    public static BallSelectionManager Instance;

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

    public List<BallType> GetSelectableBalls()
    {
        List<BallEntry> balls = BallCatalog.Instance.balls;

        List<BallType> selectableBalls = balls
            .OrderBy(_ => Random.value)
            .Select(ball => ball.type)
            .Distinct()
            .Take(3)
            .ToList();

        return selectableBalls;
    }

    public void SendSelectableBalls()
    {
        foreach (Player player in GameManager.Instance.players)
        {
            if (player.id == 0 && NetworkManager.Instance.isHost)
            {
                List<BallType> ownBallTypes = GetSelectableBalls();
                ownSelectableBalls.Clear();

                foreach (BallType ballType in ownBallTypes)
                {
                    BallEntry ball = BallCatalog.Instance.GetBall(ballType);
                    ownSelectableBalls.Add(ball);
                }

                UIManager.Instance.ballSelectionUI.DisplayBallCards();

                continue;
            }

            List<BallType> ballTypes = GetSelectableBalls();
            selectableBallsByPlayer[player.id] = ballTypes;
            
            ServerSend.SelectableBalls(player.id, ballTypes);
        }
    }

    public void HandleBallSelection(ushort playerId, int index)
    {
        if (NetworkManager.Instance.isHost)
        {
            BallEntry ball;
            
            if (playerId == 0)
            {
                ball = ownSelectableBalls[index];
                Debug.Log($"Host chose {ball.name}");
            }
            else
            {
                ball = BallCatalog.Instance.GetBall(selectableBallsByPlayer[playerId][index]);
                Debug.Log($"Player{playerId} chose {ball.name}");
            }
            
            Player player = GameManager.Instance.GetPlayer(playerId);
            player.selectedBall = ball;
            
            BallManager.Instance.SpawnBall(player.id, ball.type);
        }
        
        UIManager.Instance.SwitchToUI(UIState.Arena);
    }
}
