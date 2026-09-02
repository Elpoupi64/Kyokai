# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.1 — Gabarit Annexe A du GDD (2026-09-03)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 12 |
| Acte | III — La Marée de Yokohama |
| Nom | La Marée d'Umi-bozu |
| Propriétaire | À assigner |
| Version | 0.1 (pré-production, aucun graybox construit) |
| Durée cible | 11 minutes (fourchette GDD 10-12 min — niveau de boss, règle 9) |

## Promesse

Réorienter les lentilles du phare pour apaiser une silhouette marine géante qui bloque le port — non pas en la combattant, mais en l'illuminant, jusqu'à planer dans l'œil de la tempête qu'elle a elle-même créée.

## Mécanique signature

**Verbe** : ce niveau clôt la courbe de difficulté de l'Acte III (règle 8.3 : introduction → combinaison → renversement → **examen par le boss**) — il examine l'Ombrelle turbine seule (table des boss, règle 10.4 : Umi-bozu, examen « Ombrelle »), mais d'une façon structurellement différente des deux boss précédents : **victoire par illumination et orientation, pas par violence directe** (note de conception officielle). Namazu perdait des ancrages sous la Ruée et le rebond ; le Daitengu perdait des anneaux sous des parades de câble ; Umi-bozu n'a rien à perdre — il a besoin d'être *éclairé*.

**Règle** : les lentilles du phare, les vagues murales et les zones de calme forment un espace où la survie dépend de la lecture, pas de la frappe. Aligner une lentille sur Umi-bozu ouvre une zone de calme temporaire ; trois lentilles alignées ouvrent l'œil de la tempête lui-même, où Aiko peut enfin planer jusqu'au cœur du problème — littéralement la chaudière ou la source de désaccord qu'Umi-bozu porte, cohérent avec le principe général du jeu (aucun boss n'est tué).

**Structure du niveau de boss** (règle 9 — Approche / Combat en 3 phases / Résolution, comme les Niveaux 04 et 08) :
1. **Approche** — valider l'Ombrelle en vol libre contre une opposition ordinaire, sur une mer déjà agitée par la présence lointaine d'Umi-bozu.
2. **Combat** — trois phases, une par lentille à aligner, chacune ajoutant une règle de lecture plutôt qu'une simple augmentation de vitesse (règle 10.5).
3. **Résolution** — l'œil de la tempête s'ouvre, Aiko plane jusqu'à Umi-bozu ; pas de mise à mort, une apaisement par la lumière (note de conception officielle, conclusion la plus douce des trois premiers boss).

## Plan

| Temps cible | Macro-partie | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–4:00 | Approche | Mer agitée autour du phare, Onibi salin et tentacules d'ombre en éclaireurs lointains d'Umi-bozu | Ombrelle en vol libre, lecture d'ennemi, opposition ordinaire |
| 4:00–6:00 | Combat — Phase 1 : Survivre et aligner la première lentille | Vagues murales à éviter en planant pendant qu'une première lentille se règle | Survie par le plané, précision d'alignement sous contrainte |
| 6:00–8:00 | Combat — Phase 2 : Deuxième lentille | Tentacules d'ombre plus actives, vagues plus rapprochées, deuxième lentille à aligner avec une marge réduite | Lecture de motif plus dense, alignement plus précis |
| 8:00–9:30 | Combat — Phase 3 : Troisième lentille et œil de la tempête | Synthèse : aligner la dernière lentille PUIS planer dans l'œil qui s'ouvre | Les compétences des deux phases précédentes combinées ; accès à Umi-bozu lui-même |
| 9:30–11:00 | Résolution | La tempête se dissipe, Umi-bozu s'apaise et regagne les profondeurs, le port se rouvre | Aucun — respiration, transition narrative vers l'Acte IV |

