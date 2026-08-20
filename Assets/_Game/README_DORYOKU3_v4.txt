KATSUHIRO PROTOTYPE v4 — DORYOKU-3 POSSÉDÉ
================================================

Cette version est basée sur le Assets(1).zip fourni.

NOUVEL ENNEMI FONCTIONNEL
-------------------------
Doryoku-3 possédé :
- automate industriel d'environ 3 mètres
- torse blindé et chaudière centrale
- quatre bras/outils articulés
- tuyaux de cuivre
- lentille optique rouge
- apparition d'un yōkai interne avec le Kikai-Yūrei

IA
--
État PATROL :
- l'automate patrouille autour de son point d'apparition.

État CHASE :
- il détecte Kenjiro à proximité et le poursuit.

État ATTACK :
- télégraphie mécanique
- bras avant qui se projette
- dégâts
- recul de Kenjiro
- récupération avant la prochaine attaque.

POINTS DE VIE
-------------
- 6 PV par défaut.
- Les attaques de Kenjiro enlèvent 1 PV.
- Barre de vie visible au-dessus de l'automate.
- À 0 PV : l'automate s'effondre puis disparaît.

KIKAI-YŪREI
------------
Monde normal :
- armure métallique
- œil rouge / carmin
- corruption interne cachée.

Monde éthérique (touche K) :
- yōkai interne révélé
- noyau violet
- tête spectrale
- cornes et filaments éthériques
- lumière de corruption
- lentille optique plus intense.

KENJIRO
-------
Player_Kenjiro reçoit maintenant PlayerHealth :
- 5 PV
- invulnérabilité courte après un coup
- knockback lors de l'attaque du Doryoku-3
- respawn automatique au point de départ après défaite.

ATTAQUE DE KENJIRO
------------------
PlayerAttackPrototype est amélioré :
- fonctionne désormais à gauche ET à droite
- frappe tout composant implémentant IDamageable
- compatible PrototypeEnemy et Doryoku3Enemy.

SCÈNE
-----
Foundry_Prototype contient automatiquement deux unités :
- Doryoku3_Unit07 à droite du départ
- Doryoku3_Unit11 à gauche

La première unité est volontairement placée assez près pour tester
immédiatement détection, poursuite et combat.

PREMIÈRE OUVERTURE
------------------
1. Ouvrir le projet Unity.
2. Vérifier que Input System est installé.
3. Laisser Unity compiler.
4. Le Prefab Doryoku3_Possessed est créé automatiquement.
5. Foundry_Prototype est reconstruite avec les deux ennemis.

En cas de besoin :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

Ou uniquement pour le prefab :
Tools > Katsuhiro > Rebuild Doryoku-3 Possessed Prefab

FICHIERS AJOUTÉS
----------------
Assets/_Game/Scripts/Combat/IDamageable.cs
Assets/_Game/Scripts/Player/PlayerHealth.cs
Assets/_Game/Scripts/Enemies/Doryoku3Enemy.cs
Assets/_Game/Scripts/Enemies/Doryoku3VisualController.cs
Assets/_Game/Editor/Doryoku3PrototypeBuilder.cs

FICHIERS MODIFIÉS
-----------------
Assets/_Game/Scripts/Combat/PlayerAttackPrototype.cs
Assets/_Game/Scripts/Enemies/PrototypeEnemy.cs
Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs

NOTE GRAPHIQUE
--------------
Le Doryoku-3 est un prefab 3D fonctionnel de prototype, construit avec
des primitives Unity et des matériaux URP. Il est prêt pour gameplay,
tests d'échelle, silhouettes, VFX et IA. Le modèle artistique final
pourra ensuite remplacer ModelRoot sans réécrire l'IA.
