# Project Overview

- **Game Title:** XR Podcast Prototype (IP2A)
- **High-Level Concept:** A spatial, XR-native podcast browsing and listening experience. The player walks/teleports through a sequence of compact rooms to browse podcasts, pick a chapter, and listen while favouriting content with physical gestures.
- **Players:** Single player (first-person / XR-ready).
- **Inspiration / Reference Games:** Spatial VR media hubs (e.g., Bigscreen, Meta Quest Home media panels), diegetic spatial UI.
- **Tone / Art Direction:** Clean, minimal, room-scale spatial UI using primitive geometry + TextMeshPro.
- **Target Platform:** StandaloneOSX (desktop) now; architecture kept XR-ready for future hand tracking.
- **Screen Orientation / Resolution:** Landscape, desktop.
- **Render Pipeline:** URP (`PC_RPAsset`).

## Scope & Guardrails

**In scope (may modify):** `EpisodeRoom`, `ChapterPortal`, `PodcastExperience` — their internal geometry, hierarchy, transforms, UI presentation, and supporting interaction scripts.

**Out of scope (must NOT change):** `Podcast Hub` (Environment), `SharedCorridor`, `Player` + `CharacterController` + `PlayerMovement`, the global `NavigationManager` / `NavigationPanel` / navigation system, and podcast/chapter **content data** (episode titles/durations, chapter titles/times).

**Reuse-first principle:** Adapt existing managers (`EpisodeManager`, `ChapterManager`), `DoorController`, `ChapterDoorTeleport`, `HeartDrag`. Only add small supporting scripts and additive (non-breaking) public accessors. No manager rewrites, no data changes.

**Flow preserved:** Podcast Hub → Shared Corridor → Episode Room → Chapter Portal → Podcast Experience.

---

# Current State (verified)

## Scene roots
`Directional Light`, `Global Volume`, `Environment` (Hub + SharedCorridor), `TopicDoors`, `EpisodeRoom` (z=9.5), `ChapterPortal` (z=60), `PodcastExperience` (z=80), `NavigationPanel` (z=63.4, starts inactive), `NavigationManager`, `HubTeleportTarget`, `Player` (z=-1.5).

## EpisodeRoom (z=9.5) — carries `EpisodeManager`
- `Room` (already resized this session: Ground 5×0.1×5, BackWall 5w×3h, Left/RightWall 5long×3h, front open toward corridor). **Geometry step already complete.**
- `EpisodeUI > EpisodeCard` (single card w/ `Cover`, `EpisodeTitle`, `EpisodeDuration`) at world z≈12.3 (currently behind the new back wall).
- `Buttons > PreviousButton / NextButton / SelectButton` (`EpisodeButton`, `SelectEpisode`) at world z≈12.3.
- `EpisodeTeleportTarget` local (0,1.7,-1.5) → world (0,1.7,8.0).
- `EpisodeManager` refs: titleText=EpisodeTitle, durationText=EpisodeDuration, coverRenderer=Cover, player=**Main Camera**, chapterTarget=ChapterTeleportTarget, navigationPanel=NavigationPanel. Episodes: `Future of AI`/20mins, `Digital Life`/35mins, `AI Ethics`/28mins.

## ChapterPortal (z=60) — carries `ChapterManager`
- `ChapterTeleportTarget` local (0,2,-3).
- `PortalRoom`: Ground 10×0.5×8, walls 6m high — **oversized, needs compacting to 5×5×3**.
- `CurrentChapterDoor` at local (-2.4,0.5,3.6): `DoorPivot`(DoorController) > `DoorPanel`(ChapterDoorTeleport) + `DoorHandle`(DoorHandleClick); frames; `ChapterTitle` + `ChapterTime` (TMP, bound to ChapterManager).
- Chapters (data): Intro `00:00-05:20`, AI Basics `05:20-12:40`, Interview `12:40-18:20`, Ending `18:20-23:10`.
- No ChapterButton / chapter-station objects exist yet.

