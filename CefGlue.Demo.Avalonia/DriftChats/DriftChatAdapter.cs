using ServiceStudio.WebViewImplementation.DriftChats.DisplayMode;

namespace ServiceStudio.WebViewImplementation.DriftChats {
    internal class DriftChatAdapter {
        private readonly DriftChat driftChat;

        public DriftChatAdapter(DriftChat driftChat) {
            this.driftChat = driftChat;
        }

        public void Display(View.DriftChats.DisplayModes.DisplayMode displayModeEnum) {
            IDisplayMode displayMode = displayModeEnum switch {
                View.DriftChats.DisplayModes.Minimized _ => new MinimizedDefault(),
                View.DriftChats.DisplayModes.Engagement engagement => new Engagement(engagement.EventType),
                View.DriftChats.DisplayModes.Open open => new Open(open.PlaybookId),
                _ => null,
            };

            driftChat.Display(displayMode);
        }
    }
}
