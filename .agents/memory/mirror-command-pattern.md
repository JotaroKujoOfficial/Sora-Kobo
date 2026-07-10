---
name: Mirror Command pattern rules
description: Commands are client→server only; common mistakes in server-side code
---

**Rule:** `[Command]` methods are messages sent from a client to the server. Calling a `[Command]` from server-only code (`[Server]` context, `OnStartServer`, etc.) is a no-op or error in Mirror.

**Server-side code must call server methods directly**, not via Commands.

**Example mistake (SoraKobo WorldGenerator):**
```csharp
[Server]
public void Generate() {
    _building.CmdPlaceBlock("grass", pos, 1);  // WRONG: Command from server
}
```

**Correct pattern:**
```csharp
[Server]
public void Generate() {
    _building.ServerPlaceBlockPublic("grass", pos, 1);  // RIGHT: direct server call
}
```

**Scene NetworkBehaviour `isLocalPlayer`** is always `false` — scene objects are not player-owned. Use `NetworkClient.active` to guard client-side logic on scene NetworkBehaviours.

**`requiresAuthority = false` commands** are valid for public objects (doors, chairs) but should include proximity/sender validation to prevent cheating.
