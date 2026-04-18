using System.Linq;
using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Shared.UI.Manipulators;
using _Project._Scripts.Shared.UI.MVVM;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;
using Button = UnityEngine.UIElements.Button;

namespace _Project._Scripts.Meta.UI.Menu.Character
{
    public class CharacterView : View<CharacterViewModel>
    {
        private ScrollView _modificationsScrollView;

        private VisualElement _abilitiesGrid;

        private VisualElement _character;

        private bool Dragging =>
            Children().Any(child => child.ClassListContains("modification__draggable"));

        public CharacterView(CharacterViewModel viewModel) : base(viewModel)
        {
            AddDividers();
            AddPartyPanel();
            AddCharacterPanel();
            AddAbilityPanel();
            AddModificationPanel();

            viewModel.SelectedCharacterProp
                .Subscribe(this, (m, v) => v.OnSelectionChanged(m))
                .AddTo(ref DisposableBag);
        }

        private void AddDividers()
        {
            VisualElement container = this.CreateChild("container");

            container.CreateChild("divider", "divider--top", "divider--top-1");
            container.CreateChild("divider", "divider--top", "divider--top-2");
            container.CreateChild("divider", "divider--right", "divider--right-1");
            container.CreateChild("divider", "divider--right", "divider--right-2");
            container.CreateChild("divider", "divider--bottom");
        }

