public static class ClientSend
{
    public static void Username(string username)
    {
        NetworkManager.Instance.Client.SendPacket(ClientPacket.Username, writer =>
        {
            writer.Put(username);
        });
    }
}
