using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Meta.Data.Character.SO;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character
{
    public class CharacterModel
    {
        public readonly string Name;
        public readonly Sprite Portrait;
        public readonly Sprite PartyIcon;
        public readonly List<AbilityData> Abilities;
        public readonly List<ModificationData> Modifications;

        public readonly ReactiveProperty<int> HP;
        public readonly ReactiveProperty<int> MaxHP;
        public readonly ReactiveProperty<int> Armor;
        public readonly ReactiveProperty<int> MaxArmor;
        public ObservableDictionary<int, int> SelectedModifications;
        
        public CharacterModel(CharacterSO so)
        {
            Name = so.characterName;
            Portrait = so.portrait;
            PartyIcon = so.partyIcon;
            Abilities = so.abilities.Select(a => a.ability).ToList();
            Modifications = so.modifications.Select(m => m.modification).ToList();
            HP = new ReactiveProperty<int>(so.hp);
            MaxHP = new ReactiveProperty<int>(so.hp);
            Armor = new ReactiveProperty<int>(so.armor);
            MaxArmor = new ReactiveProperty<int>(so.armor);
            SelectedModifications = new ObservableDictionary<int, int>();
        }
    }
}