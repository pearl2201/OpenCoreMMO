﻿using NeoServer.Data.Interfaces;
using NeoServer.Loaders.Interfaces;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerAddVipHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IAccountRepository accountRepository;
        private readonly IEnumerable<IPlayerLoader> playerLoaders;
        public PlayerAddVipHandler(Game game, IAccountRepository accountRepository, IEnumerable<IPlayerLoader> playerLoaders)
        {
            this.game = game;
            this.accountRepository = accountRepository;
            this.playerLoaders = playerLoaders;
        }
        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var addVipPacket = new AddVipPacket(message);

            if (addVipPacket.Name?.Length > 20) return;
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;


            if (!game.CreatureManager.TryGetPlayer(addVipPacket.Name, out var vipPlayer))
            {
                var playerRecord = await accountRepository.GetPlayer(addVipPacket.Name);
                if (playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not IPlayerLoader playerLoader) return;

                vipPlayer = playerLoader.Load(playerRecord);
            }
            

            if (vipPlayer is null)
            {
                connection.Send(new TextMessagePacket("A player with this name does not exist.", TextMessageOutgoingType.Small));
                return;
            }

            //todo: check if player can be added to vip list

            game.Dispatcher.AddEvent(new Event(() => player.AddToVip(vipPlayer)));
        }
    }
}

