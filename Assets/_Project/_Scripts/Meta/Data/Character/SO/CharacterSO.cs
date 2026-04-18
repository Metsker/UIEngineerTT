using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character.SO
{
    [CreateAssetMenu(fileName = "Character", menuName = "Data/Character")]
    public class CharacterSO : ScriptableObject
    {
        public string characterName;
        public Sprite portrait;
        public Sprite partyIcon;
        public int hp;
        public int armor;
        public AbilitySO[] abilities;
        public ModificationSO[] modifications;
    }
}