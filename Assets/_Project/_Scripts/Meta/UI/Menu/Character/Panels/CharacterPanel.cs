using UnityEngine.UIElements;
using UnityUtils;

namespace _Project._Scripts.Meta.UI.Menu.Character.Panels
{
    class CharacterPanel : VisualElement
    {
        private const string CharacterClass = "character";
        private const string CharacterHiddenClass = "character--hidden";
        private const string CharacterBackgroundClass = "character__background";
        private const string CharacterPortraitClass = "character__portrait";
        private const string CharacterFrameClass = "character__frame";
        private const string CharacterNameClass = "character__name";
        private const string CharacterNameLabelClass = "character__name__label";
        private const string CharacterStatsClass = "character__stats";
        private const string CharacterStatsHpClass = "character__stats__hp";
        private const string CharacterStatsArmorClass = "character__stats__armor";

        public CharacterPanel(CharacterView view)
        {
            AddToClassList(CharacterClass);
            AddToClassList(CharacterHiddenClass);

            RegisterCallbackOnce<GeometryChangedEvent, CharacterPanel>((_, panel) =>
                panel.RemoveFromClassList(CharacterHiddenClass), this);

            this.CreateChild(CharacterBackgroundClass);

            VisualElement portrait = this.CreateChild(CharacterPortraitClass);
            portrait.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Portrait).ToPropertyPath()
            });

            this.CreateChild(CharacterFrameClass);
            this.CreateChild(CharacterNameClass);

            Label label = this.CreateChild<Label>(CharacterNameLabelClass);
            label.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Name).ToPropertyPath()
            });

            AddStats();

            view.Add(this);
        }

        public void OnSelectionChanged()
        {
            AddToClassList(CharacterHiddenClass);
            RegisterCallbackOnce<TransitionEndEvent, CharacterPanel>((_, panel) =>
                panel.RemoveFromClassList(CharacterHiddenClass), this);
        }

        private void AddStats()
        {
            VisualElement stats = this.CreateChild(CharacterStatsClass);

            VisualElement hp = stats.CreateChild(CharacterStatsHpClass);
            hp.CreateChild<Image>();
            Label hpLabel = hp.CreateChild<Label>();
            hpLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Hp).ToPropertyPath()
            });

            VisualElement armor = stats.CreateChild(CharacterStatsArmorClass);
            armor.CreateChild<Image>();
            Label armorLabel = armor.CreateChild<Label>();
            armorLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Armor).ToPropertyPath()
            });
        }
    }
}