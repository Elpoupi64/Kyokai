# Kurogane : Les Esprits de Vapeur — Fiche de niveau

Version 0.2 — Gabarit Annexe A du GDD (rédigé 2026-08-31, mis à jour 2026-09-03 après le graybox complet et l'extension de rythme en 7 segments — voir la section « État de production » en fin de fiche)

## Identité

| Champ | Valeur |
| --- | --- |
| Numéro | 02 |
| Acte | I — Edo sous pression |
| Nom | Les Toits sous la pluie |
| Propriétaire | À assigner |
| Version | 0.2 (graybox complet + rythme étendu ; seul niveau de la campagne à ce stade avec un build réel) |
| Durée cible | 9 à 10 minutes |

## Promesse

Courir et glisser sur des toits détrempés, accorder sa Ruée vapeur au rythme des enseignes et des fils télégraphiques, jusqu'à ce qu'un front d'orage transforme le quartier en une dernière ligne droite électrique.

## Mécanique signature

**Verbe** : la Ruée vapeur (débloquée au climax du niveau 01), ici *maîtrisée* plutôt qu'apprise — ce niveau ne l'enseigne pas, il apprend à l'enchaîner avec l'existant (course, saut, glissade, rebond).

**Règle** : la Ruée consomme une charge de Pression et casse les grilles fragiles ; elle sert autant à franchir un vide qu'à raccourcir une fenêtre de danger (front d'orage).

**Enseignement** : rappel silencieux en 0:00–1:00 (toits simples, pluie légère, aucun danger létal) avant d'introduire la première vraie contrainte au 1:00.

**Deux transformations** (contrat 8.2 du GDD) :
1. **1:00–2:30** — la Ruée doit composer avec l'inertie et le freinage des tuiles inclinées : glisser puis rueer sans perdre le contrôle.
2. **2:30–4:00** — la Ruée devient un outil de précision entre enseignes et fils télégraphiques, combinée au rebond (les enseignes doublent de plateformes et de tremplins).

**Examen** : la course finale dans l'orage (8:30–10:00) exige d'enchaîner course, saut, glissade et ruée sans rupture, au rythme visuel des éclairs — c'est le test final du verbe, pas une nouvelle leçon.

## Plan

Gabarit du niveau (règle 8.1 du GDD) appliqué à la structure spécifique fournie pour ce niveau :

| Temps cible | Phase (GDD 8.1) | Séquence | Éléments testés |
| --- | --- | --- | --- |
| 0:00–1:00 | Accroche | Toits simples sous une pluie légère | Course, saut, rappel de la Ruée |
| 1:00–2:30 | Enseignement | Tuiles inclinées et glissantes | Inertie, freinage, glissade |
| 2:30–4:00 | Développement (1/3) | Enseignes et fils télégraphiques | Ruées précises et rebonds |
| 4:00–5:30 | Développement (2/3) | Première rencontre Onibi | Mouvement autour d'un ennemi |
| 5:30–7:00 | Développement (3/3) | Gouttières et Bakeneko | Saut mural et poursuite |
| 7:00–8:30 | Climax (montée) | Paratonnerres et éclairs rythmés | Lecture des dangers et caméra |
| 8:30–10:00 | Climax (résolution) | Course finale dans l'orage | Enchaînement de toutes les mécaniques |

**Checkpoints (3, règle 5.6 — avant chaque set piece majeur)** :
- **CP1 — ≈2:30**, à la sortie des tuiles inclinées, avant que la Ruée devienne précise (enseignes/fils).
- **CP2 — ≈5:30**, avant la poursuite Bakeneko (saut mural sous pression temporelle).
- **CP3 — ≈7:00**, juste avant le front d'orage — le set piece de 45 à 90 s (règle 8.2) correspond à la séquence 8:30–10:00.
- **Mise à jour 2026-09-03** : ces trois repères restent les cibles de conception (avant chaque set piece majeur) ; en coordonnées de build réelles, après l'extension de rythme en 7 segments, les trois checkpoints se trouvent maintenant à x=11900 / 20000 / 26300 sur le niveau étendu — voir `Docs/Protocole_Playtest_Niveau02_v0.1.md`, mis à jour avec les mêmes valeurs.

**Set piece** : la course finale dans l'orage, 7:00→10:00 en tension montante mais le set piece proprement dit (rythme imposé par les éclairs, aucune décision de mouvement possible pendant plus de 5 s sans rendre le contrôle) est la tranche 8:30–10:00 — dans le budget des 45 à 90 s de la règle 8.2.

