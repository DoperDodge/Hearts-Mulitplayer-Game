# Hearts Multiplayer Game — Complete Design & Development Blueprint

> **Version:** 1.0
> **Date:** 2026-03-04
> **Platforms:** Android, iOS (with crossplay)
> **Players:** 1–4 (AI fill-ins for missing seats)
> **Distribution:** Google Play Store & Apple App Store

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Technical Architecture Overview](#2-technical-architecture-overview)
3. [Phase 1 — Pre-Production & Planning](#3-phase-1--pre-production--planning)
4. [Phase 2 — Core Engine & Game Logic](#4-phase-2--core-engine--game-logic)
5. [Phase 3 — UI/UX Design & Visual Assets](#5-phase-3--uiux-design--visual-assets)
6. [Phase 4 — Multiplayer & Networking](#6-phase-4--multiplayer--networking)
7. [Phase 5 — AI Opponents](#7-phase-5--ai-opponents)
8. [Phase 6 — Polish, Animation & Audio](#8-phase-6--polish-animation--audio)
9. [Phase 7 — Testing & QA](#9-phase-7--testing--qa)
10. [Phase 8 — Store Preparation & Deployment](#10-phase-8--store-preparation--deployment)
11. [Phase 9 — Post-Launch & Live Operations](#11-phase-9--post-launch--live-operations)
12. [Multiplayer Architecture Deep Dive](#12-multiplayer-architecture-deep-dive)
13. [Visual Style Guide](#13-visual-style-guide)
14. [Project Timeline & Dependencies](#14-project-timeline--dependencies)
15. [Pre-Publishing Checklist](#15-pre-publishing-checklist)

---

## 1. Executive Summary

This document is the complete development blueprint for **Hearts Multiplayer**, a cross-platform mobile card game based on the classic trick-taking game Hearts. The game targets both Android and iOS with real-time crossplay, AI opponents for solo and partial lobbies, polished animations, and a modern minimalist aesthetic.

### Key Goals

- Faithful implementation of standard Hearts rules (four-player, trick-taking, penalty-based scoring)
- Seamless cross-platform multiplayer with low-latency networking
- AI opponents that provide a satisfying challenge at multiple difficulty levels
- Publication-ready quality for both the Apple App Store and Google Play Store
- Elegant, modern visual design with smooth, satisfying card animations

---

## 2. Technical Architecture Overview

### 2.1 Recommended Game Engine: Unity (2022 LTS or later)

**Why Unity:**

| Criterion | Unity | Godot | Flutter | React Native |
|---|---|---|---|---|
| Cross-platform (iOS + Android) | Excellent | Good | Good | Fair |
| 2D card game support | Excellent | Excellent | Fair | Poor |
| Real-time multiplayer SDKs | Excellent (Netcode, Mirror, Photon) | Limited | Manual | Manual |
| Animation system | Excellent (DOTween, Animator) | Good | Basic | Basic |
| App Store track record | Industry standard | Growing | Limited for games | Not suited |
| Asset ecosystem | Massive (Asset Store) | Growing | N/A | N/A |
| Community & hiring pool | Largest | Growing | Non-game | Non-game |
| C# language | Strong typing, mature | GDScript/C# | Dart | JavaScript |

**Verdict:** Unity is the strongest choice for a polished, multiplayer card game targeting both stores. Its mature animation pipeline, extensive multiplayer networking options, and proven App Store deployment workflows make it the clear winner.

**Alternative consideration:** If the team prefers open-source and the project scope stays modest, **Godot 4.x** is a viable second choice with good 2D support and improving networking.

### 2.2 Language & Frameworks

| Layer | Technology | Purpose |
|---|---|---|
| Game client | Unity + C# | Game logic, rendering, UI, animations |
| Networking (option A) | Photon Unity Networking (PUN 2) / Photon Fusion | Real-time multiplayer, matchmaking, crossplay |
| Networking (option B) | Unity Netcode for GameObjects + Unity Relay | First-party alternative, tighter integration |
| Backend services | Firebase / PlayFab | Auth, leaderboards, analytics, cloud saves |
| AI logic | C# (within Unity) | Bot decision-making, difficulty levels |
| Build pipeline | Unity Cloud Build or Fastlane | Automated iOS/Android builds |

### 2.3 Repository Structure (Proposed)

```
Hearts-Multiplayer-Game/
├── DESIGN_DOCUMENT.md          # This file
├── README.md                   # Project overview
├── unity-project/              # Unity project root
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── Core/           # Game rules, state machine, card logic
│   │   │   ├── Networking/     # Multiplayer, matchmaking, sync
│   │   │   ├── AI/             # Bot players, difficulty tiers
│   │   │   ├── UI/             # Menus, HUD, overlays
│   │   │   └── Animation/     # Card animations, effects
│   │   ├── Prefabs/            # Card prefabs, UI elements
│   │   ├── Scenes/             # MainMenu, Gameplay, Lobby, Settings
│   │   ├── Art/
│   │   │   ├── Cards/          # Card face/back textures
│   │   │   ├── UI/             # Buttons, panels, icons
│   │   │   ├── Backgrounds/    # Table textures, environments
│   │   │   └── Fonts/          # Typography assets
│   │   ├── Audio/
│   │   │   ├── SFX/            # Card sounds, UI clicks
│   │   │   └── Music/          # Background music tracks
│   │   ├── Animations/         # Animation clips and controllers
│   │   └── Resources/          # Runtime-loaded assets
│   ├── Packages/               # Unity package manifest
│   ├── ProjectSettings/        # Unity project settings
│   └── Builds/                 # Build output (gitignored)
├── server/                     # Optional: custom server code
│   ├── matchmaking/
│   └── leaderboard/
├── docs/                       # Additional documentation
│   ├── art-style-guide.md
│   ├── api-reference.md
│   └── testing-plan.md
└── .github/
    └── workflows/              # CI/CD pipelines
```

---

## 3. Phase 1 — Pre-Production & Planning

**Duration:** 1–2 weeks
**Dependencies:** None (starting phase)

### 3.1 Objectives

- Finalize game rules and variations
- Select and validate technology stack
- Set up development environment and repository structure
- Define art direction and UX wireframes
- Establish team roles and workflow

### 3.2 Tasks

| # | Task | Details |
|---|---|---|
| 1.1 | Document Hearts rules | Write a formal rules spec covering: dealing, passing (left/right/across/hold), trick play, scoring (Queen of Spades = 13, each Heart = 1), Shooting the Moon, game-end condition (first player to 100+). Define any house-rule options to support. |
| 1.2 | Install Unity | Install Unity Hub, Unity 2022 LTS (or latest LTS), Visual Studio / Rider for C# |
| 1.3 | Initialize Unity project | Create a new 2D Unity project, configure for mobile (target resolution 1080x1920, scalable canvas) |
| 1.4 | Set up Git with LFS | Configure `.gitignore` for Unity, enable Git LFS for textures/audio/prefabs, push initial project skeleton |
| 1.5 | Create wireframes | Sketch UI wireframes for: Main Menu, Lobby/Matchmaking, Gameplay Table, Score Screen, Settings. Use Figma or Adobe XD. |
| 1.6 | Define art direction | Choose color palette, typography, card style. See [Visual Style Guide](#13-visual-style-guide). |
| 1.7 | Evaluate multiplayer SDKs | Prototype both Photon and Unity Netcode, evaluate latency/ease/cost. Select one. |
| 1.8 | Set up CI/CD | Configure Unity Cloud Build or GitHub Actions + Fastlane for automated builds |

### 3.3 Deliverables

- [ ] Formal game rules specification document
- [ ] Unity project initialized and building on both Android/iOS targets
- [ ] Git repository with LFS, branching strategy defined
- [ ] UI wireframes for all screens (Figma/XD file)
- [ ] Art style guide with color palette, typography, reference images
- [ ] Technology decision document (multiplayer SDK chosen with rationale)
- [ ] CI/CD pipeline producing test builds

### 3.4 Tools

- **Unity Hub + Unity 2022 LTS** — Game engine
- **Figma** — UI/UX wireframing and design
- **Git + GitHub + Git LFS** — Version control
- **Unity Cloud Build** or **GitHub Actions + Fastlane** — CI/CD

---

## 4. Phase 2 — Core Engine & Game Logic

**Duration:** 3–4 weeks
**Dependencies:** Phase 1 complete

### 4.1 Objectives

- Implement the complete Hearts game rules engine in C#
- Build a deterministic, testable state machine
- Support local single-device play (no networking yet)
- Validate correctness with unit tests

### 4.2 Tasks

| # | Task | Details |
|---|---|---|
| 2.1 | Design card data model | `Card` class/struct with `Suit` (enum), `Rank` (enum), `SortValue`, sprite reference. Create a `Deck` class with shuffle (Fisher-Yates), deal, reset. |
| 2.2 | Build game state machine | States: `WaitingForPlayers → Dealing → PassingCards → PlayingTricks → ScoringRound → GameOver`. Use a clean FSM pattern. Each state handles its own input/transitions. |
| 2.3 | Implement trick logic | Leading player, follow-suit enforcement, trick winner determination, Hearts-broken tracking. |
| 2.4 | Implement passing phase | Four-round pass rotation: Left, Right, Across, Hold. Player selects 3 cards, validates, transfers. |
| 2.5 | Implement scoring | Per-trick penalty accumulation. Queen of Spades = 13 pts, each Heart = 1 pt. Shooting the Moon detection (player takes all 26 penalty points → 0 for them, 26 for everyone else, OR subtract 26 from their score — make this configurable). |
| 2.6 | Implement game-end logic | Track cumulative scores across rounds. Game ends when any player reaches threshold (default 100). Lowest score wins. Handle ties. |
| 2.7 | Write unit tests | Test every rule edge case: breaking Hearts, Queen of Spades lead restrictions, Shooting the Moon, 2 of Clubs must lead first trick, no penalty cards on first trick. Use Unity Test Framework (NUnit). |
| 2.8 | Build player abstraction | `IPlayer` interface with methods like `SelectCardsToPass()`, `PlayCard()`, `OnTrickComplete()`. Concrete implementations: `HumanPlayer`, `AIPlayer`, `NetworkPlayer`. |

### 4.3 Deliverables

- [ ] Complete `Card`, `Deck`, `Hand` data models
- [ ] Fully functional game state machine
- [ ] Passing, trick-play, scoring, and game-end logic
- [ ] `IPlayer` abstraction with human-input implementation
- [ ] Unit test suite with 90%+ coverage on game logic
- [ ] A playable local game (debug UI acceptable) demonstrating all rules

### 4.4 Key Architecture Decisions

**State Machine Pattern:**
```
┌──────────────┐
│ WaitingFor   │
│  Players     │
└──────┬───────┘
       │ 4 players ready
       ▼
┌──────────────┐
│  Dealing     │──────────────────────┐
└──────┬───────┘                      │
       │ 13 cards each                │
       ▼                              │
┌──────────────┐                      │
│  Passing     │ (skip on Hold round) │
│  Cards       │                      │
└──────┬───────┘                      │
       │ passes resolved              │
       ▼                              │
┌──────────────┐                      │
│  Playing     │◄─── next trick ──┐   │
│  Tricks      │                  │   │
└──────┬───────┘                  │   │
       │ trick complete           │   │
       ▼                          │   │
┌──────────────┐    13 tricks     │   │
│  Trick       ├──── not done ────┘   │
│  Scored      │                      │
└──────┬───────┘                      │
       │ round complete               │
       ▼                              │
┌──────────────┐                      │
│  Round       │── no one ≥100 ───────┘
│  Scored      │                 (next round)
└──────┬───────┘
       │ someone ≥ 100
       ▼
┌──────────────┐
│  Game Over   │
└──────────────┘
```

**Separation of concerns:** The game logic must be completely independent of rendering, networking, and input. This allows:
- Unit testing without Unity runtime
- Deterministic replay
- Server-authoritative validation (if needed later)

---

## 5. Phase 3 — UI/UX Design & Visual Assets

**Duration:** 3–4 weeks (overlaps with Phase 2)
**Dependencies:** Phase 1 wireframes complete

### 5.1 Objectives

- Build all game screens with responsive, polished UI
- Create or source all visual assets (cards, backgrounds, icons)
- Implement adaptive layout for different screen sizes and aspect ratios

### 5.2 Tasks

| # | Task | Details |
|---|---|---|
| 3.1 | Build Main Menu scene | Logo, Play (Online / vs AI), Settings, Leaderboard, Profile buttons. Animated background (subtle card particles or parallax). |
| 3.2 | Build Lobby/Matchmaking screen | Player slots (1–4), ready indicators, AI fill toggle, room code for private games, "Quick Match" for public. |
| 3.3 | Build Gameplay table | Card fan (player hand), four player positions, trick pile (center), score overlay, turn indicator, Hearts-broken indicator. |
| 3.4 | Build card passing UI | Selection highlights, confirm button, directional indicator showing pass direction this round. |
| 3.5 | Build Score screen | Round-by-round score table, running totals, winner announcement with celebration animation. |
| 3.6 | Build Settings screen | Sound toggle, music volume, notification preferences, account settings, rules-variant toggles. |
| 3.7 | Create/source card assets | 52 cards + card back. Options: (A) Commission custom art. (B) Purchase high-quality asset pack. (C) Use open-source card assets and apply custom styling. Recommend option B or C initially, upgrade later. |
| 3.8 | Create UI assets | Buttons, panels, icons, avatars. Follow the style guide. Use 9-slice sprites for scalable panels. |
| 3.9 | Create table/background art | Felt table texture, subtle gradient or pattern backgrounds for menus. |
| 3.10 | Implement responsive canvas | Unity Canvas Scaler set to "Scale With Screen Size" (reference 1080x1920). Use anchors and layout groups extensively. Test on 16:9, 18:9, 19.5:9, and iPad aspect ratios. |
| 3.11 | Implement safe-area handling | Account for notches, rounded corners, and home indicators (iOS) and navigation bars (Android). Use Unity's `Screen.safeArea`. |

### 5.3 Deliverables

- [ ] All game scenes built with placeholder or final art
- [ ] Complete card asset set (52 + back) at 2x resolution minimum
- [ ] UI sprite atlas optimized for mobile
- [ ] Responsive layouts tested on at least 5 device aspect ratios
- [ ] Safe-area implementation verified on notched devices

### 5.4 Tools

- **Figma** — Design handoff, specifications
- **TextMesh Pro** — High-quality text rendering in Unity
- **Unity UI Toolkit** or **Unity UGUI** — UI framework (recommend UGUI for game UIs)
- **Sprite Atlas** — Unity's built-in atlas packing for draw-call optimization

---

## 6. Phase 4 — Multiplayer & Networking

**Duration:** 4–5 weeks
**Dependencies:** Phase 2 game logic complete

### 6.1 Objectives

- Implement real-time cross-platform multiplayer
- Build matchmaking (quick match + private rooms)
- Ensure game state synchronization and cheat resistance
- Handle disconnection, reconnection, and latency gracefully

### 6.2 Recommended Approach: Photon (PUN 2 / Fusion)

**Why Photon over Unity Netcode:**

| Factor | Photon | Unity Netcode + Relay |
|---|---|---|
| Matchmaking | Built-in, production-ready | Must build custom or use Lobby |
| Server infrastructure | Photon Cloud (managed) | Unity Relay (managed) |
| Cross-platform | Automatic | Automatic |
| CCU pricing | Free up to 20 CCU, then tiered | Free tier available |
| Ease of integration | Very high for turn-based/card games | More suited for action games |
| Room/lobby system | Built-in | Requires additional setup |

**Verdict:** Photon PUN 2 or Photon Fusion is ideal for a turn-based card game. The built-in room system maps perfectly to Hearts lobbies.

### 6.3 Tasks

| # | Task | Details |
|---|---|---|
| 4.1 | Integrate Photon SDK | Import PUN 2 or Fusion package, configure App ID, set up connection flow. |
| 4.2 | Implement lobby system | Room creation (public/private), room listing, join by code, player slots with ready-up. |
| 4.3 | Implement game state sync | Synchronize: card dealing (seeded RNG), card passes, card plays, trick results, scores. Use Photon RPCs or custom serialization. |
| 4.4 | Build authoritative host model | Designate room creator (or Photon Master Client) as authoritative host. Host validates all moves (legal card plays, passing rules). Other clients send inputs, host broadcasts confirmed state. |
| 4.5 | Handle disconnections | Detect player disconnect → replace with AI bot for remainder of round → allow reconnection within timeout (60s). Preserve game state for reconnecting player. |
| 4.6 | Handle latency | Card game is turn-based, so latency is less critical. Add input buffering and visual feedback ("Waiting for Player...") when a player's action is pending. Target < 200ms round-trip for acceptable feel. |
| 4.7 | Implement crossplay | Photon handles this natively — iOS and Android connect to same Photon Cloud rooms. Verify with cross-device testing. |
| 4.8 | Add player profiles | Basic profile: display name, avatar selection, stats (games played, win rate). Store in Firebase or PlayFab. Authenticate with anonymous auth initially, with option to link Google/Apple ID later. |

### 6.4 Deliverables

- [ ] Working online multiplayer: 4 players across devices can complete a full game
- [ ] Quick Match and Private Room matchmaking
- [ ] Disconnection handling with AI substitution
- [ ] Reconnection support within timeout window
- [ ] Cross-platform play verified (Android ↔ iOS)
- [ ] Player profiles with persistent stats

### 6.5 Data Flow Diagram

```
  Player A (iOS)          Photon Cloud           Player B (Android)
  ┌──────────┐          ┌──────────────┐         ┌──────────┐
  │  Client   │◄────────►│  Room Server  │◄───────►│  Client   │
  │  (Unity)  │          │  (Photon)    │         │  (Unity)  │
  └──────────┘          └──────┬───────┘         └──────────┘
                               │
                    ┌──────────┴──────────┐
                    │                     │
              Player C (iOS)      Player D (Android)
              ┌──────────┐        ┌──────────┐
              │  Client   │        │  Client   │
              │  (Unity)  │        │  (Unity)  │
              └──────────┘        └──────────┘

  Data Flow per Turn:
  1. Active player selects card → Client validates locally
  2. Client sends PlayCard RPC to Master Client (host)
  3. Master Client validates move against authoritative game state
  4. If valid: Master Client broadcasts CardPlayed event to all clients
  5. All clients animate the card play
  6. After 4 cards: Master Client determines trick winner, broadcasts result
  7. All clients update scores and proceed
```

---

## 7. Phase 5 — AI Opponents

**Duration:** 2–3 weeks
**Dependencies:** Phase 2 game logic complete

### 7.1 Objectives

- Build AI players that fill empty seats in lobbies
- Implement multiple difficulty levels
- Make AI play feel natural (add human-like timing delays)

### 7.2 Tasks

| # | Task | Details |
|---|---|---|
| 5.1 | Implement Easy AI | Rule-following bot: always plays a legal card, prefers to dump high Hearts/Queen, no strategic depth. Random among legal plays with mild penalty avoidance. |
| 5.2 | Implement Medium AI | Heuristic bot: tracks which cards have been played, avoids taking tricks with penalty cards, basic void-suit strategy, attempts to duck Queen of Spades. |
| 5.3 | Implement Hard AI | Advanced heuristic bot: full card counting, understands passing strategy (pass high spades, create voids), attempts to bleed Hearts strategically, can detect and execute Shooting the Moon. Uses Monte Carlo sampling for uncertain information. |
| 5.4 | Add thinking delay | AI should wait 0.8–2.5 seconds before playing (variable, scaled to difficulty). This prevents instant play that breaks immersion. |
| 5.5 | Add AI personalities | Optional: give AI players names, avatars, and subtle play-style differences (aggressive, conservative, opportunistic). |
| 5.6 | Test AI balance | Playtest extensively. Easy AI should lose to new players. Medium AI should challenge casual players. Hard AI should challenge experienced players. Adjust heuristic weights accordingly. |

### 7.3 AI Architecture

```
IPlayer
├── HumanPlayer        (reads input from touch/UI)
├── NetworkPlayer      (reads input from network)
└── AIPlayer
    ├── EasyStrategy    (random legal play + mild penalty avoidance)
    ├── MediumStrategy  (heuristic card tracking + void creation)
    └── HardStrategy    (Monte Carlo + full card counting + shoot-the-moon detection)
```

**Strategy pattern:** Each difficulty is a swappable `IPlayStrategy` behind the `AIPlayer` class, making it easy to add or tune difficulties.

### 7.4 Deliverables

- [ ] Three difficulty tiers: Easy, Medium, Hard
- [ ] AI seamlessly fills empty lobby seats
- [ ] AI replaces disconnected players mid-game
- [ ] Natural-feeling play timing
- [ ] Balance validated through playtesting

---

## 8. Phase 6 — Polish, Animation & Audio

**Duration:** 3–4 weeks
**Dependencies:** Phases 2, 3, 4 substantially complete

### 8.1 Objectives

- Add smooth, satisfying card animations
- Implement particle effects and visual feedback
- Add sound effects and background music
- Create an emotionally satisfying play experience

### 8.2 Animation Tasks

| # | Task | Details |
|---|---|---|
| 6.1 | Card dealing animation | Cards fly from deck position to each player's hand, fanning out. Use eased bezier curves (ease-out-back for a slight overshoot). Stagger timing: ~0.08s between each card. |
| 6.2 | Card play animation | Selected card lifts slightly (scale 1.05x, shadow increase), then flies to center of table on play. Use DOTween for smooth interpolation. |
| 6.3 | Trick collection animation | After trick is won, all four cards slide toward winner's position, then fade/shrink. Brief delay before next trick. |
| 6.4 | Card passing animation | Selected cards glow/highlight, then fly in the pass direction (left/right/across) with a gentle arc. |
| 6.5 | Card fan management | Player's hand fans out in an arc at bottom of screen. Cards shift smoothly as cards are played (hand compresses). Hovering/selecting a card raises it above the fan. |
| 6.6 | Shuffle animation | Visual deck shuffle at round start — cards riffle or cascade. 1–2 seconds. |
| 6.7 | Score reveal animation | Numbers count up with a ticking sound. Queen of Spades hit shows a dramatic red flash. Shooting the Moon triggers a special celebration. |
| 6.8 | Menu transitions | Screens slide/fade in with 0.3s transitions. Buttons have press-scale feedback (0.95x on press, 1.0x on release). |
| 6.9 | Turn indicator | Subtle pulsing glow around the active player's area. Arrow or highlight pointing to whose turn it is. |
| 6.10 | Hearts broken indicator | When Hearts are first played, a brief dramatic animation: heart icon pulses, subtle screen flash, sound cue. |

### 8.3 Audio Tasks

| # | Task | Details |
|---|---|---|
| 6.11 | Card play SFX | Crisp card-snap sound when a card hits the table. Slight variation (pitch shift ±5%) per play to avoid repetition. |
| 6.12 | Card shuffle SFX | Realistic card-riffle sound. |
| 6.13 | Trick collection SFX | Soft card-sweep sound. |
| 6.14 | UI interaction SFX | Button clicks, menu opens/closes, toggle switches. Subtle, non-intrusive. |
| 6.15 | Score tally SFX | Gentle ticking for score counting, dramatic sting for Queen of Spades. |
| 6.16 | Win/Lose SFX | Victory fanfare (winner), gentle consolation tone (losers). |
| 6.17 | Background music | Calm, lounge-style ambient music. Low-key jazz or classical. Must loop seamlessly. Multiple tracks for variety. Volume lower during gameplay, slightly louder in menus. |
| 6.18 | Haptic feedback | Light haptic on card play (iOS Taptic Engine, Android vibration API). Medium haptic on trick win. Strong haptic on Shooting the Moon. |

### 8.4 Animation Principles

- **Easing:** Never use linear interpolation for visual movement. Use ease-out for arrivals, ease-in-out for transitions, ease-out-back for playful overshoot.
- **Staggering:** When multiple elements animate (e.g., dealing), stagger their start times for visual rhythm.
- **Anticipation:** Cards lift slightly before flying to table (Disney's animation principle of anticipation).
- **Follow-through:** After a card lands, it settles with a tiny bounce or wobble.
- **Duration guidelines:** Card play: 0.3–0.4s. Dealing: 2–3s total. Screen transitions: 0.25–0.35s. Score counting: 0.5–1.5s.

### 8.5 Recommended Animation Library

- **DOTween Pro** (Unity Asset Store) — Industry-standard tweening library for Unity. Supports sequences, easing curves, callbacks. Far more flexible than Unity's Animator for procedural card animations.

### 8.6 Deliverables

- [ ] All card animations implemented and smooth at 60fps on target devices
- [ ] Sound effects for all interactions
- [ ] Background music with volume controls
- [ ] Haptic feedback on iOS and Android
- [ ] Particle effects for special moments (Shooting the Moon, game win)
- [ ] Consistent 60fps performance verified on mid-range devices

---

## 9. Phase 7 — Testing & QA

**Duration:** 3–4 weeks
**Dependencies:** Phases 2–6 substantially complete

### 9.1 Objectives

- Ensure game logic correctness across all edge cases
- Verify multiplayer stability and synchronization
- Performance test on a range of real devices
- Conduct user playtesting for UX validation

### 9.2 Testing Layers

| Layer | Type | Tool/Method | Scope |
|---|---|---|---|
| Unit tests | Automated | Unity Test Framework (NUnit) | Game rules, scoring, state transitions |
| Integration tests | Automated | Unity Test Framework (Play Mode) | UI interactions, scene loading, animation triggers |
| Network tests | Manual + Automated | Photon Dashboard + custom logging | Sync correctness, disconnection recovery, latency simulation |
| Device testing | Manual | Real devices + cloud labs | Performance, layout, touch responsiveness |
| Playtest sessions | Manual | Internal + external testers | UX, fun factor, AI difficulty balance |
| Store compliance | Manual | App Store review guidelines check | Content, privacy, metadata |

### 9.3 Tasks

| # | Task | Details |
|---|---|---|
| 7.1 | Unit test game logic | Cover all rule edge cases: 2-of-clubs-first, no-penalty-first-trick, hearts-breaking, Queen-of-Spades, Shooting-the-Moon, tie-breaking, pass-direction-rotation. Target 95%+ coverage on core logic. |
| 7.2 | Multiplayer stress test | Simulate: 4-player games with artificial latency (50ms, 200ms, 500ms), mid-game disconnects, rapid reconnects, host migration (if applicable). |
| 7.3 | Cross-device testing | Test on: iPhone SE (small screen), iPhone 15 Pro (notch), iPad (tablet ratio), low-end Android (e.g., Samsung A14), flagship Android (e.g., Pixel 8). |
| 7.4 | Performance profiling | Unity Profiler: verify 60fps on target devices, check for memory leaks (especially after multiple rounds), optimize draw calls (target <50 for gameplay scene). |
| 7.5 | Battery/thermal testing | Run 30-minute sessions on mobile, monitor battery drain and thermal throttling. Optimize if needed (reduce particle effects, lower update rates when idle). |
| 7.6 | Accessibility audit | Verify: color-blind-friendly card suits (use symbols not just color), adequate text size, touch target sizes (minimum 44x44pt per Apple HIG). |
| 7.7 | Localization prep | Even if launching English-only, structure all strings through a localization system for future translation. |
| 7.8 | Beta testing | Distribute via TestFlight (iOS) and Google Play Internal Testing (Android). Recruit 20–50 testers. Collect feedback via in-app form or survey. Iterate for 2–3 beta cycles. |

### 9.4 Deliverables

- [ ] Unit test suite passing at 95%+ coverage
- [ ] Multiplayer tested across 50+ simulated games with varying conditions
- [ ] Performance validated on 5+ real devices (both platforms)
- [ ] Beta test feedback collected and addressed
- [ ] Bug backlog triaged and critical/high bugs resolved
- [ ] Accessibility checklist passed

---

## 10. Phase 8 — Store Preparation & Deployment

**Duration:** 2–3 weeks
**Dependencies:** Phase 7 substantially complete

### 10.1 Objectives

- Prepare all store listing assets and metadata
- Configure app signing, entitlements, and provisioning
- Submit to both stores and pass review

### 10.2 Apple App Store Tasks

| # | Task | Details |
|---|---|---|
| 8.1 | Apple Developer account | Enroll in Apple Developer Program ($99/year). Set up App ID, certificates, provisioning profiles. |
| 8.2 | App Store Connect setup | Create app listing, configure: app name, subtitle, category (Games > Card), subcategory, age rating (4+), pricing (Free or paid). |
| 8.3 | Screenshots | Capture 6.7" (iPhone 15 Pro Max), 6.5" (iPhone 11 Pro Max), 5.5" (iPhone 8 Plus), and 12.9" (iPad Pro) screenshots. Minimum 3, recommend 5–8 per device class. Show gameplay, menus, multiplayer. |
| 8.4 | App Preview video | Optional but recommended: 15–30s gameplay video. Show dealing, trick play, multiplayer. Must meet Apple's format requirements. |
| 8.5 | Privacy policy | Create a privacy policy URL (required). Describe data collection: analytics, player profiles, multiplayer data. Host on a simple web page. |
| 8.6 | App Privacy labels | Fill out App Store Privacy "nutrition labels": data collected, data linked to identity, tracking. |
| 8.7 | Build upload | Archive from Xcode (via Unity's iOS build), upload via Xcode or Transporter. Ensure correct version/build numbers. |
| 8.8 | Review submission | Submit for review. Expect 1–3 day review. Common rejection reasons: crashes, broken links, inadequate metadata. Have demo account ready if needed. |

### 10.3 Google Play Store Tasks

| # | Task | Details |
|---|---|---|
| 8.9 | Google Play Developer account | Register ($25 one-time fee). Set up developer profile. |
| 8.10 | Play Console setup | Create app, configure: app name, short/long descriptions, category (Game > Card), content rating questionnaire, pricing. |
| 8.11 | Store listing assets | Feature graphic (1024x500), icon (512x512), screenshots (phone + 7" tablet + 10" tablet). Minimum 2, recommend 4–8. |
| 8.12 | App signing | Use Google Play App Signing (recommended). Generate upload key, configure in Unity's Player Settings (keystore). |
| 8.13 | Content rating | Complete IARC rating questionnaire. Hearts is non-violent, expect Everyone / 4+ rating. |
| 8.14 | Data safety form | Fill out data safety section: what data is collected, how it's used, whether it's shared. |
| 8.15 | AAB upload | Unity builds Android App Bundle (.aab, not .apk). Upload to Internal Testing → Closed Testing → Open Testing → Production track. |
| 8.16 | Review submission | Submit for review. Google review is typically faster (hours to 1 day). |

### 10.4 Common Tasks (Both Platforms)

| # | Task | Details |
|---|---|---|
| 8.17 | App icon | Design a distinctive icon: 1024x1024 source, follows both Apple HIG and Material Design icon guidelines. No transparency (iOS). |
| 8.18 | Version strategy | Start at 1.0.0. Use semantic versioning. Build numbers auto-increment via CI. |
| 8.19 | Crash reporting | Integrate Firebase Crashlytics for both platforms. Verify crash reports flow correctly. |
| 8.20 | Analytics | Integrate Firebase Analytics. Key events: game_start, game_complete, matchmaking_time, ai_difficulty_selected, session_length. |
| 8.21 | Legal | Terms of Service document. GDPR/CCPA compliance if collecting personal data. COPPA compliance (no data collection from children under 13 without safeguards). |

### 10.5 Deliverables

- [ ] Apple App Store listing live (or in review)
- [ ] Google Play Store listing live (or in review)
- [ ] All screenshots and promotional assets uploaded
- [ ] Privacy policy and terms of service published
- [ ] Crash reporting and analytics verified
- [ ] Both apps approved and publicly available

---

## 11. Phase 9 — Post-Launch & Live Operations

**Duration:** Ongoing
**Dependencies:** Phase 8 complete (app is live)

### 11.1 Objectives

- Monitor app stability and player feedback
- Iterate on balance and features
- Grow the player base

### 11.2 Tasks

| # | Task | Details |
|---|---|---|
| 9.1 | Monitor crash reports | Daily check of Firebase Crashlytics. Hotfix critical crashes within 24–48 hours. |
| 9.2 | Monitor reviews | Respond to App Store and Play Store reviews. Address common complaints in updates. |
| 9.3 | Analytics review | Weekly review of key metrics: DAU, retention (D1, D7, D30), average session length, matchmaking queue times. |
| 9.4 | Balance patches | Adjust AI difficulty based on player feedback and win-rate data. |
| 9.5 | Feature updates | Potential additions: tournament mode, friends list, custom card backs/themes, seasonal events, additional card game variants. |
| 9.6 | Monetization (optional) | If desired: cosmetic IAPs (card backs, table themes, avatars), remove-ads option, premium tier. Avoid pay-to-win. |
| 9.7 | Community building | Social media presence, Discord server, respond to player feedback. |

### 11.3 Deliverables

- [ ] Post-launch monitoring dashboard active
- [ ] First patch update (bug fixes) within 2 weeks of launch
- [ ] Feature roadmap for v1.1, v1.2 updates
- [ ] Player feedback loop established

---

## 12. Multiplayer Architecture Deep Dive

### 12.1 Architecture: Client-Authoritative with Host Validation

For a turn-based card game, a fully server-authoritative model (dedicated servers) is overkill. Instead, use a **host-validated peer model** via Photon:

```
┌─────────────────────────────────────────────────────┐
│                 Photon Cloud Room                    │
│                                                     │
│  Master Client (Host)                               │
│  ┌───────────────────────────────────────────┐      │
│  │  Authoritative Game State                 │      │
│  │  ├── Deck (shuffled, seeded RNG)          │      │
│  │  ├── All hands (hidden from other clients)│      │
│  │  ├── Current trick                        │      │
│  │  ├── Scores                               │      │
│  │  └── Pass-phase state                     │      │
│  └───────────────────────────────────────────┘      │
│           │            │             │               │
│      Validated    Validated    Validated             │
│       Moves        Moves        Moves               │
│           │            │             │               │
│      Client A     Client B     Client C             │
│      (own hand    (own hand    (own hand             │
│       only)        only)        only)                │
└─────────────────────────────────────────────────────┘
```

### 12.2 Anti-Cheat Considerations

- **Hidden information:** Each client should only know its own hand, the cards played, and public game state. The host does NOT send other players' hands to any client.
- **Move validation:** Host validates every card play (is it in the player's hand? does it follow suit? is it legal?).
- **RNG seeding:** Shuffle is performed on host only. Clients receive dealt cards, not the seed.
- **Replay capability:** Log all actions for post-game review and dispute resolution.

### 12.3 Matchmaking Flow

```
Player opens app
    │
    ├── Quick Match
    │   ├── Connect to Photon
    │   ├── Join random room (with skill-based filter if available)
    │   ├── If no room found → Create new room, wait for players
    │   ├── After 15s wait → Fill with AI bots, start game
    │   └── Game begins
    │
    └── Private Room
        ├── Create Room → Generate 4-character room code
        ├── Share code with friends
        ├── Friends join via code
        ├── Host can fill remaining slots with AI
        └── Host starts game when ready
```

### 12.4 Reconnection Protocol

1. Player disconnects (network loss, app backgrounded)
2. Server starts 60-second timeout
3. AI bot takes over player's seat immediately (no game interruption)
4. If player reconnects within timeout:
   - Receive full current game state
   - Resume control from AI
   - Other players notified: "Player X reconnected"
5. If timeout expires:
   - AI continues for remainder of game
   - Player can start a new game

---

## 13. Visual Style Guide

### 13.1 Design Philosophy

**Modern Minimalist Elegance** — The game should feel premium, calm, and sophisticated. Think: a high-end card lounge, not a Las Vegas casino.

### 13.2 Style References

- **Apple Human Interface Guidelines** — Clean typography, generous whitespace, subtle depth
- **Material Design 3 (Material You)** — Dynamic color, rounded shapes, accessible contrast
- **Alto's Odyssey / Monument Valley** — Calm palette, geometric elegance
- **Solitaire by MobilityWare** — Clean card game UI reference

### 13.3 Color Palette

| Role | Color | Hex | Usage |
|---|---|---|---|
| Background (primary) | Deep forest green | `#1B4332` | Table felt, gameplay background |
| Background (menus) | Warm off-white | `#FAF3E0` | Menu screens, panels |
| Accent (primary) | Rich gold | `#D4A843` | Buttons, highlights, active states |
| Accent (secondary) | Soft crimson | `#C0392B` | Hearts suit, penalty indicators |
| Text (primary) | Near-black | `#1A1A2E` | Body text, card values |
| Text (secondary) | Muted gray | `#6B7280` | Subtitles, secondary info |
| Card white | Warm white | `#FEFCF3` | Card face background |
| Success | Muted teal | `#2D9CDB` | Positive feedback, win states |
| Warning | Amber | `#F2994A` | Alerts, caution states |

### 13.4 Typography

| Element | Font Recommendation | Size (reference) | Weight |
|---|---|---|---|
| Headlines | **Playfair Display** or **Cormorant Garamond** | 28–36sp | Bold |
| Body text | **Inter** or **SF Pro Text** | 14–16sp | Regular |
| Card values | **Custom serif** or **Libre Baskerville** | 18–24sp | Bold |
| Score numbers | **Inter** or **Roboto Mono** | 20–28sp | Semi-bold |
| Buttons | **Inter** | 16–18sp | Medium |

### 13.5 Card Design

- **Card faces:** Clean white background with traditional suit symbols. Large, readable rank in top-left and bottom-right corners. Suit symbol in center for number cards. Court cards (J, Q, K) with illustrated portraits in a flat/modern style.
- **Card backs:** Deep navy or forest green with a geometric or ornamental pattern. Centered logo/monogram. Should feel premium and timeless.
- **Card dimensions:** Standard poker ratio (~2.5:3.5). On screen: approximately 70x100 points on phone, larger on tablet.
- **Card shadows:** Subtle drop shadow (2–4px blur, 10% opacity black) for depth. Increase shadow when card is lifted/selected.

### 13.6 UI Components

- **Buttons:** Rounded rectangles (12px radius), filled primary style for main actions, outlined for secondary. Scale animation on press (0.95x).
- **Panels:** Rounded corners (16px), subtle shadow, warm white background on menu screens, semi-transparent dark overlay on gameplay screens.
- **Icons:** Line-style icons, consistent 24x24 touch area, 2px stroke weight. Use Material Icons or custom set.
- **Avatars:** Circular, 48x48 on gameplay screen, 64x64 in lobby. Default set of 12–16 illustrated characters.

### 13.7 Table Layout

```
              ┌─────────────────────────────────┐
              │        Opponent (Top)            │
              │    [Avatar] Name    Score: 15    │
              │    ░░░░░ (face-down cards) ░░░░░ │
              ├────────┬───────────┬─────────────┤
              │        │           │             │
              │  Opp   │  TRICK    │  Opp        │
              │ (Left) │  PILE     │ (Right)     │
              │        │  ┌──┐┌──┐│             │
              │ ░░░    │  │7♠││Q♥││    ░░░     │
              │ ░░░    │  └──┘└──┘│    ░░░     │
              │ ░░░    │  ┌──┐┌──┐│    ░░░     │
              │        │  │3♦││J♣││             │
              │        │  └──┘└──┘│             │
              ├────────┴───────────┴─────────────┤
              │         Your Hand (You)          │
              │   ┌──┐┌──┐┌──┐┌──┐┌──┐┌──┐     │
              │   │2♣││5♦││8♥││K♠││A♦││9♣│     │
              │   └──┘└──┘└──┘└──┘└──┘└──┘     │
              │    [Avatar] You     Score: 8     │
              └─────────────────────────────────┘
```

---

## 14. Project Timeline & Dependencies

### 14.1 Gantt-Style Overview

```
Week:  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20
       ├──┤
Phase 1 ██                                              Pre-Production

          ├────────────┤
Phase 2    ████████████                                  Core Game Logic

          ├────────────┤
Phase 3    ████████████                                  UI/UX & Art
                                                         (parallel w/ Phase 2)

                        ├──────────────────┤
Phase 4                  ██████████████████              Multiplayer

                        ├────────┤
Phase 5                  ████████                        AI Opponents
                                                         (parallel w/ Phase 4)

                                    ├────────────┤
Phase 6                              ████████████        Polish & Audio

                                              ├────────────┤
Phase 7                                        ████████████ Testing & QA

                                                        ├──────┤
Phase 8                                                  ██████ Store Deploy

                                                              ├──►
Phase 9                                                        ██► Post-Launch
```

**Total estimated timeline: 18–20 weeks** (for a small team of 1–3 developers)

### 14.2 Dependency Graph

```
Phase 1 (Pre-Production)
    │
    ├──► Phase 2 (Game Logic)──────────────────────────────────┐
    │         │                                                │
    │         ├──► Phase 4 (Multiplayer)                       │
    │         │         │                                      │
    │         ├──► Phase 5 (AI) ────────────┐                  │
    │         │                             │                  │
    └──► Phase 3 (UI/UX Art) ──┐            │                  │
                               │            │                  │
                               ▼            ▼                  ▼
                          Phase 6 (Polish, Animation, Audio)
                               │
                               ▼
                          Phase 7 (Testing & QA)
                               │
                               ▼
                          Phase 8 (Store Deployment)
                               │
                               ▼
                          Phase 9 (Post-Launch)
```

### 14.3 Critical Path

The **critical path** (longest sequential dependency chain) is:

**Phase 1 → Phase 2 → Phase 4 → Phase 6 → Phase 7 → Phase 8**

Any delay on these phases directly delays the launch date. Phases 3 and 5 can be parallelized to save time.

---

## 15. Pre-Publishing Checklist

This is the final checklist that must be completed before submitting to both app stores.

### 15.1 Game Quality

- [ ] All Hearts rules implemented correctly and tested
- [ ] Multiplayer works reliably across iOS and Android
- [ ] AI opponents functional at all difficulty levels
- [ ] No game-breaking bugs (crashes, freezes, soft locks)
- [ ] 60fps performance on mid-range devices (iPhone 11, Samsung Galaxy A54 or equivalent)
- [ ] Memory usage under 300MB during gameplay
- [ ] App size under 150MB (ideally under 100MB)
- [ ] Battery drain within acceptable limits (30-min session < 10% drain)

### 15.2 UI/UX

- [ ] All screens responsive on phones and tablets
- [ ] Safe areas respected on all notched/edge devices
- [ ] Touch targets minimum 44x44 points
- [ ] Text readable at all sizes
- [ ] Color-blind accessible suit indicators
- [ ] Landscape/portrait orientation handled (or locked with rationale)
- [ ] Loading states shown for all async operations
- [ ] Error states handled gracefully (network loss, server errors)

### 15.3 Animations & Audio

- [ ] All card animations smooth and satisfying
- [ ] Sound effects present for all interactions
- [ ] Audio can be independently muted/adjusted
- [ ] Haptic feedback functional on supported devices
- [ ] No audio glitches or memory leaks from repeated sounds

### 15.4 Multiplayer & Networking

- [ ] Quick Match and Private Room matchmaking tested
- [ ] Cross-platform play verified (iOS ↔ Android)
- [ ] Disconnection handling works correctly
- [ ] Reconnection within timeout works correctly
- [ ] No desync issues across 100+ test games
- [ ] Matchmaking timeout fills seats with AI correctly

### 15.5 Apple App Store Requirements

- [ ] Apple Developer account active
- [ ] App ID, certificates, and provisioning profiles configured
- [ ] App Store Connect listing complete (name, description, keywords, category)
- [ ] Screenshots for all required device sizes
- [ ] App icon (1024x1024, no transparency, no rounded corners — Apple rounds automatically)
- [ ] Privacy policy URL active and accessible
- [ ] App Privacy labels filled out
- [ ] Age rating set (likely 4+)
- [ ] Build uploaded via Xcode/Transporter
- [ ] TestFlight beta testing completed
- [ ] App Review Information filled (demo account if needed, contact info, notes)
- [ ] IDFA declaration (if using advertising identifier)
- [ ] Export compliance information provided

### 15.6 Google Play Store Requirements

- [ ] Google Play Developer account active
- [ ] Play Console app listing complete (title, short/full description, category)
- [ ] Feature graphic (1024x500)
- [ ] Screenshots for phone and tablet
- [ ] App icon (512x512)
- [ ] Privacy policy URL active and accessible
- [ ] Data safety form completed
- [ ] Content rating (IARC) questionnaire completed
- [ ] App signing configured (Google Play App Signing)
- [ ] AAB (Android App Bundle) uploaded
- [ ] Internal/Closed testing completed
- [ ] Target API level meets current Google Play requirements
- [ ] Permissions justified and minimized

### 15.7 Legal & Compliance

- [ ] Privacy policy published and linked in both stores
- [ ] Terms of Service published (if applicable)
- [ ] GDPR compliance (consent flow for EU users if collecting data)
- [ ] CCPA compliance (opt-out mechanism for California users)
- [ ] COPPA compliance (if applicable — no collection from children under 13)
- [ ] All third-party licenses accounted for (fonts, assets, SDKs)
- [ ] No copyrighted assets used without license

### 15.8 Analytics & Monitoring

- [ ] Firebase Crashlytics integrated and tested on both platforms
- [ ] Firebase Analytics tracking key events
- [ ] Remote Config set up for kill switches and feature flags
- [ ] Dashboard or alerts configured for crash rate spikes

### 15.9 Post-Submission

- [ ] Monitor review status daily
- [ ] Prepare hotfix branch in case of critical bugs found post-launch
- [ ] Social media / marketing launch plan ready
- [ ] App Store Optimization (ASO) keywords researched and applied
- [ ] Respond to initial user reviews within 24 hours

---

## Appendix A: Technology Summary

| Component | Recommended Tool | Alternative |
|---|---|---|
| Game engine | Unity 2022 LTS+ | Godot 4.x |
| Language | C# | GDScript (Godot) |
| Multiplayer networking | Photon PUN 2 / Fusion | Unity Netcode + Relay |
| Backend services | Firebase | PlayFab, Supabase |
| Authentication | Firebase Auth (anonymous + linked) | Apple Game Center + Google Play Games |
| Crash reporting | Firebase Crashlytics | Sentry, Bugsnag |
| Analytics | Firebase Analytics | Mixpanel, Amplitude |
| CI/CD | Unity Cloud Build or GitHub Actions + Fastlane | Bitrise, Codemagic |
| UI design | Figma | Adobe XD, Sketch |
| Animation | DOTween Pro | LeanTween, Unity Animator |
| Audio middleware | FMOD (if complex) | Unity native audio (sufficient for card game) |
| Version control | Git + GitHub + Git LFS | GitLab, Bitbucket |
| Project management | GitHub Projects / Linear | Jira, Trello |

## Appendix B: Estimated Costs

| Item | Cost | Notes |
|---|---|---|
| Apple Developer account | $99/year | Required for App Store |
| Google Play Developer account | $25 (one-time) | Required for Play Store |
| Unity license | Free (Personal) | Revenue < $100K/year |
| Photon | Free (20 CCU) | Scale-up plans available |
| Firebase | Free (Spark plan) | Generous free tier |
| DOTween Pro | ~$15 | Unity Asset Store |
| Card assets (if purchased) | $20–$100 | Varies by quality |
| Sound effects (if purchased) | $20–$50 | Packs on Freesound/similar |
| Background music (license) | $20–$100 | Royalty-free music libraries |
| **Total initial investment** | **~$200–$500** | Excluding developer hardware |

---

*This document serves as the living blueprint for Hearts Multiplayer. Update it as decisions are made, requirements evolve, and phases are completed.*
