using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Shared.UI.Manipulators;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;
using static _Project._Scripts.Meta.UI.Menu.Character.Panels.ModificationsPanel;

namespace _Project._Scripts.Meta.UI.Menu.Character.Panels
{
    public class AbilitiesPanel : VisualElement
    {
        private const string AbilityItemCompatibleClass = "ability__item--compatible";
        private const string AbilityItemIncompatibleClass = "ability__item--incompatible";
        private const string AbilityWithModificationClass = "ability__with-modification";
        private const string AbilityBackgroundClass = "ability__background";
        private const string AbilityModificationEmptyClass = "ability__modification--empty";
        private const string AbilityContentClass = "ability__content";
        private const string AbilitiesClass = "abilities";
        private const string AbilitiesHeaderClass = "abilities__header";
        private const string AbilitiesGridClass = "abilities__grid";
        private const string AbilityItemClass = "ability__item";
        private const string AbilityItemHiddenClass = "ability__item--hidden";
        private const string AbilityIconClass = "ability__icon";
        private const string AbilityLabelClass = "ability__label";

        private readonly CharacterView _view;

        public VisualElement Grid { get; }

        public AbilitiesPanel(CharacterView view)
        {
            _view = view;

            AddToClassList(AbilitiesClass);
            pickingMode = PickingMode.Ignore;

            Label header = this.CreateChild<Label>(AbilitiesHeaderClass);
            header.text = "ABILITIES";
            Grid = this.CreateChild(AbilitiesGridClass);

            view.Add(this);
        }

