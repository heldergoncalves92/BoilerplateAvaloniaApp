namespace ServiceStudio.View.DriftChats.DisplayModes {
    public class Engagement : DisplayMode {
        public Engagement(EngagementEventType eventType) {
            EventType = eventType;
        }

        public EngagementEventType EventType { get; }
    }
}
