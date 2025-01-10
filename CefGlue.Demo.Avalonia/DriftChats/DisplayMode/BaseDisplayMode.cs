using Avalonia.Controls;
using ServiceStudio.View.DriftChats;

namespace ServiceStudio.WebViewImplementation.DriftChats.DisplayMode {
    public abstract class BaseDisplayMode : IDisplayMode {
        public virtual PlaybookId PlaybookId { get; protected set; } = PlaybookId.Default;

        public abstract (double width, double height, int bottomRightOffset) DetermineDimensions();
        public abstract void SetVisible(Panel chatIconContainer, Border chatContainerBorder, StackPanel chatEngagementContainer);
    }
}
