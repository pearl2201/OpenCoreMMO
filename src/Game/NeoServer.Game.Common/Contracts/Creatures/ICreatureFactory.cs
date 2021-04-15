﻿using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureFactory
    {
        IMonster CreateMonster(string name, ISpawnPoint spawn = null);
        INpc CreateNpc(string name, ISpawnPoint spawn = null);
        IPlayer CreatePlayer(IPlayer playerModel);
    }
}
