using UnityEngine;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.Data
{
    [CreateAssetMenu(fileName = "StyleSheetRepository", menuName = "Repositories/StyleSheetRepository")]
    public class StyleSheetRepository : ScriptableObject
    {
        public StyleSheet global;
        [Space]
        public StyleSheet meta;
        [Space]
        public StyleSheet menu;
        public StyleSheet character;
    }
}