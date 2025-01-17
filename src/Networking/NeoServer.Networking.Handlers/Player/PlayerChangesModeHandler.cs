﻿using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerChangesModeHandler : PacketHandler
{
    private readonly IGameServer game;

    public PlayerChangesModeHandler(IGameServer game)
    {
        this.game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var changeMode = new ChangeModePacket(message);

        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        game.Dispatcher.AddEvent(new Event(() =>
        {
            player.ChangeFightMode(changeMode.FightMode);
            player.ChangeChaseMode(changeMode.ChaseMode);
            player.ChangeSecureMode(changeMode.SecureMode);
        }));
    }
}