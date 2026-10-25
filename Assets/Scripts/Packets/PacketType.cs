public enum ClientPacket : ushort
{
    Username,
    SelectBall,
    LockDirection,
}

public enum ServerPacket : ushort
{
    OwnID,
    CurrentGameState,
    PlayerConnected,
    PlayerDisconnected,
    SelectableBalls,
    SpawnBall,
    BallPosition,
    BallHealth,
    DamageMarker,
}