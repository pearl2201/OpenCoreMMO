﻿using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public sealed class MonsterType : IMonsterType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Race Race { get; set; }
        public uint Experience { get; set; }
        public ushort Speed { get; set; }
        public ushort ManaCost { get; set; }
        public uint MaxHealth { get; set; }
        public IDictionary<LookType, ushort> Look { get; set; }
        public IIntervalChance TargetChance { get; set; }
        public CombatStrategy CombatStrategy { get; set; }
        public IDictionary<CreatureFlagAttribute, byte> Flags { get; set; } = new Dictionary<CreatureFlagAttribute, byte>();
        public CombatAttackOption[] Attacks { get; set; }
        public ushort Armor { get; set; }
        public ushort Defence { get; set; }
        public List<ICombatDefense> Defenses { get; set; }
        public IIntervalChance VoiceConfig { get; set; }
        public string[] Voices { get; set; }
        public ImmutableDictionary<DamageType, sbyte> Immunities { get; set; }
    }
}
