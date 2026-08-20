KATSUHIRO PROTOTYPE v9 — VERTICAL SLICE DE LA FONDERIE KATSUHIRO
==================================================================

Cette version est cumulative et construite à partir de Assets(7).zip.
Les versions précédentes restent incluses : Kikai-Yūrei, combat Kenjiro,
Animator, VFX, Doryoku-3, mini-boss, caméra de boss et polish v8.

PARCOURS JOUABLE
================

DÉBUT — x ≈ -72
↓
Tutoriel déplacement
- A / D ou flèches
- Espace pour sauter
- deux plateformes servent à tester la locomotion

↓
INTRODUCTION KIKAI-YŪREI
- le sol s'interrompt
- K active le monde éthérique
- quatre plateformes spectrales apparaissent
- leurs collisions n'existent qu'en mode éthérique

↓
PREMIER DORYOKU-3
- rencontre de combat avant le boss
- patrouille et détection standard
- rappel du combo J / I / Shift / K + L

↓
CHECKPOINT
- balise de synchronisation
- devient cyan lorsqu'elle est activée
- PlayerHealth reçoit un nouveau point de respawn
- après une mort, Kenjiro réapparaît à cette balise

↓
POURSUITE
- un Doryoku-3 plus rapide s'active dans le corridor
- vitesse de poursuite accrue
- le joueur doit continuer vers la droite
- arrivée dans la zone sûre : explosion du poursuivant

↓
APPROCHE DU MINI-BOSS
- court corridor avant l'arène
- la rencontre v6/v8 du mini-boss reste utilisée

↓
MINI-BOSS UNITÉ 07
- 30 PV
- phase I
- phase II à 50 %
- frappe au sol
- attaque spectrale liée au Kikai-Yūrei
- caméra cinématique et barre de boss

↓
VICTOIRE
- explosion du Doryoku-3
- ouverture de l'arène

↓
COURTE SÉQUENCE NARRATIVE
1. Le Doryoku-3 s'effondre.
2. Le Kikai-Yūrei détecte une signature comparable aux Forges Impériales.
3. Kenjiro comprend que quelqu'un reproduit volontairement la catastrophe.

↓
FIN DE LA VERTICAL SLICE
Chapitre 1 — Les Murmures de l'Acier
Fonderie Katsuhiro • Tokyo • 1889

NOUVEAUX SCRIPTS
================
Assets/_Game/Scripts/VerticalSlice/VerticalSliceDirector.cs
Assets/_Game/Scripts/VerticalSlice/VerticalSliceCheckpoint.cs
Assets/_Game/Scripts/VerticalSlice/VerticalSliceChaseSequence.cs

NOUVEAUX BUILDERS
=================
Assets/_Game/Editor/FoundryVerticalSliceBuilder.cs
Assets/_Game/Editor/FoundryVerticalSliceUpgradeBuilder.cs

MODIFICATIONS
=============
Assets/_Game/Scripts/Player/PlayerHealth.cs
- SetRespawnPoint(Vector3)
- GetRespawnPoint()

Assets/_Game/Scripts/Enemies/Doryoku3BossEncounter.cs
- respecte le verrouillage de contrôle pendant l'épilogue v9

Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs
- génère désormais la vertical slice complète

SCÈNE GÉNÉRÉE
==============
La hiérarchie principale devient approximativement :

LEVEL
└── VERTICAL_SLICE_V9
    ├── 01_Tutorial_Movement
    ├── 02_Kikai_Yurei_Bridge
    ├── 03_First_Doryoku3_Combat
    ├── 04_Checkpoint_And_Chase_Corridor
    └── Industrial_Background_Greybox
└── MINI_BOSS_ARENA

GAMEPLAY
├── Player_Kenjiro
├── KikaiWorld
├── HitStopManager
├── VerticalSliceDirector
└── VerticalSlice_v9

COMMANDES
=========
A / D ou flèches : déplacement
Espace            : saut
J                  : combo léger / attaque aérienne / contre après esquive
I                  : lourd / finisher après J → J
Shift gauche       : esquive
K                  : monde normal ↔ éthérique
L                  : attaque spéciale Kikai-Yūrei

MISE À NIVEAU
=============
À la première compilation, FoundryVerticalSliceUpgradeBuilder détecte
l'ancienne scène v8 et reconstruit automatiquement Foundry_Prototype.

Forçage manuel :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

IMPORTANT
=========
Cette v9 est une VERTICAL SLICE DE GAMEPLAY / GREYBOX.
Elle organise maintenant correctement le rythme et les systèmes du niveau.
Le prochain jalon graphique pourra remplacer le greybox par la vraie
fonderie Meiji-steampunk sans refaire le parcours de gameplay.
