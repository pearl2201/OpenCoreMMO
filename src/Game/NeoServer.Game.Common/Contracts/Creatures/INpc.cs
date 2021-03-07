﻿using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate string ReplaceKeyword(string message, object replace);
    public delegate void Answer(INpc from, ICreature to, IDialog dialog, string message, SpeechType type);
    public delegate void DialogAction(INpc from, ICreature to, IDialog dialog, string action, Dictionary<string,string> lastKeywords);
    
    public delegate IItem CreateItem(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
    public interface INpc : IWalkableCreature, ISociableCreature
    {
        INpcType Metadata { get; }
        event Answer OnAnswer;
        event DialogAction OnDialogAction;

        void BackInDialog(ISociableCreature creature, byte count);
        Dictionary<string, string> GetPlayerStoredValues(ISociableCreature sociableCreature);
        void StopTalkingToCustomer(IPlayer player);
    }
    
}
