KATSUHIRO PROTOTYPE v15 — PRODUCTION / OPTIMIZATION / DEMO BUILD PASS
======================================================================

Cette version est cumulative et construite à partir du dernier Assets.zip fourni.
Elle conserve les versions précédentes jusqu'à la v14.

OBJECTIF
--------
Transformer la vertical slice en une première démo réellement présentable :
- écran titre ;
- continuer / nouvelle partie ;
- pause ;
- checkpoint persistant ;
- quality presets ;
- budget particules ;
- optimisation de la scène ;
- HUD performances ;
- Build Settings prêts ;
- commande Editor de build.

ÉCRAN TITRE
-----------
Nouvelle scène :
Assets/_Game/Scenes/Demo/TitleScreen.unity

Le builder la crée automatiquement.

Menu :
- Continuer (si checkpoint sauvegardé)
- Nouvelle partie
- Qualité
- Galerie des personnages
- Quitter

La galerie utilise les références visuelles fournies pour :
- Kenjiro
- Yuki
- Takeda

CHECKPOINT PERSISTANT
---------------------
Le checkpoint de la fonderie est maintenant sauvegardé avec PlayerPrefs.

Le système conserve :
- la scène ;
- X / Y / Z du point de réapparition.

En choisissant Continuer :
- Kenjiro réapparaît au checkpoint ;
- la vertical slice restaure l'étape Chase ;
- le checkpoint est déjà allumé.

NOUVELLE PARTIE
---------------
Efface uniquement le checkpoint de démonstration avant de charger la fonderie.

PAUSE
-----
Échap ou Start manette.

Menu :
- Reprendre
- Recommencer au checkpoint
- Qualité
- Retour au titre
- Quitter

QUALITY PRESETS
---------------
Performance
- particules ≈ 45 %
- shadows distance réduite
- anti-aliasing réduit

Balanced
- particules ≈ 72 %
- réglage recommandé pour la démo

Cinematic
- particules 100 %
- shadows distance plus grande
- anti-aliasing supérieur

PERFORMANCES
------------
F3 affiche :
- FPS
- quality preset
- nombre de particules actives

OPTIMISATION EDITOR
-------------------
KatsuhiroProductionOptimizationBuilder :
- désactive les shadows des arrière-plans ;
- marque les décorations réellement statiques pour le batching ;
- évite de marquer les gears, banners, particles et parallax comme statiques ;
- désactive les shadows des petites Point / Spot lights ;
- active HDR et Dynamic Resolution sur la caméra ;
- ajoute DemoParticleBudgetTag à chaque ParticleSystem.

BUILD SETTINGS
--------------
Le setup v15 place :
0. TitleScreen
1. Foundry_Prototype

MENU EDITOR
-----------
Tools > Katsuhiro > Prepare v15 Demo Build

Prépare :
- la scène Foundry_Prototype v15 ;
- la scène TitleScreen ;
- les Build Settings.

Tools > Katsuhiro > Build Demo - Current Platform

Tente de générer automatiquement une build avec la plateforme Unity active.
Le module de build correspondant doit être installé dans Unity Hub.

IMPORTANT
---------
L'archive est préparée statiquement ici, mais la compilation finale C#,
le rendu URP et la génération de l'exécutable doivent être validés dans
votre installation Unity.

NOUVEAUX SCRIPTS
----------------
Assets/_Game/Scripts/Demo/
- DemoCheckpointPersistence.cs
- DemoParticleBudgetTag.cs
- DemoQualityManager.cs
- DemoTitleScreenController.cs
- DemoPauseMenu.cs
- DemoPerformanceHUD.cs

NOUVEAUX BUILDERS
-----------------
Assets/_Game/Editor/
- KatsuhiroProductionOptimizationBuilder.cs
- KatsuhiroDemoTitleSceneBuilder.cs
- KatsuhiroDemoBuildMenu.cs
- KatsuhiroProductionDemoUpgradeBuilder.cs

RÉFÉRENCES VISUELLES
--------------------
Assets/_Game/Art/Characters/ProductionReferences/v15/

Les 6 images fournies dans cette étape ont été conservées sous des noms
de production clairs. Trois d'entre elles sont référencées par le menu
titre / galerie.

PROCHAINE ÉTAPE
---------------
Tester la démo dans Unity sur la machine cible, puis faire une v16 de
corrections basée sur :
- erreurs Console éventuelles ;
- FPS réels ;
- captures Game View ;
- sensations de combat ;
- lisibilité du boss ;
- temps de chargement.
