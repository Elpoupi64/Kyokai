KATSUHIRO PROTOTYPE v12 — ART DIRECTION FINAL PASS
====================================================

Cette version est cumulative et construite à partir de Assets(10).zip.
Toutes les versions précédentes restent incluses, jusqu'à la v11.

OBJECTIF
--------
Passer d'un décor modulaire propre à une présentation plus proche d'une
direction artistique finale : davantage de hiérarchie visuelle, des
silhouettes plus illustrées, une lecture plus forte du parcours et un
habillage narratif plus marqué.

CE QUE LA v12 AJOUTE
--------------------
1. FINAL ART PASS
   - nouveau builder : FoundryFinalArtPassBuilder.cs
   - nouveau marker de scène : FoundryFinalArt_v12
   - reconstruction automatique si la scène n'a pas encore reçu ce pass

2. ATMOSPHÈRE VISUELLE PLUS FORTE
   - fog légèrement retravaillé
   - contraste coloré chaud / cyan plus marqué
   - fond caméra plus dramatique
   - palette plus directionnelle

3. BACKDROP "PAINTERLY"
   - nouvelles couches parallaxe dédiées
   - disque solaire / lueur industrielle lointaine
   - silhouettes profondes d'usines
   - cartes de fumée et lecture plus illustrée

4. DRESSING NARRATIF
   - bannières suspendues
   - panneaux de signalétique
   - lanternes suspendues
   - zones identifiées : KATSUHIRO, FOUNDRY, ETHER LOCK, DANGER,
     SYNCHRONIZE, KEEP MOVING, UNIT 07, BREACH

5. PROFONDEUR DE PREMIER PLAN
   - cadres de premier plan
   - gros tuyaux proches caméra
   - piliers silhouettes
   - bannières d'avant-plan

6. BOSS ARENA FINAL DRESS
   - bannières UNIT / 07
   - panneau KIKAI-YUREI
   - halo de faille éthérique
   - lanternes au-dessus de l'arène
   - encadrement plus théâtral

NOUVEAU SCRIPT RUNTIME
----------------------
Assets/_Game/Scripts/Environment/FoundryAutoSway.cs

Utilisé pour donner un mouvement subtil aux bannières et aux éléments
suspendus, afin de casser l'effet trop rigide des primitives.

NOUVEAUX BUILDERS
-----------------
Assets/_Game/Editor/FoundryFinalArtPassBuilder.cs
Assets/_Game/Editor/FoundryFinalArtPassUpgradeBuilder.cs

NOUVEAUX MATÉRIAUX
------------------
Assets/_Game/Art/Materials/FoundryFinalPass/

Exemples :
- Final_BannerRed
- Final_Parchment
- Final_Jade
- Final_InkDark
- Final_WarmGlow
- Final_CyanGlow
- Final_SunDisc
- Final_SmokeCard
- Final_SilhouetteDeep
- Final_ForegroundFrame

RÉSULTAT DANS LA HIÉRARCHIE
---------------------------
LEVEL
├── FOUNDRY_ART_V11
└── FOUNDRY_ART_V12
    ├── Painterly_Backdrop_Far
    ├── Painterly_Backdrop_Mid
    ├── Painterly_Backdrop_Near
    ├── Narrative_SetDressing
    ├── Foreground_Frame
    └── FoundryFinalArt_v12

MINI_BOSS_ARENA
└── BossArena_FinalDress

MISE À NIVEAU
-------------
À la première compilation, FoundryFinalArtPassUpgradeBuilder détecte une
scène plus ancienne et déclenche la reconstruction de Foundry_Prototype.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

BUT DE CETTE VERSION
--------------------
La v12 ne remplace pas encore tout par des assets peints finaux, mais elle
pose clairement la direction artistique du prototype :
- lecture plus forte,
- ambiance plus mémorable,
- meilleur storytelling visuel,
- meilleur sentiment de "lieu".

PROCHAINE ÉTAPE CONSEILLÉE
--------------------------
La prochaine vraie étape artistique serait une v13 centrée sur :
- remplacement de certains modules par des meshes peints définitifs ;
- variations plus organiques par zone ;
- vrai arrière-plan illustré multi-plans ;
- modules spécifiques au style final du projet.
