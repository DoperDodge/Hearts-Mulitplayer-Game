# Hearts — Official Game Rules Specification

> **Purpose:** This document defines every rule, edge case, and configurable variant for the Hearts Multiplayer game. All game logic implementation must conform to this spec.
>
> **Version:** 1.0
> **Date:** 2026-03-04

---

## Table of Contents

1. [Overview](#1-overview)
2. [Players & Seating](#2-players--seating)
3. [The Deck](#3-the-deck)
4. [Dealing](#4-dealing)
5. [Card Passing](#5-card-passing)
6. [Trick Play](#6-trick-play)
7. [Scoring](#7-scoring)
8. [Shooting the Moon](#8-shooting-the-moon)
9. [Game End & Winning](#9-game-end--winning)
10. [Edge Cases & Clarifications](#10-edge-cases--clarifications)
11. [Configurable Variants](#11-configurable-variants)
12. [Glossary](#12-glossary)

---

## 1. Overview

Hearts is a four-player trick-taking card game where the objective is to **avoid taking penalty points**. Penalty cards are all 13 Hearts (1 point each) and the Queen of Spades (13 points). The player with the **lowest score** when the game ends wins.

**Core principle:** You do NOT want to win tricks that contain penalty cards — unless you're Shooting the Moon.

---

## 2. Players & Seating

### 2.1 Player Count

- The game is always played with **exactly 4 players**.
- If fewer than 4 human players are present, **AI bots fill the remaining seats**.

### 2.2 Seating Arrangement

- Players are seated in a fixed circular arrangement: **North, East, South, West**.
- The local (human) player is always displayed at the **South** position.
- Turn order proceeds **clockwise**: South → West → North → East → South.

### 2.3 Player Identification

Each player has:
- A **display name**
- An **avatar** (selected or assigned)
- A **seat position** (North/East/South/West)
- A **cumulative score** (persists across rounds)

---

## 3. The Deck

### 3.1 Composition

- Standard 52-card deck
- **4 suits:** Clubs (♣), Diamonds (♦), Spades (♠), Hearts (♥)
- **13 ranks per suit:** 2, 3, 4, 5, 6, 7, 8, 9, 10, Jack, Queen, King, Ace

### 3.2 Rank Order (Low to High)

```
2 < 3 < 4 < 5 < 6 < 7 < 8 < 9 < 10 < Jack < Queen < King < Ace
```

### 3.3 Card Values for Sorting

| Rank | Sort Value |
|------|-----------|
| 2    | 2         |
| 3    | 3         |
| 4    | 4         |
| 5    | 5         |
| 6    | 6         |
| 7    | 7         |
| 8    | 8         |
| 9    | 9         |
| 10   | 10        |
| Jack | 11        |
| Queen| 12        |
| King | 13        |
| Ace  | 14        |

Suit sort order for display: Clubs, Diamonds, Spades, Hearts.

---

## 4. Dealing

### 4.1 Shuffle

- The deck is shuffled using a **Fisher-Yates shuffle** algorithm.
- In multiplayer, the **host (Master Client)** performs the shuffle and distributes cards.

### 4.2 Deal

- All 52 cards are dealt evenly.
- Each player receives **exactly 13 cards**.
- Cards are dealt one at a time in clockwise order (for animation purposes), though the final result is the same as dealing 13 at once.

### 4.3 Post-Deal

- After receiving cards, each player's hand is **sorted by suit, then by rank** (for display).
- Players can see **only their own hand**. Other players' cards are face-down.

---

## 5. Card Passing

### 5.1 Pass Rotation

Passing occurs at the **start of each round**, before any tricks are played. The pass direction rotates each round in a 4-round cycle:

| Round | Pass Direction | Description |
|-------|---------------|-------------|
| 1     | **Left**      | Pass to the player on your left (clockwise) |
| 2     | **Right**     | Pass to the player on your right (counter-clockwise) |
| 3     | **Across**    | Pass to the player sitting opposite you |
| 4     | **Hold**      | No passing — play with the cards you were dealt |

After round 4, the cycle repeats (round 5 = Left, round 6 = Right, etc.).

### 5.2 Pass Rules

- Each player selects **exactly 3 cards** from their hand to pass.
- All 4 players select their cards **simultaneously** (no player sees what others are passing before choosing).
- Once all players have selected, the passes are **executed simultaneously**.
- After receiving passed cards, the player's hand is re-sorted.
- On a **Hold round**, the passing phase is skipped entirely.

### 5.3 Pass Restrictions

- **No restrictions** on which cards can be passed. Players may pass any 3 cards from their hand, including penalty cards (Hearts, Queen of Spades).

---

## 6. Trick Play

### 6.1 Opening Lead

- The player holding the **2 of Clubs (2♣)** leads the first trick of each round.
- This player **must** play the 2♣ as their first card. No other card may be played.

### 6.2 Playing a Card (Follow Suit)

For each trick after the lead card is played:

1. Each subsequent player (clockwise) **must follow suit** — play a card of the same suit as the lead card.
2. If a player **has no cards of the led suit**, they may play **any card** from their hand (this is called "sloughing" or "discarding").

### 6.3 Restrictions on the First Trick

On the **first trick of each round** (the 2♣ trick):

- **No penalty cards may be played** on the first trick. This means:
  - No Hearts (♥)
  - No Queen of Spades (Q♠)
- **Exception:** If a player's entire hand consists of only penalty cards (Hearts and/or Queen of Spades) with no other cards to play, they **may** play a penalty card. This is extremely rare but must be handled.

### 6.4 Leading with Hearts ("Breaking Hearts")

- A player **may not lead with a Heart** until Hearts have been "broken."
- Hearts are **broken** when any player plays a Heart on a trick (typically by discarding a Heart when they cannot follow suit).
- Once Hearts are broken, any player may lead with a Heart for the remainder of the round.
- **Exception:** If a player's hand contains **only Hearts**, they may lead a Heart even if Hearts have not been broken.

### 6.5 Winning a Trick

- The player who played the **highest-ranking card of the led suit** wins the trick.
- Cards of other suits (discards) **never win the trick**, regardless of rank.
- The winner of the trick **collects all 4 cards** and places them face-down in their trick pile.
- The winner of the trick **leads the next trick**.

### 6.6 Trick Sequence

- A round consists of **exactly 13 tricks** (one for each card in hand).
- After all 13 tricks are played, the round ends and scoring occurs.

---

## 7. Scoring

### 7.1 Penalty Cards

| Card | Point Value |
|------|-------------|
| Each Heart (♥) — 2♥ through A♥ | **1 point** each |
| Queen of Spades (Q♠) | **13 points** |
| All other cards | **0 points** |

**Total penalty points per round:** 26 (13 Hearts + Queen of Spades)

### 7.2 Round Scoring

At the end of each round:

1. Each player counts the penalty points in their collected tricks.
2. The sum of all players' penalty points for the round **must always equal 26**. (Use this as a validation check.)
3. Each player's round penalty points are added to their **cumulative score**.

### 7.3 Score Display

- Show a **round-by-round score table** after each round.
- Show **running cumulative totals** for each player.
- Highlight significant events (e.g., "Player took the Queen of Spades!" or "Shoot the Moon!").

---

## 8. Shooting the Moon

### 8.1 Definition

A player **Shoots the Moon** if they collect **all 26 penalty points** in a single round. This means they took:
- All 13 Hearts (♥), AND
- The Queen of Spades (Q♠)

### 8.2 Effect (Default: Add to Others)

When a player Shoots the Moon, instead of receiving 26 points:
- The shooting player receives **0 points** for the round.
- All **three other players** each receive **26 points**.

### 8.3 Alternative Effect (Configurable: Subtract from Self)

As a configurable variant:
- The shooting player **subtracts 26 points** from their cumulative score (minimum score is 0 — score cannot go negative).
- All other players receive **0 points** for the round.

### 8.4 Detection

- Shooting the Moon is detected **at the end of the round** after all 13 tricks are complete.
- The game checks if any single player's penalty points for the round equal exactly 26.
- This should trigger a special animation/celebration.

---

## 9. Game End & Winning

### 9.1 End Condition

The game ends at the **conclusion of any round** where at least one player's cumulative score **reaches or exceeds 100 points**.

### 9.2 Winner Determination

- The player with the **lowest cumulative score** wins.
- If the player who triggered the end condition (reached 100+) also has the lowest score (possible through Shooting the Moon effects on others), they still win.

### 9.3 Tie-Breaking

If two or more players are tied for the lowest score:

1. **Play another round.** Continue playing until the tie is broken.
2. If a tie persists after the additional round and at least one player is still at or above 100, the tied players **share the victory** (co-winners).

### 9.4 Post-Game

After the game ends:
- Display the **final scoreboard** with all rounds.
- Announce the **winner** with a celebration animation.
- Show options: **Play Again** (same lobby), **Return to Menu**, **Share Results**.

---

## 10. Edge Cases & Clarifications

### 10.1 First Trick — All Penalty Cards in Hand

**Scenario:** A player has no clubs and their entire hand is Hearts and/or Queen of Spades.

**Rule:** They may play a penalty card on the first trick as an exception to the first-trick penalty restriction. This is the only situation where penalty cards are allowed on trick one.

### 10.2 Leading When Only Hearts Remain

**Scenario:** It's a player's turn to lead, Hearts have NOT been broken, but their hand contains only Hearts.

**Rule:** They may lead a Heart. This also breaks Hearts for the remainder of the round.

### 10.3 Queen of Spades and Leading

**Clarification:** The Queen of Spades is NOT a Heart. The "no leading Hearts until broken" rule does not apply to Spades. A player may lead the Queen of Spades at any time (it is a Spade, and leading Spades is unrestricted).

### 10.4 Queen of Spades on First Trick

**Clarification:** The Queen of Spades **cannot** be played on the first trick (it is a penalty card). However, if a player has no Clubs and their only non-Heart cards are the Queen of Spades... they still cannot play it on the first trick if they have ANY non-penalty card. Only if their entire hand is penalty cards may they play Q♠ on trick one.

### 10.5 Simultaneous 100+ Scores

**Scenario:** At the end of a round, multiple players reach or exceed 100 simultaneously.

**Rule:** The game still ends. The player with the lowest score wins. If those 100+ players are tied for lowest, apply tie-breaking rules (Section 9.3).

### 10.6 Shooting the Moon — Partial Collection

**Scenario:** A player takes 12 Hearts and the Queen of Spades (25 points) but misses one Heart.

**Rule:** This is NOT Shooting the Moon. The player receives 25 penalty points normally. Shooting the Moon requires ALL 26 points.

### 10.7 Score of Zero After Shoot the Moon Subtraction

**Scenario:** Using the "subtract from self" variant, a player with a cumulative score of 10 Shoots the Moon (subtract 26).

**Rule:** Their score becomes **0**, not -16. The minimum cumulative score is 0.

### 10.8 Card Passing — Receiving Your Own Pass

**Clarification:** This cannot happen. You always pass to a different player and receive from a different player. On "Hold" rounds, no passing occurs at all.

### 10.9 Disconnection During Passing

**Rule:** If a player disconnects during the passing phase, AI takes over and auto-selects 3 cards to pass (using the AI's strategy for the assigned difficulty). The game continues without delay.

### 10.10 Disconnection During Trick Play

**Rule:** If a player disconnects during trick play, AI immediately takes control of their hand and plays on their behalf. The game continues without interruption. Other players are notified that a bot has taken over.

---

## 11. Configurable Variants

These settings can be toggled in the game settings or lobby configuration:

| Setting | Default | Options | Description |
|---------|---------|---------|-------------|
| Score limit | 100 | 50, 75, 100, 150, 200 | Cumulative score that triggers game end |
| Shoot the Moon effect | Add to others | Add to others / Subtract from self | What happens when a player shoots the moon |
| Jack of Diamonds bonus | Off | On / Off | If on, taking J♦ subtracts 10 points from your score |
| Pass rotation | Standard (L/R/A/H) | Standard / No passing | Whether card passing occurs |
| First trick penalty restriction | On | On / Off | Whether penalty cards are blocked on trick one |

### 11.1 Jack of Diamonds Variant

When enabled:
- The Jack of Diamonds (J♦) is worth **-10 points** (reduces your score by 10).
- Minimum score remains 0.
- This does NOT affect Shooting the Moon — a player must still collect all 13 Hearts + Q♠ to Shoot the Moon. The J♦ bonus is applied separately.

---

## 12. Glossary

| Term | Definition |
|------|-----------|
| **Trick** | A set of 4 cards, one played by each player. The highest card of the led suit wins. |
| **Lead** | The first card played in a trick. Determines the suit that must be followed. |
| **Follow suit** | Play a card of the same suit as the lead card. |
| **Slough / Discard** | Play a card of a different suit when you cannot follow suit. |
| **Void** | Having no cards of a particular suit in your hand. |
| **Break Hearts** | Play a Heart for the first time in a round, allowing Hearts to be led afterward. |
| **Shoot the Moon** | Collect all 26 penalty points in one round, reversing the penalty. |
| **Penalty cards** | All Hearts (1 pt each) and the Queen of Spades (13 pts). |
| **Round** | One complete cycle of dealing, passing, and playing 13 tricks. |
| **Game** | A series of rounds played until a player reaches the score limit. |
| **Cumulative score** | A player's total penalty points across all rounds in a game. |
| **Master Client / Host** | The player whose device runs the authoritative game state in multiplayer. |

---

*This specification is the single source of truth for all Hearts game logic. Any ambiguity in implementation should be resolved by referencing this document first.*
