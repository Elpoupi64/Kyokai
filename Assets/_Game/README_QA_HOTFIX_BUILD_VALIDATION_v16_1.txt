KATSUHIRO PROTOTYPE v16.1 RC2 — QA HOTFIX + BUILD VALIDATION
================================================================

Cette archive est cumulative et part du dernier Assets.zip fourni.
Les systèmes et contenus des versions précédentes restent présents.

BUT
---
La v16.1 ne rajoute pas de nouveau niveau.
Elle transforme la v16 RC1 en RC2 plus fiable avant le test réel dans Unity.

HOTFIX PRINCIPAL
----------------
Le faux warning Animator est corrigé.

Ancien chemin incorrect :
Assets/_Game/Animations/Kenjiro/Kenjiro_Prototype.controller

Chemin réel validé :
Assets/_Game/Animations/Kenjiro/KenjiroAnimatorController.controller

Le validateur contrôle maintenant les 14 states :
Idle, Run, Jump, Fall,
Attack1, Attack2, Attack3,
HeavyAttack, AirAttack,
Dodge, DodgeCounter,
KikaiSpecial, Hurt, Death.

QA PASS / WARNING / BLOCKER
---------------------------
Menu :
Tools > Katsuhiro > Run v16.1 QA Validation

Rapport :
Assets/_Game/QA/v16_1_QA_REPORT.txt

PASS
- élément correct et prêt.

WARNING
- problème non bloquant ou réglage à vérifier.

BLOCKER
- problème empêchant de considérer la build comme candidate.

Le validateur contrôle désormais :
- TitleScreen ;
- Foundry_Prototype ;
- prefab Kenjiro ;
- prefab Doryoku-3 ;
- Input Actions et bindings essentiels ;
- Animator Controller, states et paramètres ;
- scripts manquants dans les prefabs et scènes ;
- références sérialisées critiques de la vertical slice ;
- checkpoint ;
- boss encounter ;
- Build Settings ;
- plateforme active ;
- URP actif ;
- PC_RPAsset / Renderer ;
- Volume Profile et overrides post-process ;
- caméra et HDR ;
- quality presets Performance / Balanced / Cinematic ;
- références personnages ;
- marqueurs historiques v7/v8/v9/v14/v15/v16 ;
- marqueur v16.1 ;
- contrats structurels du flow :
  Title -> Game -> Checkpoint -> Boss -> End -> Title.

BUILD PREFLIGHT
---------------
Menu recommandé :
Tools > Katsuhiro > Prepare v16.1 Demo RC2

Cette commande :
1. reconstruit Foundry_Prototype ;
2. reconstruit TitleScreen ;
3. rétablit les Build Settings ;
4. exécute le QA ;
5. indique clairement si des blockers subsistent.

Build :
Tools > Katsuhiro > Build v16.1 RC2 - Current Platform

La build est maintenant ANNULÉE automatiquement si le QA détecte un BLOCKER.

Le nom de sortie est versionné :
- Katsuhiro_Demo_v16_1_RC2.exe
- Katsuhiro_Demo_v16_1_RC2.app
- Katsuhiro_Demo_v16_1_RC2.x86_64

RÉFÉRENCES VISUELLES
--------------------
Les six références personnages les plus récentes fournies dans cette étape
remplacent les images de ProductionReferences/v16 en conservant leurs .meta
et donc leurs GUID Unity existants.

Cela évite de casser les références du TitleScreen.

VERSION
-------
L'interface affiche maintenant :
Prototype v16.1 RC2 — QA Hotfix / Build Validation

VALIDATION RÉELLE ENCORE NÉCESSAIRE
-----------------------------------
Cette archive ne peut pas être compilée avec Unity dans cet environnement.

Après import :
1. attendre la fin de compilation ;
2. vérifier qu'il n'y a aucune erreur rouge Console ;
3. Tools > Katsuhiro > Prepare v16.1 Demo RC2 ;
4. lire Assets/_Game/QA/v16_1_QA_REPORT.txt ;
5. obtenir BLOCKER=0 ;
6. jouer le parcours complet ;
7. vérifier F3 / FPS ;
8. tester Performance, Balanced et Cinematic ;
9. Build v16.1 RC2 - Current Platform ;
10. tester l'exécutable hors Editor.

RC2 est prête pour un test externe uniquement après ces dix vérifications.
