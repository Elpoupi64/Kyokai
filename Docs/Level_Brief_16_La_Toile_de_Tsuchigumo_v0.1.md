# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.1 — Gabarit Annexe A du GDD (2026-09-03)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 16 |
| Acte | IV — Kyoto, Mémoire des objets |
| Nom | La Toile de Tsuchigumo |
| Propriétaire | À assigner |
| Version | 0.1 (pré-production, aucun graybox construit) |
| Durée cible | 11 minutes (fourchette GDD 10-12 min — niveau de boss, règle 9) |

## Promesse

Examiner toutes les bascules de plan apprises depuis la fonderie de Fushimi, à travers fils-portails, cocons de machines et un plafond devenu praticable, jusqu'à couper les attaches du Kokoro qui retiennent Tsuchigumo — et libérer les objets qu'il a capturés, pas seulement le vaincre.

## Mécanique signature

**Verbe** : ce niveau clôt la courbe de difficulté de l'Acte IV (règle 8.3 : introduction → combinaison → renversement → **examen par le boss**) — il examine l'Accord spectral seul (table des boss, règle 10.4 : Tsuchigumo, examen « Accord spectral »), mais de façon exhaustive plutôt que ponctuelle : la note de conception officielle dit explicitement « examiner toutes les bascules de plan », pas une seule technique répétée.

**Règle** : les fils-portails relient les deux plans de façon inhabituelle (traverser un fil peut faire basculer le plan sans action volontaire du joueur) ; les cocons de machines contiennent des objets captifs à libérer un par un ; le plafond jouable inverse la notion même de « sol » à mesure que la toile se déploie. Chaque élément a été rencontré séparément dans un niveau antérieur de l'acte (bascule de plan au Niveau 13, lecture indirecte au Niveau 14, topologie double au Niveau 15) — ce niveau les synthétise tous.

**Structure du niveau de boss** (règle 9 — Approche / Combat en 3 phases / Résolution, comme les Niveaux 04, 08 et 12) :
1. **Approche** — valider la maîtrise complète de l'Accord spectral contre Araignées de suie et Onibi, sur un terrain déjà tissé de fils-portails.
2. **Combat** — Tsuchigumo en trois phases (couper les attaches du Kokoro, inverser la toile, libérer les objets captifs), chacune ajoutant une règle plutôt qu'une simple augmentation de vitesse (règle 10.5).
3. **Résolution** — contrairement aux trois bosses précédents (calmé, devenu allié, apaisé par la lumière), ce niveau ajoute un geste supplémentaire : les objets captifs, d'autres esprits que Tsuchigumo avait cocoonnés, sont explicitement libérés — un acte de sauvetage, pas seulement d'apaisement.

## Plan

| Temps cible | Macro-partie | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–4:00 | Approche | Terrain tissé de fils-portails, Araignées de suie et Onibi en éclaireurs de la toile | Accord spectral complet, lecture d'ennemi, opposition ordinaire |
| 4:00–6:00 | Combat — Phase 1 : Couper les attaches du Kokoro | Premiers fils qui relient Tsuchigumo au Kokoro, à couper par bascule de plan précise | Bascule de plan sous contrainte ; première attache coupée |
| 6:00–8:00 | Combat — Phase 2 : Inverser la toile | Le plafond devient sol et le sol devient plafond ; orientation à réapprendre en continu | Nouvelle règle de la phase : navigation en environnement qui s'inverse ; deuxième attache coupée |
| 8:00–9:30 | Combat — Phase 3 : Libérer les objets captifs | Synthèse : bascule de plan, inversion et lecture de fils-portails combinées pour atteindre chaque cocon | Toutes les compétences de l'acte examinées ensemble ; les captifs libérés |
| 9:30–11:00 | Résolution | La toile se dissout en fils de soie, les objets libérés retrouvent leur place, Tsuchigumo se calme | Aucun — respiration, transition narrative vers l'Acte V |

