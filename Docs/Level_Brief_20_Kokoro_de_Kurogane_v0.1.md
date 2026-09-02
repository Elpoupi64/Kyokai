# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.1 — Gabarit Annexe A du GDD (2026-09-03)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 20 (dernier niveau de la campagne) |
| Acte | V — Le Cœur Kurogane |
| Nom | Kokoro de Kurogane |
| Propriétaire | À assigner |
| Version | 0.1 (pré-production, aucun graybox construit) |
| Durée cible | 12 minutes (fourchette GDD 11-12 min, valeur haute retenue — boss final de toute la campagne) |

## Promesse

Conclure toutes les mécaniques du jeu et son thème central — l'accord plutôt que la destruction — face à Masanori appareillé et à l'avatar du Kokoro lui-même : un duel mobile, une course à l'intérieur de la machine, un accordage final en Surpression, puis un épilogue jouable où plus rien n'est mis à l'épreuve, seulement vécu.

## Mécanique signature

**Verbe** : ce niveau clôt la courbe de difficulté de l'Acte V et de la campagne entière (règle 8.3 : introduction → combinaison → renversement → **examen par le boss**) — il examine tout à la fois (table des boss, règle 10.4 : Masanori/Kokoro, examen « Synthèse »). C'est ici, et seulement ici, que la **Surpression** est accordée et utilisée : la cinquième et dernière aptitude du jeu, qui combine les quatre autres pour une fenêtre courte, réservée explicitement au climax de ce niveau (règle 5.4).

**Règle** : cœur rotatif, conduites vivantes et plateformes de résonance forment l'intérieur du Kokoro lui-même — pas une métaphore mais un lieu littéral. **Aucune QTE** (note de conception officielle, sans ambiguïté) : chaque action décisive du combat utilise une commande déjà apprise dans un niveau antérieur, jamais une suite de touches inédite improvisée pour l'occasion.

**Structure du niveau de boss, avec un ajout unique à toute la campagne** (règle 9 — Approche / Combat en 3 phases / Résolution / **Épilogue jouable**) :
1. **Approche** — dernière validation de la maîtrise complète des quatre aptitudes contre Masanori appareillé et l'avatar du Kokoro, séparément.
2. **Combat, Phase 1 — Duel mobile** : un affrontement direct contre Masanori, testant le déplacement et le combat léger déjà maîtrisés.
3. **Combat, Phase 2 — Course interne** : une poursuite à l'intérieur du Kokoro, testant la vitesse et la lecture d'environnement à travers conduites vivantes et cœur rotatif.
4. **Combat, Phase 3 — Accordage en Surpression** : la Surpression est accordée puis immédiatement mise à l'épreuve pour aligner les plateformes de résonance, synthèse des quatre aptitudes en une seule fenêtre d'action.
5. **Résolution** — Masanori, la machine et l'esprit du Kokoro sont séparés (règle 10.4 : « séparer l'homme, la machine et l'esprit »), sans destruction, cohérent avec le thème central du jeu.
6. **Épilogue jouable** — contrairement aux trois bosses précédents, ce niveau ne se termine pas sur la Résolution : un dernier segment, entièrement jouable mais sans enjeu d'échec, laisse le joueur marcher dans les conséquences immédiates de ce qui vient de se passer.

## Plan

| Temps cible | Macro-partie | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–4:00 | Approche | Dernière ligne droite vers le cœur du Kokoro, Masanori appareillé et avatar du Kokoro en éclaireurs séparés | Les quatre aptitudes en maîtrise complète, dernière opposition ordinaire de la campagne |
| 4:00–6:00 | Combat — Phase 1 : Duel mobile | Affrontement direct contre Masanori, terrain qui bouge avec le cœur rotatif en arrière-plan | Déplacement et combat léger déjà appris |
| 6:00–8:00 | Combat — Phase 2 : Course interne | Poursuite à travers les conduites vivantes à l'intérieur du Kokoro | Vitesse, lecture d'environnement organique-mécanique |
| 8:00–9:30 | Combat — Phase 3 : Accordage en Surpression | La Surpression est accordée ; alignement des plateformes de résonance sous cette aptitude combinée | Synthèse des quatre aptitudes ; seule utilisation de la Surpression du jeu |
| 9:30–10:30 | Résolution | Séparation de l'homme, de la machine et de l'esprit ; le Kokoro cesse d'imposer son accord forcé | Aucun test — la conclusion narrative du contrat de conception (règle 2.3) |
| 10:30–12:00 | Épilogue jouable | Aiko marche dans les conséquences immédiates, sans enjeu d'échec possible | Aucun test — un dernier segment vécu, pas éprouvé |

