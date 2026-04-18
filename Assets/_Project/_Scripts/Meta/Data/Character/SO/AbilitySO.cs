using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character.SO
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Data/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public AbilityData ability;
    }
}