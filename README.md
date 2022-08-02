# Nats Connector for Vintage Story

## Why?
To connect multiple servers of course.

This allows you to connect Vintage Story to a nats cluster, exposing sent events for all connected servers. This can be use for cross-server plugins or just locally for plugins written in another language (as long as there are bindings for that language).

## Usage Example
Download nats server from https://nats.io/download/.
Download nats cli from https://github.com/nats-io/natscli.

Start nats server in a terminal.


```sh
nats subscribe "vintage_story.>"
```

Start the server with the mod loaded. This will create a GUID for the server and send events to the nats server.

Currently the following events are sent:
- start
- PlayerJoin
- PlayerDisconnect
- PlayerChat