**Checkpoint (1, règles 5.6 ET 10.5 — même interprétation que les Niveaux 04, 08, 12 et 16)** : un seul checkpoint réel, à 4:00 (fin de l'Approche, juste avant le Combat), avec reprise de la phase en cours en mode Assistance (règle 10.5). **L'Épilogue jouable n'a besoin d'aucun checkpoint** : sans enjeu d'échec, il n'y a rien à reprendre.

**Rythme des dégâts** (règle 10.5) : chaque phase du Combat doit offrir une fenêtre d'action exploitable toutes les 15 à 25 secondes pour un joueur qui a compris le motif — y compris la Phase 3, où la fenêtre est celle de l'alignement en Surpression plutôt que d'un dégât classique.

**Set piece** : le Combat entier (4:00–9:30, 5:30) fait office de set piece principal, comme pour les quatre boss précédents — mais ce niveau ajoute un second segment non-test (l'Épilogue) qu'aucune autre fiche de la campagne ne possède.

**Secrets — trois sceaux d'harmonie (règle 8.2), placés pendant l'Approche, derniers de toute la campagne** :
- **Sceau de lecture** — visible depuis le chemin principal ; récompense le joueur qui repère un détail du Kokoro annonçant sa vraie nature avant même le Combat.
- **Sceau de maîtrise** — exige une chaîne des quatre aptitudes sans la Surpression, dernière épreuve de maîtrise pure avant que l'aptitude finale ne change la donne.
- **Sceau de risque** — près de l'avatar du Kokoro ; accessible en frôlant sa présence, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — le dernier fragment de toute la campagne, placé pendant l'Approche ou intégré à l'Épilogue lui-même (règle 8.2) : probablement lié à Tetsu (le tsukumogami-compagnon d'Aiko, dont le métal d'origine vient de la même forge que le Kokoro selon la structure narrative macro du GDD) — à coordonner étroitement avec l'écriture narrative plutôt que déduit seul de cette fiche.

**Route experte** — même à ce niveau, requise par le contrat de contenu (règle 8.2) si l'on suit la règle à la lettre, mais son sens change en fin de campagne : plutôt qu'un raccourci de vitesse, elle pourrait ici représenter un chemin d'observation supplémentaire du Kokoro pendant l'Approche. À confirmer avec la direction si un niveau final de campagne doit strictement respecter ce point du contrat de contenu ou s'il en est explicitement exempté.

## Opposition

**Pré-boss (Approche uniquement, règle 10.1 : deux familles maximum simultanément)** :

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Masanori appareillé** | L'antagoniste principal du jeu, désormais équipé par le Kokoro — non répertorié dans le bestiaire ordinaire (règle 10.2), une opposition unique à ce niveau et narrativement centrale | Dernière validation de toutes les réponses de combat léger déjà apprises, avant le Duel mobile du Combat lui-même. |
| **Avatar du Kokoro** | Manifestation de la machine elle-même avant le Combat final — non répertorié dans le bestiaire ordinaire | Lecture combinée des quatre aptitudes, une répétition générale du Combat à suivre. |

**Boss (Combat, règle 10.4)** :

| Boss | Menace | Objectif | Examen |
| --- | --- | --- | --- |
| **Masanori / Kokoro** | Duel, course interne, accordage | Séparer l'homme, la machine et l'esprit | Synthèse |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1) ; le Combat lui-même n'oppose qu'une seule entité complexe (Masanori/Kokoro) à travers ses trois phases, sans add.

## Art

- **Palette Acte V** (règle 11.4) : cendre, or incandescent, cyan spectral — à son intensité maximale ici, dernière apparition de toute la campagne.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = plateformes de résonance et mécanismes du Kokoro, cyan pâle = avatar du Kokoro (spirituel), rouge vermillon = attaques de Masanori et instabilité du cœur rotatif (danger immédiat) ; la Surpression elle-même mérite une signature visuelle propre, distincte des quatre aptitudes individuelles, réservée à ce seul niveau.
- **Couches de profondeur** (règle 11.3, table complète) :
  - L0 — vapeur et étincelles du cœur en avant-plan, jamais d'information critique.
  - L1 — conduites vivantes, plateformes de résonance, Masanori et l'avatar du Kokoro, contraste maximal.
  - L2 — architecture interne du Kokoro non jouable, synchronisée au cœur rotatif.
  - L3 — vues fragmentaires d'Edo visibles à travers les conduites, rappel discret de toute la campagne.
  - L4 — cœur du Kokoro lui-même, source de lumière centrale, porteur du climax visuel et de l'Épilogue.
- **Hero shot candidat** (règle 8.2) : Aiko en pleine Surpression, les quatre aptitudes visibles simultanément (traînée de Ruée, câble tendu, ombrelle ouverte, bascule de plan) convergeant vers les plateformes de résonance, Masanori et le Kokoro qui se séparent en arrière-plan — l'image de communication la plus ambitieuse de toute la campagne, probable candidat de couverture.

## Audio

- Battement continu du cœur rotatif, base sonore de tout le niveau, qui accélère ou ralentit selon la phase.
- Chaque attaque de Masanori annoncée par un signal sonore distinct au moins 400 ms avant l'impact (règle 10.5).
- Voix de Masanori (dialogues courts, règle 14.3) présente pendant le Duel mobile — première fois qu'un boss de la campagne a une voix parlée pendant le Combat lui-même plutôt qu'un simple cri animal.
- Signature sonore propre à la Surpression, distincte des quatre aptitudes individuelles (règle 5.4 : « un son de chaudière, la lueur du sac dorsal... indiquent l'état sans détourner le regard » — poussé ici à son maximum).
- Motif musical de l'Acte V (règle 14.1), qui cite discrètement les motifs des quatre actes précédents pendant l'Épilogue — un dernier rappel musical de tout le trajet du joueur.
- Résolution et Épilogue : le battement du cœur rotatif s'apaise progressivement jusqu'au silence ou une note tenue, dernier geste sonore de toute la campagne.

## Technique

- **Caméra** : le défi le plus exigeant de la campagne entière — doit rester lisible à travers trois phases de nature très différente (duel rapproché, course en environnement organique, alignement de précision en Surpression), plus un Épilogue au rythme totalement différent (règle 8.4/18.3, 12.4).
- **VFX (Niagara)** : synthèse visuelle des quatre aptitudes en Surpression (le système VFX le plus complexe de toute la campagne), battement du cœur rotatif, séparation homme/machine/esprit à la Résolution. Limiter à trois informations simultanées par effet de gameplay (règle 12.3) — un vrai défi pendant la Phase 3, où quatre aptitudes agissent ensemble.
- **Streaming** : espace unique et continu (l'intérieur du Kokoro), moins fragmenté que le Niveau 19 mais avec une exigence de fluidité totale à travers cœur rotatif et conduites vivantes.
- **Dépendances nouvelles à construire (aucune n'existe encore en code/contenu)** :
  - **La Surpression elle-même** (`UResonanceAbilityComponent`, cinquième et dernière aptitude) — combine les quatre aptitudes précédentes pour une fenêtre courte ; n'existe pas en code à ce jour, et contrairement aux quatre autres, elle n'est utilisée que dans CE seul niveau, jamais ailleurs dans la campagne.
  - Masanori appareillé et avatar du Kokoro — dernières entités du bestiaire à construire.
  - Cœur rotatif, conduites vivantes, plateformes de résonance (mécaniques de niveau propres à ce lieu).
  - Système d'Épilogue jouable sans échec possible — genre de séquence sans précédent dans le reste de la campagne, plus proche d'une scène interactive que d'un niveau au sens habituel.
  - **Final sans QTE, note de conception officielle explicite** : vérifier explicitement, phase par phase, qu'aucune action décisive ne demande une commande qui n'a pas déjà été enseignée dans un niveau antérieur — une contrainte de conception à auditer, pas seulement à respecter par défaut.
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique, en particulier pendant la Phase 3 où les quatre systèmes d'aptitude tournent simultanément (règle 15.6/18.3).
- **Note d'ordre de production** : aucun graybox construit à ce jour. La Surpression est la dernière et la plus risquée des cinq aptitudes du jeu à construire, dépendant elle-même de la stabilité complète des quatre autres — ce niveau ne peut raisonnablement être commencé qu'après tous les autres niveaux qui introduisent une aptitude (01, 05, 09, 13).

## Accessibilité

- **Surpression (Phase 3)** : la fenêtre de synthèse doit rester lisible malgré la densité d'informations visuelles — contour distinct par aptitude active (table 13.3), pas seulement une addition de VFX.
- **Masanori (Duel mobile)** : télégraphie non uniquement colorimétrique — signal sonore à 400 ms (ci-dessus) et contour lisible (table 13.3).
- **Masanori/Kokoro (Combat)** : reprise de la phase en cours en mode Assistance (règle 10.5), comme aux quatre boss précédents.
- **Épilogue jouable** : par nature déjà accessible à tout profil, puisqu'aucun échec n'y est possible — vaut la peine d'être documenté comme un exemple positif dans la bible d'accessibilité du jeu.

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), adaptés une dernière fois pour refléter la structure unique de ce niveau (Épilogue en plus) :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane entre 8 et 12 min (cible spécifique : 11-12 min, approche + combat + résolution + épilogue incluses) ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le motif de chaque phase du Combat sans aide directe |
| Échec | Aucune phase du Combat ne concentre plus de 20 % des abandons du niveau |
| Rythme | Fenêtres d'action exploitables toutes les 15 à 25 s pour un joueur qui a compris (règle 10.5) |
| Caméra | Zéro mort attribuable à un danger non visible, dans les trois phases comme dans l'Approche |
| Performance | 60 fps cible, absence de hitch sur la ligne critique, en particulier pendant la Phase 3 (Surpression) |
| Contenu | 3 sceaux, 1 mémoire, les trois phases du Combat, la Résolution ET l'Épilogue jouable tous fonctionnels |
| Sans QTE | Audit explicite confirmant qu'aucune action décisive n'utilise une commande non enseignée au préalable (note de conception officielle) |
| Checkpoint | Le checkpoint unique avant le Combat fonctionne, y compris la reprise de phase en mode Assistance (règle 10.5) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. Construire uniquement le chemin principal, en cubes (Approche + intérieur du Kokoro pour les trois phases + Épilogue).
3. Chronométrer le niveau sans opposition ni boss.
4. Ajouter les obstacles environnementaux (cœur rotatif, conduites vivantes, plateformes de résonance).
5. Ajouter Masanori appareillé, l'avatar du Kokoro et le Combat en trois phases en graybox — après construction de la Surpression elle-même.
6. Placer le checkpoint unique ; l'Épilogue jouable ne nécessite aucun système de checkpoint dédié.
7. Tester avec cinq joueurs.
8. Corriger rythme et caméra.
9. Habillage artistique — seulement à ce stade, avec une attention particulière à l'Épilogue, seul segment de toute la campagne pensé pour être contemplatif plutôt qu'éprouvant.

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0` — la Surpression combine les quatre aptitudes existantes, elle n'en retouche aucune individuellement.
- **La Surpression est la seule des cinq aptitudes du jeu qui n'est utilisée que dans un seul niveau** — contrairement à Ruée/Câble/Ombrelle/Accord spectral, qui sont chacune réutilisées dans tous les niveaux suivant leur introduction, la Surpression est explicitement réservée au climax de ce niveau (règle 5.4). À confirmer si une version future du GDD prévoit d'autres usages ponctuels ailleurs dans la campagne (une route experte tardive, par exemple), ou si son exclusivité à ce niveau est un choix de conception définitif.
- **L'Épilogue jouable est une structure sans précédent dans les vingt fiches de cette campagne** — ni un niveau régulier, ni un combat de boss classique, ni une simple cinématique (puisqu'il reste jouable). Mérite sa propre réflexion de conception, distincte du gabarit 8.1 et du patron boss établi au Niveau 04, avant toute implémentation.
- **La route experte de ce niveau est signalée comme potentiellement non pertinente** pour un niveau final de campagne — à trancher explicitement avec la direction plutôt que d'appliquer le contrat de contenu (règle 8.2) par défaut sans questionnement, contrairement à tous les niveaux précédents où son inclusion allait de soi.
- **Ce niveau clôt les vingt fiches de niveau de toute la campagne** (Niveaux 01 à 20, Actes I à V). Toutes les aptitudes introduites (Ruée 01, Câble 05, Ombrelle 09, Accord spectral 13, Surpression 20) et toutes les familles du bestiaire canonique trouvent ici, directement ou par écho, leur dernière apparition ou leur point de convergence.
