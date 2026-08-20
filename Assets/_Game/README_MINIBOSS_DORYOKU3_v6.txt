KATSUHIRO PROTOTYPE v6 — MINI-BOSS DORYOKU-3
================================================

Cette archive est basée sur le Assets(3).zip fourni.

RENCONTRE MINI-BOSS
-------------------
Le combat de la Fonderie devient maintenant une vraie rencontre de boss :
DORYOKU-3 // UNITÉ 07 — AUTOMATE POSSÉDÉ

Le boss ne s'active pas immédiatement.
Kenjiro doit s'approcher de l'arène.

INTRO CINÉMATIQUE
-----------------
Lorsque Kenjiro entre dans la zone :
- les portes invisibles de l'arène se ferment ;
- les contrôles sont brièvement verrouillés ;
- la caméra passe en cadrage "boss" et garde Kenjiro + Doryoku-3 à l'écran ;
- le nom du boss apparaît ;
- la barre de vie cinématique apparaît en haut de l'écran ;
- après environ 2 secondes, les contrôles sont rendus au joueur.

PHASE I — PROTOCOLE D'EXÉCUTION
--------------------------------
100 % à 51 % de vie.

Attaques :
1. Pince mécanique de mêlée.
2. Frappe au sol :
   - le boss frappe le sol ;
   - deux ondes partent à gauche et à droite.
3. Attaque Kikai-Yurei :
   - uniquement lorsque Kenjiro active le monde éthérique avec K ;
   - le Doryoku-3 charge puis tire un projectile spectral.

PHASE II — FUREUR ÉTHÉRIQUE
-----------------------------
Déclenchée automatiquement à 50 % de vie.

Pendant la transition :
- le boss devient temporairement invulnérable ;
- charge éthérique ;
- secousse de caméra ;
- double onde au sol.

Modifications :
- déplacement plus rapide ;
- mêlée plus rapide et plus dangereuse ;
- frappe au sol plus fréquente ;
- deuxième salve d'ondes au sol ;
- attaque spectrale plus fréquente ;
- trois projectiles spectraux au lieu d'un.

ATTAQUE AU SOL
--------------
Les ondes de choc sont de vrais objets de gameplay :
- collision avec Kenjiro ;
- dégâts ;
- knockback ;
- déplacement horizontal ;
- version violette renforcée en phase II.

KIKAI-YUREI
------------
La spéciale spectrale du boss reste liée au système déjà créé :
- monde normal : impossible de lancer la spéciale ;
- monde éthérique : la spéciale devient disponible ;
- si Kenjiro revient au monde normal pendant la charge, elle est annulée ;
- les projectiles spectraux déjà en vol disparaissent si le Kikai-Yurei est coupé.

BARRE DE VIE
------------
L'ancienne petite barre au-dessus du Doryoku-3 est désactivée pour le boss.
Une grande barre cinématique est maintenant affichée en haut de l'écran avec :
- nom ;
- sous-titre ;
- phase actuelle ;
- transition de Phase II.

CAMÉRA DE BOSS
--------------
CameraFollow25D a été amélioré avec :
- EnterBossMode();
- ExitBossMode();
- cadrage dynamique entre Kenjiro et le boss ;
- recul automatique de la caméra selon leur distance ;
- secousses pour les impacts, l'enrage et la mort.

FIN DU COMBAT
-------------
À la mort du boss :
- explosion VFX v5 ;
- secousse de caméra ;
- barrières de l'arène désactivées ;
- caméra rendue à Kenjiro ;
- barre de boss masquée.

COMMANDES
---------
A / D ou flèches : déplacement
Espace            : saut
J                  : attaque Kenjiro
K                  : Kikai-Yurei normal / éthérique

FICHIERS AJOUTÉS
----------------
Assets/_Game/Scripts/Enemies/Doryoku3MiniBoss.cs
Assets/_Game/Scripts/Enemies/Doryoku3GroundShockwave.cs
Assets/_Game/Scripts/Enemies/Doryoku3BossEncounter.cs
Assets/_Game/Scripts/UI/Doryoku3BossHUD.cs
Assets/_Game/Editor/Doryoku3MiniBossBuilder.cs

FICHIERS MODIFIÉS
-----------------
Assets/_Game/Scripts/Camera/CameraFollow25D.cs
Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs

CRÉATION AUTOMATIQUE
--------------------
À la première compilation, Doryoku3MiniBossBuilder vérifie la scène.
Si la rencontre v6 n'est pas présente, Foundry_Prototype est reconstruite.

Forçage manuel :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

NOTE
----
Le système reste volontairement un prototype Unity sans dépendance à
TextMeshPro ou à un framework de boss externe. La barre cinématique est
faite avec l'IMGUI intégré afin de fonctionner dès l'import du projet.
