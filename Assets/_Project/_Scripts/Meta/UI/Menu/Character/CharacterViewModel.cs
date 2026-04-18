using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Shared.UI.MVVM;
using JetBrains.Annotations;
using R3;
using Unity.Properties;
using UnityEngine;

namespace _Project._Scripts.Meta.UI.Menu.Character
{
    [UsedImplicitly]
    public class CharacterViewModel : ViewModel
    {
        [CreateProperty] public string Name => SelectedCharacter.Name;

        [CreateProperty] public Sprite Portrait => SelectedCharacter.Portrait;

        [CreateProperty] public string Hp => $"{SelectedCharacter.HP.Value}/{SelectedCharacter.MaxHP.Value}";

        [CreateProperty] public string Armor => $"{SelectedCharacter.Armor.Value}/{SelectedCharacter.MaxArmor.Value}";

        
        [CreateProperty]
        public IReadOnlyList<string> AbilityNames => SelectedCharacter.Abilities.Select(a => a.name).ToList();

        [CreateProperty]
        public IReadOnlyList<Sprite> AbilityIcons => SelectedCharacter.Abilities.Select(a => a.icon).ToList();
        
        
        [CreateProperty]
        public IReadOnlyList<Sprite> ModificationIcons => SelectedCharacter.Modifications.Select(m => m.icon).ToList();
        
        [CreateProperty]
        public IReadOnlyList<Sprite> ModificationIconBgs => SelectedCharacter.Modifications.Select(m => m.background).ToList();
        
        [CreateProperty]
        public IReadOnlyList<string> ModificationNames => SelectedCharacter.Modifications.Select(m => m.name).ToList();
        
        [CreateProperty]
        public IReadOnlyList<string> ModificationSubLabels =>
            SelectedCharacter.Modifications.Select(m => m.ModificationType.ToString().ToUpper()).ToList();

        [CreateProperty]
        public IReadOnlyList<bool> SelectedModifications =>
            Modifications.Select(m => SelectedCharacter.SelectedModifications.All(p => p.Value != SelectedCharacter.Modifications.IndexOf(m))).ToList();
        
        public IReadOnlyList<ModificationData> Modifications => SelectedCharacter.Modifications;
        
        
        [CreateProperty]
        public IReadOnlyList<Sprite> PartyIcons => Characters.Select(character => character.PartyIcon).ToList();
        
        public int AbilitiesCount => SelectedCharacter.Abilities.Count;
        public int ModificationCount => SelectedCharacter.Modifications.Count;

        
        public IReadOnlyList<CharacterModel> Characters => _partyModel.Characters;
        public ReadOnlyReactiveProperty<CharacterModel> SelectedCharacterProp => _selectedCharacterProp;
        public int SelectedCharacterIndex
        {
            get => _partyModel.Characters.IndexOf(SelectedCharacter);
            set
            {
                if (value < 0 || value >= Characters.Count)
                    return;

                _selectedCharacterProp.Value = Characters[value];
            }
        }

        private CharacterModel SelectedCharacter => _selectedCharacterProp.Value;

        private readonly ReactiveProperty<CharacterModel> _selectedCharacterProp;
        private readonly PartyModel _partyModel;

        public CharacterViewModel(PartyModel partyModel)
        {
            _partyModel = partyModel;
            _selectedCharacterProp = new ReactiveProperty<CharacterModel>(partyModel.Characters.First());

            BindProperties(new[]
            {
                nameof(Name),
                nameof(Portrait),
                nameof(Hp),
                nameof(Armor),
            }, _selectedCharacterProp);

            BindProperties(nameof(Hp), new[]
            {
                SelectedCharacter.HP,
                SelectedCharacter.MaxHP
            });

            BindProperties(nameof(Armor), new[]
            {
                SelectedCharacter.Armor,
                SelectedCharacter.MaxArmor
            });
        }

        public void AddModification(AbilityData ability, ModificationData modification)
        {
            SelectedCharacter.SelectedModifications[SelectedCharacter.Abilities.IndexOf(ability)] =
                SelectedCharacter.Modifications.IndexOf(modification);
            
            Publish(caller: nameof(SelectedModifications));
        }

        public void RemoveModification(AbilityData ability)
        {
            SelectedCharacter.SelectedModifications.Remove(SelectedCharacter.Abilities.IndexOf(ability));
            
            Publish(caller: nameof(SelectedModifications));
        }

        public bool TryGetModificationIndex(AbilityData ability, out int modificationIndex) =>
            SelectedCharacter.SelectedModifications.TryGetValue(SelectedCharacter.Abilities.IndexOf(ability), out modificationIndex);
    }
}