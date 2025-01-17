﻿using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Items.Types.Containers;

public delegate void RemoveItem(IContainer fromContainer, byte slotIndex, IItem item, byte amountRemoved);

public delegate void AddItem(IItem item, IContainer container);

public delegate void UpdateItem(IContainer onContainer,byte slotIndex, IItem item, sbyte amount);

public delegate void Move(IContainer container);

public interface IContainer : IInventoryEquipment, IHasItem
{
    IItem this[int index] { get; }

    /// <summary>
    ///     Items on container
    /// </summary>
    List<IItem> Items { get; }

    /// <summary>
    ///     Container's capacity. It indicates how many slots are available on container
    /// </summary>
    byte Capacity { get; }

    /// <summary>
    ///     Indicates if container has any parent
    /// </summary>
    bool HasParent { get; }

    byte SlotsUsed { get; }
    IThing Parent { get; }
    bool IsFull { get; }
    bool HasItems { get; }
    IThing RootParent { get; }

    /// <summary>
    ///     A map of all items in container and their total amount
    /// </summary>
    IDictionary<ushort, uint> Map { get; }

    /// <summary>
    ///     Number of free slots of this and inner containers
    /// </summary>
    uint TotalOfFreeSlots { get; }

    new string InspectionText => $"(Vol:{Capacity})";
    new string CloseInspectionText => InspectionText;

    event RemoveItem OnItemRemoved;
    event AddItem OnItemAdded;
    event UpdateItem OnItemUpdated;
    event Move OnContainerMoved;

    bool GetContainerAt(byte index, out IContainer container);
    void SetParent(IThing thing);

    void Clear();
    void UpdateId(byte id);
    void RemoveId();

    /// <summary>
    ///     Remove item on container
    /// </summary>
    /// <param name="itemToRemove"></param>
    /// <param name="amount"></param>
    void RemoveItem(IItemType itemToRemove, byte amount);

    Result<OperationResultList<IItem>> AddItem(IItem item, bool addToAnyChild);
    void RemoveItem(IItem item, byte amount);
    (IItem, IContainer, byte) GetFirstItem(ushort clientId);
    void ClosedBy(IPlayer player);
    void Use(IPlayer usedBy, byte openAtIndex);
}