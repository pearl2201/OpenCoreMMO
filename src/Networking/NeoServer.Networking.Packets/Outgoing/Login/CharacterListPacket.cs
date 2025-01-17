﻿using NeoServer.Data.Model;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Login;

public class CharacterListPacket : OutgoingPacket
{
    private readonly AccountModel _accountModel;
    private readonly string _ipAddress;
    private readonly string _serverName;

    public CharacterListPacket(AccountModel account, string serverName, string ipAddress)
    {
        _accountModel = account;
        _serverName = serverName;
        _ipAddress = ipAddress;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        AddCharList(message);
    }

    private void AddCharList(INetworkMessage message)
    {
        message.AddByte(0x64); //todo charlist
        message.AddByte((byte)_accountModel.Players.Count);

        var ipAddress = ParseIpAddress(_ipAddress);

        foreach (var player in _accountModel.Players)
        {
            if (!string.IsNullOrWhiteSpace(player.World?.Ip)) ipAddress = ParseIpAddress(player.World.Ip);

            message.AddString(player.Name);
            message.AddString(player.World?.Name ?? _serverName ?? string.Empty);

            message.AddByte(ipAddress[0]);
            message.AddByte(ipAddress[1]);
            message.AddByte(ipAddress[2]);
            message.AddByte(ipAddress[3]);

            message.AddUInt16(7172);
        }

        message.AddUInt16((ushort)_accountModel.PremiumTime);
    }

    private static byte[] ParseIpAddress(string ip)
    {
        var localhost = new byte[] { 127, 0, 0, 1 };

        if (string.IsNullOrEmpty(ip)) return localhost;

        var parsedIp = new byte[4];

        var numbers = ip.Split(".");

        if (numbers.Length != 4) return localhost;

        var i = 0;

        foreach (var number in numbers)
        {
            if (!byte.TryParse(numbers[i], out var ipNumber)) return localhost;
            parsedIp[i++] = ipNumber;
        }

        return parsedIp;
    }
}