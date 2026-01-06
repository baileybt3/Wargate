# WARGATE: Cards to Combat — Backlog

## MVP Goal
A playable online 1v1 match where both players join, get roles, pick 1 card, fight in a simple battle, and a winner is declared.

## MVP Scope (In)
- Online 1v1: host + client connect and stay connected
- Server-authoritative match flow (states)
- Coin flip decides attacker/defender + spawn sides
- Each player selects exactly 1 card (or auto-assigned for first test)
- Battle phase with 1 simple win condition
- Results screen (winner + rematch/exit)

## Out of Scope (For Now)
- Full board game progression / multiple spaces
- Deckbuilding, inventory, progression, cosmetics
- Multiple maps, ranked, matchmaking, chat
- Complex abilities, upgrades, or deep balance

---

## NOW (Build the Vertical Slice)
### Epic: Match Flow
- [ ] Implement match state machine: Lobby → CoinFlip → CardSelect → Battle → Results
- [ ] Server flips coin and syncs roles/sides to clients
- [ ] Transition into battle (scene load or camera switch)

### Epic: Win Condition
- [ ] Decide MVP win condition (recommended: first to 3 hits)
- [ ] Server-authoritative scoring + match end trigger
- [ ] Basic HUD: show score/timer and winner

### Epic: Cards & Units
- [ ] Create 3 placeholder cards (Swordsman / Archer / Knight)
- [ ] Store card data (ScriptableObject or simple config)
- [ ] Spawn the correct unit prefab based on selected card
- [ ] Basic combat loop (hit detection → damage → score)

### Epic: Results & Reset
- [ ] Show Results UI (winner, score)
- [ ] Rematch flow (reset state + respawn) OR return to lobby

---

## NEXT (Make it Feel Like a Game)
- [ ] Simple lobby UI (ready button)
- [ ] Simple card select UI (3 choices + confirm + timer)
- [ ] Better hit feedback (sound/VFX, hit markers)
- [ ] Basic animations (run/attack)
- [ ] Basic camera + arena boundaries

---

## LATER (Icebox)
- [ ] Board/space control system (multiple turns/spaces)
- [ ] More cards, abilities, and balance pass
- [ ] Matchmaking + dedicated servers
- [ ] Progression, cosmetics, ranked
