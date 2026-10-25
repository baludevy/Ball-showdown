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

    public void HandleBallSelection(int playerId, int index)
    {
        if (NetworkManager.Instance.isHost)
        {
            if (playerId == 0)
            {
                BallEntry ball = ownSelectableBalls[index];
                
                Debug.Log($"Host chose {ball.name}");
            }
            else
            {
                BallEntry ball = BallCatalog.Instance.GetBall(selectableBallsByPlayer[(ushort)playerId][index]);
                
                
                Debug.Log($"Player{playerId} chose {ball.name}");
            }
        }
    }
}
