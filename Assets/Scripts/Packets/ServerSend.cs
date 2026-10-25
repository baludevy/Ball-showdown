public static class ServerSend
{
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
}
