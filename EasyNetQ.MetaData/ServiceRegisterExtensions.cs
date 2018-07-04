namespace EasyNetQ.MetaData {
    using System;
    using EasyNetQ.DI;

    /// <summary>
    /// Defines extension methods for the <see cref="IServiceRegister"/> interface.
    /// </summary>
    public static class ServiceRegisterExtensions {
        /// <summary>
        /// Enables message meta-data binding for the bus.
        /// </summary>
        /// <param name="registrar">The <see cref="IServiceRegister"/>instance.</param>
        /// <returns>The <see cref="IServiceRegister"/>instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="registrar"/> parameter is <c>null</c>.
        /// </exception>
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