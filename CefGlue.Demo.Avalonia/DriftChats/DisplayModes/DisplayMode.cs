namespace ServiceStudio.View.DriftChats.DisplayModes {
    public abstract class DisplayMode {

        /// <summary>
        /// Drift chat Playbook to use in the Drift chat widget.
        /// </summary>
        public PlaybookId PlaybookId { get; protected set; } = PlaybookId.Default;

        // public virtual void ExecuteBeforeDisplayLogic(IDriftChatView innerView) { }
    }
}