## PodcastExperience (z=80)
- `ExperienceTeleportTarget` local (0,2,-3).
- `ExperienceRoom`: Ground 10×0.5×8, walls 6m high — **oversized, needs compacting to 5×5×3**.
- `PodcastContent > PodcastCover / ExperienceTitle / Subtitle`.
- `FavouriteArea` (local -3.1,2.3,3.55) contains BOTH `HeartShelf > Heart_1`(HeartDrag, kinematic RB, sphere collider) AND `FavouriteDropAre`(trigger, tag `FavouriteArea`) `> HeartSnapPoint`. **Shelf and drop-area are on the same side — must be split to opposite sides.**
- `NavigationButton`(ToggleNavigationPanel).

## Reusable scripts
`EpisodeManager`, `ChapterManager`, `DoorController`, `ChapterDoorTeleport`, `DoorHandleClick`, `HeartDrag`, `NavigationVisibility`, `ToggleNavigationPanel`.

---

# Game Mechanics

## Core Gameplay Loop
Enter Episode Room → **swipe** through a spatial podcast carousel → **two-hand enlarge** the chosen card → auto-portal to Chapter Portal → **explore** chapter stations, **proximity-preview**, **explicitly select** a chapter → the Current Chapter Door updates → **push door** → arrive in Podcast Experience → **grab the Heart** and **place it in the Favourite Area** to favourite the podcast.

## Controls and Input Methods (desktop now, XR-ready)
Because no XR Interaction Toolkit / hand-tracking package is installed, all new gestures are implemented behind a thin intent boundary so the same methods can later be driven by XR hands.

- **Horizontal swipe (browse):** desktop = horizontal mouse drag over the carousel + Left/Right arrow fallback. Swipe left = next, swipe right = previous. Single public intent method `Swipe(int direction)` on `PodcastCarousel`; desktop input is separate and replaceable by XR.
- **Enlarge current card (enter):** desktop = mouse-wheel up and/or drag-based growth while interacting with the CURRENT card; card visibly grows; past a clear threshold it invokes the **existing** episode selection/transition (`EpisodeManager.SelectCurrentEpisode()`, which teleports to `ChapterTeleportTarget`). No second teleport system. XR-ready intent boundary: `BeginEnlarge()`, `UpdateEnlarge(float amount01)`, `Confirm()`.
- **Chapter proximity preview:** player trigger volume around each station → highlight ONLY (never auto-selects).
- **Chapter explicit select:** desktop click on station → reuse `ChapterManager.ShowChapter(index)`; single-selection highlight; this is the existing behaviour that updates the Current Chapter Door.
- **Grab & place heart:** existing `HeartDrag` mouse grab/drag; snap on trigger enter into `FavouriteArea` (unchanged mechanic), plus optional non-invasive visual confirmation that does not alter favourite-state logic.

## Global Implementation Rules (from reviewer)
- 1 Unity unit ≈ 1 metre.
- Prefer LOCAL positions derived from each room's teleport target position + `forward` direction; avoid hard-coded world coordinates.
- No negative scales in any new/edited object. (Note: existing `FavouriteDropAre` has scale z = -0.15 — move it *as-is* without touching its scale to preserve `HeartDrag` snap behaviour; do not introduce new negatives.)
- No colliders placed across room entrances.
- Do not delete working objects/scripts — disable only.
- Do not duplicate application state; reuse `EpisodeManager` / `ChapterManager` as the single source of truth.
- Verified real APIs (do NOT invent): `EpisodeManager.NextEpisode()/PreviousEpisode()/SelectCurrentEpisode()`, `episodes[]` (title/duration/coverMaterial); `ChapterManager.ShowChapter(int)/GetCurrentChapterIndex()/GetCurrentChapterTitle()`, `chapters[]` (title/time); `DoorController.ToggleDoor()/CloseDoor()`; `ChapterDoorTeleport` (OnMouseDown driven); `HeartDrag.snapPoint`.
- Neighbour (prev/next) cards are VISUAL ONLY — no interaction scripts, no serialized `EpisodeManager` references.
- No XR package install and no real hand tracking in this pass.

---

# UI

