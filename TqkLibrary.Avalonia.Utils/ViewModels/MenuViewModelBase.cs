using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Input;
using TqkLibrary.Avalonia.Utils.Attributes;
using TqkLibrary.Avalonia.Utils.Collections.ObservableCollections;
using TqkLibrary.Avalonia.Utils.Commands;
using TqkLibrary.Utils;

namespace TqkLibrary.Avalonia.Utils.ViewModels
{
    public abstract class MenuViewModelBase : ObservableObject
    {
        public MenuViewModelBase()
        {
            Childs.OnItemAdded += Childs_OnItemAdded;
            Childs.OnItemRemoved += Childs_OnItemRemoved;
        }

        public virtual required string DisplayName { get; init; }
        public virtual ICommand? Command { get; protected set; }
        public virtual IImage? Icon { get; protected set; }

        public virtual MenuViewModelBase? Parent { get; private set; }
        public virtual DispatcherObservableCollection<MenuViewModelBase> Childs { get; } = new();
        private void Childs_OnItemAdded(MenuViewModelBase item)
        {
            if (item is not null)
            {
                if (item.Parent is not null)
                    throw new InvalidOperationException($"Menu '{item.DisplayName}' are under '{item.Parent.DisplayName}'");

                item.Parent = this;
            }
        }
        private void Childs_OnItemRemoved(MenuViewModelBase item)
        {
            if (item is not null)
            {
                if (item.Parent is null)
                    throw new InvalidOperationException($"Menu '{item.DisplayName}' are not under parent");

                item.Parent = null;
            }
            throw new NotImplementedException();
        }
    }
    public class MenuViewModelBase<TEnum> : MenuViewModelBase where TEnum : Enum
    {
        public TEnum Value { get; }

        [SetsRequiredMembers]
        public MenuViewModelBase(TEnum @enum, ICommand command)
        {
            this.Value = @enum;
            this.DisplayName = @enum.GetAttribute<DisplayNameAttribute>()?.Name ?? @enum.ToString();
            this.Icon = @enum.GetAttribute<ImageSourceAttribute>()?.GetImage();
            this.Command = command;
        }

        [SetsRequiredMembers]
        public MenuViewModelBase(TEnum @enum, Action execute) : this(@enum, new BaseCommand(execute))
        {

        }
        [SetsRequiredMembers]
        public MenuViewModelBase(TEnum @enum, Func<Task> execute) : this(@enum, new BaseCommand(execute))
        {

        }



        [SetsRequiredMembers]
        public MenuViewModelBase(TEnum @enum, IEnumerable<MenuViewModelBase> childs)
        {
            this.Value = @enum;
            this.DisplayName = @enum.GetAttribute<DisplayNameAttribute>()?.Name ?? @enum.ToString();
            this.Icon = @enum.GetAttribute<ImageSourceAttribute>()?.GetImage();
            foreach (var child in childs)
            {
                this.Childs.Add(child);
            }
        }
    }
}
