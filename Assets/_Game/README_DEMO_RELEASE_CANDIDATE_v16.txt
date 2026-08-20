KATSUHIRO PROTOTYPE v16 RC1 — QA + KENJIRO CHARACTER INTEGRATION
=================================================================

Base cumulative :
- v15 Production / Optimization / Demo Build Pass
- v14 True Painted Assets + Atmosphere
- étapes gameplay, combat et vertical slice précédentes conservées

OBJECTIF
--------
La v16 n'ajoute pas un nouveau niveau. Elle prépare une Demo Release Candidate :
1. stabilisation structurelle ;
2. polish gameplay et caméra ;
3. amélioration de la silhouette de Kenjiro ;
4. chargements plus propres ;
5. validation QA automatisable dans l'éditeur ;
6. boucle complète menu -> jeu -> boss -> fin -> titre.

KENJIRO — CHARACTER INTEGRATION v16
-----------------------------------
Le prefab existant est conservé et enrichi sans casser les chemins utilisés
par l'Animator.

Ajouts de proxy de production :
- manteau long et pans de coat ;
- revers / col / gilet ;
- mains plus lisibles ;
- cheveux à silhouette plus forte ;
- sacoche enrichie + sangle + boucles ;
- montre à gousset ;
- boussole d'éther ;
- bottes avec semelles / boucles ;
- Kikai-Yūrei plus détaillé :
  lentille, anneau, tubes de cuivre, coil et cœur éthérique.

Le marqueur est :
ModelRoot/KenjiroCharacter_v16

IMPORTANT :
Il s'agit encore d'un proxy 3D stylisé construit avec des primitives Unity.
Il est beaucoup plus proche de la fiche de référence, mais ce n'est pas
encore un mesh organique final sculpté / riggé dans Blender ou Maya.

GAMEPLAY POLISH
---------------
Le builder v16 ajuste légèrement :
- fenêtres du combo ;
- rythme des attaques légères ;
- lourde ;
- esquive et cooldown ;
- coût de la spéciale Kikai-Yūrei ;
- hit-stop ;
- caméra normale et caméra boss ;
- timings Doryoku-3 ;
- PV et rythme du mini-boss.

Le but est de rendre la démo plus lisible et moins punitive sans changer
le système de combat.

LOADING / FLOW
--------------
Nouveau DemoSceneLoader :
- chargement asynchrone ;
- écran sombre ;
- barre de progression ;
- version affichée.

Il est utilisé pour :
- TitleScreen -> Foundry_Prototype ;
- Pause -> restart ;
- Pause -> TitleScreen ;
- Fin de vertical slice -> TitleScreen.

FIN DE DÉMO
-----------
La carte finale affiche maintenant :
Prototype v16 RC1 — Demo Candidate

Un bouton "Retour au titre" termine proprement la boucle de démonstration.

QA UNITY
--------
Menu :
Tools > Katsuhiro > Run v16 QA Validation

Le validateur vérifie notamment :
- présence de TitleScreen ;
- présence de Foundry_Prototype ;
- prefab Kenjiro ;
- Input Actions ;
- Animator Controller ;
- ordre des Build Settings ;
- marqueurs des passes principales ;
- présence du Kikai-Yūrei ;
- présence du marqueur KenjiroCharacter_v16.

Rapport :
Assets/_Game/QA/v16_QA_REPORT.txt

PREPARATION BUILD
-----------------
Tools > Katsuhiro > Prepare v16 Demo Release Candidate

Un alias historique `Prepare v15 Demo Build` est également conservé pour compatibilité.
Les deux commandes préparent désormais la v16 RC1 et exécutent le QA à la fin.

Puis :
Tools > Katsuhiro > Build Demo - Current Platform

RÉFÉRENCES v16
---------------
Assets/_Game/Art/Characters/ProductionReferences/v16/

Les 6 références les plus récentes fournies sont incluses :
- portraits / concepts Kenjiro, Yuki, Takeda ;
- model sheets Kenjiro, Yuki, Takeda.

CE QUI RESTE À VALIDER DANS UNITY
---------------------------------
Cet environnement ne peut pas effectuer la compilation Unity finale.
Après import :
1. attendre la compilation ;
2. corriger toute erreur Console rouge ;
3. lancer Tools > Katsuhiro > Create or Rebuild Foundry Prototype ;
4. lancer Tools > Katsuhiro > Prepare v15 Demo Build ;
5. lancer Tools > Katsuhiro > Run v16 QA Validation ;
6. jouer la démo entièrement ;
7. tester Performance / Balanced / Cinematic ;
8. produire une build hors éditeur.

CRITÈRE DE RELEASE CANDIDATE
----------------------------
La v16 est considérée candidate lorsqu'un joueur peut :
TitleScreen
-> Nouvelle partie / Continuer
-> tutoriel
-> Kikai-Yūrei
-> combat
-> checkpoint
-> poursuite
-> mini-boss
-> épilogue
-> Retour au titre

sans erreur bloquante ni sauvegarde incohérente.
