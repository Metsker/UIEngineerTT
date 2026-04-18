using UnityEngine.UIElements;
using UnityUtils;
using Button = UnityEngine.UIElements.Button;

namespace _Project._Scripts.Meta.UI.Menu.Character.Panels
{
    public class PartyPanel : VisualElement
    {
        private const string ContainerClass = "container";
        private const string PartyContainerClass = "party__container";
        private const string PartyClass = "party";
        private const string PartyMemberClass = "party__member";
        private const string PartyMemberFrameClass = "party__member__frame";

        public PartyPanel(CharacterView view)
        {
            AddToClassList(ContainerClass);
            AddToClassList(PartyContainerClass);

            ToggleButtonGroup party = this.CreateChild<ToggleButtonGroup>(PartyClass);

            for (var i = 0; i < view.ViewModel.Characters.Count; i++)
            {
                int index = i;
                Button memberButton = party.CreateChild<Button>(PartyMemberClass);
                VisualElement frame = memberButton.CreateChild(PartyMemberFrameClass);
                frame.pickingMode = PickingMode.Ignore;

                memberButton.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.PartyIcons)}[{index}]".ToPropertyPath()
                });
                memberButton.clicked += () => view.ViewModel.SelectedCharacterIndex = index;
            }

            view.Add(this);
        }
    }
}