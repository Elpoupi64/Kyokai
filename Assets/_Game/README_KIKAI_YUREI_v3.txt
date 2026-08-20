KATSUHIRO PROTOTYPE v3 — KENJIRO + KIKAI-YUREI
==================================================

CE QUI CHANGE
-------------
1. La capsule visuelle est remplacée par un vrai Prefab Unity :
   Assets/_Game/Prefabs/Characters/Kenjiro/Kenjiro_Prototype.prefab

   Le Prefab est généré dans Unity à partir d'un blockout 3D stylisé :
   - tête / cheveux
   - costume
   - bras et jambes séparés
   - sacoche
   - Kikai-Yurei visible avec noyau lumineux
   - composant Animator prêt à recevoir un Animator Controller

   Le CapsuleCollider reste volontairement sur Player_Kenjiro :
   il sert uniquement à la physique du platformer.

2. Nouveau système Kikai-Yurei :
   - KikaiWorldManager.cs
   - KikaiWorldVisibility.cs
   - KikaiYureiController.cs
   - PrototypeWorldModeHUD.cs

3. Contrôle :
   K = Normal <-> Éthérique
   Gamepad = bouton North

4. Dans le monde éthérique :
   - les plateformes SpiritPlatform apparaissent
   - leurs colliders deviennent actifs
   - des silhouettes de yokai et une faille d'éther apparaissent
   - l'atmosphère/fog change
   - le noyau du Kikai-Yurei devient plus lumineux

5. Dans le monde normal :
   - les éléments spirituels sont invisibles et non-collisionnels
   - les machines normales de la fonderie restent visibles

PREMIÈRE OUVERTURE DANS UNITY
-----------------------------
- Vérifier que le package Input System est installé.
- Laisser Unity compiler.
- Le script Editor détecte l'absence du nouveau Prefab et reconstruit
  automatiquement Foundry_Prototype.
- Si nécessaire :
  Tools > Katsuhiro > Create or Rebuild Foundry Prototype

PREFAB KENJIRO
--------------
Le blockout fourni est un premier vrai Prefab de production/prototypage,
mais PAS le modèle 3D final sculpté/riggé.

Quand le modèle Blender/FBX définitif sera prêt, il suffira de remplacer
le contenu visuel de Visual_Kenjiro sans changer :
- Rigidbody
- CapsuleCollider
- PlayerMotor25D
- KikaiYureiController
- GroundCheck
- AttackPoint

RÉFÉRENCE
---------
Le concept art de Kenjiro est copié dans :
Assets/_Game/Art/Characters/Kenjiro/Reference/Kenjiro_Concept.png