**Secrets — trois sceaux d'harmonie (règle 8.2 : un de lecture, un de maîtrise, un de risque)** :
- **Sceau de lecture** — visible depuis le chemin principal pendant l'Enseignement (1:00–2:30) ; récompense le joueur qui repère un renfoncement de toit sans indication.
- **Sceau de maîtrise** — dans la section enseignes/fils (2:30–4:00) ; exige une chaîne ruée→rebond→ruée précise, pas de raccourci de mouvement de base.
- **Sceau de risque** — près des paratonnerres (7:00–8:30) ; accessible en frôlant une fenêtre d'éclair, coûte potentiellement un segment d'intégrité si mal timé.

**Mémoire gravée** — une courte route narrative pendant l'Accroche ou l'Enseignement (jamais derrière une exécution difficile, règle 8.2) : un fragment lié à un locataire du quartier ou à un objet éveillé bénin, cohérent avec le ton "esprits ambivalents" du GDD.

**Route experte** — continue au-dessus des enseignes sur toute la longueur du niveau (note de conception officielle, section 9), alimentée par une ligne de rivets de cuivre qui maintient la jauge de Pression pour tenir le rythme de ruées/rebonds sans jamais retoucher le toit principal. Visible depuis le chemin principal dès l'Accroche, jamais obligatoire pour progresser. Doit rester terminable sans interruption forcée (critère de validation).

## Opposition

Maximum deux familles actives simultanément sur le chemin principal (règle 10.1) — respecté : Onibi et Bakeneko de gouttière ne se chevauchent jamais dans le plan ci-dessus.

| Famille | Rôle dans ce niveau | Réponse enseignée ici |
| --- | --- | --- |
| **Onibi** | Première rencontre de la campagne (4:00–5:30). Projectile lent à trajectoire lisible. | **Mouvement de base uniquement** — l'Ombrelle turbine (réponse "canonique" du bestiaire, section 10.2) ne se débloque qu'à l'Acte III : ici on apprend à lire la trajectoire et se repositionner par la course/le saut, pas à dévier. La réponse par déviation reste indisponible aux Onibi salins du Niveau 09 (l'Ombrelle n'est accordée qu'à son propre climax) ; elle n'est utilisée sans réserve pour la première fois qu'au Niveau 12, puis réengagée aux Niveaux 16 et 17 — mise à jour 2026-09-03 après la passe de cohérence transversale sur les 20 fiches, qui a corrigé cette même chaîne dans les fiches 16 et 17. |
| **Bakeneko de gouttière** | Poursuite le long des gouttières (5:30–7:00). Bond annoncé par un miaulement audio depuis l'arrière-plan. | Lecture du miaulement + saut mural pour gagner de la hauteur pendant la poursuite ; contre aérien optionnel si le joueur maîtrise déjà le timing. |

Densité : jamais plus de trois ennemis menaçants simultanés hors set piece (règle 10.1) ; la poursuite Bakeneko reste un ennemi unique et persistant, pas un groupe.

## Art

- **Palette Acte I** (règle 11.4) : pluie indigo, lanternes ambre, cuivre neuf.
- **Lisibilité couleur** (règle 8.4) : cuivre chaud = enseignes/fils interactifs (mécaniques), cyan pâle = Onibi/Bakeneko (spirituel), rouge vermillon = éclairs et fenêtres de paratonnerre (danger immédiat).
- **Couches de profondeur** (règle 11.3) : L1 (plan jouable) = toits, tuiles, enseignes, fils — contraste maximal ; L2 = façades et enseignes non jouables en parallaxe modérée ; L3 = skyline d'Edo sous la pluie, valeurs simplifiées ; L4 = ciel d'orage, éclairs, silhouettes de nuages — porteur du climax visuel.
- **Hero shot candidat** (règle 8.2, composition emblématique) : Aiko en ruée au-dessus des enseignes illuminées, pluie et reflets cuivrés, silhouette d'éclair en fond — bon candidat pour la communication du jeu.

## Audio

- Pluie continue en boucle, intensité croissante vers le climax (léger en 0:00, orage plein en 8:30).
- Chaque éclair est annoncé par un signal sonore distinct avant l'impact visuel (règle 8.4 : aucun danger létal sans signal sonore ET visuel).
- Miaulement distinct et directionnel pour chaque bond de Bakeneko, y compris hors champ (règle 14.2 : priorité de mixage pour les attaques hors champ).
- Rythme des enseignes qui se balancent signale la fenêtre de plateforme/rebond avant le contact visuel (règle 14.2).
- Motif musical de l'Acte I (4 à 8 notes, règle 14.1) qui se densifie avec la vitesse et la Pression du joueur pendant la course finale.

