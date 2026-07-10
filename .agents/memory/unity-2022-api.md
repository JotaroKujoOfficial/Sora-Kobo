---
name: Unity 2022.3 vs Unity 6 API differences
description: APIs that exist in Unity 6 but not Unity 2022.3 — causes compile errors in Cloud Build
---

**Rigidbody2D.linearVelocity** — Unity 6 (6000.x) only. In Unity 2022.3, use `Rigidbody2D.velocity`.

**How to apply:** Any script using `linearVelocity` will fail to compile in Unity 2022.3.45f1 with a "member not found" error. This is a silent trap because Unity 6 docs show linearVelocity but 2022.3 LTS does not have it.

**Why:** Unity renamed `velocity` to `linearVelocity` in Unity 6 as part of physics API cleanup. The project targets 2022.3.45f1.
