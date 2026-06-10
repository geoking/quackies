# Quackies Architecture

Quackies is split into a reusable core library, a Unity presentation layer, and a command-line debug front end.

## Core

`Quackies.Core` contains platform-independent game domain code. It must never reference `UnityEngine`, Unity packages, Unity asset APIs, MonoBehaviours, ScriptableObjects, or any Unity runtime type.

Keeping the core independent makes it possible to build, test, and reason about the game rules without loading Unity.

## Unity

`Quackies.Unity` is reserved for the future Unity project. Unity is only the presentation layer: it should render state, collect player input, play effects, and adapt Unity-specific lifecycle events to the core model.

Unity-specific code may reference `Quackies.Core`, but `Quackies.Core` must not reference Unity-specific code.

## CLI

`Quackies.Cli` is a debug and test front end. It can be used to exercise core behavior from a terminal while developing, reproducing issues, or writing quick diagnostics.

The CLI is not the authoritative game implementation. It should stay thin and delegate domain behavior to `Quackies.Core`.
