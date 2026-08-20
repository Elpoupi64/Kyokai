KATSUHIRO PROTOTYPE v7 — COMBAT COMPLET DE KENJIRO
====================================================

Cette version est cumulative et basée sur le Assets(5).zip fourni.
Les systèmes précédents restent présents :
- déplacement 2,5D ;
- Kikai-Yūrei normal / éthérique ;
- Doryoku-3 ;
- VFX ;
- mini-boss deux phases ;
- caméra boss et barre cinématique.

NOUVEAU COMBAT KENJIRO
----------------------

1. COMBO LÉGER — J
   - J : coup 1
   - J, J : coup 2
   - J, J, J : coup 3 / finisher
   - dégâts : 1 / 1 / 2
   - chaque impact recharge un peu l'éther.

2. ATTAQUE LOURDE — I
   - attaque plus lente ;
   - plus grande portée ;
   - 3 dégâts ;
   - forte recharge d'éther.

3. ATTAQUE AÉRIENNE — J EN L'AIR
   - lorsque Kenjiro est en saut, J déclenche automatiquement l'attaque aérienne ;
   - 2 dégâts ;
   - légère plongée vers le sol ;
   - une seule attaque aérienne par saut.

4. ESQUIVE — SHIFT GAUCHE
   - dash rapide dans la direction regardée ;
   - courte invulnérabilité ;
   - cooldown court ;
   - idéale pour traverser les ondes du mini-boss.

5. KIKAI-YŪREI — K
   - K conserve sa fonction précédente :
     monde normal <-> monde éthérique.

6. ATTAQUE SPÉCIALE KIKAI-YŪREI — L
   - fonctionne uniquement lorsque Kikai-Yūrei est actif ;
   - coûte 50 points d'éther ;
   - lance une décharge cyan ;
   - 4 dégâts ;
   - le projectile disparaît si Kenjiro retourne au monde normal.

JAUGE D'ÉTHER
-------------
- maximum : 100 ;
- départ : 100 pour faciliter les tests ;
- combo léger : +7 par cible touchée ;
- lourd : +14 ;
- aérien : +10 ;
- régénération lente en monde éthérique ;
- spéciale : -50.

HUD
---
Le HUD de combat affiche :
- PV ;
- jauge d'éther ;
- combo en cours ;
- disponibilité de la spéciale ;
- rappel des touches.

MANETTE
-------
Bouton Ouest      : attaque légère / aérienne
Right Shoulder    : attaque lourde
Bouton Est        : esquive
Right Trigger     : spéciale Kikai-Yūrei
Bouton Nord       : activer/désactiver Kikai-Yūrei
Bouton Sud        : saut

ANIMATIONS
----------
Le modèle blockout actuel reçoit maintenant des animations procédurales :
- alternance des bras pendant le combo ;
- préparation et frappe lourde ;
- attaque aérienne ;
- inclinaison pendant le dash ;
- charge lumineuse du Kikai-Yūrei pendant la spéciale.

Ces animations pourront être remplacées plus tard par un vrai Animator
et des clips provenant de Blender/Maya sans modifier le gameplay.

COMPATIBILITÉ MINI-BOSS
-----------------------
Doryoku3BossEncounter verrouille maintenant KenjiroCombatController
pendant l'introduction cinématique et le réactive au début du combat.

PlayerHealth désactive également le nouveau combat lors d'une défaite
et le réactive au respawn.

MISE À NIVEAU AUTOMATIQUE
-------------------------
Lors de l'import, KenjiroCombatUpgradeBuilder détecte une ancienne scène
v6 et reconstruit Foundry_Prototype avec CombatSystem_v7.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

FICHIERS AJOUTÉS
----------------
Assets/_Game/Scripts/Combat/KenjiroCombatController.cs
Assets/_Game/Scripts/Combat/KenjiroCombatVisuals.cs
Assets/_Game/Scripts/Combat/KenjiroKikaiBurstProjectile.cs
Assets/_Game/Scripts/UI/KenjiroCombatHUD.cs
Assets/_Game/Editor/KenjiroCombatUpgradeBuilder.cs

FICHIERS MODIFIÉS
-----------------
Assets/_Game/Scripts/Player/PlayerHealth.cs
Assets/_Game/Scripts/Enemies/Doryoku3BossEncounter.cs
Assets/_Game/Editor/Doryoku3MiniBossBuilder.cs
Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs

TOUCHES RÉSUMÉES
----------------
A / D ou flèches : déplacement
Espace            : saut
J                  : combo léger / attaque aérienne
I                  : attaque lourde
Shift gauche       : esquive
K                  : Kikai-Yūrei
L                  : attaque spéciale Kikai-Yūrei
