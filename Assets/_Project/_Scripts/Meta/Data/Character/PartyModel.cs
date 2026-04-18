using System.Collections.Generic;
using ObservableCollections;

namespace _Project._Scripts.Meta.Data.Character
{
    public class PartyModel
    {
        public readonly ObservableList<CharacterModel> Characters;
        
        public PartyModel(IEnumerable<CharacterModel> characters)
        {
            Characters = new ObservableList<CharacterModel>(characters);
        }
    }
}