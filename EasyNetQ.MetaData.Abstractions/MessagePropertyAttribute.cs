namespace EasyNetQ.MetaData.Abstractions {
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MessagePropertyAttribute : Attribute {
        public Property Property { get; private set; }

        public MessagePropertyAttribute(Property property) {
            if (!Enum.IsDefined(typeof(Property), property))
                throw new InvalidEnumArgumentException("property", (int)property, typeof(Property));

            Property = property;
        }
    }
}