Diegetic, world-space (no screen-space canvas changes). Wireframes (top-down, user facing +Z):

**Episode Room carousel**
```
      [ prev card ]   [  CURRENT CARD  ]   [ next card ]
        (small,          (large, Y~1.5,        (small,
        angled)          1.5-2m ahead)         angled)
                     ^  user (teleport z=8) enters facing +Z
```

**Chapter Portal (shallow arc)**
```
        (Intro) (AI Basics) (Interview) (Ending)   <- chapter stations, arc
                     |
              [ Current Chapter Door ]  <- beyond the arc, the exit
                     ^ user
```

**Podcast Experience**
```
 [Heart Shelf]        (Podcast Cover / Title)        [Favourite Area]
  Y~1.0-1.3            primary landmark ahead          (~1.5-2m from shelf)
      \______________ user in middle ______________/
```

---

# Key Asset & Context

## Scripts to ADD
- `Assets/Scripts/PodcastCarousel.cs` — positions the 3 cards relative to `EpisodeTeleportTarget` (current largest/centred ~1.5–2m ahead at Y≈1.4–1.6; neighbours smaller, offset horizontally, slightly back, angled toward user). Exposes `Swipe(int direction)` → calls `EpisodeManager.NextEpisode()/PreviousEpisode()`; refreshes card visuals from `EpisodeManager` episode data on change. Neighbour cards are visual-only.
- `Assets/Scripts/SwipeInput.cs` — desktop swipe detector (horizontal mouse drag + Left/Right arrow), calls `PodcastCarousel.Swipe()`. Thin, XR-replaceable.
- `Assets/Scripts/CardEnlargeGesture.cs` — desktop SIMULATION of the future two-hand enlarge (not real hand tracking). On the CURRENT card; `BeginEnlarge()/UpdateEnlarge(float amount01)/Confirm()`; grows card scale; past threshold calls existing `EpisodeManager.SelectCurrentEpisode()` then resets scale. Desktop driver: mouse-wheel/drag while interacting with the current card. No new teleport system.
- `Assets/Scripts/ChapterStation.cs` — per station; stores ONLY `chapterIndex` and reads title/time from `ChapterManager.chapters[]` (no duplicated content/state). Proximity highlight (OnTriggerEnter/Exit with Player, preview only) + explicit select (OnMouseDown) → `ChapterManager.ShowChapter(chapterIndex)`; requests single-selection highlight via controller.
- `Assets/Scripts/ChapterStationController.cs` (small) — tracks the 4 stations, enforces single-selection highlight. Selection state lives in `ChapterManager` (no duplicate state).
- `Assets/Scripts/FavouritePlacementFeedback.cs` — optional non-invasive snap confirmation (e.g., emissive pulse / scale pop); does NOT touch favourite-state logic.

## Scripts to MODIFY (minimal / additive only — do more only if required)
- `EpisodeManager.cs` — add read-only accessors ONLY if required by the carousel (e.g., `int CurrentIndex`, `EpisodeData GetEpisode(int i)`). Add `OnEpisodeChanged` ONLY if it materially simplifies carousel sync; otherwise `PodcastCarousel` refreshes directly after calling the existing `NextEpisode()/PreviousEpisode()`. No behaviour/data change; existing methods untouched.
- `HeartDrag.cs` — only if needed, add a hook to invoke `FavouritePlacementFeedback` on successful snap. Core drag/snap/state and `snapPoint` logic unchanged.

## Scripts REUSED unchanged
`ChapterManager`, `DoorController`, `ChapterDoorTeleport`, `DoorHandleClick`, `NavigationVisibility`, `ToggleNavigationPanel`, `PlayerMovement`.

