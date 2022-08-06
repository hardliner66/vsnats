using VSNats.Utility;
using VSNats.Config;
using VSNats.Events;
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
        public PlayerChatEvent() { }
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
        public PlayerDeathEvent() { }
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
        public PlayerSwitchGameModeEvent() { }
        public PlayerSwitchGameModeEvent(EnumGameMode gameMode)
        {
            GameMode = gameMode;
        }
        public EnumGameMode GameMode { get; private set; }
    }

    class OnPlayerInteractEntityEvent : NatsEvent
    {
        public OnPlayerInteractEntityEvent() { }
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
        public BeforeActiveSlotChangedEvent() { }
        public BeforeActiveSlotChangedEvent(ActiveSlotChangeEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
        public ActiveSlotChangeEventArgs EventArgs { get; private set; }
    }

    class AfterActiveSlotChangedEvent : NatsEvent
    {
        public AfterActiveSlotChangedEvent() { }
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

    class DidPlaceBlockEvent : NatsEvent
    {
        public DidPlaceBlockEvent() { }
        public DidPlaceBlockEvent(int oldblockId, BlockSelection blockSelection, ItemStack withItemStack)
        {
            OldblockId = oldblockId;
            BlockSelection = new BlockSelectionWrapper(blockSelection);
            WithItemStack = withItemStack;
        }
        public int OldblockId { get; private set; }
        public BlockSelectionWrapper BlockSelection { get; private set; }
        public ItemStack WithItemStack { get; private set; }
    }

    class CanPlaceOrBreakBlockEvent : NatsEvent
    {
        public CanPlaceOrBreakBlockEvent() { }
        public CanPlaceOrBreakBlockEvent(BlockSelection blockSelection)
        {
            BlockSelection = new BlockSelectionWrapper(blockSelection);
        }
        public BlockSelectionWrapper BlockSelection { get; private set; }
    }

    class BreakBlockEvent : NatsEvent
    {
        public BreakBlockEvent() { }
        public BreakBlockEvent(BlockSelection blockSelection, float dropQuantityMultiplier)
        {
            BlockSelection = new BlockSelectionWrapper(blockSelection);
            DropQuantityMultiplier = dropQuantityMultiplier;
        }
        public BlockSelectionWrapper BlockSelection { get; private set; }
        public float DropQuantityMultiplier { get; private set; }
    }

    class DidBreakBlockEvent : NatsEvent
    {
        public DidBreakBlockEvent() { }
        public DidBreakBlockEvent(int oldblockId, BlockSelection blockSelection)
        {
            OldblockId = oldblockId;
            BlockSelection = new BlockSelectionWrapper(blockSelection);
        }
        public int OldblockId { get; private set; }
        public BlockSelectionWrapper BlockSelection { get; private set; }
    }

    class DidUseBlockEvent : NatsEvent
    {
        public DidUseBlockEvent() { }
        public DidUseBlockEvent(BlockSelection blockSelection)
        {
            BlockSelection = new BlockSelectionWrapper(blockSelection);
        }
        public BlockSelectionWrapper BlockSelection { get; private set; }
    }

    class ChunkColumnLoadedEvent : NatsEvent
    {
        public ChunkColumnLoadedEvent() { }
        public ChunkColumnLoadedEvent(Vec2i chunkCoord, IWorldChunk[] chunks)
        {
            ChunkCoord = new Vec2iWrapper(chunkCoord);
            // Chunks = chunks;
        }
        public Vec2iWrapper ChunkCoord { get; private set; }
        // public IWorldChunk[] Chunks { get; private set; }
    }

    class ChunkColumnUnloadedEvent : NatsEvent
    {
        public ChunkColumnUnloadedEvent() { }
        public ChunkColumnUnloadedEvent(Vec3i chunkCoord)
        {
            ChunkCoord = new Vec3iWrapper(chunkCoord);
        }
        public Vec3iWrapper ChunkCoord { get; private set; }
    }

    class MapRegionLoadedEvent : NatsEvent
    {
        public MapRegionLoadedEvent() { }
        public MapRegionLoadedEvent(Vec2i mapCoord, IMapRegion mapRegion)
        {
            MapCoord = new Vec2iWrapper(mapCoord);
            // MapRegion = mapRegion;
        }
        public Vec2iWrapper MapCoord { get; private set; }
        // public IMapRegion MapRegion { get; private set; }
    }

    class ChunkColumnSnowUpdateEvent : NatsEvent { }

    class OnTrySpawnEntityEvent : NatsEvent
    {
        public OnTrySpawnEntityEvent() { }
        public OnTrySpawnEntityEvent(EntityProperties properties, Vec3d spawnPosition, long herdId)
        {
            // Properties = properties;
            SpawnPosition = new Vec3dWrapper(spawnPosition);
            HerdId = herdId;
        }
        // public EntityProperties Properties { get; private set; }
        public Vec3dWrapper SpawnPosition { get; private set; }
        public long HerdId { get; private set; }
    }

    class MapRegionUnloadedEvent : NatsEvent
    {
        public MapRegionUnloadedEvent() { }
        public MapRegionUnloadedEvent(Vec2i mapCoord, IMapRegion mapRegion)
        {
            MapCoord = new Vec2iWrapper(mapCoord);
            // MapRegion = mapRegion;
        }
        public Vec2iWrapper MapCoord { get; private set; }
        // public IMapRegion MapRegion { get; private set; }
    }

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

            string SERVER_PREFIX = $"{config.NatsPrefix}.servers";
            string THIS_SERVER_PREFIX = $"{SERVER_PREFIX}.{config.ServerId}";
            string PLAYER_PREFIX = $"{SERVER_PREFIX}.{config.ServerId}.players";
            string SERVER_EVENTS = $"{SERVER_PREFIX}.{config.ServerId}.events";
            string SERVER_SUBSCRIPTION = $"{SERVER_PREFIX}.*.events";
            string PLAYER_SUBSCRIPTION = $"{SERVER_PREFIX}.*.players.*.events";
            var GetPlayerPrefix = (IPlayer player) => $"{SERVER_PREFIX}.{config.ServerId}.players.{player.PlayerName}";
            var GetPlayerEventSubject = (IPlayer player) => $"{GetPlayerPrefix(player)}.events";

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

            api.Event.DidPlaceBlock += (player, oldblockId, blockSelection, withItemStack) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new DidPlaceBlockEvent(oldblockId, blockSelection, withItemStack));
            };

            api.Event.SaveGameLoaded += () =>
            {
                nats.PublishTyped(SERVER_EVENTS, new SaveGameLoadedEvent());
            };

            api.Event.SaveGameCreated += () =>
            {
                nats.PublishTyped(SERVER_EVENTS, new SaveGameCreatedEvent());
            };

            api.Event.GameWorldSave += () =>
            {
                nats.PublishTyped(SERVER_EVENTS, new GameWorldSaveEvent());
            };

            api.Event.ServerSuspend += () =>
            {
                nats.PublishTyped(SERVER_EVENTS, new ServerSuspendEvent());
                return EnumSuspendState.Ready;
            };

            api.Event.ServerResume += () =>
            {
                nats.PublishTyped(SERVER_EVENTS, new ServerResumeEvent());
            };

            api.Event.CanPlaceOrBreakBlock += (player, blockSelection) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new CanPlaceOrBreakBlockEvent(blockSelection));
                // todo: check if returning true makes problems
                return true;
            };

            api.Event.BreakBlock += (IServerPlayer player, BlockSelection blockSelection, ref float dropQuantityMultiplier, ref EnumHandling handling) =>
            {
                nats.PublishTyped(GetPlayerEventSubject(player), new BreakBlockEvent(blockSelection, dropQuantityMultiplier));
                handling = EnumHandling.PassThrough;
            };

            api.Event.ChunkColumnLoaded += (chunkCoord, chunks) =>
            {
                nats.PublishTyped(SERVER_EVENTS, new ChunkColumnLoadedEvent(chunkCoord, chunks));
            };

            api.Event.ChunkColumnUnloaded += (chunkCoord) =>
            {
                nats.PublishTyped(SERVER_EVENTS, new ChunkColumnUnloadedEvent(chunkCoord));
            };

            api.Event.MapRegionLoaded += (mapCoord, region) =>
            {
                nats.PublishTyped(SERVER_EVENTS, new MapRegionLoadedEvent(mapCoord, region));
            };

            // api.Event.ChunkColumnSnowUpdate += (mapChunk, chunkX, chunkZ, chunks) => { };

            api.Event.OnTrySpawnEntity += (ref EntityProperties properties, Vec3d spawnPosition, long herdId) =>
            {
                nats.PublishTyped(SERVER_EVENTS, new OnTrySpawnEntityEvent(properties, spawnPosition, herdId));
                return true;
            };

            api.Event.MapRegionUnloaded += (mapCoord, region) =>
            {
                nats.PublishTyped(SERVER_EVENTS, new MapRegionUnloadedEvent(mapCoord, region));
            };

            nats.SubscribeAsync($"{PLAYER_SUBSCRIPTION}.PlayerChat", (a, message) =>
            {
                var msg = message.Message;
                if (!msg.Subject.StartsWith(THIS_SERVER_PREFIX))
                {
                    var cut1 = msg.Subject.Replace($"{PLAYER_PREFIX}.", "");
                    var player_name = cut1.Replace(".events.PlayerChat", "");
                    var player = api.PlayerData.GetPlayerDataByLastKnownName(player_name);
                    var ev_string = System.Text.Encoding.UTF8.GetString(msg.Data);
                    var ev = Json.Net.JsonNet.Deserialize<PlayerChatEvent>(ev_string);

                    api.SendMessageToGroup(ev.ChannelId, $"<font color=\"#03ff2d\"><strong>external </strong></font>{ev.Message}", EnumChatType.OthersMessage, ev.Data);
                }
            });

            nats.PublishTyped(SERVER_EVENTS, new ServerStartedEvent());
        }
    }
}
