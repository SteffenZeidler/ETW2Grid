namespace ETWDeserializer
{
    using System;

    public sealed class EventMetadata
    {
        internal EventMetadata(Guid providerGuid, ushort id, byte version, string name, PropertyMetadata[] properties, string message = "")
        {
            this.ProviderGuid = providerGuid;
            this.Id = id;
            this.Version = version;
            this.Name = name;
            this.Properties = properties;
            this.Message = message;
        }

        public Guid ProviderGuid { get; private set; }

        public ushort Id { get; private set; }

        public byte Version { get; private set; }

        public string Name { get; private set; }

        public PropertyMetadata[] Properties { get; private set; }

        public string Message { get; private set; }
    }
}