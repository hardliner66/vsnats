using VSNats.Utility;
using VSNats.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using NATS.Client;

[assembly: ModInfo("VSNats",
    Description = "Adds the capability to connect the server to a nats instance",
    Website = "https://github.com/hardliner66/VSNats",
    Authors = new[] { "IAmHardliner" })]
[assembly: ModDependency("game", "1.16.5")]

namespace VSNats
{
    /// <summary> Main system for the "VSNats" mod. </summary>
    public class NatsSystem : ModSystem
    {
        public static string MOD_ID = "vsnats";

        public override bool AllowRuntimeReload => true;

        // Server
        public ICoreServerAPI ServerAPI { get; private set; }

        IConnection nats;


        public override void Start(ICoreAPI api)
        {
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            ServerAPI = api;
            var config = api.LoadModConfig<NatsConfig>("nats.json");
            bool dirty = false;
            if (config == null)
            {
                config = new NatsConfig();
                dirty = true;
            }

            if (string.IsNullOrWhiteSpace(config.Url))
            {
                config.Url = "nats://127.0.0.1:4222";
                dirty = true;
            }
            if (string.IsNullOrWhiteSpace(config.ServerId))
            {
                config.ServerId = System.Guid.NewGuid().ToString();
                dirty = true;
            }

            if (dirty)
            {
                api.StoreModConfig(config, "nats.json");
            }

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = config.Url;
            nats = new ConnectionFactory().CreateConnection(opts);

            string SERVER_EVENTS = $"server.{config.ServerId}";

            nats.Publish(SERVER_EVENTS, System.Text.Encoding.ASCII.GetBytes("{\"name\":\"start\"}"));

            api.Event.PlayerJoin += (player) =>
            {
                nats.Publish(SERVER_EVENTS, System.Text.Encoding.ASCII.GetBytes($"{{\"name\":\"player_join\", \"player_name\": \"{player.PlayerName}\" }}"));
            };

            api.Event.PlayerDisconnect += (player) =>
            {
                nats.Publish(SERVER_EVENTS, System.Text.Encoding.ASCII.GetBytes($"{{\"name\":\"player_disconnect\", \"player_name\": \"{player.PlayerName}\" }}"));
            };

            api.Event.PlayerChat += (IServerPlayer player, int channelId, ref string message, ref string data, BoolRef consumed) =>
            {
                nats.Publish(SERVER_EVENTS, System.Text.Encoding.ASCII.GetBytes($"{{\"name\":\"player_chat\", \"player_name\": \"{player.PlayerName}\", \"channel_id\": \"{channelId}\", \"message\": \"{message}\", \"data\": \"{data}\" }}"));
            };
        }
    }
}
