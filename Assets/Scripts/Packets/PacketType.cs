public enum ClientPacket : ushort
{
    Username,
    SelectBall,
}

public enum ServerPacket : ushort
{
    CurrentGameState,
    PlayerConnected,
    PlayerDisconnected,
    SelectableBalls,
    SpawnBall,
}