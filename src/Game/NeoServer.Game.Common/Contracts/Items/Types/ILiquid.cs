﻿using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface ILiquid : IDecayable, IItem
{
    bool IsLiquidPool { get; }

    bool IsLiquidSource { get; }

    bool IsLiquidContainer { get; }
    LiquidColor LiquidColor { get; }
}