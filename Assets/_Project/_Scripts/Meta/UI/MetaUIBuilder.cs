using _Project._Scripts.Meta.Data;
using _Project._Scripts.Meta.UI.Menu;
using _Project._Scripts.Meta.UI.Menu.Character;
using _Project._Scripts.Shared.Data;
using _Project._Scripts.Shared.UI;
using UnityEngine.UIElements;

namespace _Project._Scripts.Meta.UI
{
    public class MetaUIBuilder : UIBuilder
    {
        protected override void Build(VisualElement root, StyleSheetRepository styleSheets)
        {
            root.styleSheets.Add(styleSheets.meta);

            MenuView menuView = AttachView<MenuView, MenuViewModel>(
                vm => new MenuView(vm),
                root, styleSheets.menu);

            CharacterView _ = AttachView<CharacterView, CharacterViewModel>(
                vm => new CharacterView(vm),
                menuView, styleSheets.character);
        }
    }
}