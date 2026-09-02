# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.1 — Gabarit Annexe A du GDD (2026-09-03)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 14 |
| Acte | IV — Kyoto, Mémoire des objets |
| Nom | Le Théâtre des cent visages |
| Propriétaire | À assigner |
| Version | 0.1 (pré-production, aucun graybox construit) |
| Durée cible | 9 minutes |

## Promesse

Transformer silhouettes et changements de plan en un puzzle rythmique dans un théâtre hanté par cent visages de masques oubliés, jusqu'à ce qu'une représentation s'emballe et qu'Aiko traverse quatre scènes sans la moindre coupure visible.

## Mécanique signature

**Verbe** : ce niveau occupe la position **combinaison** de la courbe de difficulté de l'Acte IV (règle 8.3 : introduction → **combinaison** → renversement → examen par le boss) — l'Accord spectral, accordé à la toute fin du Niveau 13, est ici pour la première fois combiné systématiquement avec la lecture d'ombres projetées, dans un espace pensé comme un puzzle rythmique plutôt qu'une simple traversée.

**Règle** : les ombres projetées ne correspondent pas toujours à ce qui les projette — une plateforme peut n'exister que dans l'ombre qu'elle projette sur l'autre plan, ou inversement. Les trappes s'ouvrent sur un rythme scénique plutôt qu'un minuteur mécanique (écho du motif à quatre temps du Niveau 03, mais théâtral plutôt qu'industriel). Lire l'ombre devient une compétence à part entière, combinée à la bascule de plan déjà acquise.

**Enseignement** : chaque élément (décor de scène avec trappe, puis lecture d'ombre projetée) est présenté seul, avant de les combiner (0:00–3:00).

**Deux transformations** (contrat 8.2 du GDD) :
1. **3:00–4:30** — l'Accord spectral doit désormais composer avec une trappe dont le timing suit le rythme scénique, pas seulement un motif fixe.
2. **4:30–6:00** — la lecture d'ombre projetée devient obligatoire pour repérer les vraies plateformes, combinée à la bascule de plan, pendant que Masques éveillés puis Rokurokubi stylisé sont introduits séparément — les deux familles déjà rencontrées au Niveau 07, ici sur leur propre terrain d'origine thématique.

**Examen** : la traversée des quatre scènes sans coupure (7:00–8:30) exige d'enchaîner lecture d'ombre, bascule de plan et trappe rythmée en une seule séquence continue, sans jamais rompre l'illusion d'un seul mouvement ininterrompu — un défi autant technique (streaming, règle 15.5) que de gameplay.

## Plan

Gabarit du niveau (règle 8.1, mis à l'échelle sur 9:00 comme pour les fiches des Niveaux 01, 05, 09 et 13) :

| Temps cible | Phase (GDD 8.1) | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–1:00 | Accroche | Entrée dans le théâtre, décor de scène simple, aucun danger | Course, saut, rappel de l'Accord spectral |
| 1:00–3:00 | Enseignement | Décor de scène avec trappe, puis lecture d'ombre projetée, chacun isolé | Timing scénique, lecture d'ombre |
| 3:00–4:30 | Développement (1/2) | Accord spectral combiné au rythme des trappes, sans ennemi | Bascule de plan synchronisée |
| 4:30–6:00 | Développement (2/2) | Lecture d'ombre obligatoire ; Masques éveillés puis Rokurokubi stylisé introduits | Combinaison complète, lecture d'ennemi déjà connu |
| 6:00–7:00 | Climax (montée) | La représentation commence à s'emballer, les scènes se succèdent plus vite | Anticipation d'un rythme qui s'accélère |
| 7:00–8:30 | Climax (quatre scènes, set piece) | Traversée continue de quatre scènes sans coupure visible | Enchaînement complet sans interruption de séquence |
| 8:30–9:00 | Résolution | La représentation s'achève, le théâtre retrouve son calme | Aucune contrainte — respiration et sortie |

**Checkpoints (3, règle 5.6)** :
- **CP1 — ≈3:00**, à la transition Enseignement→Développement.
- **CP2 — ≈6:00**, juste avant que la représentation ne s'emballe.
- **CP3 — ≈7:00**, immédiatement avant le set piece des quatre scènes.

**Set piece** : la traversée des quatre scènes sans coupure, 7:00–8:30 (90 s, à la borne haute du budget de la règle 8.2, aucune décision de mouvement libre pendant plus de 5 s sans rendre le contrôle — règle 8.5). Note technique : « sans coupure visible » implique un chargement/streaming continu des quatre décors, à traiter comme une contrainte de production autant que de design.

**Secrets — trois sceaux d'harmonie (règle 8.2)** :
- **Sceau de lecture** — pendant l'Enseignement ; récompense le joueur qui repère une plateforme qui n'existe que dans une ombre projetée.
- **Sceau de maîtrise** — dans la section de combinaison (4:30–6:00) ; exige une chaîne trappe+ombre+bascule de plan précise, calée sur le rythme scénique.
- **Sceau de risque** — près d'une trappe dont le rythme s'accélère déjà ; accessible en frôlant sa fenêtre de fermeture, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — une courte route narrative pendant l'Accroche ou l'Enseignement (règle 8.2) : un fragment lié à un acteur ou un marionnettiste disparu, ou à l'un des cent masques qui garde le souvenir d'un rôle jamais rejoué, cohérent avec le nom du niveau et le ton "esprits ambivalents".

**Route experte** — non mise en avant dans la note de conception officielle, mais requise par le contrat de contenu (règle 8.2). Conçue ici comme une passerelle de cintres au-dessus des scènes, visible dès l'Accroche, qui exige de lire les ombres projetées depuis un angle inhabituel. Jamais obligatoire ; doit rester terminable sans interruption forcée.

## Opposition

Maximum deux familles actives simultanément sur le chemin principal (règle 10.1) — respecté : Masques éveillés et Rokurokubi stylisé ne se chevauchent que pendant le climax, jamais avant.

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Masques éveillés** | Change le comportement d'un objet (bestiaire 10.2), déjà rencontré au Niveau 07, ici sur son propre terrain d'origine thématique (théâtre nô/kabuki) | Briser le lien, pas l'objet — réponse canonique déjà connue. |
| **Rokurokubi stylisé** | Déjà rencontré au Niveau 07 comme opposition hors bestiaire | Lire l'extension du cou avant qu'elle n'atteigne sa portée maximale — même réponse qu'au Niveau 07, désormais familière au joueur. |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1).

## Art

- **Palette Acte IV** (règle 11.4) : noir d'encre, érable vermillon, bronze ancien — partagée avec le Niveau 13, ici enrichie par les teintes chaudes des costumes et masques de scène.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = décors de scène et trappes interactifs, cyan pâle = Masques éveillés/Rokurokubi (spirituel), rouge vermillon = trappes sur le point de se refermer (danger immédiat).
- **Consultation culturelle nécessaire, note de conception officielle explicite pour ce niveau** : les références au théâtre nô et kabuki demandent une relecture dédiée (règle 11.6 : documenter source et période, consultant japonais rémunéré, distinguer folklore/religion vécue/invention du jeu) — ce niveau est explicitement signalé par le GDD lui-même comme le plus sensible culturellement de l'acte.
- **Couches de profondeur** (règle 11.3, table complète) :
  - L0 — rideaux et éléments de scène proches en avant-plan, jamais d'information critique.
  - L1 — trappes, décors de scène, personnages et VFX de gameplay, contraste maximal.
  - L2 — machinerie de cintres non jouable, parallaxe modérée.
  - L3 — gradins et arrière-scène en profondeur, valeurs simplifiées.
  - L4 — obscurité de la salle, porteuse de la transition vers le climax (éclairage qui s'affole).
- **Hero shot candidat** (règle 8.2) : Aiko en pleine bascule de plan entre deux décors de scène, ombre projetée qui ne correspond pas à sa silhouette réelle, masques suspendus en arrière-plan — bon candidat pour illustrer le puzzle rythmique du niveau.

## Audio

- Ambiance théâtrale (bois de scène, tissu, silence habité) — ponctuée par la musique diégétique de la représentation elle-même plutôt que par un simple habillage.
- Grincement rythmé des trappes, calé sur le motif scénique (règle 14.2).
- Masques éveillés et Rokurokubi stylisé avec les mêmes sons distinctifs qu'au Niveau 07, renforçant la reconnaissance.
- Accélération progressive de la musique de scène à partir de 6:00, signal principal de l'emballement de la représentation.
- Motif musical de l'Acte IV (règle 14.1), ici entrelacé avec la musique diégétique du théâtre plutôt que joué séparément.
- La Pression (règle 5.4) sollicitée par l'Accord spectral en rythme avec les trappes — une gestion de ressource plus musicale que dans les niveaux précédents de l'acte.

## Technique

- **Caméra** : doit suivre la traversée des quatre scènes sans jamais révéler de coupure technique (règle 15.5 : rendu continu) — contrainte de cadrage la plus stricte de la campagne jusqu'ici.
- **VFX (Niagara)** : éclairage de scène dynamique, ombres projetées qui ne correspondent pas toujours à leur source, poussière de rideaux. Limiter à trois informations simultanées par effet de gameplay (règle 12.3).
- **Streaming** : le set piece des quatre scènes sans coupure est le défi de streaming le plus exigeant de la campagne documentée jusqu'ici — à traiter comme un risque de production à part entière, pas seulement un défi de gameplay.
- **Dépendances nouvelles à construire (aucune n'existe encore en code/contenu)** :
  - Ombre projetée dissociée de sa source (mécanique de lecture authentiquement nouvelle).
  - Trappe à rythme scénique (probable extension de la presse à motif musical du Niveau 03, adaptée à un contexte théâtral).
  - Système de streaming continu entre quatre décors sans coupure visible.
  - L'Accord spectral suppose que le Niveau 13 (ou son équivalent en code) l'a déjà construit ; ce niveau est le premier à le combiner avec un système de lecture visuelle indirecte (l'ombre).
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique — risque de performance élevé pendant le set piece à cause du streaming continu (règle 15.6/18.3).
- **Note d'ordre de production** : aucun graybox construit à ce jour. Le streaming sans coupure du set piece est la dépendance technique la plus risquée de ce niveau, indépendamment du gameplay lui-même.

## Accessibilité

- **Lecture d'ombre** : doit fonctionner sans perception fine des contrastes — un contour distinct (table 13.3) pour les plateformes révélées par l'ombre, pas seulement une variation de luminosité.
- **Trappes rythmées** : réduction d'élan optionnelle (profil "Vitesse 85/70 %", table 13.3) pour les timings les plus serrés.
- **Masques éveillés / Rokurokubi stylisé** : réponses déjà connues du joueur depuis le Niveau 07, cohérent avec une accessibilité progressive.
- **Route experte** : reste un contenu de maîtrise optionnel, jamais requis pour terminer le niveau (principe 13.3).

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), mêmes neuf critères que les fiches précédentes :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane entre 8 et 12 min (cible spécifique : 9 min) ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le chemin principal sans aide directe, y compris la lecture d'ombre |
| Échec | Aucun obstacle non-boss ne concentre plus de 20 % des abandons |
| Rythme | Aucune attente forcée > 5 s ; aucune zone sans décision de mouvement > 7 s |
| Caméra | Zéro mort attribuable à un danger non visible ; zéro coupure technique perceptible pendant le set piece |
| Performance | 60 fps cible, absence de hitch sur la ligne critique, y compris pendant le streaming continu |
| Contenu | 3 sceaux, 1 mémoire, route experte, set piece et traversée des quatre scènes tous fonctionnels |
| Route experte | Terminable sans interruption forcée |
| Checkpoints | Les 3 checkpoints fonctionnent (reprise < 2 s, dégâts non subis en double) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. Construire uniquement le chemin principal, en cubes.
3. Chronométrer le niveau sans ennemis.
4. Ajouter les obstacles environnementaux (trappes, ombres projetées).
5. Ajouter Masques éveillés et Rokurokubi stylisé en graybox.
6. Placer les 3 checkpoints et construire la route experte.
7. Tester avec cinq joueurs.
8. Corriger rythme et caméra.
9. Habillage artistique — seulement à ce stade, avec consultation culturelle dédiée pour les références nô/kabuki (note de conception officielle).

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0`.
- **Consultation culturelle nô/kabuki explicitement requise par le GDD lui-même** — à planifier en amont de l'étape 9, pas en fin de production, étant donné la sensibilité du sujet.
- **Le streaming sans coupure du set piece mérite une évaluation technique dédiée** avant tout engagement de contenu sur ce niveau, indépendamment de sa mécanique de gameplay.
- Ce niveau réutilise intégralement les deux familles d'opposition du Niveau 07 (Masques éveillés, Rokurokubi stylisé) sur leur propre terrain thématique — un choix de cohérence narrative plutôt qu'une nécessité de conception, à confirmer si le GDD préfère introduire de la variété d'opposition à ce stade de la campagne.
