# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.1 — Gabarit Annexe A du GDD (2026-09-03)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 04 |
| Acte | I — Edo sous pression |
| Nom | Le Pont de Namazu |
| Propriétaire | À assigner |
| Version | 0.1 (pré-production, aucun graybox construit) |
| Durée cible | 10 à 12 minutes (niveau de boss — règle 9 : « les niveaux de boss restent dans la même enveloppe de 8 à 12 minutes en intégrant l'approche, le combat et la résolution ») |

## Promesse

Valider tout ce qu'Edo a appris à Aiko — course, saut, rebond, glissade, Ruée vapeur — sur un pont qui se dérobe sous les secousses d'un poisson-chat géant, jusqu'à ce qu'elle comprenne que Namazu ne fait pas trembler la ville par malice mais parce qu'une chaudière est rivée à son dos.

## Mécanique signature

**Verbe** : ce niveau clôt la courbe de difficulté de l'Acte I (règle 8.3 : introduction → combinaison → renversement → **examen par le boss**) — il n'introduit aucun nouveau verbe de déplacement, il **examine** Ruée et rebond ensemble (table des boss, règle 10.4 : Namazu, examen « Ruée et rebond »), les deux verbes les plus sollicités depuis le Niveau 02.

**Règle** : le pont mobile réagit aux ondes sismiques de Namazu — certaines planches se dérobent en rythme, d'autres restent stables et servent d'appui pour un rebond. Lire l'onde avant qu'elle n'atteigne Aiko (règle 8.4 : aucun danger létal sans signal visuel ET sonore) devient la compétence centrale du niveau, testée d'abord sur des obstacles ordinaires (Approche) puis sur Namazu lui-même (Combat).

**Structure du niveau de boss** (différente du gabarit 8.1 en 5 phases utilisé pour les niveaux réguliers — la règle 9 sépare explicitement les niveaux de boss en trois macro-parties) :
1. **Approche** — valider les bases (Ruée, rebond, lecture d'onde) contre une opposition ordinaire, sur le pont encore globalement stable.
2. **Combat** — Namazu en trois phases (poursuite, lecture des ondes, accordage des piliers), chacune ajoutant une règle plutôt qu'une simple augmentation de vitesse (règle 10.5).
3. **Résolution** — la chaudière est retirée, pas le poisson-chat tué (note de conception officielle) ; conclusion courte, lisible, cohérente avec l'apaisement (règle 10.5).

## Plan

| Temps cible | Macro-partie | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–4:00 | Approche | Traversée du pont encore stable, Kappa sapeur (variante Suie, tempo plus rapide) et avatars de pierre réveillés par les premières secousses | Ruée, rebond, lecture d'onde sur opposition ordinaire |
| 4:00–6:00 | Combat — Phase 1 : Poursuite | Namazu plonge et refait surface le long du pont ; premières planches instables | Course/Ruée sous pression de temps ; premier ancrage de chaudière exposé |
| 6:00–8:00 | Combat — Phase 2 : Lecture des ondes | Les secousses dessinent un motif d'ondes lisible avant impact ; certaines planches ne sont sûres qu'entre deux ondes | Lecture pure, positionnement plutôt que réflexe ; deuxième ancrage exposé |
| 8:00–9:30 | Combat — Phase 3 : Accordage des piliers | Synthèse : lire l'onde ET enchaîner Ruée→rebond pour aligner les piliers porteurs du pont | Les deux verbes examinés ensemble ; troisième et dernier ancrage retiré |
| 9:30–11:00 | Résolution | La chaudière tombe, les secousses cessent, Namazu se calme et regagne les profondeurs ; le pont se stabilise | Aucun — respiration, sortie, score |

**Checkpoint (1, règles 5.6 ET 10.5 — les niveaux de boss diffèrent ici des niveaux réguliers)** : contrairement aux Niveaux 01, 02 et 03 (3 checkpoints chacun, avant chaque set piece majeur), la règle 10.5 demande explicitement un **checkpoint avant le boss, avec reprise de la phase en cours en mode Assistance** — un mécanisme différent d'un simple point de contrôle spatial. Ce niveau n'a donc qu'**un seul checkpoint réel, à 4:00** (fin de l'Approche, juste avant le Combat) ; hors mode Assistance, un échec pendant n'importe quelle phase du Combat renvoie à ce même checkpoint et reprend au début de la Phase 1 — cohérent avec la conception classique d'un combat de boss appris par répétition. En mode Assistance, la phase en cours est conservée plutôt que redémarrée entièrement.

**Rythme des dégâts** (règle 10.5) : chaque phase du Combat doit offrir une fenêtre de retrait d'ancrage exploitable toutes les 15 à 25 secondes pour un joueur qui a compris le motif — pas un unique gros bouton de fin de phase après une longue attente.

**Set piece** : contrairement aux niveaux réguliers (où le set piece est une tranche isolée de 45 à 90 secondes dans un climax plus large), le Combat entier (4:00–9:30, 5:30) fait office de set piece du niveau — un combat de boss en trois phases rythmées est par nature la séquence à contrôle imposé la plus longue de la campagne pour ce niveau, ce qui dépasse volontairement le budget de 45-90 s pensé pour les niveaux réguliers.

**Secrets — trois sceaux d'harmonie (règle 8.2 : un de lecture, un de maîtrise, un de risque), placés pendant l'Approche** (jamais pendant le Combat lui-même, qui doit rester un rythme imposé sans distraction — règle 8.5) :
- **Sceau de lecture** — visible depuis le chemin principal, sur une pile de pont annexe ; récompense le joueur qui repère un appui stable au milieu des planches déjà instables.
- **Sceau de maîtrise** — exige une chaîne Ruée→rebond précise entre deux avatars de pierre, sans les toucher, une répétition à l'échelle réduite de l'examen final de la Phase 3.
- **Sceau de risque** — près d'un groupe d'avatars de pierre agités ; accessible en frôlant leur zone de réveil, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — une courte route narrative pendant l'Approche (jamais derrière une exécution difficile, règle 8.2) : un fragment lié à un pêcheur ou un gardien du pont qui a toujours su que Namazu n'était pas hostile par nature, cohérent avec l'idée d'apaisement du climax.

**Route experte** — non mise en avant dans la note de conception officielle de ce niveau, mais requise par le contrat de contenu (règle 8.2). Conçue ici comme une corniche haute le long du pont, visible dès l'Approche, qui permet d'observer (et d'anticiper) le motif des ondes sismiques une fraction de seconde plus tôt que depuis le tablier principal — un avantage de lecture, pas une nouvelle mécanique. Jamais obligatoire, y compris pendant le Combat ; doit rester terminable sans interruption forcée.

## Opposition

**Pré-boss (Approche uniquement, règle 10.1 : deux familles maximum simultanément)** :

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Kappa sapeur (variante Suie)** | Jet d'eau et sabotage de plateforme, tempo plus rapide que sa première apparition au Niveau 01 (règle 10.3 : les variantes augmentent le tempo, jamais seulement les points de vie) | Parade du jet ou attaque par-dessus, désormais sur un rythme plus serré. |
| **Avatars de pierre** | Gardiens de pont réveillés par les premières secousses de Namazu — non répertoriés dans le bestiaire ordinaire (règle 10.2), traités ici comme des figures protectrices au sens de la règle 10.1 (« apaisées, libérées ou contournées, jamais du bétail à score ») | Utiliser le rythme des ondes pour les contourner ou les stabiliser plutôt que les affronter frontalement — annonce directement la lecture d'onde du Combat. |

**Boss (Combat, règle 10.4)** :

| Boss | Menace | Objectif | Examen |
| --- | --- | --- | --- |
| **Namazu** | Ondes sismiques | Retirer les trois ancrages de la chaudière rivée à son dos | Ruée et rebond |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1) ; Namazu seul occupe l'écran pendant le Combat, sans add d'opposition ordinaire pour ne pas diluer la lecture d'onde.

## Art

- **Palette Acte I** (règle 11.4) : pluie indigo, lanternes ambre, cuivre neuf — partagée avec les Niveaux 01, 02 et 03, dernière apparition de cette palette avant la transition vers l'Acte II.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = ancrages de chaudière et piliers interactifs (mécaniques), cyan pâle = avatars de pierre (spirituel), rouge vermillon = fronts d'onde sismique et planches sur le point de céder (danger immédiat).
- **Couches de profondeur** (règle 11.3, table complète du GDD) :
  - L0 (avant-plan décoratif) — embruns et éclats de bois, jamais d'information critique.
  - L1 (plan jouable) — tablier du pont, piliers, ancrages, Namazu lui-même, contraste maximal.
  - L2 (architecture proche) — structures du pont non jouables, synchronisées visuellement aux ondes.
  - L3 (paysage) — rivière et berges d'Edo la nuit, valeurs simplifiées.
  - L4 (ciel et phénomène) — éclaboussures massives de Namazu, silhouette du poisson-chat en arrière-plan avant chaque phase, porteur du climax visuel.
- **Hero shot candidat** (règle 8.2, composition emblématique) : Aiko en rebond au-dessus d'une planche qui cède, silhouette de Namazu émergeant de la rivière en contre-jour, dernier ancrage de chaudière visible sur son dos — bon candidat de fin d'acte, montre l'échelle du boss sans le réduire à un simple ennemi.

## Audio

- Grondement sourd continu, croissant à chaque phase, distinct du motif musical de l'Acte I plutôt qu'un simple volume qui monte.
- Chaque onde sismique annoncée par un signal sonore distinct au moins 400 ms avant l'impact (règle 10.5 : toute attaque de boss annoncée au moins 400 ms avec signal sonore distinct — plus strict que la règle générale 8.4).
- Craquement du bois spécifique à chaque planche qui va céder, directionnel (règle 14.2), pour permettre une lecture même hors du centre de l'écran.
- Kappa sapeur (variante Suie) conserve le même son de jet que sa première apparition, mais sur un tempo audiblement plus rapide, pas un son différent — cohérent avec la règle 10.3 (variantes = tempo, jamais un nouveau vocabulaire sonore).
- Motif musical de l'Acte I (règle 14.1), présent depuis le Niveau 01, atteint ici sa forme la plus dense — dernière fois qu'il joue avant la transition de palette vers l'Acte II.
- La Pression (règle 5.4) est pleinement fonctionnelle depuis le Niveau 03 ; ici, la gérer entre les fenêtres de dégâts de 15 à 25 secondes (règle 10.5) devient une vraie décision de combat, pas seulement de traversée.

## Technique

- **Caméra** : élargissement significatif dès le Combat (4:00) pour que Namazu, ses ondes ET les planches instables restent visibles simultanément (règle 8.4/18.3) — le défi caméra le plus exigeant de l'Acte I, puisqu'un boss de cette échelle occupe une part importante du cadre sans jamais masquer le danger au sol.
- **VFX (Niagara)** : embruns, éclats de bois des planches qui cèdent, onde de choc sismique visible au sol, vapeur de la chaudière de Namazu qui s'échappe progressivement à mesure que les ancrages sont retirés (télégraphie visuelle de la progression du combat). Limiter à trois informations simultanées par effet de gameplay (règle 12.3).
- **Streaming** : le pont et la rivière forment un espace plus ouvert que les Niveaux 01-03 ; à revalider une fois la géométrie posée, notamment pour la corniche de la route experte qui longe tout le niveau.
- **Dépendances nouvelles à construire (aucune n'existe encore en code/contenu)** :
  - Pont mobile réactif aux ondes sismiques (planches qui se dérobent en rythme) — mécanique authentiquement nouvelle, plus proche conceptuellement du tapis roulant du Niveau 03 (vecteur externe) que de rien d'existant.
  - Comportement avatars de pierre (gardiens réveillés, réponse d'apaisement plutôt que de combat pur).
  - Namazu lui-même — premier boss du jeu, aucune classe de boss n'existe en code à ce jour ; trois phases avec logique de retrait d'ancrage, checkpoint-avant-boss et reprise de phase en mode Assistance (règle 10.5) sont toutes des systèmes neufs.
  - Système de checkpoint (`AFlowCheckpoint`/`ACheckpoint` — déjà construit pour le Niveau 02, réutilisable, mais le mode Assistance avec reprise de phase est une extension neuve).
  - Variante Suie de Kappa sapeur (probable multiplicateur de tempo sur le comportement déjà existant, pas une nouvelle classe).
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique, en particulier lors du surgissement de Namazu à chaque transition de phase (règle 15.6/18.3).
- **Note d'ordre de production** : comme les Niveaux 01 et 03, aucun graybox construit à ce jour (seul le Niveau 02 existe, comme vertical slice — règle 17.5). Ce niveau est en plus le tout premier boss de la campagne : sa classe de boss et ses règles de checkpoint spécifiques (10.5) n'ont aucun précédent dans le code existant, contrairement aux niveaux réguliers qui peuvent réutiliser le système de checkpoint standard tel quel.

## Accessibilité

- **Ondes sismiques** : télégraphie non uniquement colorimétrique — signal sonore à 400 ms (ci-dessus) et contour lisible (option "contours", table 13.3) doivent suffire seuls à anticiper une planche qui cède.
- **Avatars de pierre / Kappa sapeur (Suie)** : la "parade élargie" (table 13.3) et le ralenti de vitesse doivent permettre de lire le tempo plus rapide sans réflexe pur.
- **Namazu (Combat)** : reprise de la phase en cours en mode Assistance (règle 10.5) est elle-même une mesure d'accessibilité explicite du GDD, pas seulement une règle de boss — à documenter comme telle dans le profil de difficulté (table 13.3 : intégrité 3/5/infinie s'applique aussi ici).
- **Route experte** : reste un contenu de maîtrise optionnel (avantage de lecture, pas de puissance), jamais requis pour terminer le niveau (principe 13.3).

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), appliqués ici — mêmes neuf critères que les fiches des Niveaux 01, 02 et 03, avec deux seuils explicitement adaptés à un niveau de boss (Rythme, Checkpoints) :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane de première réussite entre 8 et 12 min (cible spécifique du niveau : 10-12 min, approche + combat + résolution incluses) ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le motif de chaque phase du Combat sans aide directe |
| Échec | Aucune phase du Combat ne concentre plus de 20 % des abandons du niveau |
| Rythme | Dégâts possibles toutes les 15 à 25 s pour un joueur qui a compris (règle 10.5, remplace ici le seuil générique de 7 s des niveaux réguliers) |
| Caméra | Zéro mort attribuable à un danger non visible (onde, planche instable, jet de Kappa) |
| Performance | 60 fps cible, absence de hitch sur la ligne critique, y compris aux transitions de phase |
| Contenu | 3 sceaux, 1 mémoire, route experte, les trois phases du Combat et la résolution d'apaisement tous fonctionnels |
| Route experte | Terminable sans interruption forcée |
| Checkpoint | Le checkpoint unique avant le Combat fonctionne, y compris la reprise de phase en mode Assistance (règle 10.5) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. Construire uniquement le chemin principal, en cubes (Approche + tablier de pont pour les trois phases).
3. Chronométrer le niveau sans opposition ni boss.
4. Ajouter les obstacles environnementaux (pont mobile, ondes sismiques téléguidées).
5. Ajouter Kappa sapeur (Suie), avatars de pierre et Namazu (les trois phases) en graybox.
6. Placer le checkpoint unique et construire la route experte.
7. Tester avec cinq joueurs.
8. Corriger rythme et caméra.
9. Habillage artistique — seulement à ce stade.

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0` — Ruée et rebond sont ici examinés, pas retouchés.
- **« Avatars de pierre » n'apparaît pas dans la table du bestiaire ordinaire (règle 10.2)** — traité ici comme une opposition spécifique à ce niveau plutôt qu'une famille récurrente de la campagne, en cohérence avec la règle 10.1 sur les figures protectrices. À confirmer si une future version du GDD leur donne un statut de famille à part entière.
- **Namazu est le premier boss construit** — aucune des règles 10.4/10.5 (checkpoint-avant-boss, reprise de phase en Assistance, dégâts toutes les 15-25 s) n'a encore de précédent technique dans ce projet ; ce niveau devra donc établir le patron technique que les quatre autres boss (Daitengu, Umi-bozu, Tsuchigumo, Masanori/Kokoro) réutiliseront.
- La structure « Approche / Combat en 3 phases / Résolution » remplace ici le gabarit 8.1 en 5 phases (Accroche/Enseignement/Développement/Climax/Résolution) utilisé pour les Niveaux 01-03 — lecture directe de la règle 9 du GDD, qui sépare explicitement les niveaux de boss. À valider que cette lecture correspond bien à l'intention de conception si une version future du GDD détaille davantage le gabarit des niveaux de boss.
- Comme les Niveaux 01 et 03, ce niveau suppose que la Ruée vapeur existe déjà en code (construite pour le Niveau 01) et que le tapis à vecteur externe du Niveau 03 pourrait informer la conception du pont mobile, sans être directement réutilisable (le pont réagit à un rythme d'ondes, pas à un vecteur constant).
