namespace EasyNetQ.MetaData {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class MetaDataMessageSerializationStrategy : IMessageSerializationStrategy {
        private readonly ITypeNameSerializer _typeNameSerializer;
        private readonly ISerializer _serializer;
        private readonly ICorrelationIdGenerationStrategy _correlationIdGenerator;

        public MetaDataMessageSerializationStrategy(ITypeNameSerializer typeNameSerializer, ISerializer serializer, ICorrelationIdGenerationStrategy correlationIdGenerator) {
            if (typeNameSerializer == null)
                throw new ArgumentNullException("typeNameSerializer");

            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (correlationIdGenerator == null)
                throw new ArgumentNullException("correlationIdGenerator");

            _typeNameSerializer = typeNameSerializer;
            _serializer = serializer;
            _correlationIdGenerator = correlationIdGenerator;
        }

        public IMessage DeserializeMessage(MessageProperties properties, Byte[] body) {
            var messageType = _typeNameSerializer.DeSerialize(properties.Type);
            var messageBody = _serializer.BytesToMessage(properties.Type, body);

            ScanForBindings(messageType)
                .ForEach(binding => binding.FromMessageMetaData(properties, messageBody));

            return MessageFactory.CreateInstance(messageType, messageBody, properties);
        }

        public IMessage<T> DeserializeMessage<T>(MessageProperties properties, Byte[] body) where T : class {
            var messageBody = _serializer.BytesToMessage<T>(body);

            ScanForBindings(typeof(T))
                .ForEach(binding => binding.FromMessageMetaData(properties, messageBody));

            return new Message<T>(messageBody, properties);
        }

        public SerializedMessage SerializeMessage(IMessage message) {
            var messageBody = message.GetBody();
            var messageProperties = message.Properties;

            ScanForBindings(message.MessageType)
                .ForEach(binding => binding.ToMessageMetaData(messageBody, messageProperties));

            // Override Type property - this is critical for EasyNetQ to function...
            var typeName = _typeNameSerializer.Serialize(message.MessageType);
            messageProperties.Type = typeName;

            if (String.IsNullOrEmpty(messageProperties.CorrelationId))
                messageProperties.CorrelationId = _correlationIdGenerator.GetCorrelationId();

            var messageBytes = _serializer.MessageToBytes(messageBody);
            return new SerializedMessage(messageProperties, messageBytes);
        }

        static List<HeaderBinding> ScanForBindings(IReflect messageType) {
            var bindings = messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite)
                .Where(property => property.GetCustomAttribute<MessageHeaderAttribute>() != null)
                .Select(property => new HeaderBinding {
                    BoundProperty = property,
                    HeaderKey = property.GetCustomAttribute<MessageHeaderAttribute>().Key
                })
                .ToList();

            return bindings;
        }
    }

    class HeaderBinding {
        public PropertyInfo BoundProperty { get; set; }
        public String HeaderKey { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);
            var headerValue   = typeConverter.ConvertToInvariantString(propertyValue);

            destination.Headers[HeaderKey] = headerValue;
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var headerBytes       = (Byte[])source.Headers[HeaderKey];
            var headerStringValue = Encoding.UTF8.GetString(headerBytes);
            var typeConverter     = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue     = typeConverter.ConvertFromInvariantString(headerStringValue);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}