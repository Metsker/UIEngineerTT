using System;
using _Project._Scripts.Meta.Data.Character.Enums;
using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character
{
    [Serializable]
    public class AbilityData
    {
        public string name;
        public Sprite icon;
        public ModificationFlags compatibleModifications;
    }
}