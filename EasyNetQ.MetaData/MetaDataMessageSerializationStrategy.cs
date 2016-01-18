namespace EasyNetQ.MetaData {
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using EasyNetQ.MetaData.Bindings;

    internal class MetaDataMessageSerializationStrategy : IMessageSerializationStrategy {
        readonly ITypeNameSerializer _typeNameSerializer;
        readonly ISerializer _serializer;
        readonly ICorrelationIdGenerationStrategy _correlationIdGenerator;
        readonly ConcurrentDictionary<Type, List<IMetaDataBinding>> _bindingCache;
        static readonly IDictionary<Property, Func<PropertyInfo, IMetaDataBinding>> _bindingBuilders;

        static MetaDataMessageSerializationStrategy() {
            _bindingBuilders = new Dictionary<Property, Func<PropertyInfo, IMetaDataBinding>> {
                { Property.ContentType,     (property) => new ContentTypeBinding     { BoundProperty = property } },
                { Property.ContentEncoding, (property) => new ContentEncodingBinding { BoundProperty = property } },
                { Property.Timestamp,       (property) => new TimestampBinding       { BoundProperty = property } },
                { Property.DeliveryMode,    (property) => new DeliveryModeBinding    { BoundProperty = property } },
                { Property.Priority,        (property) => new PriorityBinding        { BoundProperty = property } },
                { Property.CorrelationId,   (property) => new CorrelationIdBinding   { BoundProperty = property } },
                { Property.ReplyTo,         (property) => new ReplyToBinding         { BoundProperty = property } },
                { Property.Expiration,      (property) => new ExpirationBinding      { BoundProperty = property } },
                { Property.MessageId,       (property) => new MessageIdBinding       { BoundProperty = property } },
            };
        }

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
            _bindingCache = new ConcurrentDictionary<Type, List<IMetaDataBinding>>();
        }

        public IMessage DeserializeMessage(MessageProperties properties, Byte[] body) {
            var messageType = _typeNameSerializer.DeSerialize(properties.Type);
            var messageBody = _serializer.BytesToMessage(properties.Type, body);

            _bindingCache.GetOrAdd(messageType, ScanForBindings)
                .ForEach(binding => binding.FromMessageMetaData(properties, messageBody));

            return MessageFactory.CreateInstance(messageType, messageBody, properties);
        }

        public IMessage<T> DeserializeMessage<T>(MessageProperties properties, Byte[] body) where T : class {
            var messageBody = _serializer.BytesToMessage<T>(body);

            _bindingCache.GetOrAdd(typeof(T), ScanForBindings)
                .ForEach(binding => binding.FromMessageMetaData(properties, messageBody));

            return new Message<T>(messageBody, properties);
        }

        public SerializedMessage SerializeMessage(IMessage message) {
            var messageBody = message.GetBody();
            var messageProperties = message.Properties;

            _bindingCache.GetOrAdd(message.MessageType, ScanForBindings)
                .ForEach(binding => binding.ToMessageMetaData(messageBody, messageProperties));

            // The remainder of this method duplicates the behaviour of the default EasyNetQ message serialization
            // strategy (it would be wonderful if we could just wrap it with a decorator)...
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

            if (messageHeaderAttribute != null) {
                yield return new HeaderBinding(property, messageHeaderAttribute.Key);
                yield break;
            }

            var messagePropertyAttribute = property.GetCustomAttribute<MessagePropertyAttribute>();

            if (messagePropertyAttribute != null && _bindingBuilders.ContainsKey(messagePropertyAttribute.Property)) {
                yield return _bindingBuilders[messagePropertyAttribute.Property].Invoke(property);
            }
        }
    }
}