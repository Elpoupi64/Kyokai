KATSUHIRO PROTOTYPE v8 — COMBAT POLISH / ANIMATOR / GAME FEEL
================================================================

Cette version est cumulative et construite à partir du Assets(6).zip fourni.

LES ÉTAPES PRÉCÉDENTES RESTENT INCLUSES
---------------------------------------
- déplacement et plateforme 2,5D ;
- caméra ;
- Kenjiro prototype ;
- Kikai-Yūrei normal / éthérique ;
- plateformes spectrales ;
- Doryoku-3 possédé ;
- vapeur / étincelles / explosion ;
- mini-boss Doryoku-3 à deux phases ;
- barre de boss et caméra cinématique ;
- combo de Kenjiro, lourd, aérien, esquive et spéciale.

NOUVEAUTÉS v8
=============

1. VRAI ANIMATOR UNITY
-----------------------
Un Animator Controller est généré automatiquement :

Assets/_Game/Animations/Kenjiro/KenjiroAnimatorController.controller

Les AnimationClips de prototype sont également créés automatiquement :
- Idle
- Run
- Jump
- Fall
- Attack1
- Attack2
- Attack3
- HeavyAttack
- AirAttack
- Dodge
- DodgeCounter
- KikaiSpecial
- Hurt
- Death

Il s'agit de vrais assets AnimationClip / AnimatorController Unity.
Les clips animent le blockout actuel et pourront être remplacés plus tard
par les animations FBX définitives sans refaire la logique de combat.

2. HIT-STOP
-----------
Les impacts ralentissent brièvement le temps réel :
- léger : environ 0,045 s ;
- finisher : environ 0,070 s ;
- lourd : environ 0,080 s ;
- aérien : environ 0,055 s ;
- contre après esquive : environ 0,070 s.

HitStopManager est créé automatiquement dans GAMEPLAY.

3. VFX DE COMBAT
----------------
CombatImpactFX :
- étincelles mécaniques orange lors des coups ;
- énergie cyan pour le Kikai-Yūrei.

AttackTrail :
- traînée sur le bras avant pendant les attaques.

LandingFX :
- poussière à l'atterrissage selon la vitesse de chute.

DodgeFX :
- burst de poussière au démarrage de l'esquive.

KikaiSpecialFX :
- concentration de particules cyan ;
- amplification de la lumière du Kikai-Yūrei ;
- burst au tir.

4. RÉACTION AUX DÉGÂTS
-----------------------
KenjiroDamageReaction ajoute :
- animation Hurt ;
- flash blanc ;
- hit-stop court ;
- secousse de caméra.

PlayerHealth expose maintenant :
- Damaged ;
- Defeated ;
- Respawned.

La mort déclenche l'animation Death.
Le respawn réinitialise l'Animator.

5. COMBOS AMÉLIORÉS
-------------------

J → J → J
    combo léger complet

J → J → I
    finisher lourd
    dégâts supérieurs au lourd standard

Espace → J
    attaque aérienne

Shift → J
    esquive puis contre-attaque
    le contre frappe plus fort et recharge davantage l'éther

K → L
    monde éthérique puis décharge Kikai-Yūrei

COMMANDES
---------
A / D ou flèches : déplacement
Espace            : saut
J                  : léger / aérien / contre après esquive
I                  : lourd / finisher après J → J
Shift gauche       : esquive
K                  : Kikai-Yūrei
L                  : attaque spéciale Kikai-Yūrei

MANETTE
-------
Bouton Ouest       : léger / aérien
Right Shoulder     : lourd
Bouton Est         : esquive
Bouton Nord        : Kikai-Yūrei
Right Trigger      : spéciale
Bouton Sud         : saut

FICHIERS AJOUTÉS
----------------
Assets/_Game/Scripts/Core/HitStopManager.cs
Assets/_Game/Scripts/Player/KenjiroAnimatorDriver.cs
Assets/_Game/Scripts/Player/KenjiroDamageReaction.cs
Assets/_Game/Scripts/FX/CombatImpactFX.cs
Assets/_Game/Scripts/FX/AttackTrail.cs
Assets/_Game/Scripts/FX/DodgeFX.cs
Assets/_Game/Scripts/FX/LandingFX.cs
Assets/_Game/Scripts/FX/KikaiSpecialFX.cs
Assets/_Game/Editor/KenjiroAnimatorBuilder.cs
Assets/_Game/Editor/KenjiroCombatPolishUpgradeBuilder.cs

FICHIERS MODIFIÉS
-----------------
Assets/_Game/Scripts/Combat/KenjiroCombatController.cs
Assets/_Game/Scripts/Combat/KenjiroCombatVisuals.cs
Assets/_Game/Scripts/Player/PlayerHealth.cs
Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs

MISE À NIVEAU AUTOMATIQUE
-------------------------
Lors de la première compilation, la scène v7 est détectée et reconstruite
en v8 si CombatPolish_v8 est absent.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

TEST CONSEILLÉ
--------------
1. Tester J → J → J contre le boss.
2. Tester J → J → I.
3. Sauter puis J.
4. Shift puis J.
5. Activer K puis L.
6. Se laisser toucher pour observer Hurt + flash + camera shake.
7. Tomber d'une plateforme pour observer LandingFX.
