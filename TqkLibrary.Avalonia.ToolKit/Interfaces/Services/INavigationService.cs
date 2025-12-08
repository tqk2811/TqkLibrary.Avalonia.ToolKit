using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections;
using TqkLibrary.Avalonia.ToolKit.ViewModels;

namespace TqkLibrary.Avalonia.ToolKit.Interfaces.Services
{
    public interface INavigationService<TBaseViewModel>
    {
        TBaseViewModel? CurrentView { get; }
        bool IsBackAvalable { get; }
        bool IsNextAvalable { get; }
        void NavigateTo<TViewModel>() where TViewModel : TBaseViewModel;
        void NavigateTo(TBaseViewModel viewModel);
        void NavigateTo(Type type);
        bool Back();
        bool Next();
    }
}
