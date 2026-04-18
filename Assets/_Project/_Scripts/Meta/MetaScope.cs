using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Meta.Data.Character.SO;
using Reflex.Core;
using UnityEngine;

namespace _Project._Scripts.Meta
{
    public class MetaScope : MonoBehaviour, IInstaller
    {
        [SerializeField] private CharacterSO[] characters;
        
    
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            IEnumerable<CharacterModel> characterModels = characters.Select(so => new CharacterModel(so));
            PartyModel partyModel = new(characterModels);
            
            containerBuilder.RegisterValue(partyModel);
        }
    }
}
