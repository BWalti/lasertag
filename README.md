# Lasertag Backend

This repository is aiming to provide a backend for my DIY Lasertag.

At the end there shall be:
- multiple Lasertag "Sets" (GameSets)
- a few "targets" (the target part of the GameSet)
- one Server (this repository)
- a PostgresDB as persistence

## Software

As in a Lasertag game everything _happens_ I am intending to implement the backend in an EventSourced way using MartenDB and applying some CQRS.

### Technology choices

As this whole project is not only a DIY Lasertag implementation, but also a platform for me to learn some things,
I am using quite a specific combination of technologies and some of it most certainly could be "removed" / simplified.

## Hardware

For the Hardware I am currently doing different PoCs using ESP32 and .NET nanoFramework.


## Event Storming

There are most probably different _contexts_ in a Lasertag system:
1. Infrastructure (like configuring server, (de-)registering GameSets/Targets, configuration, etc.)
2. Actual Games (like creating a Lobby, wait until all players have put on their GameSets and activated them, Shooting, getting hit, hit another player / target, etc.)
3. Game (round) Statistics
4. Personal statistics (& achievements)

Each context might have it's own events, data store, ...

Events for Infrastructure might be:
- GameServerStarted
- GameServerConfigurationChanged
- GameSetRegistered
- GameSetUnregistered

Events for actual games might be:
- LobbyOpened
- GameSetActivated
- RoundStarted
- ShotFired
- PlayerHit
- TargetHit
- RoundFinished

For statistics some events need to be correlated (like ShotFired & PlayerHit).
