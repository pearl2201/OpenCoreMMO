﻿using System;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems;

public class FloorChangerUsableItem : UsableOnItem
{
    public FloorChangerUsableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public override bool AllowUseOnDistance => false;

    public override bool Use(ICreature usedBy, IItem onItem)
    {
        if (usedBy is not IPlayer player) return false;
        var canUseOnItems = Metadata.OnUse?.GetAttributeArray<ushort>(ItemAttribute.UseOn) ?? Array.Empty<ushort>();

        if (!canUseOnItems.Contains(onItem.Metadata.TypeId)) return false;

        if (Metadata.OnUse?.GetAttribute(ItemAttribute.FloorChange) == "up")
        {
            var toLocation = new Location(onItem.Location.X, onItem.Location.Y, (byte)(onItem.Location.Z - 1));

            player.TeleportTo(toLocation);
            return true;
        }

        return false;
    }

    public new static bool IsApplicable(IItemType type)
    {
        return UsableOnItem.IsApplicable(type) &&
               (type.OnUse?.HasAttribute(ItemAttribute.FloorChange) ?? false);
    }
}