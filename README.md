# Hearts Multiplayer Game

A cross-platform mobile card game based on the classic trick-taking game **Hearts**, built with Unity and playable on both **Android** and **iOS** with seamless crossplay.

## Features

- **1–4 players** with AI fill-ins for empty seats
- **Cross-platform multiplayer** — Android and iOS players in the same game
- **Three AI difficulty levels** — Easy, Medium, Hard
- **Polished visuals** — Smooth card animations, modern minimalist design
- **Quick Match & Private Rooms** — Play with strangers or friends
- **Full Hearts rules** — Passing, trick-taking, Shooting the Moon, and more

## Documentation

See [DESIGN_DOCUMENT.md](DESIGN_DOCUMENT.md) for the complete development blueprint, including:

- Phased development plan with deliverables
- Technical architecture and multiplayer networking design
- Visual style guide with color palette and typography
- Pre-publishing checklist for both app stores
- Timeline, dependencies, and cost estimates

## Tech Stack

| Component | Technology |
|---|---|
| Game Engine | Unity 2022 LTS+ |
| Language | C# |
| Multiplayer | Photon PUN 2 / Fusion |
| Backend | Firebase |
| Platforms | iOS, Android |

## Development Phases

> Mark phases: `[ ]` = Not Started, `[~]` = In Progress, `[x]` = Complete

- [x] **Phase 1 — Pre-Production & Planning** — Finalize rules, set up Unity project, wireframes, art direction, CI/CD
- [~] **Phase 2 — Core Engine & Game Logic** — Card data models, game state machine, trick/scoring logic, unit tests
- [ ] **Phase 3 — UI/UX Design & Visual Assets** — All game screens, card assets, responsive layouts, safe-area handling
- [ ] **Phase 4 — Multiplayer & Networking** — Photon integration, matchmaking, state sync, crossplay, reconnection
- [ ] **Phase 5 — AI Opponents** — Easy/Medium/Hard difficulty bots, card counting, natural play timing
- [ ] **Phase 6 — Polish, Animation & Audio** — Card animations, SFX, music, haptics, particle effects
- [ ] **Phase 7 — Testing & QA** — Unit tests, multiplayer stress tests, device testing, beta testing
- [ ] **Phase 8 — Store Preparation & Deployment** — App Store & Play Store listings, screenshots, compliance, submission
- [ ] **Phase 9 — Post-Launch & Live Operations** — Crash monitoring, reviews, balance patches, feature updates

## Status

Currently in **Phase 2 — Core Engine & Game Logic**.
