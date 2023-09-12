using System;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using ReactiveUI;

using IViewLocator = POETradeHelper.Common.Contract.IViewLocator;

namespace POETradeHelper
{
    public class ViewLocator : IDataTemplate, IViewLocator
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            string? name = data.GetType().AssemblyQualifiedName?.Replace("ViewModel", "View");

            if (!string.IsNullOrEmpty(name))
            {
                Type? type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public object GetView(object viewModel)
        {
            if (this.Match(viewModel))
            {
                return this.Build(viewModel);
            }

            throw new ArgumentException(
                $"The type {viewModel.GetType().FullName} of {nameof(viewModel)} does not inherit from {nameof(ReactiveObject)}.");
        }

        public bool Match(object data) => data is ReactiveObject;
    }
}