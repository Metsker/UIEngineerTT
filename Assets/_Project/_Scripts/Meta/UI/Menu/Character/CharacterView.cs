using System.Linq;
using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Meta.UI.Menu.Character.Panels;
using _Project._Scripts.Shared.UI.MVVM;
using R3;
using UnityEngine.UIElements;
using UnityUtils;

namespace _Project._Scripts.Meta.UI.Menu.Character
{
    public class CharacterView : View<CharacterViewModel>
    {
        private const string DividerClass = "divider";
        private const string DividerTopClass = "divider--top";
        private const string DividerTop1Class = "divider--top-1";
        private const string DividerTop2Class = "divider--top-2";
        private const string DividerRightClass = "divider--right";
        private const string DividerRight1Class = "divider--right-1";
        private const string DividerRight2Class = "divider--right-2";
        private const string DividerBottomClass = "divider--bottom";
        private const string ModificationDraggableClass = "modification__draggable";

        public bool Dragging => Children().Any(child => child.ClassListContains(ModificationDraggableClass));

        public AbilitiesPanel AbilitiesPanel { get; }
        public ModificationsPanel ModificationsPanel { get; }

        private readonly CharacterPanel _characterPanel;
        private readonly PartyPanel _partyPanel;

        public CharacterView(CharacterViewModel viewModel) : base(viewModel)
        {
            AddDividers();

            _partyPanel = new PartyPanel(this);
            _characterPanel = new CharacterPanel(this);
            AbilitiesPanel = new AbilitiesPanel(this);
            ModificationsPanel = new ModificationsPanel(this);

            viewModel.SelectedCharacterProp
                .Subscribe(this, (m, v) => v.OnSelectionChanged(m))
                .AddTo(ref DisposableBag);
        }

        private void AddDividers()
        {
            VisualElement container = this.CreateChild(ContainerClass);

            container.CreateChild(DividerClass, DividerTopClass, DividerTop1Class);
            container.CreateChild(DividerClass, DividerTopClass, DividerTop2Class);
            container.CreateChild(DividerClass, DividerRightClass, DividerRight1Class);
            container.CreateChild(DividerClass, DividerRightClass, DividerRight2Class);
            container.CreateChild(DividerClass, DividerBottomClass);
        }

        private void OnSelectionChanged(CharacterModel model)
        {
            _characterPanel.OnSelectionChanged();
            AbilitiesPanel.Bind(model);
            ModificationsPanel.Bind(model);
        }
    }
}