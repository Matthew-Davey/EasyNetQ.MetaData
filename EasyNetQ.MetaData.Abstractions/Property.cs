namespace EasyNetQ.MetaData.Abstractions {
    /// <summary>
    /// Defines available message attributes for binding.
    /// </summary>
    public enum Property {
        /// <summary>
        /// The property will be bound to the messages 'content_type' attribute.
        /// </summary>
        ContentType,

        /// <summary>
        /// The property will be bound to the messages 'content_encoding' attribute.
        /// </summary>
        ContentEncoding,

        /// <summary>
        /// The property will be bound to the messages delivery mode.
        /// </summary>
        DeliveryMode,

        /// <summary>
        /// The property will be bound to the messages 'priority' attribute.
        /// </summary>
        Priority,

        /// <summary>
        /// The property will be bound to the messages 'correlation_id' attribute.
        /// </summary>
        CorrelationId,

        /// <summary>
        /// The property will be bound to the messages 'reply_to' attribute.
        /// </summary>
        ReplyTo,

        /// <summary>
        /// The property will be bound to the messages 'expiration' attribute.
        /// </summary>
        Expiration,

        /// <summary>
        /// The property will be bound to the messages 'message_id' attribute.
        /// </summary>
        MessageId,

        /// <summary>
        /// The property will be bound to the messages 'timestamp' attribute.
        /// </summary>
        Timestamp
    }
}