**Checkpoint (1, règles 5.6 ET 10.5 — même interprétation que les Niveaux 04, 08 et 12)** : un seul checkpoint réel, à 4:00 (fin de l'Approche, juste avant le Combat), avec reprise de la phase en cours en mode Assistance (règle 10.5).

**Rythme des dégâts** (règle 10.5) : chaque phase du Combat doit offrir une fenêtre d'action exploitable (couper une attache, franchir une inversion, atteindre un cocon) toutes les 15 à 25 secondes pour un joueur qui a compris le motif.

**Set piece** : comme aux Niveaux 04, 08 et 12, le Combat entier (4:00–9:30, 5:30) fait office de set piece du niveau.

**Secrets — trois sceaux d'harmonie (règle 8.2), placés pendant l'Approche** :
- **Sceau de lecture** — visible depuis le chemin principal, sur un fil-portail en retrait ; récompense le joueur qui repère une bascule de plan cachée avant même le Combat.
- **Sceau de maîtrise** — exige une chaîne de bascules à travers plusieurs fils-portails sans jamais toucher le sol d'un plan « faux », une répétition à échelle réduite du Combat.
- **Sceau de risque** — près d'un groupe d'Araignées de suie ; accessible en frôlant leur toile, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — une courte route narrative pendant l'Approche (règle 8.2) : un fragment lié à l'un des objets déjà captifs dans un cocon, dont la mémoire reste accessible avant même sa libération complète pendant le Combat — cohérent avec le nom de l'acte (« Kyoto, mémoire des objets »).

**Route experte** — non mise en avant dans la note de conception officielle, mais requise par le contrat de contenu (règle 8.2). Conçue ici comme un chemin de fils-portails secondaires, visible dès l'Approche, offrant une lecture anticipée de l'inversion de la toile avant qu'elle ne devienne critique en Phase 2 — un avantage de lecture, comme aux Niveaux 04, 08 et 12.

## Opposition

**Pré-boss (Approche uniquement, règle 10.1 : deux familles maximum simultanément)** :

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Araignées de suie** | Non répertorié dans le bestiaire ordinaire (règle 10.2) — traité ici comme éclaireurs de Tsuchigumo, cohérent avec son thème arachnéen ; tissent de petites toiles locales qui gênent la lecture des fils-portails | Traverser rapidement ou basculer de plan pour éviter leurs toiles, plutôt que les affronter directement. |
| **Onibi** | Projectile lent à trajectoire lisible (bestiaire 10.2), rencontré pour la première fois au Niveau 01 ; réponse canonique complète disponible depuis le Niveau 12 | Déviation par ombrelle — dernière apparition d'Onibi dans les quatre premiers actes documentés, mais pas la première utilisation de sa réponse complète (voir Niveau 12). |

**Boss (Combat, règle 10.4)** :

| Boss | Menace | Objectif | Examen |
| --- | --- | --- | --- |
| **Tsuchigumo** | Toiles entre plans | Libérer les cocons-machines | Accord spectral |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1) ; Tsuchigumo seul occupe l'écran pendant le Combat, sans add d'opposition ordinaire.

## Art

- **Palette Acte IV** (règle 11.4) : noir d'encre, érable vermillon, bronze ancien — dernière apparition avant la transition vers la palette cendre/or de l'Acte V.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = cocons-machines et attaches du Kokoro (mécaniques), cyan pâle = Araignées de suie et Tsuchigumo lui-même (spirituel), rouge vermillon = fils-portails sur le point de basculer sans prévenir (danger immédiat).
- **Éviter l'imagerie gore, note de conception officielle explicite pour ce niveau** : « privilégier papier, fil de soie et ombres » — directive artistique stricte, à appliquer à Tsuchigumo lui-même (silhouette évoquant l'origami et le fil plutôt que l'arachnide littérale), aux Araignées de suie, et à toute représentation de la toile.
- **Couches de profondeur** (règle 11.3, table complète) :
  - L0 — fils de soie flottants en avant-plan, jamais d'information critique.
  - L1 — fils-portails, cocons, plafond jouable, Tsuchigumo lui-même, contraste maximal.
  - L2 — structure de la toile non jouable, synchronisée aux bascules de phase.
  - L3 — vestiges de Kyoto visibles à travers la toile, valeurs simplifiées.
  - L4 — obscurité tissée, silhouette de Tsuchigumo avant chaque phase, porteuse du climax visuel.
