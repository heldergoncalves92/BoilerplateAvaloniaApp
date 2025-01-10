using Avalonia.Controls;
using ServiceStudio.View.DriftChats;

namespace ServiceStudio.WebViewImplementation.DriftChats.DisplayMode {

    /// <inheritdoc cref="IDisplayMode"/>
    internal class Open : BaseDisplayMode {
        public Open(PlaybookId playbookId = PlaybookId.Default) {
            PlaybookId = playbookId;
        }

        public override (double width, double height, int bottomRightOffset) DetermineDimensions() {
            const double chatWidthWithShadow = DriftChat.ChatWidth + 48;
            const double chatHeightWithShadow = DriftChat.ChatHeight + 48;

            return (chatWidthWithShadow, chatHeightWithShadow, 0);
        }

        public override void SetVisible(Panel chatIconContainer, Border chatContainerBorder, StackPanel chatEngagementContainer) {
            chatIconContainer.IsVisible = false;
            chatContainerBorder.IsVisible = true;
            chatEngagementContainer.IsVisible = false;
        }
    }
}
