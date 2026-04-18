using _Project._Scripts.Shared.UI.MVVM;
using UnityUtils;

namespace _Project._Scripts.Meta.UI.Menu
{
    public class MenuView : View<MenuViewModel>
    {
        public MenuView(MenuViewModel viewModel) : base(viewModel)
        {
            this.CreateChild("tabs");
            this.CreateChild("exit_button");
            this.CreateChild("seal");
        }
    }
}