        private void AddCharacterPanel()
        {
            _character = this.CreateChild("character", "character--hidden");
            _character.RegisterCallbackOnce<GeometryChangedEvent, VisualElement>((_, ve) =>
                ve.RemoveFromClassList("character--hidden"), _character);

            _character.CreateChild("character__background");

            VisualElement portrait = _character.CreateChild("character__portrait");
            portrait.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Portrait).ToPropertyPath()
            });

            _character.CreateChild("character__frame");
            _character.CreateChild("character__name");

            Label label = _character.CreateChild<Label>("character__name__label");
            label.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Name).ToPropertyPath()
            });

            AddStats(_character);
        }

        private static void AddStats(VisualElement parent)
        {
            VisualElement stats = parent.CreateChild("character__stats");

            VisualElement hp = stats.CreateChild("character__stats__hp");
            hp.CreateChild<Image>();
            Label hpLabel = hp.CreateChild<Label>();
            hpLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Hp).ToPropertyPath()
            });

            VisualElement armor = stats.CreateChild("character__stats__armor");
            armor.CreateChild<Image>();
            Label armorLabel = armor.CreateChild<Label>();
            armorLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = nameof(CharacterViewModel.Armor).ToPropertyPath()
            });
        }

        private void AddAbilityPanel()
        {
            VisualElement element = this.CreateChild("abilities");
            element.pickingMode = PickingMode.Ignore;

            Label header = element.CreateChild<Label>("abilities__header");
            header.text = "ABILITIES";
            _abilitiesGrid = element.CreateChild("abilities__grid");
        }

        private void AddModificationPanel()
        {
            VisualElement element = this.CreateChild("modifications");
            element.pickingMode = PickingMode.Ignore;

            VisualElement header = element.CreateChild("modifications__header");
            Label label = header.CreateChild<Label>();
            label.text = "MODIFICATIONS";

            _modificationsScrollView = element.CreateChild<ScrollView>();
            _modificationsScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            element.CreateChild("modifications__divider");
        }

        private void AddPartyPanel()
        {
            VisualElement partyContainer = this.CreateChild("container", "party__container");
            ToggleButtonGroup party = partyContainer.CreateChild<ToggleButtonGroup>("party");

            for (var i = 0; i < ViewModel.Characters.Count; i++)
            {
                int index = i;
                Button memberButton = party.CreateChild<Button>("party__member");
                VisualElement frame = memberButton.CreateChild("party__member__frame");
                frame.pickingMode = PickingMode.Ignore;

                memberButton.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.PartyIcons)}[{index}]".ToPropertyPath()
                });
                memberButton.clicked += () => ViewModel.SelectedCharacterIndex = index;
            }
        }

        private void OnSelectionChanged(CharacterModel model)
        {
            _character.AddToClassList("character--hidden");
            _character.RegisterCallbackOnce<TransitionEndEvent, VisualElement>((_, c) =>
                c.RemoveFromClassList("character--hidden"), _character);

            BindAbilities(model);
            BindModifications(model);
        }

        private void BindAbilities(CharacterModel model)
        {
            _abilitiesGrid.Clear();

            for (int i = 0; i < ViewModel.AbilitiesCount; i++)
            {
                AbilityData abilityData = model.Abilities[i];

                VisualElement item = _abilitiesGrid.CreateChild("ability__item", "ability__item--hidden");
                item.userData = abilityData;

                item.RegisterCallbackOnce<GeometryChangedEvent, VisualElement>((_, ve) =>
                    ve.RemoveFromClassList("ability__item--hidden"), item);
                item.RegisterCallback<PointerEnterEvent, AbilityData>(OnAbilityEnter, abilityData);
                item.RegisterCallback<PointerLeaveEvent>(OnAbilityLeave);

                item.CreateChild("ability__background");
                VisualElement content = item.CreateChild("ability__content");
                var modSlot = content.CreateChild("ability__modification--empty");

                if (ViewModel.TryGetModificationIndex(abilityData, out var index))
                {
                    item.AddToClassList("ability__with-modification");
                    VisualElement mod = modSlot.CreateChild("modification__draggable");
                    BindModification(mod, index);
                    DragManipulator manipulator = new();
                    manipulator.OnDrop += OnDrop;
                    AddDetachCallback(mod, item);
                    mod.AddManipulator(manipulator);
                }

                VisualElement icon = content.CreateChild("ability__icon");
                icon.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.AbilityIcons)}[{i}]".ToPropertyPath()
                });

                Label label = content.CreateChild<Label>("ability__label");
                label.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.AbilityNames)}[{i}]".ToPropertyPath()
                });

                if ((i + 1) % 4 == 0)
                    item.style.marginRight = 0;
            }
        }

        private void BindModifications(CharacterModel model)
        {
            _modificationsScrollView.Clear();

            for (var i = 0; i < ViewModel.ModificationCount; i++)
            {
                ModificationData modificationData = model.Modifications[i];

                VisualElement item =
                    _modificationsScrollView.CreateChild("modification__item", "modification__item--hidden");
                item.userData = modificationData;
                
                item.SetBinding("enabledSelf", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.SelectedModifications)}[{i}]".ToPropertyPath()
                });
                
                item.RegisterCallbackOnce<GeometryChangedEvent, VisualElement>((_, ve) =>
                    ve.RemoveFromClassList("modification__item--hidden"), item);

                item.RegisterCallback<PointerEnterEvent, ModificationData>(OnModificationEnter, modificationData);
                item.RegisterCallback<PointerLeaveEvent>(OnModificationLeave);
                item.RegisterCallback<PointerDownEvent, int>(OnModificationDown, i);

                VisualElement box = item.CreateChild("modification__item__box");

                Label itemLabel = box.CreateChild<Label>("modification__item__label");
                itemLabel.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationNames)}[{i}]".ToPropertyPath()
                });

                Label itemSubLabel = box.CreateChild<Label>("modification__item__sublabel");
                itemSubLabel.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationSubLabels)}[{i}]".ToPropertyPath()
                });
                
                VisualElement iconBg = item.CreateChild("modification__item__icon-bg");
                iconBg.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationIconBgs)}[{i}]".ToPropertyPath()
                });

                VisualElement icon = iconBg.CreateChild("modification__item__icon");
                icon.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationIcons)}[{i}]".ToPropertyPath()
                });

                if (i == ViewModel.ModificationCount - 1)
                    item.style.marginBottom = 0;
            }
        }

        private void OnModificationDown(PointerDownEvent evt, int index)
        {
            if (evt.button != 0)
                return;

            VisualElement currentTarget = (VisualElement)evt.currentTarget;
            Vector2 relativePos = currentTarget.LocalToWorld(evt.localPosition);
            VisualElement draggable = this.CreateChild("modification__draggable", "modification__draggable--hidden");

            draggable.RegisterCallbackOnce<GeometryChangedEvent, (VisualElement e, Vector2 rp, int i)>((_, data) =>
            {
                VisualElement drag = data.e;
                drag.RemoveFromClassList("modification__draggable--hidden");
                
                Vector2 halfSize = new Vector2(drag.resolvedStyle.width, drag.resolvedStyle.height) / 2;
                DragManipulator manipulator = new();
                manipulator.OnDrop += OnDrop;
                
                BindModification(drag, data.i);
                
                drag.RegisterCallbackOnce<PointerCaptureEvent, ModificationData>(OnModificationCapture, (ModificationData)drag.userData);
                drag.RegisterCallbackOnce<PointerCaptureOutEvent>(OnModificationCaptureOut);
                
                drag.style.translate = data.rp - halfSize;
                drag.AddManipulator(manipulator);
                drag.CapturePointer(evt.pointerId);
            }, (draggable, relativePos, index));
        }

        private void BindModification(VisualElement element, int index)
        {
            var modificationData = ViewModel.Modifications[index];
            element.userData = modificationData;
            
            element.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = $"{nameof(CharacterViewModel.ModificationIconBgs)}[{index}]".ToPropertyPath()
            });

            VisualElement icon = element.CreateChild("modification__item__icon");
            icon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = $"{nameof(CharacterViewModel.ModificationIcons)}[{index}]".ToPropertyPath()
            });
        }

        private void OnDrop(VisualElement target, Vector2 dropPosition)
        {
            foreach (VisualElement ability in _abilitiesGrid.Children())
            {
                if (ability.ClassListContains("ability__with-modification"))
                    continue;
                
                ability.RemoveFromClassList("ability__item--incompatible");
                ability.RemoveFromClassList("ability__item--compatible");
                
                VisualElement slot = ability.Q(className: "ability__modification--empty");
                VisualElement bg = ability.Q(className: "ability__background");

                if (!bg.worldBound.Contains(dropPosition))
                    continue;
                
                AbilityData abilityData = (AbilityData)ability.userData;
                ModificationData modificationData = (ModificationData)target.userData;

                if (!abilityData.compatibleModifications.HasFlag(modificationData.ModificationType))
                    continue;
                
                ability.AddToClassList("ability__with-modification");
                ViewModel.AddModification(abilityData, modificationData);
                slot.Add(target);
                target.style.translate = new StyleTranslate();
                AddDetachCallback(target, ability);
                return;
            }
            target.AddToClassList("modification__draggable--hidden");
            target.RegisterCallbackOnce<TransitionEndEvent, VisualElement>((_, t) =>
                t.RemoveFromHierarchy(), target);
        }

        private void AddDetachCallback(VisualElement target, VisualElement ability) =>
            target.RegisterCallbackOnce<PointerDownEvent, VisualElement>((evt, ele) =>
            {
                ability.RemoveFromClassList("ability__with-modification");
                AbilityData abilityData = (AbilityData)ability.userData;
                ViewModel.RemoveModification(abilityData);

                Add(ele);
                VisualElement slot = ability.Q(className: "ability__modification--empty");
                Vector2 relativePos = slot.LocalToWorld(evt.localPosition);
                Vector2 halfSize = new Vector2(ele.resolvedStyle.width, ele.resolvedStyle.height) / 2;
                
                ele.RegisterCallbackOnce<PointerCaptureEvent, ModificationData>(OnModificationCapture, (ModificationData)ele.userData);
                ele.RegisterCallbackOnce<PointerCaptureOutEvent>(OnModificationCaptureOut);
                
                ele.style.translate = relativePos - halfSize;
                ele.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            }, target);

        private void OnAbilityEnter(PointerEnterEvent evt, AbilityData data)
        {
            if (Dragging)
                return;
            
            foreach (VisualElement modification in _modificationsScrollView.contentContainer.Children())
            {
                modification.RemoveFromClassList("modification__item--incompatible");
                modification.RemoveFromClassList("modification__item--compatible");

                ModificationData modificationData = (ModificationData)modification.userData;
                bool isCompatible = data.compatibleModifications.HasFlag(modificationData.ModificationType);

                modification.AddToClassList(isCompatible
                    ? "modification__item--compatible"
                    : "modification__item--incompatible");
            }
        }

        private void OnAbilityLeave(PointerLeaveEvent evt)
        {
            if (Dragging)
                return;
            
            foreach (VisualElement modification in _modificationsScrollView.contentContainer.Children())
            {
                modification.RemoveFromClassList("modification__item--incompatible");
                modification.RemoveFromClassList("modification__item--compatible");
            }
        }

        private void OnModificationCapture(PointerCaptureEvent _, ModificationData  data) =>
            CheckAbilitiesCompatibility(data);

        private void OnModificationEnter(PointerEnterEvent evt, ModificationData data)
        {
            if (Dragging)
                return;
            
            VisualElement target = (VisualElement)evt.target;
            
            if (!target.enabledSelf)
                return;
            
            CheckAbilitiesCompatibility(data);
        }

        private void OnModificationLeave(PointerLeaveEvent evt)
        {
            if (Dragging)
                return;
            
            foreach (VisualElement ability in _abilitiesGrid.Children())
            {
                ability.RemoveFromClassList("ability__item--incompatible");
                ability.RemoveFromClassList("ability__item--compatible");
            }
        }

        private void OnModificationCaptureOut(PointerCaptureOutEvent evt)
        {
            foreach (VisualElement ability in _abilitiesGrid.Children())
            {
                ability.RemoveFromClassList("ability__item--incompatible");
                ability.RemoveFromClassList("ability__item--compatible");
            }
        }

        private void CheckAbilitiesCompatibility(ModificationData data)
        {
            foreach (VisualElement ability in _abilitiesGrid.Children())
            {
                ability.RemoveFromClassList("ability__item--incompatible");
                ability.RemoveFromClassList("ability__item--compatible");

                AbilityData abilityData = (AbilityData)ability.userData;
                bool isCompatible = abilityData.compatibleModifications.HasFlag(data.ModificationType);

                ability.AddToClassList(isCompatible ? "ability__item--compatible" : "ability__item--incompatible");
            }
        }
    }
}