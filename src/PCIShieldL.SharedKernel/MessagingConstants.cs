namespace PCIShieldLib.SharedKernel
{
    public static class MessagingConstants
    {
        public static class Credentials
        {
            public const string DEFAULT_USERNAME = "guest";
            public const string DEFAULT_PASSWORD = "guest";
        }
        public static class Exchanges
        {
            public const string PCISHIELD_BUSINESSMANAGEMENT_EXCHANGE = "pciShield-queueservice-2-mq";
            public const string PCISHIELD_RABBITMQMONGOWATCHER_EXCHANGE =
                "pciShield-rabbitmqmongowatcher";
        }
        public static class NetworkConfig
        {
            public const int DEFAULT_PORT = 5672;
            public const string DEFAULT_VIRTUAL_HOST = "/";
        }
        public static class Queues
        {
            public const string FDCM_BUSINESSMANAGEMENT_IN = "fdcm-businessmanagement-in";
            public const string FDCM_PCISHIELD_IN = "fdcm-pciShield-in";
            public const string FDVCP_PCISHIELD_IN = "fdvcp-pciShield-in";
            public const string FDVCP_RABBITMQMONGOWATCHER_IN = "fdvcp-rabbitmqmongowatcher-in";
        }
    }
}