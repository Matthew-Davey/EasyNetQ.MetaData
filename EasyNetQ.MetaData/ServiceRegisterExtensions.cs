namespace EasyNetQ.MetaData {
    using System;

    public static class ServiceRegisterExtensions {
        public static IServiceRegister EnableMessageMetaDataBinding(this IServiceRegister registrar) {
            if (registrar == null)
                throw new ArgumentNullException("registrar");

            registrar.Register<IMessageSerializationStrategy>(services =>
                new MetaDataMessageSerializationStrategy(
                    services.Resolve<ITypeNameSerializer>(),
                    services.Resolve<ISerializer>(),
                    services.Resolve<ICorrelationIdGenerationStrategy>()
                )
            );

            return registrar;
        }
    }
}