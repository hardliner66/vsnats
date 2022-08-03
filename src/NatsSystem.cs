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
    public abstract class NatsEvent
    {
        public string EventName { get => this.GetType().Name.RemoveFromEnd("Event"); }
    }

    class StartEvent : NatsEvent
    {
    }

    abstract class PlayerEvent : NatsEvent
    {
        public string PlayerName { get; private set; }
        public PlayerEvent(string player_name)
        {
            PlayerName = player_name;
        }
    }

    class PlayerJoinEvent : PlayerEvent
    {
        public PlayerJoinEvent(string player_name) : base(player_name)
        {
        }
    }

    class PlayerDisconnectEvent : PlayerEvent
    {
        public PlayerDisconnectEvent(string player_name) : base(player_name)
        {
        }
    }

    class PlayerChatEvent : PlayerEvent
    {
        public PlayerChatEvent(string player_name, int channel_id, string message, string data) : base(player_name)
        {
            ChannelId = channel_id;
            Message = message;
            Data = data;
        }

        public int ChannelId { get; private set; }
        public string Message { get; private set; }
        public string Data { get; private set; }
    }

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

            if (string.IsNullOrWhiteSpace(config.NatsPrefix))
            {
                config.NatsPrefix = "vintage_story";
                dirty = true;
            }

            if (dirty)
            {
                api.StoreModConfig(config, "nats.json");
            }

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = config.Url;
            nats = new ConnectionFactory().CreateConnection(opts);

            string GLOBAL_EVENTS = $"{config.NatsPrefix}.{config.ServerId}";
            string PLAYER_EVENTS = $"{GLOBAL_EVENTS}.player";

            nats.PublishTyped(GLOBAL_EVENTS, new StartEvent());

            api.Event.PlayerJoin += (player) =>
            {
                nats.PublishTyped<PlayerJoinEvent>(GLOBAL_EVENTS, new PlayerJoinEvent(player.PlayerName));
            };

            api.Event.PlayerDisconnect += (player) =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new PlayerDisconnectEvent(player.PlayerName));
            };

            api.Event.PlayerChat += (IServerPlayer player, int channelId, ref string message, ref string data, BoolRef consumed) =>
            {
                consumed.SetValue(false);
                nats.PublishTyped(GLOBAL_EVENTS, new PlayerChatEvent(player.PlayerName, channelId, message, data));
            };
        }
    }
}
