KATSUHIRO PROTOTYPE v13 — HERO ASSETS + PAINTED BACKGROUNDS PASS
==================================================================

Cette version est cumulative et construite à partir de Assets(20260820-061416).zip.
Toutes les versions précédentes restent incluses jusqu'à la v12.

OBJECTIF
--------
Commencer la vraie montée en qualité vers des assets "signature" :
- quelques pièces héro plus imposantes ;
- davantage de distinction entre les zones ;
- arrière-plans plus riches et plus illustrés ;
- meilleure sensation de lieu et de progression.

CE QUE LA v13 APPORTE
---------------------

1. HERO ASSET LIBRARY
   Un nouveau builder génère une bibliothèque de prefabs dédiés :
   Assets/_Game/Prefabs/Environment/HeroAssets/

   Prefabs générés :
   - Hero_MeijiGate.prefab
   - Hero_TitanBoiler.prefab
   - Hero_KikaiShrine.prefab
   - Hero_CraneAssembly.prefab
   - Hero_PipeSpine.prefab
   - Hero_PaintedPanelWarm.prefab
   - Hero_PaintedPanelCyan.prefab
   - Hero_RooflineCluster.prefab
   - Hero_ClothStrip.prefab

2. PASS SCÈNE v13
   Le nouveau FoundryHeroAssetsPassBuilder :
   - ajoute un fond plus riche en plusieurs couches ;
   - installe des hero assets spécifiques dans les zones ;
   - fait varier visuellement l'entrée, la zone éthérique,
     la production et l'approche du boss ;
   - ajoute un habillage dédié dans l'arène du mini-boss.

3. VARIATIONS VISUELLES PAR ZONE
   - EntryZone_HeroAssets
   - EtherZone_HeroAssets
   - ProductionZone_HeroAssets
   - BossApproach_HeroAssets

   Chaque zone reçoit un accent visuel différent :
   - entrée : porte Meiji + chaudière majeure ;
   - zone Kikai-Yūrei : sanctuaire / cœur éthérique ;
   - production : grue, chaudière et dorsales de tuyaux ;
   - boss : reprise des marqueurs éthériques et industriels.

4. PAINTED BACKGROUNDS PASS
   Trois couches sont ajoutées :
   - HeroBackdrop_Far
   - HeroBackdrop_Mid
   - HeroBackdrop_Near

   Elles utilisent :
   - panneaux peints chauds ;
   - panneaux peints cyan ;
   - silhouettes de toitures industrielles ;
   - léger drift pour éviter une scène trop figée.

5. BOSS ARENA HERO PASS
   L'arène obtient :
   - un grand MeijiGate de fond ;
   - un KikaiShrine central ;
   - deux grandes dorsales de tuyaux ;
   - des bandes de tissu suspendues.

NOUVEAUX SCRIPTS
----------------
Assets/_Game/Scripts/Environment/
- FoundryBackdropDrift.cs

Utilisé sur certains éléments de fond pour créer un mouvement
très léger, plus organique, dans l'arrière-plan.

NOUVEAUX BUILDERS
-----------------
Assets/_Game/Editor/
- FoundryHeroAssetLibraryBuilder.cs
- FoundryHeroAssetsPassBuilder.cs
- FoundryHeroAssetsUpgradeBuilder.cs

NOUVEAUX MATÉRIAUX
------------------
Assets/_Game/Art/Materials/FoundryHeroPass/

Exemples :
- Hero_BodyIron
- Hero_PaintedCrimson
- Hero_Brass
- Hero_FurnaceGlow
- Hero_EtherCore
- Hero_BackdropInk
- Hero_Cloth
- etc.

HIÉRARCHIE
----------
LEVEL
└── FOUNDRY_ART_V13
    ├── HeroBackdrop_Far
    ├── HeroBackdrop_Mid
    ├── HeroBackdrop_Near
    ├── EntryZone_HeroAssets
    ├── EtherZone_HeroAssets
    ├── ProductionZone_HeroAssets
    ├── BossApproach_HeroAssets
    └── FoundryHeroAssets_v13

MINI_BOSS_ARENA
└── BossArena_HeroAssets_v13

MISE À NIVEAU
-------------
À la première compilation, FoundryHeroAssetsUpgradeBuilder détecte une
scène plus ancienne et relance la reconstruction de Foundry_Prototype.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

PROCHAINE ÉTAPE CONSEILLÉE
--------------------------
La suite la plus naturelle serait une v14 orientée :
- vrais meshes peints manuellement ;
- remplacement de certaines silhouettes par des sprites/illustrations ;
- VFX ambiants plus avancés ;
- réglages finaux lumière / post-process / lisibilité gameplay.