**Checkpoint (1, règles 5.6 ET 10.5 — même interprétation que les Niveaux 04 et 08)** : un seul checkpoint réel, à 4:00 (fin de l'Approche, juste avant le Combat), avec reprise de la phase en cours en mode Assistance (règle 10.5). Hors mode Assistance, un échec pendant n'importe quelle phase du Combat renvoie à ce même checkpoint et reprend au début de la Phase 1.

**Rythme des dégâts** (règle 10.5, adapté ici puisque le combat n'est pas centré sur les dégâts au sens strict) : chaque phase doit offrir une fenêtre d'alignement de lentille exploitable toutes les 15 à 25 secondes pour un joueur qui a compris le motif des vagues — l'équivalent structurel d'une fenêtre de dégâts, mais orientée vers la précision plutôt que l'impact.

**Set piece** : comme aux Niveaux 04 et 08, le Combat entier (4:00–9:30, 5:30) fait office de set piece du niveau, dépassant volontairement le budget de 45-90 s pensé pour les niveaux réguliers.

**Secrets — trois sceaux d'harmonie (règle 8.2), placés pendant l'Approche** :
- **Sceau de lecture** — visible depuis le chemin principal, sur un rocher isolé ; récompense le joueur qui repère une zone de calme naturelle avant même le Combat.
- **Sceau de maîtrise** — exige un plané précis entre plusieurs vagues murales sans jamais toucher l'eau, une répétition à échelle réduite du Combat.
- **Sceau de risque** — près d'un groupe de tentacules d'ombre ; accessible en frôlant leur zone d'action, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — une courte route narrative pendant l'Approche (règle 8.2) : un fragment lié à un marin ou à la légende locale d'Umi-bozu, cohérent avec le nom même du niveau et avec l'idée que cette créature n'a jamais été purement malveillante.

**Route experte** — non mise en avant dans la note de conception officielle, mais requise par le contrat de contenu (règle 8.2). Conçue ici comme un plané extérieur au-dessus des vagues murales elles-mêmes, visible dès l'Approche, offrant une lecture anticipée du motif des vagues avant qu'il ne devienne critique — un avantage de lecture, comme aux Niveaux 04 et 08, pas une nouvelle mécanique.

## Opposition

**Pré-boss (Approche uniquement, règle 10.1 : deux familles maximum simultanément)** :

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Onibi salin** | Projectile lent à trajectoire lisible (bestiaire 10.2), déjà rencontré au Niveau 09 — ici la réponse canonique complète (déviation par ombrelle) s'applique enfin pleinement, l'aptitude étant désormais acquise depuis plusieurs niveaux | Déviation par ombrelle — première fois que cette réponse canonique est utilisée sans réserve depuis son introduction dans le bestiaire. |
| **Tentacules d'ombre** | Non répertorié dans le bestiaire ordinaire (règle 10.2) — traité ici comme éclaireurs lointains d'Umi-bozu, cohérent avec la silhouette marine du boss | Rester en mouvement et planer au-dessus plutôt que les affronter — annonce directement l'approche du Combat, où l'évitement prime sur la confrontation. |

**Boss (Combat, règle 10.4)** :

| Boss | Menace | Objectif | Examen |
| --- | --- | --- | --- |
| **Umi-bozu** | Vagues et ombre | Aligner les lentilles du phare | Ombrelle |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1) ; Umi-bozu seul occupe l'écran pendant le Combat, sans add d'opposition ordinaire — cohérent avec la nature du combat (lecture et orientation, pas gestion de foule).

## Art

- **Palette Acte III** (règle 11.4) : turquoise profond, bois mouillé, laiton salin — dernière apparition avant la transition vers le noir d'encre de l'Acte IV.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = lentilles et mécanismes du phare, cyan pâle = tentacules d'ombre et Umi-bozu lui-même (spirituel), rouge vermillon = vagues murales (danger immédiat) ; les zones de calme utilisent une teinte dorée distincte, seule zone du niveau où le rouge et le cyan s'effacent.
- **Couches de profondeur** (règle 11.3, table complète) :
  - L0 — embruns et écume en avant-plan, jamais d'information critique.
  - L1 — lentilles, vagues murales, Umi-bozu lui-même, contraste maximal.
  - L2 — structure du phare non jouable, synchronisée aux alignements de lentille.
  - L3 — port de Yokohama visible au loin, valeurs simplifiées — dernier rappel visuel du Niveau 09 avant la fin de l'acte.
  - L4 — ciel de tempête, silhouette d'Umi-bozu avant chaque phase, porteur du climax visuel.
- **Hero shot candidat** (règle 8.2) : Aiko en plein plané dans l'œil de la tempête, lumière des trois lentilles convergeant sur la silhouette apaisée d'Umi-bozu, mer soudain calme autour du faisceau — bon candidat de fin d'acte, cohérent avec « victoire par illumination ».

## Audio

- Grondement des vagues murales, distinct du roulis continu du Niveau 11 par son caractère plus abrupt et localisé.
- Chaque vague murale annoncée par un signal sonore distinct au moins 400 ms avant l'impact (règle 10.5).
- Cliquetis mécanique de chaque lentille qui s'aligne, signal de progression clair.
- Tentacules d'ombre avec un bruit d'eau sombre, distinct du grondement des vagues normales.
- Silence relatif dans les zones de calme — contraste sonore volontaire, seule vraie accalmie du niveau avant la Résolution.
- Motif musical de l'Acte III (règle 14.1), qui s'efface progressivement dans l'œil de la tempête au profit d'une tonalité plus pure, cohérente avec l'idée d'apaisement par la lumière plutôt que par la force.

## Technique

- **Caméra** : élargissement significatif dès le Combat pour que les vagues murales, les lentilles ET Umi-bozu restent visibles simultanément (règle 8.4/18.3) — complexifié par le fait que ce combat se joue autant à la verticale (plané) qu'à l'horizontale.
- **VFX (Niagara)** : embruns, vagues murales, faisceaux de lumière convergents des lentilles, halo de l'œil de la tempête. Limiter à trois informations simultanées par effet de gameplay (règle 12.3).
- **Streaming** : espace ouvert en mer, différent du pont (Niveau 04) et du terrain vertical (Niveau 08) — le plus grand espace continu rencontré jusqu'ici dans la campagne, à valider tôt pour le budget de rendu.
- **Dépendances nouvelles à construire (aucune n'existe encore en code/contenu)** :
  - Lentille alignable (système de visée/orientation, mécanique authentiquement nouvelle — aucun des deux boss précédents n'a de mécanique de ce type).
  - Vague murale (obstacle de plané à éviter, probable extension du système de vagues du Niveau 11).
  - Umi-bozu lui-même — troisième boss du jeu, réutilise le patron technique établi par Namazu et Daitengu (checkpoint-avant-boss, reprise de phase en Assistance, règle 10.5) mais avec une logique de victoire non-violente à construire spécifiquement (pas de retrait d'ancrage ni de bris d'anneau, un système d'alignement à la place).
  - Zone de calme (état temporaire du terrain, lié à l'alignement de lentille).
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique, en particulier lors de l'ouverture de l'œil de la tempête (règle 15.6/18.3).
- **Note d'ordre de production** : aucun graybox construit à ce jour. Contrairement à Namazu et au Daitengu (retrait d'ancrage/bris d'anneau, mécaniquement proches), ce boss introduit une VRAIE nouvelle logique de victoire (alignement de lentille, pas de dégât au sens habituel) — sa dépendance la plus risquée n'est donc pas seulement le contenu mais une extension réelle du patron de boss établi au Niveau 04.

## Accessibilité

- **Vagues murales** : télégraphie non uniquement colorimétrique — signal sonore à 400 ms (ci-dessus) et contour lisible (table 13.3) doivent suffire seuls à anticiper une vague.
- **Alignement de lentille** : viser peut être un geste fin — prévoir une zone de tolérance ajustable par le profil de difficulté (table 13.3), au même titre que la "parade élargie" pour les autres boss.
- **Umi-bozu (Combat)** : reprise de la phase en cours en mode Assistance (règle 10.5), comme aux Niveaux 04 et 08.
- **Route experte** : reste un contenu de maîtrise optionnel (avantage de lecture), jamais requis pour terminer le niveau (principe 13.3).

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), mêmes neuf critères que les fiches des Niveaux 04 et 08 :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane entre 8 et 12 min (cible spécifique : 10-12 min, approche + combat + résolution incluses) ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le motif de chaque phase du Combat sans aide directe |
| Échec | Aucune phase du Combat ne concentre plus de 20 % des abandons du niveau |
| Rythme | Fenêtres d'alignement exploitables toutes les 15 à 25 s pour un joueur qui a compris (règle 10.5) |
| Caméra | Zéro mort attribuable à un danger non visible (vague murale, tentacule hors champ) |
| Performance | 60 fps cible, absence de hitch sur la ligne critique, y compris à l'ouverture de l'œil de la tempête |
| Contenu | 3 sceaux, 1 mémoire, route experte, les trois phases du Combat et la résolution d'apaisement tous fonctionnels |
| Route experte | Terminable sans interruption forcée |
| Checkpoint | Le checkpoint unique avant le Combat fonctionne, y compris la reprise de phase en mode Assistance (règle 10.5) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. Construire uniquement le chemin principal, en cubes (Approche + espace maritime pour les trois phases).
3. Chronométrer le niveau sans opposition ni boss.
4. Ajouter les obstacles environnementaux (vagues murales, lentilles).
5. Ajouter Onibi salin, tentacules d'ombre et Umi-bozu (les trois phases) en graybox.
6. Placer le checkpoint unique et construire la route experte.
7. Tester avec cinq joueurs.
8. Corriger rythme et caméra.
9. Habillage artistique — seulement à ce stade.

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0` — l'Ombrelle est ici examinée, pas retouchée.
- **« Tentacules d'ombre » n'apparaît pas dans la table du bestiaire ordinaire (règle 10.2)** — traité ici comme opposition spécifique au niveau, même statut que les entités hors bestiaire des niveaux précédents.
- **Umi-bozu est le troisième boss construit et le premier dont la logique de victoire n'est pas une variation de « retirer/briser quelque chose »** — sa mécanique d'alignement de lentille est une extension réelle du patron établi, pas une simple réutilisation ; à concevoir en gardant à l'esprit que les deux boss suivants (Tsuchigumo, Masanori/Kokoro) pourraient chacun demander leur propre extension similaire.
- La note de conception « victoire par illumination et orientation, pas par violence directe » est la formulation la plus explicite de la philosophie non-violente du GDD (esprits ambivalents, jamais tués) rencontrée jusqu'ici dans les fiches de niveau — vaut la peine d'être citée telle quelle dans tout document de pitch ou de communication externe du jeu.
