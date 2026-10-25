using UnityEngine;

public static class ClientSend
{
    public static void Username(string username)
    {
        NetworkManager.Instance.Client.SendPacket(ClientPacket.Username, writer =>
        {
            writer.Put(username);
        });
    }

    public static void SelectBall(int index)
    {
        NetworkManager.Instance.Client.SendPacket(ClientPacket.SelectBall, writer =>
        {
            writer.Put((byte)index);
        });
    }
    
    public static void LockDirection(Vector2 direction)
    {
        NetworkManager.Instance.Client.SendPacket(ClientPacket.LockDirection, writer =>
        {
            writer.Put(direction);
        });
    }
}