## Technique

- **Caméra** : doit élargir le champ pendant la séquence 7:00–8:30 pour que chaque éclair soit visible avant impact (aucune mort attribuable à un danger hors champ, règle 8.4/18.3) ; look-ahead renforcé pendant la course finale.
- **VFX (Niagara)** : pluie, éclaboussures sur tuiles mouillées, flash d'éclair, vapeur de la Ruée, poussière de bond Bakeneko. Limiter à trois informations simultanées par effet de gameplay (origine, trajectoire, zone de danger — règle 12.3).
- **Streaming** : niveau compact, une seule sublevel a priori suffisant à ce stade de graybox ; à revalider si la route experte alourdit le budget de rendu.
- **Dépendances nouvelles à construire** — **toutes construites depuis (mise à jour 2026-09-03)** : tuiles à friction variable (`PM_Tile_Slippery`/`PM_Tile_Grippy`), enseignes plateforme+rebond (`ABouncePad`, confirmé réutilisé tel quel), fils télégraphiques parcourables (`PM_Wire`), système de rafale de vent télégraphiée (`AWindGust`), système d'éclair télégraphié (`ALightningStrike`), comportement Onibi et Bakeneko (`AOnibi`/`ABakeneko`), système de checkpoint (`ACheckpoint`/`AKyokaiGameMode`, pas `AFlowCheckpoint` — le nom du GDD n'a jamais été repris dans le code, écart de nommage mineur déjà noté ailleurs dans le projet). Voir [[kyokai-level02-toits-pluie]] (mémoire projet) pour l'historique complet de construction, commit par commit.
- **Performance** : 60 fps cible, aucun hitch perceptible sur la ligne critique (règle 15.6/18.3).

## Accessibilité

- **Tuiles glissantes** : proposer une réduction d'élan optionnelle (profil de difficulté "Vitesse 85/70 %", règle 13.3) pour les joueurs qui subissent la glissade involontaire.
- **Éclairs** : télégraphie non uniquement colorimétrique — le signal sonore (ci-dessus) et un contour lisible (option "contours" de la table 13.3) doivent suffire seuls à anticiper l'impact.
- **Bakeneko** : le bond est un timing de réaction ; la "parade élargie" (table 13.3) et le ralenti de vitesse doivent permettre de le lire sans réflexe pur.
- **Route experte** : reste un contenu de maîtrise optionnel, jamais requis pour terminer le niveau (principe 13.3 : les aides n'annulent ni progression ni collectibles, et l'inverse — la difficulté d'un contenu optionnel ne bloque jamais la campagne).

## Validation

Critères d'acceptation d'un niveau (règle 18.3 du GDD), appliqués ici :

| Axe | Seuil |
| --- | --- |
| Durée | Médiane de première réussite entre 8 et 12 min ; 80 % des testeurs ≤ 15 min |
| Compréhension | ≥ 80 % identifient le chemin principal sans aide directe |
| Échec | Aucun obstacle non-boss ne concentre plus de 20 % des abandons |
| Rythme | Aucune attente forcée > 5 s ; aucune zone sans décision de mouvement > 7 s |
| Caméra | Zéro mort attribuable à un danger non visible (éclairs, Bakeneko hors champ) |
| Performance | 60 fps cible, absence de hitch sur la ligne critique |
| Contenu | 3 sceaux, 1 mémoire, route experte, set piece et score final tous fonctionnels |
| Route experte | Terminable sans interruption forcée |
| Checkpoints | Les 3 checkpoints fonctionnent (reprise < 2 s, dégâts non subis en double) |

## Ordre de réalisation (rappel, ne pas sauter d'étape)

1. ~~Rédiger cette fiche.~~ (fait, v0.1)
2. ~~Construire uniquement le chemin principal, en cubes.~~ (fait, 2026-08-31)
3. ~~Chronométrer le niveau sans ennemis.~~ (fait, 2026-08-31 — six vrais bugs de traversée trouvés et corrigés)
4. ~~Ajouter les obstacles environnementaux (tuiles, enseignes, fils, rafales, éclairs).~~ (fait, 2026-08-31)
5. ~~Ajouter Onibi et Bakeneko en graybox.~~ (fait, 2026-08-31)
6. ~~Placer les 3 checkpoints et construire la route experte.~~ (fait, 2026-08-31 — route experte étendue à toute la longueur du niveau le 2026-09-02 ; Pression et rivets de cuivre construits le même jour)
7. **Tester avec cinq joueurs.** En cours — lancé cette semaine (à partir du 2026-09-03), retours pas encore reçus.
8. Corriger rythme et caméra. **Partiellement anticipé** : le bug caméra remonté par le propre playtest de l'utilisateur (perte de vue des plateformes en saut) a déjà été corrigé le 2026-09-02, et une extension de rythme en 7 segments a déjà été faite en réponse au même retour (temps de traversée bot ~21.8s→~47.8s) — voir État de production ci-dessous. Reste à confirmer/ajuster avec les vraies données de l'étape 7.
9. Habillage artistique — seulement à ce stade. **Note** : une passe d'habillage anticipée a déjà eu lieu le 2026-08-31 (décision délibérée de l'utilisateur de réordonner, pas une violation du process — voir [[kyokai-level02-toits-pluie]]).

