namespace EasyNetQ.MetaData {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EasyNetQ.MetaData.Bindings;

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

        static List<IMetaDataBinding> ScanForBindings(IReflect messageType) {
            var bindings = messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite)
                .SelectMany(MakeBindings)
                .ToList();

            return bindings;
        }

        static IEnumerable<IMetaDataBinding> MakeBindings(PropertyInfo property) {
            var messageHeaderAttribute = property.GetCustomAttribute<MessageHeaderAttribute>();
            var messagePropertyAttribute = property.GetCustomAttribute<MessagePropertyAttribute>();

            if (messageHeaderAttribute != null) {
                yield return new HeaderBinding {
                    BoundProperty = property,
                    HeaderKey = messageHeaderAttribute.Key
                };
            }

            if (messagePropertyAttribute != null) {
                switch (messagePropertyAttribute.Property) {
                    case Property.ContentType:
                        yield return new ContentTypeBinding { BoundProperty = property };
                        break;
                    case Property.ContentEncoding:
                        yield return new ContentEncodingBinding { BoundProperty = property };
                        break;
                    case Property.Timestamp:
                        yield return new TimestampPropertyBinding { BoundProperty = property };
                        break;
                    case Property.DeliveryMode:
                        yield return new DeliveryModeBinding { BoundProperty = property };
                        break;
                    case Property.Priority:
                        yield return new PriorityBinding { BoundProperty = property };
                        break;
                }
            }
        }
    }
}