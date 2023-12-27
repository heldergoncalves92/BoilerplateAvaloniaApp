using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation.Framework {
    internal interface IExtendedReactView : ISetLogicalParent {

        SizeToContent AdjustSizeToContent { get; set; }
    }
}
