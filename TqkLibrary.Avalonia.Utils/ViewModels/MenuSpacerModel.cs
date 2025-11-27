using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TqkLibrary.Avalonia.Utils.ViewModels
{
    public class MenuSpacerModel : MenuViewModelBase
    {
        [SetsRequiredMembers]
        public MenuSpacerModel()
        {
            this.DisplayName = "-";
        }
    }
}