- **Hero shot candidat** (règle 8.2) : Aiko suspendue tête en bas sur le plafond jouable, fils de soie qui se dénouent autour d'un cocon libéré, silhouette de Tsuchigumo en papier et ombre plutôt qu'en chair — bon candidat qui illustre directement la directive « papier, fil de soie et ombres ».

## Audio

- Bruissement continu de la toile, base sonore du Combat, distincte du grondement des trois boss précédents.
- Chaque bascule involontaire de fil-portail annoncée par un signal sonore distinct au moins 400 ms avant l'effet (règle 10.5).
- Craquement feutré à chaque attache du Kokoro qui cède, distinct des ancrages métalliques de Namazu ou des anneaux du Daitengu.
- Araignées de suie avec un cliquetis léger et collectif ; Onibi avec le même son établi depuis le Niveau 01.
- Motif musical de l'Acte IV (règle 14.1), à sa forme la plus dense ici, dernière fois qu'il joue avant la transition vers l'Acte V.
- Résolution : un son de dénouement progressif (fils qui se détendent un à un) pendant que les objets captifs retrouvent leur mémoire — cohérent avec « papier, fil de soie et ombres » plutôt qu'un effet sonore de victoire classique.

## Technique

- **Caméra** : défi le plus complexe de la campagne jusqu'ici — doit rester lisible pendant l'inversion complète sol/plafond de la Phase 2, sans jamais désorienter le joueur au point de perdre le lien avec ses commandes (règle 8.4/18.3, même principe que la préservation du contrôle au Niveau 11, appliqué ici à un boss entier).
- **VFX (Niagara)** : fils de soie, dénouement des cocons, transition d'inversion sol/plafond, halo de libération à la Résolution. Limiter à trois informations simultanées par effet de gameplay (règle 12.3).
- **Streaming** : plafond jouable et inversion complète de la géométrie — contrainte technique proche du retournement du Niveau 11, mais appliquée à un espace de boss entier plutôt qu'à 45 secondes de set piece.
- **Dépendances nouvelles à construire (aucune n'existe encore en code/contenu)** :
  - Fil-portail à bascule de plan involontaire (nouvelle interaction avec l'Accord spectral, distincte de la bascule volontaire des Niveaux 13-15).
  - Plafond jouable avec inversion complète de l'orientation (mécanique authentiquement nouvelle, la plus ambitieuse techniquement de la campagne documentée jusqu'ici).
  - Cocon-machine libérable (système de captifs à délivrer, lié à la mémoire narrative du niveau).
  - Tsuchigumo lui-même — quatrième boss du jeu, réutilise le patron technique établi (checkpoint-avant-boss, reprise de phase en Assistance, règle 10.5) mais avec une logique de résolution à trois temps (couper/inverser/libérer) plus complexe que les trois précédents.
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique, en particulier lors de l'inversion complète de la Phase 2 (règle 15.6/18.3).
- **Note d'ordre de production** : aucun graybox construit à ce jour. Le plafond jouable avec inversion complète est la dépendance technique la plus risquée de l'Acte IV entier — mérite un prototype dédié avant tout engagement sur ce niveau, plus encore que le retournement du Niveau 11 puisqu'il s'applique ici à un espace de combat complet, pas à 45 secondes de traversée linéaire.

## Accessibilité

- **Inversion sol/plafond (Phase 2)** : option de réduction de mouvement/parallaxe (table 13.3) particulièrement critique ici, au même titre que le retournement du Niveau 11.
- **Fils-portails à bascule involontaire** : télégraphie non uniquement colorimétrique — signal sonore à 400 ms (ci-dessus) et contour lisible (table 13.3) doivent suffire seuls à anticiper une bascule non voulue.
- **Tsuchigumo (Combat)** : reprise de la phase en cours en mode Assistance (règle 10.5), comme aux Niveaux 04, 08 et 12.
- **Route experte** : reste un contenu de maîtrise optionnel (avantage de lecture), jamais requis pour terminer le niveau (principe 13.3).

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), mêmes neuf critères que les fiches des Niveaux 04, 08 et 12 :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane entre 8 et 12 min (cible spécifique : 10-12 min, approche + combat + résolution incluses) ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le motif de chaque phase du Combat sans aide directe |
| Échec | Aucune phase du Combat ne concentre plus de 20 % des abandons du niveau |
| Rythme | Fenêtres d'action exploitables toutes les 15 à 25 s pour un joueur qui a compris (règle 10.5) |
| Caméra | Zéro mort attribuable à un danger non visible, y compris pendant l'inversion sol/plafond |
| Performance | 60 fps cible, absence de hitch sur la ligne critique, y compris à l'inversion de la Phase 2 |
| Contenu | 3 sceaux, 1 mémoire, route experte, les trois phases du Combat et la libération des captifs tous fonctionnels |
| Route experte | Terminable sans interruption forcée |
| Checkpoint | Le checkpoint unique avant le Combat fonctionne, y compris la reprise de phase en mode Assistance (règle 10.5) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. Construire uniquement le chemin principal, en cubes (Approche + terrain à fils-portails pour les trois phases).
3. Chronométrer le niveau sans opposition ni boss.
4. Ajouter les obstacles environnementaux (fils-portails, cocons-machines).
5. Ajouter Araignées de suie, Onibi et Tsuchigumo (les trois phases) en graybox.
6. Placer le checkpoint unique et construire la route experte.
7. Tester avec cinq joueurs.
8. Corriger rythme et caméra.
9. Habillage artistique — seulement à ce stade, avec attention particulière à la directive « éviter l'imagerie gore » (note de conception officielle).

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0` — l'Accord spectral est ici examiné dans toute son étendue, pas retouché.
- **« Araignées de suie » n'apparaît pas dans la table du bestiaire ordinaire (règle 10.2)** — traité ici comme opposition spécifique au niveau, cohérent avec le thème arachnéen de Tsuchigumo.
- **Le plafond jouable avec inversion complète est la dépendance technique la plus ambitieuse de la campagne documentée jusqu'ici** — mérite un prototype isolé avant tout engagement de contenu, avec une attention particulière au maintien du contrôle joueur (même principe que le retournement du Niveau 11, mais à une échelle de boss entier).
- **Ce niveau est le premier boss dont la Résolution ajoute un geste narratif au-delà de l'apaisement du boss lui-même** (libérer d'autres captifs, pas seulement calmer Tsuchigumo) — cohérent avec le nom de l'acte (« mémoire des objets ») et à considérer comme un gabarit possible pour le boss final de l'Acte V, qui implique une synthèse narrative encore plus large (séparer l'homme, la machine et l'esprit, selon la table des boss 10.4).
- **Correction de cohérence (passe transversale du 2026-09-03)** : ce niveau ne referme PAS la boucle d'Onibi (première apparition, réponse limitée au mouvement de base, au Niveau 01) — cette boucle s'est refermée quatre niveaux plus tôt, au Niveau 12, où la réponse canonique complète (déviation par ombrelle) est utilisée pour la première fois sans réserve. Ce niveau n'est que la dernière apparition de la famille dans les quatre premiers actes documentés, une réutilisation parmi d'autres (voir aussi Niveau 17, Acte V), pas un jalon en soi.
