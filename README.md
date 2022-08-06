# Nats Connector for Vintage Story

# ‼️ ATTENTION: This project is currently alpha status. All APIs are subject to change ‼️

## Why?
To connect multiple servers of course.

This allows you to connect Vintage Story to a nats cluster, exposing sent events for all connected servers. This can be use for cross-server plugins or just locally for plugins written in another language (as long as there are bindings for that language).

## Usage Example
Download nats server from https://nats.io/download/.
Download nats cli from https://github.com/nats-io/natscli.

Start nats server in a terminal.

## Setting up a cluster
To create a cluster, check out the nats specific documentation:
https://docs.nats.io/running-a-nats-service/configuration/clustering

You should probably also check out the section on securing nats:
https://docs.nats.io/running-a-nats-service/configuration/securing_nats

```sh
nats subscribe ">"
```

Start the server with the mod loaded. This will create a GUID for the server and send events to the nats server.

## API
### Global Events
```csharp
$"{config.NatsPrefix}.servers.{config.ServerId}.events"
```
- ServerStarted
- SaveGameLoaded
- SaveGameCreated
- GameWorldSave
- ServerSuspend
- ServerResume
- ChunkColumnLoaded
- ChunkColumnUnloaded
- MapRegionLoaded
- OnTrySpawnEntity
- MapRegionUnloaded

### Player Events
```csharp
$"{config.NatsPrefix}.servers.{config.ServerId}.players.{player.PlayerName}.events"
```
- PlayerChat
- PlayerCreate
- PlayerDeath
- PlayerDisconnect
- PlayerJoin
- PlayerLeave
- PlayerNowPlaying
- PlayerRespawn
- PlayerSwitchGameMode
- OnPlayerInteractEntity
- BeforeActiveSlotChanged
- AfterActiveSlotChanged
- DidBreakBlock
- DidUseBlock
- DidPlaceBlock
- CanPlaceOrBreakBlock
- BreakBlock

### Subscribed Events
```csharp
$"{config.NatsPrefix}.servers.{config.ServerId}.players.{player.PlayerName}.events."
```
- PlayerChat - print messages from other servers to chat

### API
```csharp
// call for a specific server
$"{config.NatsPrefix}.servers.{config.ServerId}.api."
// call for all servers
$"{config.NatsPrefix}.servers.ALL.api."
```
- chat_all msg

e.g.:
```sh
# To send a message to all servers that are connected to a cluster:
nats pub "vintage_story.servers.ALL.api.chat_all" "Hello World!"
```