KATSUHIRO PROTOTYPE v10 — FONDERIE KATSUHIRO MEIJI-STEAMPUNK
================================================================

Cette version est cumulative et construite à partir de Assets(8).zip.
Toutes les étapes précédentes restent incluses :
- base 2,5D ;
- Kikai-Yūrei ;
- Kenjiro combat v8 ;
- Doryoku-3 ;
- mini-boss ;
- vertical slice v9.

OBJECTIF DE CETTE VERSION
-------------------------
Transformer le greybox de la vertical slice en une vraie Fonderie Katsuhiro
de style Meiji-steampunk, sans casser le gameplay déjà construit.

CE QUI A ÉTÉ AJOUTÉ
-------------------

1. DÉCOR ENTIÈREMENT HABILLÉ
   - façades industrielles ;
   - arches métalliques ;
   - passerelles ;
   - garde-corps ;
   - chaînes suspendues ;
   - habillage du couloir et de l’approche du boss.

2. MACHINES ET CHAÎNES DE PRODUCTION
   - chaudières ;
   - fourneaux ;
   - conduites de cuivre ;
   - convoyeurs décoratifs ;
   - grosses roues dentées animées ;
   - rigs mécaniques et massifs d’usine.

3. PARALLAXE MULTICOUCHE
   Trois couches ont été ajoutées derrière le gameplay :
   - Parallax_Far
   - Parallax_Mid
   - Parallax_Near

   Elles utilisent le nouveau script :
   Assets/_Game/Scripts/Environment/ParallaxLayer.cs

4. LUMIÈRES ET ATMOSPHÈRE
   - lampes à gaz orange ;
   - lumières éthériques cyan ;
   - scintillement des lampes avec FoundryLightFlicker ;
   - brouillard atmosphérique ;
   - fond caméra sombre ;
   - meilleure ambiance pour la fonderie.

5. VAPEUR VOLUMÉTRIQUE
   Des SteamVent ont été ajoutés dans :
   - les arrière-plans ;
   - la zone d’entrée ;
   - la zone de combat ;
   - le corridor ;
   - l’arène du boss.

6. HABILLAGE DU BOSS
   L’arène du mini-boss reçoit :
   - un mur arrière dédié ;
   - des lampes ;
   - des roues dentées ;
   - des jets de vapeur ;
   - un cadre éthérique central.

NOUVEAUX SCRIPTS
----------------
Assets/_Game/Scripts/Environment/ParallaxLayer.cs
Assets/_Game/Scripts/Environment/FoundryLightFlicker.cs
Assets/_Game/Scripts/Environment/RotatingGear.cs

NOUVEAUX OUTILS EDITOR
----------------------
Assets/_Game/Editor/FoundrySteampunkArtBuilder.cs
Assets/_Game/Editor/FoundrySteampunkArtUpgradeBuilder.cs

FONCTIONNEMENT
--------------
FoundrySteampunkArtBuilder est appelé automatiquement à la fin de la
reconstruction de Foundry_Prototype. Il :
- masque l’ancien background greybox v9 ;
- ajuste l’atmosphère de scène ;
- recolore les surfaces de gameplay ;
- génère les couches de parallaxe ;
- habille le décor ;
- ajoute les machines, la vapeur et les lumières.

RÉSULTAT DANS LA HIÉRARCHIE
---------------------------
LEVEL
├── VERTICAL_SLICE_V9
├── MINI_BOSS_ARENA
└── FOUNDRY_ART_V10
    ├── Parallax_Far
    ├── Parallax_Mid
    ├── Parallax_Near
    ├── Architecture_And_Machines
    ├── Foreground_Decor
    └── FoundryArt_v10

MISE À NIVEAU
-------------
À la première compilation, FoundrySteampunkArtUpgradeBuilder détecte
une scène v9 sans art pass et reconstruit automatiquement la scène.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

PROCHAINE ÉTAPE RECOMMANDÉE
---------------------------
La meilleure suite logique est :
- remplacer progressivement certains blocs décoratifs par de vrais meshes ;
- intégrer des textures peintes ;
- créer des modules de décor réutilisables ;
- enrichir l’arrière-plan avec illustrations multicouches ;
- préparer le vrai style « Rayman Legends Retold » du projet.
