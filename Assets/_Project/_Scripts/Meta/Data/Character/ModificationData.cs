using System;
using _Project._Scripts.Meta.Data.Character.Enums;
using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character
{
    [Serializable]
    public class ModificationData
    {
        public string name;
        [SerializeField] 
        private ModificationType modificationType;
        public Sprite background;
        public Sprite icon;
        
        public ModificationFlags ModificationType => (ModificationFlags)modificationType;
    }
}