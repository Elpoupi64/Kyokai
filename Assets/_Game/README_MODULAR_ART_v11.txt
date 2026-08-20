KATSUHIRO PROTOTYPE v11 — MODULES ARTISTIQUES ET PASS MODULAIRE
==================================================================

Cette version est cumulative et construite à partir de Assets(9).zip.
Toutes les étapes précédentes restent incluses jusqu'à la v10.

OBJECTIF
--------
Commencer à remplacer le décor purement généré de la v10 par de vrais
modules artistiques réutilisables, sous forme de PREFABS dédiés,
afin de préparer une montée en qualité plus proche du rendu final.

NOUVEAU FLUX
------------
1. FoundrySteampunkArtBuilder continue d'établir :
   - la palette de matériaux,
   - l'ambiance,
   - la base de l'art pass.

2. FoundryModularArtPassBuilder applique ensuite :
   - une bibliothèque de modules préfabriqués ;
   - l'assemblage de la scène à partir de ces modules ;
   - un remplacement progressif des volumes décoratifs par des éléments
     de décor identifiables, réutilisables et extensibles.

BIBLIOTHÈQUE DE MODULES
-----------------------
Un nouveau builder crée les prefabs sous :

Assets/_Game/Prefabs/Environment/FoundryModules/

Modules générés :
- WallFacade_A.prefab
- WallFacade_B.prefab
- ArchGate_A.prefab
- Boiler_Small.prefab
- Boiler_Large.prefab
- Furnace_Machine.prefab
- Conveyor_Line.prefab
- PipeRack_Modular.prefab
- GearAssembly_Small.prefab
- GearAssembly_Large.prefab
- GasLamp_Modular.prefab
- SteamVent_Modular.prefab
- EtherNode_Modular.prefab
- RailSection_Modular.prefab
- ChainHang_Modular.prefab
- BackgroundFactory_A.prefab
- BackgroundFactory_B.prefab

NOUVEAUX SCRIPTS RUNTIME
------------------------
Assets/_Game/Scripts/Environment/
- FoundryModulePulse.cs

Ce composant donne une pulsation légère aux éléments émissifs
(ex: fourneaux, nœuds éthériques) pour rendre les modules plus vivants.

NOUVEAUX BUILDERS EDITOR
------------------------
Assets/_Game/Editor/
- FoundryArtModuleLibraryBuilder.cs
- FoundryModularArtPassBuilder.cs
- FoundryModularArtUpgradeBuilder.cs

CE QUI CHANGE VISUELLEMENT
--------------------------
- la scène est réassemblée avec de vrais modules de décor ;
- les façades sont plus cohérentes ;
- les boilers, machines, convoyeurs et racks de tuyaux deviennent
  des blocs artistiques identifiables et réutilisables ;
- le background de parallaxe s'appuie sur des modules factory dédiés ;
- l'arène du boss reçoit aussi une version modulaire ;
- l'étape suivante consistera à remplacer certains modules par de vrais
  meshes peints / sculptés sans toucher au layout gameplay.

HIÉRARCHIE AJOUTÉE
------------------
LEVEL
└── FOUNDRY_ART_V11
    ├── Parallax_Far_Modular
    ├── Parallax_Mid_Modular
    ├── Parallax_Near_Modular
    ├── Environment_Modules
    └── FoundryModularArt_v11

SCÈNE DU BOSS
-------------
MINI_BOSS_ARENA
└── BossArena_ModularDress

MISE À NIVEAU AUTOMATIQUE
-------------------------
À la première compilation, FoundryModularArtUpgradeBuilder détecte
une scène antérieure sans marqueur v11 et reconstruit la scène.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

PROCHAINE ÉTAPE CONSEILLÉE
--------------------------
La suite logique est maintenant :
1. remplacer certains prefabs par de vrais meshes peints ;
2. créer des variantes artistiques A/B/C par type de module ;
3. ajouter textures stylisées et silhouettes plus organiques ;
4. intégrer progressivement le rendu final proche de la direction
   graphique visée.
