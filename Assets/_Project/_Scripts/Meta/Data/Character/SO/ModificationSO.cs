using UnityEngine;

namespace _Project._Scripts.Meta.Data.Character.SO
{
    [CreateAssetMenu(fileName = "Modification", menuName = "Data/Modification")]
    public class ModificationSO : ScriptableObject
    {
        public ModificationData modification;
    }
}