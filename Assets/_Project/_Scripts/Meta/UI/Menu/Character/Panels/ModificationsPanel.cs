using _Project._Scripts.Meta.Data.Character;
using _Project._Scripts.Shared.UI.Manipulators;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;
using static _Project._Scripts.Meta.UI.Menu.Character.Panels.AbilitiesPanel;

namespace _Project._Scripts.Meta.UI.Menu.Character.Panels
{
    public class ModificationsPanel : VisualElement
    {
        public const string ModificationItemCompatibleClass = "modification__item--compatible";
        public const string ModificationItemIncompatibleClass = "modification__item--incompatible";
        public const string ModificationDraggableClass = "modification__draggable";
        public const string ModificationDraggableHiddenClass = "modification__draggable--hidden";
        private const string ModificationsClass = "modifications";
        private const string ModificationsHeaderClass = "modifications__header";
        private const string ModificationsDividerClass = "modifications__divider";
        private const string ModificationItemClass = "modification__item";
        private const string ModificationItemHiddenClass = "modification__item--hidden";
        private const string ModificationItemBoxClass = "modification__item__box";
        private const string ModificationItemLabelClass = "modification__item__label";
        private const string ModificationItemSublabelClass = "modification__item__sublabel";
        private const string ModificationItemIconBgClass = "modification__item__icon-bg";
        private const string ModificationItemIconClass = "modification__item__icon";

        private readonly CharacterView _view;

        public ScrollView ScrollView { get; }

        public ModificationsPanel(CharacterView view)
        {
            _view = view;

            AddToClassList(ModificationsClass);
            pickingMode = PickingMode.Ignore;

            VisualElement header = this.CreateChild(ModificationsHeaderClass);
            header.CreateChild<Label>().text = "MODIFICATIONS";

            ScrollView = this.CreateChild<ScrollView>();
            ScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            this.CreateChild(ModificationsDividerClass);

            view.Add(this);
        }

        public void Bind(CharacterModel model)
        {
            ScrollView.Clear();

            for (var i = 0; i < _view.ViewModel.ModificationCount; i++)
            {
                ModificationData modificationData = model.Modifications[i];

                VisualElement item = ScrollView.CreateChild(ModificationItemClass, ModificationItemHiddenClass);
                item.userData = modificationData;

                item.SetBinding("enabledSelf", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.SelectedModifications)}[{i}]".ToPropertyPath()
                });

                item.RegisterCallbackOnce<GeometryChangedEvent, VisualElement>((_, ve) =>
                    ve.RemoveFromClassList(ModificationItemHiddenClass), item);
                item.RegisterCallback<PointerEnterEvent, ModificationData>(OnModificationEnter, modificationData);
                item.RegisterCallback<PointerLeaveEvent>(OnModificationLeave);
                item.RegisterCallback<PointerDownEvent, int>(OnModificationDown, i);

                VisualElement box = item.CreateChild(ModificationItemBoxClass);

                Label itemLabel = box.CreateChild<Label>(ModificationItemLabelClass);
                itemLabel.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationNames)}[{i}]".ToPropertyPath()
                });

                Label itemSubLabel = box.CreateChild<Label>(ModificationItemSublabelClass);
                itemSubLabel.SetBinding("text", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationSubLabels)}[{i}]".ToPropertyPath()
                });

                VisualElement iconBg = item.CreateChild(ModificationItemIconBgClass);
                iconBg.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationIconBgs)}[{i}]".ToPropertyPath()
                });

                VisualElement icon = iconBg.CreateChild(ModificationItemIconClass);
                icon.SetBinding("style.backgroundImage", new DataBinding
                {
                    dataSourcePath = $"{nameof(CharacterViewModel.ModificationIcons)}[{i}]".ToPropertyPath()
                });

                if (i == _view.ViewModel.ModificationCount - 1)
                    item.style.marginBottom = 0;
            }
        }

        public void BindModification(VisualElement element, int index)
        {
            element.userData = _view.ViewModel.Modifications[index];

            element.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = $"{nameof(CharacterViewModel.ModificationIconBgs)}[{index}]".ToPropertyPath()
            });

            VisualElement icon = element.CreateChild(ModificationItemIconClass);
            icon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = $"{nameof(CharacterViewModel.ModificationIcons)}[{index}]".ToPropertyPath()
            });
        }

        private void OnModificationDown(PointerDownEvent evt, int index)
        {
            if (evt.button != 0)
                return;

            Vector2 relativePos = ((VisualElement)evt.currentTarget).LocalToWorld(evt.localPosition);
            VisualElement draggable = _view.CreateChild(ModificationDraggableClass, ModificationDraggableHiddenClass);

            draggable.RegisterCallbackOnce<GeometryChangedEvent, (VisualElement e, Vector2 rp, int i)>((_, data) =>
            {
                VisualElement drag = data.e;
                drag.RemoveFromClassList(ModificationDraggableHiddenClass);

                Vector2 halfSize = new Vector2(drag.resolvedStyle.width, drag.resolvedStyle.height) / 2;
                DragManipulator manipulator = new();
                drag.RegisterCallbackOnce<PointerCaptureOutEvent, VisualElement>(
                    _view.AbilitiesPanel.OnModificationCaptureOut, null);

                BindModification(drag, data.i);

                drag.style.translate = data.rp - halfSize;
                drag.AddManipulator(manipulator);
                drag.CapturePointer(evt.pointerId);
            }, (draggable, relativePos, index));
        }

        private void OnModificationEnter(PointerEnterEvent evt, ModificationData data)
        {
            if (_view.Dragging)
                return;

            if (!((VisualElement)evt.target).enabledSelf)
                return;

            _view.AbilitiesPanel.ShowCompatibility(data);
        }

        private void OnModificationLeave(PointerLeaveEvent evt)
        {
            if (_view.Dragging)
                return;

            foreach (VisualElement ability in _view.AbilitiesPanel.Grid.Children())
                AbilitiesPanel.HideCompatibility(ability);
        }
        
        public static void HideCompatibility(VisualElement modification)
        {
            modification.RemoveFromClassList(ModificationItemIncompatibleClass);
            modification.RemoveFromClassList(ModificationItemCompatibleClass);
        }
        
        public void HideCompatibility()
        {
            foreach (VisualElement modification in ScrollView.contentContainer.Children())
                HideCompatibility(modification);
        }
    }
}