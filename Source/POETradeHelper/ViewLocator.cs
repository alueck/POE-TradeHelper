using Avalonia.Controls;
using Avalonia.Controls.Templates;

using ReactiveUI;

using System;

namespace POETradeHelper
{
    public class ViewLocator : IDataTemplate, POETradeHelper.Common.Contract.IViewLocator
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().AssemblyQualifiedName?.Replace("ViewModel", "View");

            if (!string.IsNullOrEmpty(name))
            {
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public object GetView(object viewModel)
        {
            if (Match(viewModel))
            {
                return Build(viewModel);
            }

            throw new ArgumentException($"The type {viewModel.GetType().FullName} of {nameof(viewModel)} does not inherit from {nameof(ReactiveObject)}.");
        }

        public bool Match(object data)
        {
            return data is ReactiveObject;
        }
    }
}