## Existing references that could break (mitigation)
- `EpisodeManager.titleText/durationText/coverRenderer` point at the old single `EpisodeCard` children. Mitigation: **keep the old EpisodeCard objects alive** and repurpose them as the CURRENT carousel card, so serialized refs stay valid; disable only the visible Prev/Next/Select buttons after the carousel works.
- `EpisodeButton`/`SelectEpisode` live on the Buttons; they will be **disabled, not deleted**, until the swipe/enlarge path is verified.
- `ChapterDoorTeleport` on `DoorPanel` references door/manager/player/target — untouched; still the exit mechanic.
- `HeartDrag.snapPoint` → `HeartSnapPoint` (child of FavouriteDropAre). When splitting layout, move the whole `FavouriteDropAre` (with its `HeartSnapPoint`) so the ref stays valid.

---

# Implementation Steps

**Execution order (strict):** (A) Episode Room — complete & verify → (B) Chapter Portal — complete & verify → (C) Podcast Experience — complete & verify → (D) final integration test. After each room, compile and check the Console; fix only errors caused by these changes. Disable obsolete Episode buttons only after the swipe+enlarge replacement works.

### Stage 0 — Confirm baseline (EpisodeRoom geometry already done)
- **Description:** Verify EpisodeRoom `Room` is 5×5×3 with open front (already applied last session). No change unless drifted.
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes

### Stage 1 — Minimal EpisodeManager access
- **Description:** Inspect `EpisodeManager` first. Add only the minimum read-only access needed by the carousel (e.g. `CurrentIndex` / `GetEpisode(int)`) if not already available. Add `OnEpisodeChanged` ONLY if it materially simplifies synchronization; otherwise the carousel refreshes directly after existing `NextEpisode()/PreviousEpisode()`. Do not change existing behaviour, data, or public methods.
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes

### Stage 2 — Episode Room carousel geometry
- **Description:** Repurpose existing `EpisodeCard` as the CURRENT card (keeps EpisodeManager refs valid). Create two VISUAL-ONLY neighbour cards (prev/next) as primitives with TMP title + cover renderer — no interaction scripts, no manager refs. Position ALL cards relative to `EpisodeTeleportTarget` (position + forward): current largest/centred ~1.5–2m ahead at Y≈1.4–1.6; neighbours smaller, offset ±X, slightly further back, yaw-angled toward user. Add `PodcastCarousel` on `EpisodeUI` referencing the 3 cards + `EpisodeManager`; neighbours display adjacent episode titles via accessors.
- **Assigned role:** developer
- **Dependencies:** Stage 1
- **Parallelizable:** No

### Stage 3 — Swipe browsing
- **Description:** Add `SwipeInput` + wire to `PodcastCarousel.Swipe()`. Swipe left → `NextEpisode()`, right → `PreviousEpisode()`; carousel refreshes on `OnEpisodeChanged`. Keep architecture XR-mappable (intent method boundary).
- **Assigned role:** developer
- **Dependencies:** Stage 2
- **Parallelizable:** No

### Stage 4 — Enlarge current card → portal
- **Description:** Add `CardEnlargeGesture` to the CURRENT card. Desktop simulation grows the card via mouse wheel and/or drag interaction. Crossing the threshold calls the existing `EpisodeManager.SelectCurrentEpisode()` (existing teleport to ChapterTeleportTarget), then resets the card scale. Preserve `BeginEnlarge()`, `UpdateEnlarge(float amount01)`, and `Confirm()` as the XR-ready intent boundary.
- **Assigned role:** developer
- **Dependencies:** Stage 3
- **Parallelizable:** No

### Stage 5 — Disable obsolete Episode buttons
- **Description:** After Stages 3–4 verified, `SetActive(false)` on `PreviousButton`, `NextButton`, `SelectButton` (NOT deleted; refs preserved). Confirm no NullRefs.
- **Assigned role:** developer
- **Dependencies:** Stage 4
- **Parallelizable:** No

### Stage 6 — Chapter Portal compaction (5×5×3)
- **Description:** Resize `PortalRoom` Ground to 5×0.1×5 and walls to 3m high, centre unchanged; reposition `CurrentChapterDoor` to sit in the far wall/beyond the arc. Preserve materials/colliders and door scripts.
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes (independent room)

