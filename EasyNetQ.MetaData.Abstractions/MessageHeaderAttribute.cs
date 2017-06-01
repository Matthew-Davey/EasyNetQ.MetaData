namespace EasyNetQ.MetaData.Abstractions {
    using System;

    /// <summary>
    /// Indicates that a property should be bound to message header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MessageHeaderAttribute : Attribute {
        private readonly String _key;

        /// <summary>
        /// Initializes a new instances of the <see cref="MessageHeaderAttribute"/> class.
        /// </summary>
        /// <param name="key">The key used for the message header.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="key"/> parameter is <c>null</c>, empty, or a whitespace
        /// string.
        /// </exception>
        public MessageHeaderAttribute(String key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            _key = key;
        }

        /// <summary>
        /// Gets the message header key.
        /// </summary>
        public String Key {
            get { return _key; }
        }
    }
}