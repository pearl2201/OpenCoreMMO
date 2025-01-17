﻿using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseOnItemHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly PlayerUseItemOnCommand playerUseItemOnCommand;

    public PlayerUseOnItemHandler(IGameServer game, PlayerUseItemOnCommand playerUseItemOnCommand)
    {
        this.game = game;
        this.playerUseItemOnCommand = playerUseItemOnCommand;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnPacket(message);
        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            game.Dispatcher.AddEvent(new Event(2000,
                () => playerUseItemOnCommand.Execute(player,
                    useItemOnPacket))); //todo create a const for 2000 expiration time
    }
}