### Stage 7 — Chapter stations (arc + preview + explicit select)
- **Description:** Create 4 `ChapterStation` objects in a shallow arc placed relative to `ChapterTeleportTarget` (position + forward), each with TMP title + time from `ChapterManager.chapters`. Proximity trigger (Player) → highlight only (never selects); click → `ChapterManager.ShowChapter(index)`, single-select highlight via `ChapterStationController`, which drives the existing `ChapterTitle/ChapterTime` on the Current Chapter Door. Reposition the Current Chapter Door beyond the arc, integrated into the far side of the room. Door remains the explicit exit (push handle → existing `ChapterDoorTeleport`).
- **Assigned role:** developer
- **Dependencies:** Stage 6
- **Parallelizable:** No

### Stage 8 — Podcast Experience compaction (5×5×3)
- **Description:** Resize `ExperienceRoom` Ground to 5×0.1×5, walls to 3m, centre unchanged. Preserve materials/colliders.
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes (independent room)

### Stage 9 — Podcast Experience layout + heart
- **Description:** Position relative to `ExperienceTeleportTarget` (position + forward): `HeartShelf`(+`Heart_1`) on one side (Y~1.0–1.3, within reach); move the WHOLE `FavouriteDropAre` (incl. `HeartSnapPoint`) as-is to the opposite side ~1.5–2m away (do not touch its existing scale to avoid breaking `HeartDrag.snapPoint`); `PodcastCover` centred ahead as primary landmark. PRESERVE the existing `Heart_1` root GameObject + Rigidbody + Collider + HeartDrag + snapPoint; prefer resizing/restyling the existing Heart — add child visual geometry only if necessary to read as a clear 3D grabbable. Snapped heart stays attached (existing behaviour). Add non-invasive `FavouritePlacementFeedback` only if it doesn't alter favourite-state logic. `NavigationButton` stays secondary; navigation untouched.
- **Assigned role:** developer
- **Dependencies:** Stage 8
- **Parallelizable:** No

### Stage 10 — Integration verification
- **Description:** Play-test full flow end to end; confirm reused managers/data intact, no broken references, navigation untouched.
- **Assigned role:** developer
- **Dependencies:** Stages 5, 7, 9
- **Parallelizable:** No

---

# Verification & Testing

- **Episode Room:** Enter room; carousel shows prev/current/next with correct titles/durations from `EpisodeManager`. Swipe left/right cycles episodes (matches old Next/Previous). Two-hand enlarge past threshold teleports to Chapter Portal (same as old Select). Old buttons disabled, no NullRef. Room stays 5×5×3, front open, corridor connection intact.
- **Chapter Portal:** Room is 5×5×3. Four stations in arc show correct chapter titles/times. Walking near a station highlights it but does NOT select. Clicking selects, highlights single station, updates Current Chapter Door title/time via `ChapterManager.ShowChapter`. Pushing the door teleports to Podcast Experience (existing `ChapterDoorTeleport`).
- **Podcast Experience:** Room is 5×5×3. Heart Shelf and Favourite Area on opposite sides ~1.5–2m apart; heart is large, obviously grabbable. Grab → move → place snaps into Favourite Area, stays attached, shows confirmation. Navigation button still toggles panel.
- **Regressions:** Hub, SharedCorridor, Player, NavigationManager unchanged. Episode/chapter data unchanged. No console errors on load or during flow.
- **XR-readiness check:** Confirm swipe and enlarge logic sit behind intent methods (`Swipe`, `BeginEnlarge/UpdateEnlarge/Confirm`) so desktop drivers can be swapped for XR hand input without touching managers.

# Final Report Format (deliver at completion)
A concise report only (no long design write-up), containing:
1. Scripts created / modified.
2. GameObjects created / moved / disabled.
3. Reused existing systems.
4. Anything that could not be completed.
5. Whether the Console has errors.

# Not implemented now (XR unavailable)
- Real XR hand-tracking gestures (true two-hand grab distance, physical swipe). No XR Interaction Toolkit / hands package is installed. Desktop drivers stand in, but the intent-method boundary is preserved so XR input can be mapped later without changing `EpisodeManager` / `ChapterManager` / `HeartDrag` core logic.
