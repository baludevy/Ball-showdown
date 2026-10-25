using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public static class PacketRouter
{
    public delegate void ServerMessageCallback(NetPeer sender, NetDataReader reader);
    public delegate void ClientMessageCallback(NetDataReader reader);

    private static readonly Dictionary<ushort, ServerMessageCallback> serverCallbacks = new Dictionary<ushort, ServerMessageCallback>();
    private static readonly Dictionary<ushort, ClientMessageCallback> clientCallbacks = new Dictionary<ushort, ClientMessageCallback>();

    private const int HeaderSize = sizeof(ushort);
    
    public static int ServerHandlerCount => serverCallbacks.Count;
    public static int ClientHandlerCount => clientCallbacks.Count;

    public static void RegisterServerPacket(ushort packetId, ServerMessageCallback callback)
    {
        if (callback == null) return;
        serverCallbacks[packetId] = callback;
    }

    public static void RegisterClientPacket(ushort packetId, ClientMessageCallback callback)
    {
        if (callback == null) return;
        clientCallbacks[packetId] = callback;
    }

    public static void ProcessServerMessage(NetPeer sender, NetPacketReader reader)
    {
        if (reader ==  null) return;

        try
        {
            if (!IsPacketValid(reader)) return;   

            ushort packetId = reader.GetUShort();
            if (!TryGetClientHandler(packetId, out var callback)) return;

            callback(reader);
        }
        catch (Exception)
        {
            
        }
        finally
        {
            reader.Recycle();
        }
    }

    public static void ProcessClientMessage(NetPacketReader reader)
    {
        if (reader == null) return;

        try
        {
            if (!IsPacketValid(reader)) return;

            ushort packetId = reader.GetUShort();
            if (!TryGetClientHandler(packetId, out var callback)) return;
        }
        catch (Exception)
        {
            
        }
        finally
        {
            reader.Recycle();
        }
    }

    private static bool IsPacketValid(NetPacketReader reader)
    {
        return reader != null && reader.AvailableBytes >= HeaderSize;
    }

    public static bool TryGetClientHandler(ushort packetId, out ClientMessageCallback callback)
    {
        return clientCallbacks.TryGetValue(packetId, out callback);
    }

    public static bool TryGetServerHandler(ushort packetId, out ServerMessageCallback callback)
    {
        return serverCallbacks.TryGetValue(packetId, out callback);
    }

    public static bool HasServerHandler (ushort packetId)
    {
        return serverCallbacks.ContainsKey(packetId);
    }

    public static bool HasClientHandler(ushort packetId)
    {
        return clientCallbacks.ContainsKey(packetId);
    }

    public static bool UnregisterServerPacket(ushort packetId)
    {
        return serverCallbacks.Remove(packetId);
    }

    public static bool UnregisterClientPacket(ushort packetId)
    {
        return clientCallbacks.Remove(packetId);
    }

    public static void ResetServerHandlers()
    {
        serverCallbacks.Clear();
    }

    public static void ResetClientHandlers()
    {
        clientCallbacks.Clear();
    }

    public static void ResetAllHandlers()
    {
        ResetServerHandlers();
        ResetClientHandlers();
    }
}
