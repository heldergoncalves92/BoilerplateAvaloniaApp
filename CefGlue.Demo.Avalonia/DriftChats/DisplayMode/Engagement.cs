using Avalonia.Controls;
using ServiceStudio.View.DriftChats;

namespace ServiceStudio.WebViewImplementation.DriftChats.DisplayMode {

    internal class Engagement : Minimized {

        public Engagement(EngagementEventType eventType) {
            EventType = eventType;
        }

        public EngagementEventType EventType { get; }

        public override (double width, double height, int bottomRightOffset) DetermineDimensions()
            => (DriftChat.EngagementWithArrowWidth
                + DriftChat.ShadowBorderThickness
                + DriftChat.ChatIconWidth, DriftChat.FullHeightWithEngagementAndShadow, -12);

        public override void SetVisible(Panel chatIconContainer, Border chatContainerBorder, StackPanel chatEngagementContainer) {
            chatIconContainer.IsVisible = true;
            chatContainerBorder.IsVisible = false;
            chatEngagementContainer.IsVisible = true;
        }
    }
}
