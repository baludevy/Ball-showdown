public enum ClientPacket : ushort
{
    Username,
}

public enum ServerPacket : ushort
{
    CurrentGameState,
    PlayerConnected,
    PlayerDisconnected,
}