﻿using System.Linq;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Outgoing.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Chat
{
    public class PlayerChannelListRequestHandler : PacketHandler
    {
        private readonly IGameServer _game;
        private readonly IChatChannelStore _chatChannelStore;

        public PlayerChannelListRequestHandler(IGameServer game, IChatChannelStore chatChannelStore)
        {
            _game = game;
            _chatChannelStore = chatChannelStore;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var channels = _chatChannelStore.All.Where(x => x.PlayerCanJoin(player));
            channels = player.PersonalChannels is null ? channels : channels.Concat(player.PersonalChannels);
            channels = player.PrivateChannels is null ? channels : channels.Concat(player.PrivateChannels);

            connection.OutgoingPackets.Enqueue(new PlayerChannelListPacket(channels.ToArray()));
            connection.Send();
        }
    }
}