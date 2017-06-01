namespace EasyNetQ.MetaData {
    using System;

    interface IMetaDataBinding {
        void ToMessageMetaData(Object source, MessageProperties destination);
        void FromMessageMetaData(MessageProperties source, Object destination);
    }
}