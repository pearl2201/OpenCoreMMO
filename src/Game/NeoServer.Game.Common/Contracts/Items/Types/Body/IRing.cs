﻿using System.Collections.Generic;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IRing : IChargeable, IBodyEquipmentItem, IDecayable, IProtectionItem, ISkillBonus
    {
        bool Expired { get; }
        byte Defense { get; }
        Dictionary<DamageType, byte> DamageProtection { get; }
    }
}