        public void Bind(CharacterModel model)
        {
            Grid.Clear();

            for (int i = 0; i < _view.ViewModel.AbilitiesCount; i++)
            {
                AbilityData abilityData = model.Abilities[i];

                VisualElement item = Grid.CreateChild(AbilityItemClass, AbilityItemHiddenClass);
                item.userData = abilityData;

                item.RegisterCallbackOnce<GeometryChangedEvent, VisualElement>((_, ve) =>
                    ve.RemoveFromClassList(AbilityItemHiddenClass), item);
                item.RegisterCallback<PointerEnterEvent, AbilityData>(OnAbilityEnter, abilityData);
                item.RegisterCallback<PointerLeaveEvent>(OnAbilityLeave);

                item.CreateChild(AbilityBackgroundClass);
                VisualElement content = item.CreateChild(AbilityContentClass);
                VisualElement modSlot = content.CreateChild(AbilityModificationEmptyClass);

                if (_view.ViewModel.TryGetModificationIndex(abilityData, out int index))
                {
                    item.AddToClassList(AbilityWithModificationClass);
                    VisualElement mod = modSlot.CreateChild(ModificationDraggableClass);
                    _view.ModificationsPanel.BindModification(mod, index);
                    DragManipulator manipulator = new();
                    mod.RegisterCallbackOnce<PointerCaptureOutEvent, VisualElement>(
                        _view.AbilitiesPanel.OnModificationCaptureOut, item);

                    AddDetachCallback(mod, item);
                    mod.AddManipulator(manipulator);
                }

                VisualElement icon = content.CreateChild(AbilityIconClass);
                icon.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.AbilityIcons)}[{i}]".ToPropertyPath()
                });

                Label label = content.CreateChild<Label>(AbilityLabelClass);
                label.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.AbilityNames)}[{i}]".ToPropertyPath()
                });

                if ((i + 1) % 4 == 0)
                    item.style.marginRight = 0;
            }
        }

        public void ShowCompatibility(ModificationData data)
        {
            foreach (VisualElement ability in _view.AbilitiesPanel.Grid.Children())
            {
                HideCompatibility(ability);

                AbilityData abilityData = (AbilityData)ability.userData;
                bool isCompatible = abilityData.compatibleModifications.HasFlag(data.ModificationType);

                ability.AddToClassList(isCompatible ? AbilityItemCompatibleClass : AbilityItemIncompatibleClass);
            }
        }

        public static void HideCompatibility(VisualElement ability)
        {
            ability.RemoveFromClassList(AbilityItemIncompatibleClass);
            ability.RemoveFromClassList(AbilityItemCompatibleClass);
        }
        
        public void HideCompatibility()
        {
            foreach (VisualElement ability in _view.AbilitiesPanel.Grid.Children()) 
                HideCompatibility(ability);
        }

        private void AddDetachCallback(VisualElement target, VisualElement ability) =>
            target.RegisterCallbackOnce<PointerDownEvent, VisualElement>((evt, t) =>
            {
                ability.RemoveFromClassList(AbilityWithModificationClass);
                _view.ViewModel.RemoveModification((AbilityData)ability.userData);

                _view.Add(t);
                VisualElement slot = ability.Q(className: AbilityModificationEmptyClass);
                Vector2 halfSize = new Vector2(t.resolvedStyle.width, t.resolvedStyle.height) / 2;

                t.RegisterCallbackOnce<PointerCaptureEvent, ModificationData>(OnModificationCapture,
                    (ModificationData)t.userData);
                t.RegisterCallbackOnce<PointerCaptureOutEvent, VisualElement>(OnModificationCaptureOut, ability);

                t.style.translate = slot.LocalToWorld(evt.localPosition) - halfSize;
                t.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            }, target);

        private void OnAbilityEnter(PointerEnterEvent evt, AbilityData data)
        {
            if (_view.Dragging)
                return;

            foreach (VisualElement modification in _view.ModificationsPanel.ScrollView.contentContainer.Children())
            {
                ModificationsPanel.HideCompatibility(modification);

                ModificationData modificationData = (ModificationData)modification.userData;
                bool isCompatible = data.compatibleModifications.HasFlag(modificationData.ModificationType);

                modification.AddToClassList(isCompatible
                    ? ModificationItemCompatibleClass
                    : ModificationItemIncompatibleClass);
            }
        }

        private void OnAbilityLeave(PointerLeaveEvent evt)
        {
            if (_view.Dragging)
                return;

            _view.ModificationsPanel.HideCompatibility();
        }

        private void OnModificationCapture(PointerCaptureEvent _, ModificationData data)
        {
            ShowCompatibility(data);
            _view.ModificationsPanel.HideCompatibility();
        }

        public void OnModificationCaptureOut(PointerCaptureOutEvent evt, [CanBeNull] VisualElement originAbility)
        {
            VisualElement target = (VisualElement)evt.target;
            Vector2 dropPosition = target.LocalToWorld(target.layout.center);
            
            if (DroppedOverModifications(target, dropPosition))
                return;
            
            ModificationData modificationData = (ModificationData)target.userData;
            bool resolved = false;
            
            foreach (VisualElement ability in Grid.Children())
            {
                HideCompatibility(ability);
                
                if (resolved) 
                    continue;

                VisualElement bg = ability.Q(className: AbilityBackgroundClass);

                if (!bg.worldBound.Contains(dropPosition))
                    continue;

                AbilityData abilityData = (AbilityData)ability.userData;

                if (!abilityData.compatibleModifications.HasFlag(modificationData.ModificationType) || ability.ClassListContains(AbilityWithModificationClass))
                {
                    if (originAbility != null)
                    {
                        BindModificationToAbility(target, originAbility, abilityData, modificationData);
                        resolved = true;
                    }
                    continue;
                }

                BindModificationToAbility(target, ability, abilityData, modificationData);
                resolved = true;
            }

            if (!resolved) 
                RemoveDraggable(target);
        }

        private bool DroppedOverModifications(VisualElement target, Vector2 dropPosition)
        {
            if (!_view.ModificationsPanel.ScrollView.contentContainer.worldBound.Contains(dropPosition))
                return false;
            
            RemoveDraggable(target);
            foreach (VisualElement modification in _view.ModificationsPanel.ScrollView.contentContainer.Children())
            {
                if (!modification.enabledSelf || !modification.worldBound.Contains(dropPosition)) 
                    continue;
                    
                ShowCompatibility((ModificationData)modification.userData);
                return true;
            }
            HideCompatibility();
            return true;
        }

        private void BindModificationToAbility(
            VisualElement modification,
            VisualElement ability,
            AbilityData abilityData,
            ModificationData modificationData)
        {
            VisualElement slot = ability.Q(className: AbilityModificationEmptyClass);
            ability.AddToClassList(AbilityWithModificationClass);
            _view.ViewModel.AddModification(abilityData, modificationData);
            modification.RegisterCallbackOnce<PointerCaptureEvent, ModificationData>(OnModificationCapture, modificationData);
            modification.RegisterCallbackOnce<PointerCaptureOutEvent, VisualElement>(OnModificationCaptureOut, ability);
            slot.Add(modification);
            modification.style.translate = new StyleTranslate();
            AddDetachCallback(modification, ability);
        }

        private static void RemoveDraggable(VisualElement target)
        {
            target.AddToClassList(ModificationDraggableHiddenClass);
            target.RegisterCallbackOnce<TransitionEndEvent, VisualElement>((_, t) =>
                t.RemoveFromHierarchy(), target);
        }
    }
}