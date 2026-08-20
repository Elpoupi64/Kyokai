KATSUHIRO PROTOTYPE v14 — TRUE PAINTED ASSETS + ATMOSPHERE PASS
=================================================================

Cette version est cumulative et construite à partir du zip fourni.
Toutes les étapes précédentes restent incluses jusqu'à la v13.

IMPORTANT
---------
Les textures peintes de cette v14 sont de vrais fichiers PNG intégrés
dans le projet. Elles constituent un placeholder artistique avancé et
cohérent pour valider la direction visuelle ; elles ne remplacent pas
encore des illustrations finales produites par un artiste.

NOUVEAUX VISUELS PEINTS
-----------------------
Assets/_Game/Art/Textures/FoundryPainted/

- FoundrySky_WarmPainted.png
- FoundrySky_EtherPainted.png
- FoundrySmoke_Painted.png
- FoundryInk_Silhouette.png
- FoundryBoss_RiftPainted.png
- FoundrySoftParticle.png

Ces textures sont désormais utilisées sur de vrais plans de décor
multicouches plutôt que seulement sur des matériaux plats.

SILHOUETTES PLUS ORGANIQUES
----------------------------
Le builder génère également de vrais Mesh assets :
Assets/_Game/Art/Meshes/FoundryPainted/

- OrganicSkyline_A.asset
- OrganicSkyline_B.asset

Leur ligne de toit est irrégulière afin de casser l'aspect géométrique
des anciens blocs.

VFX AMBIANTS
------------
Nouveau groupe Atmosphere_VFX :
- poussières lentes ;
- braises de production ;
- brume basse ;
- particules d'éther visibles uniquement avec le Kikai-Yūrei.

ÉCLAIRAGE
---------
- une lumière de lisibilité suit Kenjiro ;
- elle devient plus cyan dans le monde éthérique ;
- lumière réactive près du pont spectral ;
- lumière réactive près de l'arène du boss.

LISIBILITÉ GAMEPLAY
-------------------
Des bandes très fines et légèrement émissives soulignent :
- le sol de départ ;
- la zone de combat ;
- le corridor ;
- les plateformes du tutoriel ;
- le pont spectral en cyan.

Les bandes sont décoratives et n'ont aucun collider.

POST-PROCESS
------------
Un Volume global est créé :
Assets/_Game/Art/PostProcess/Foundry_Atmosphere_v14.asset

Le builder tente de configurer dynamiquement les overrides URP :
- Bloom ;
- Color Adjustments ;
- Vignette ;
- Tonemapping ACES.

La configuration utilise la réflexion afin d'éviter une dépendance
C# directe à une version précise d'URP.

NOUVEAUX SCRIPTS
----------------
Assets/_Game/Scripts/Environment/
- FoundryGameplayFocusLight.cs
- FoundryWorldReactiveLight.cs
- FoundryWorldReactiveParticles.cs

NOUVEAUX BUILDERS
-----------------
Assets/_Game/Editor/
- FoundryTruePaintedAtmosphereBuilder.cs
- FoundryTruePaintedAtmosphereUpgradeBuilder.cs

HIÉRARCHIE AJOUTÉE
------------------
LEVEL
└── FOUNDRY_ART_V14
    ├── TruePainted_Far
    ├── TruePainted_Mid
    ├── TruePainted_Near
    ├── Atmosphere_VFX
    ├── GameplayFocusLight
    ├── Gameplay_Readability
    ├── GlobalPostProcess_v14
    └── FoundryTruePainted_v14

MINI_BOSS_ARENA
└── BossRift_Painted_v14

MISE À NIVEAU
-------------
À la première compilation, FoundryTruePaintedAtmosphereUpgradeBuilder
détecte une scène plus ancienne et reconstruit Foundry_Prototype.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

TEST VISUEL CONSEILLÉ
---------------------
1. Lancer la scène en monde normal.
2. Traverser le niveau pour observer le fond peint et les braises.
3. Activer K : le ciel éthérique, les motes et les lumières cyan apparaissent.
4. Vérifier que les plateformes restent plus lisibles que le décor.
5. Affronter le boss avec K actif pour observer le rift peint.

PROCHAINE ÉTAPE
---------------
Après cette v14, la meilleure suite est une passe d'optimisation et de
production finale :
- profiler URP / particles ;
- régler les intensités à partir de captures réelles de gameplay ;
- remplacer les placeholders PNG principaux par les illustrations
  définitives de production ;
- préparer une build jouable de démonstration.
