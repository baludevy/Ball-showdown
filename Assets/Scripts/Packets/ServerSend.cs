using System.Collections.Generic;
using LiteNetLib;

public static class ServerSend
{
    public static void OwnID(int id)
    {
        NetworkManager.Instance.Server.SendPacketTo(ServerPacket.OwnID, id, writer =>
        {
            writer.Put(id);
        });
    }
    
    public static void CurrentGameState(int toId, GameState gameState)
    {
        NetworkManager.Instance.Server.SendPacketTo(ServerPacket.CurrentGameState, toId, writer =>
        {
            writer.Put((ushort)gameState);
        });
    }

    public static void CurrentGameState(GameState gameState)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.CurrentGameState, writer =>
        {
            writer.Put((ushort)gameState);
        });
    }

    public static void PlayerConnected(Player player)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.PlayerConnected, writer =>
        {
            writer.Put(player.id);
            writer.Put(player.username);
        });
    }

    public static void PlayerConnected(int toId, Player player)
    {
        NetworkManager.Instance.Server.SendPacketTo(ServerPacket.PlayerConnected, toId, writer =>
        {
            writer.Put(player.id);
            writer.Put(player.username);
        });
    }

    public static void PlayerDisconnected(ushort id)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.PlayerDisconnected, writer =>
        {
            writer.Put(id);
        });
    }

    public static void SelectableBalls(ushort toId, List<BallType> balls)
    {
        NetworkManager.Instance.Server.SendPacketTo(ServerPacket.SelectableBalls, toId, writer =>
        {
            writer.Put(balls.Count);

            foreach (BallType ball in balls)
            {
                writer.Put((byte)ball);
            }
        });
    }

    public static void SpawnBall(ushort ownerId, BallType ball)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.SpawnBall, writer =>
        {
            writer.Put(ownerId);
            writer.Put((byte)ball);
        });
    }

    public static void BallPosition(uint tick, List<BallUpdate> ballUpdates)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.BallPosition, writer =>
        {
            writer.Put(ballUpdates.Count);
            writer.Put(tick);
            
            foreach (BallUpdate update in ballUpdates)
            {
                writer.Put(update.id);
                writer.Put(update.scale);
                writer.Put(update.position);
            }
        }, DeliveryMethod.Unreliable);
    }

    public static void BallHealth(ushort id, int health)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.BallHealth, writer =>
        {
            writer.Put(id);
            writer.Put(health);
        });
    }
    
    public static void DamageMarker(ushort id, int health)
    {
        NetworkManager.Instance.Server.SendPacketToAll(ServerPacket.DamageMarker, writer =>
        {
            writer.Put(id);
            writer.Put(health);
        });
    }
}
