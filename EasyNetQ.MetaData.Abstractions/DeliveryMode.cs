namespace EasyNetQ.MetaData.Abstractions {
    /// <summary>
    /// Defines message delivery modes.
    /// </summary>
    public enum DeliveryMode : byte {
        /// <summary>
        /// The message will not be saved to disk, even when delivered to a durable queue.
        /// </summary>
        NonPersistent = 1,

        /// <summary>
        /// The message will be saved to disk when delivered to a durable queue.
        /// </summary>
        Persistent = 2
    }
}