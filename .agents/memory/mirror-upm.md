---
name: Mirror UPM install
description: Correct package name, version, and registry for Mirror Networking in Unity UPM
---

The correct Mirror Networking package details for manifest.json:

**Package name:** `com.mirrornetworking.mirror` (no dashes; all dots)
**Wrong name used historically:** `com.mirror-networking.mirror` — does NOT exist on OpenUPM

**Version (stable, Unity 2022.3 compatible):** `89.8.0`

**Registry:**
```json
"scopedRegistries": [{
  "name": "OpenUPM",
  "url": "https://package.openupm.com",
  "scopes": ["com.mirrornetworking"]
}]
```

**Why:** Mirror's GitHub repo default branch (master) has no package.json at root. The `upm` branch does not exist. The only reliable install is OpenUPM with the exact package name above.

**Assembly names inside the package** (for .asmdef references): `Mirror` and `kcp2k` — these are stable across versions.
