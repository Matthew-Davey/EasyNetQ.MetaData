namespace EasyNetQ.MetaData.Abstractions {
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Indicates that this property will be mapped to a message attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MessagePropertyAttribute : Attribute {
        /// <summary>
        /// Initializes a new instances of the <see cref="MessagePropertyAttribute"/> class.
        /// </summary>
        /// <param name="property">The message attribute that the property will bind to.</param>
        /// <exception cref="InvalidEnumArgumentException">
        /// Thrown when the <paramref name="property"/> parameter is not a valid value.
        /// </exception>
        public MessagePropertyAttribute(Property property) {
            if (!Enum.IsDefined(typeof(Property), property))
                throw new InvalidEnumArgumentException("property", (int)property, typeof(Property));

            Property = property;
        }

        /// <summary>
        /// Gets or sets the message attribute that the property will be bound to.
        /// </summary>
        public Property Property { get; private set; }
    }
}