## État de production (ajouté 2026-09-03)

Ce niveau est le seul de toute la campagne (20 fiches) à avoir dépassé la pré-production — construit en premier comme vertical slice, conformément à la recommandation explicite du GDD (section 17.5 : prouver un niveau de 10 minutes avant d'engager le reste). Résumé, détails complets dans la mémoire projet [[kyokai-level02-toits-pluie]] :

- **Graybox complet** (2026-08-31) : chemin principal, cinq obstacles environnementaux, deux ennemis, trois checkpoints, route experte étendue à toute la longueur du niveau, Pression fonctionnelle avec rivets de cuivre, trois sceaux d'harmonie et la mémoire gravée — tous les éléments du contrat de contenu (règle 8.2) sont construits et vérifiés par bot.
- **Vrai playtest utilisateur (2026-09-02)** a révélé deux problèmes réels : (1) un bug caméra (perte de vue des plateformes en saut, corrigé le jour même) et (2) une durée réelle inférieure à 2 minutes contre la cible de 8-12 minutes de la règle 18.3 — confirmant par la mesure ce que les rappels de rythme théoriques (règle 8.5) laissaient présager.
- **Extension de rythme en 7 segments** (2026-09-02, tous commits séparément vérifiés par bot) a suivi ce retour : chaque segment du niveau a été physiquement allongé (jamais juste retardé), avec ajout de contenu supplémentaire (ennemis, obstacles) dans l'espace gagné, sans jamais toucher aux systèmes les plus fragiles (puits de saut mural, tunnel de glissade, chute en dash) qui restent intacts depuis leur construction initiale. Temps de traversée bot (un parcours en ligne droite sans lecture ni hésitation, **jamais une mesure de la durée réelle de jeu**) : passé de ~21.8s à ~47.8s, soit environ ×2.2.
- **Ce que l'extension NE valide PAS** : le temps de traversée bot ne confirme en rien que la médiane réelle de première réussite (règle 18.3) atteint maintenant 8-12 minutes — seul un vrai test à cinq joueurs (étape 7) peut le confirmer. C'est explicitement pourquoi l'étape 8 (corriger rythme et caméra) reste listée comme non complétée malgré tout ce travail : le rythme n'est corrigé qu'une fois validé par de vraies données, pas par une intuition de bot.
- **Prochaine étape** : dépouillement des cinq sessions de playtest dès leur réception, contre les neuf critères de la section Validation ci-dessus.

## Notes ouvertes

- Aucun élément de cette fiche ne modifie les paramètres du contrôleur figés dans `Controller_v1.0` — confirmé tenu tout au long de la construction réelle, y compris pendant l'extension de rythme.
- Les enseignes-plateformes-rebonds réutilisent bien `ABouncePad` tel quel, confirmé à la construction.
- Le système de checkpoint est construit sous les noms `ACheckpoint`/`AKyokaiGameMode` plutôt que `AFlowCheckpoint` (nom du GDD) — écart de nommage mineur, sans conséquence fonctionnelle, déjà noté dans la mémoire du projet.
- **Le playtest à cinq joueurs (étape 7) est la seule chose qui manque encore pour clore ce niveau** — tout le reste de cette fiche (Plan, Opposition, Art, Audio, Technique, Accessibilité) reste la référence de conception valide ; seul l'« Ordre de réalisation » et cette section « État de production » ont changé depuis la v0.1.
