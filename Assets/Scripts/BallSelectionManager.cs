using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSelectionManager : MonoBehaviour
{
    public List<BallEntry> ownSelectableBalls = new List<BallEntry>();

    private Dictionary<ushort, List<BallType>> selectableBallsByPlayer = new Dictionary<ushort, List<BallType>>();

    public static BallSelectionManager Instance;

    public int needsSelection;

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
        selectableBallsByPlayer.Clear();

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

    public void ResetSelections()
    {
        selectableBallsByPlayer.Clear();
        ownSelectableBalls.Clear();
        needsSelection = 0;

        foreach (Player player in GameManager.Instance.players)
            player.selectedBall = null;
    }


    public void HandleBallSelection(ushort playerId, int index)
    {
        Player player = GameManager.Instance.GetPlayer(playerId);
        if (player == null || player.selectedBall != null)
            return;

        BallEntry ball;

        if (playerId == 0)
        {
            if (index < 0 || index >= ownSelectableBalls.Count)
                return;

            ball = ownSelectableBalls[index];
            Debug.Log($"Host chose {ball.name}");

            UIManager.Instance.SwitchToUI(UIState.Waiting);
        }
        else
        {
            if (!selectableBallsByPlayer.TryGetValue(playerId, out List<BallType> choices) ||
                index < 0 || index >= choices.Count)
                return;

            ball = BallCatalog.Instance.GetBall(choices[index]);
        }

        if (ball == null)
            return;

        if (playerId != 0)
            Debug.Log($"Player{playerId} chose {ball.name}");


        player.selectedBall = ball;

        needsSelection--;

        if (needsSelection == 0)
        {
            foreach (Player p in GameManager.Instance.players)
            {
                if (p.selectedBall != null)
                {
                    BallManager.Instance.SpawnBall(p.id, p.selectedBall.type);
                }
            }

            GameManager.Instance.ChangeGameState(GameState.DirectionChoose);
        }

    }
}
