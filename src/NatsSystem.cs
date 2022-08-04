using VSNats.Utility;
using VSNats.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;
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

    class ServerStartedEvent : NatsEvent
    {
    }

    class PlayerJoinEvent : NatsEvent
    {
    }

    class PlayerDisconnectEvent : NatsEvent
    {
    }

    class PlayerChatEvent : NatsEvent
    {
        public PlayerChatEvent(int channel_id, string message, string data)
        {
            ChannelId = channel_id;
            Message = message;
            Data = data;
        }

        public int ChannelId { get; private set; }
        public string Message { get; private set; }
        public string Data { get; private set; }
    }

    class PlayerCreateEvent : NatsEvent { }

    class PlayerDeathEvent : NatsEvent
    {
        public PlayerDeathEvent(DamageSource damageSource)
        {
            DamageSource = damageSource;
        }

        public DamageSource DamageSource { get; private set; }
    }

    class PlayerLeaveEvent : NatsEvent { }
    class PlayerNowPlayingEvent : NatsEvent { }
    class PlayerRespawnEvent : NatsEvent { }
    class PlayerSwitchGameModeEvent : NatsEvent
    {
        public PlayerSwitchGameModeEvent(EnumGameMode gameMode)
        {
            GameMode = gameMode;
        }
        public EnumGameMode GameMode { get; private set; }
    }

    class OnPlayerInteractEntityEvent : NatsEvent
    {
        public OnPlayerInteractEntityEvent(Entity entity, ItemSlot slot, Vec3d hitPosition, int mode)
        {
            Entity = entity;
            Slot = slot;
            HitPosition = hitPosition;
            Mode = mode;
        }
        public Entity Entity { get; private set; }
        public ItemSlot Slot { get; private set; }
        public Vec3d HitPosition { get; private set; }
        public int Mode { get; private set; }
    }

    class BeforeActiveSlotChangedEvent : NatsEvent
    {
        public BeforeActiveSlotChangedEvent(ActiveSlotChangeEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
        public ActiveSlotChangeEventArgs EventArgs { get; private set; }
    }

    class AfterActiveSlotChangedEvent : NatsEvent
    {
        public AfterActiveSlotChangedEvent(ActiveSlotChangeEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
        public ActiveSlotChangeEventArgs EventArgs { get; private set; }
    }

    class SaveGameLoadedEvent : NatsEvent { }

    class SaveGameCreatedEvent : NatsEvent { }

    class GameWorldSaveEvent : NatsEvent { }

    class ServerSuspendEvent : NatsEvent { }

    class ServerResumeEvent : NatsEvent { }

    class DidPlaceBlockEvent : NatsEvent {
        public DidPlaceBlockEvent(int oldblockId, BlockSelection blockSelection, ItemStack withItemStack) {
            OldblockId = oldblockId;
            BlockSelection = blockSelection;
            WithItemStack = withItemStack;
        }
        public int OldblockId { get; private set; }
        public BlockSelection BlockSelection { get; private set; }
        public ItemStack WithItemStack { get; private set; }
    }

    class CanPlaceOrBreakBlockEvent : NatsEvent {
        public CanPlaceOrBreakBlockEvent(BlockSelection blockSelection) {
            BlockSelection = blockSelection;
        }
        public BlockSelection BlockSelection { get; private set; }
    }

    class BreakBlockEvent : NatsEvent {
        public BreakBlockEvent(BlockSelection blockSelection, float dropQuantityMultiplier) {
            BlockSelection = blockSelection;
            DropQuantityMultiplier = dropQuantityMultiplier;
        }
        public BlockSelection BlockSelection { get; private set; }
        public float DropQuantityMultiplier { get; private set; }
    }

    class DidBreakBlockEvent : NatsEvent
    {
        public DidBreakBlockEvent(int oldblockId, BlockSelection blockSelection)
        {
            OldblockId = oldblockId;
            BlockSelection = blockSelection;
        }
        public int OldblockId { get; private set; }
        public BlockSelection BlockSelection { get; private set; }
    }

    class DidUseBlockEvent : NatsEvent
    {
        public DidUseBlockEvent(BlockSelection blockSelection)
        {
            BlockSelection = blockSelection;
        }
        public BlockSelection BlockSelection { get; private set; }
    }

    class ChunkColumnLoadedEvent : NatsEvent { }

    class ChunkColumnUnloadedEvent : NatsEvent { }

    class MapRegionLoadedEvent : NatsEvent { }

    class ChunkColumnSnowUpdateEvent : NatsEvent { }

    class OnTrySpawnEntityEvent : NatsEvent { }

    class MapRegionUnloadedEvent : NatsEvent { }

    /// <summary> Main system for the "VSNats" mod. </summary>
    public class NatsSystem : ModSystem
    {
        public static string MOD_ID = "VSNats";

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
            var config_name = $"{MOD_ID}.json";
            var config = api.LoadModConfig<NatsConfig>(config_name);
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
                api.StoreModConfig(config, config_name);
            }

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = config.Url;
            nats = new ConnectionFactory().CreateConnection(opts);

            string GLOBAL_PREFIX = $"{config.NatsPrefix}.servers.{config.ServerId}"
            string GLOBAL_EVENTS = $"{GLOBAL_PREFIX}.events";
            var GetPlayerPrefix = (IPlayer player) => $"{GLOBAL_PREFIX}.players.{player.PlayerName}";
            var GetPlayerEventSubject = (IPlayer player) => $"{GetPlayerPrefix(player)}.events";

            nats.PublishTyped(GLOBAL_EVENTS, new ServerStartedEvent());

            api.Event.PlayerChat += (IServerPlayer player, int channelId, ref string message, ref string data, BoolRef consumed) =>
            {
                consumed.SetValue(false);
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerChatEvent(channelId, message, data));
            };

            api.Event.PlayerCreate += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerCreateEvent());
            };

            api.Event.PlayerDeath += (player, damageSource) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerDeathEvent(damageSource));
            };

            api.Event.PlayerDisconnect += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerDisconnectEvent());
            };

            api.Event.PlayerJoin += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerJoinEvent());
            };

            api.Event.PlayerLeave += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerLeaveEvent());
            };

            api.Event.PlayerNowPlaying += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerNowPlayingEvent());
            };

            api.Event.PlayerRespawn += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerRespawnEvent());
            };

            api.Event.PlayerSwitchGameMode += (player) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new PlayerSwitchGameModeEvent(player.WorldData.CurrentGameMode));
            };

            api.Event.OnPlayerInteractEntity += (Entity entity, IPlayer player, ItemSlot slot, Vec3d hitPosition, int mode, ref EnumHandling handling) =>
            {
                handling = EnumHandling.PassThrough;
                nats.PublishTyped(GetPlayerEventSubject(player), new OnPlayerInteractEntityEvent(entity, slot, hitPosition, mode));
            };

            api.Event.BeforeActiveSlotChanged += (player, ev) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new BeforeActiveSlotChangedEvent(ev));
                return EnumHandling.PassThrough;
            };

            api.Event.AfterActiveSlotChanged += (player, ev) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new AfterActiveSlotChangedEvent(ev));
            };

            api.Event.DidBreakBlock += (player, oldblockId, blockSelection) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new DidBreakBlockEvent(oldblockId, blockSelection));
            };

            api.Event.DidUseBlock += (player, blockSelection) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new DidUseBlockEvent(blockSelection));
            };

            api.Event.DidPlaceBlock += (player, oldblockId, blockSelection, withItemStack) => {
                nats.PublishTyped(GetPlayerEventSubject(player), new DidPlaceBlockEvent(oldblockId, blockSelection, withItemStack));
            };

            api.Event.SaveGameLoaded += () =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new SaveGameLoadedEvent());
            };

            api.Event.SaveGameCreated += () =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new SaveGameCreatedEvent());
            };

            api.Event.GameWorldSave += () =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new GameWorldSaveEvent());
            };

            api.Event.ServerSuspend += () =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new ServerSuspendEvent());
                return EnumSuspendState.Ready;
            };

            api.Event.ServerResume += () =>
            {
                nats.PublishTyped(GLOBAL_EVENTS, new ServerResumeEvent());
            };

            // crashes the server
            // api.Event.CanPlaceOrBreakBlock += (player, blockSelection) =>
            // {
            //     nats.PublishTyped(GetPlayerSubject(player), new CanPlaceOrBreakBlockEvent(blockSelection));
            //     // todo: check if returning true makes problems
            //     return true;
            // };

            // crashes the server
            // api.Event.BreakBlock += (IServerPlayer player, BlockSelection blockSelection, ref float dropQuantityMultiplier, ref EnumHandling handling) =>
            // {
            //     nats.PublishTyped(GetPlayerSubject(player), new BreakBlockEvent(blockSelection, dropQuantityMultiplier));
            //     handling = EnumHandling.PassThrough;
            // };

            // api.Event.ChunkColumnLoaded += (chunkCoord, chunks) => { };

            // api.Event.ChunkColumnUnloaded += (chunkCoord) => { };

            // api.Event.MapRegionLoaded += (mapCoord, region) => { };

            // api.Event.ChunkColumnSnowUpdate += (mapChunk, chunkX, chunkZ, chunks) => { };

            // api.Event.OnTrySpawnEntity += (ref EntityProperties properties, Vec3d spawnPosition, long herdId) =>
            // {
            //     return true;
            // };

            // api.Event.MapRegionUnloaded += (mapCoord, region) => { };
        }
    }
}
