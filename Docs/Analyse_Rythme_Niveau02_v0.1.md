# Kurogane : Les Esprits de Vapeur — Analyse de rythme, Niveau 02

Version 0.1 — note d'investigation (2026-09-03), déclenchée par 2 sessions de vrais testeurs sur le build post-extension. **Document d'analyse et de proposition, rien construit à ce stade.**

## Le constat chiffré

Deux vraies sessions de test (`Playtest_20260903_003144.jsonl`, `Playtest_20260903_091716.jsonl`), sur le build actuel (checkpoints à x=11900/20000/26300, donc bien post-extension en 7 segments) :

| | Session 1 | Session 2 |
|---|---|---|
| Temps total | 51.4s | 57.8s |
| Morts | 0 | 0 |
| Sceau / mémoire ramassés | 1 / 1 | 1 / 1 |

Le bot de référence (`Saved/Level02TimingReport.json`, même build) : **47.75s**, découpé segment par segment :

| Segment | Durée bot | Distance | Vitesse moyenne |
|---|---|---|---|
| Seg1_Accroche | 16.30s | 13244u | 812 u/s |
| Seg2_Enseignement | 7.75s | 7299u | 942 u/s |
| Seg3_Enseignes | 6.15s | 5104u | 830 u/s |
| Seg4_Onibi | 3.85s | 3272u | 850 u/s |
| Seg5_Gouttieres | 5.90s | 3335u | 565 u/s |
| Seg6_Paratonnerres | 3.50s | 2975u | 850 u/s |
| Seg7_Finish | 4.30s | 3989u | 928 u/s |

**Ratio temps réel / temps bot : ~1.14 en moyenne (1.08 et 1.21).** C'est le vrai problème. Un joueur en tout premier contact ne prend quasiment aucun temps de plus qu'un bot qui ne lit rien et n'hésite jamais — et ce sur les 7 segments, climax et paratonnerres inclus, où la vitesse moyenne du bot reste collée à 850 u/s (vitesse de course max) quasiment sans interruption. Zéro segment ne force un ralentissement réel.

## Pourquoi l'extension en 7 segments n'a presque rien changé

Le motif utilisé pour l'extension (« élargir avec point d'entrée fixe » — voir [[kyokai-level02-toits-pluie]]) a fait passer le temps bot de 21.8s à 47.8s (×2.2), en allongeant physiquement des plateformes. Mais élargir une plateforme n'ajoute que de la distance parcourue à vitesse de croisière constante — ça ne crée aucune décision, aucune lecture, aucun ralentissement forcé. Un joueur qui n'a besoin de rien évaluer avance à la même vitesse qu'un bot. C'est exactement ce que confirment les deux vraies sessions : elles suivent le bot à moins de 25 % d'écart, alors que le protocole de test prévoyait 15-20 minutes par session.

**Distance ≠ rythme.** Le levier qui manque, c'est la densité de décision par unité de distance, pas la distance elle-même.

## L'ampleur réelle du problème

Cible de la fiche : 9-10 min (règle 18.3 : médiane 8-12 min). Le tableau ci-dessous calcule le temps bot nécessaire pour différentes hypothèses de ratio réel/bot (R) une fois de la vraie friction ajoutée — en prenant 570s (9.5 min) comme cible médiane :

| Hypothèse de ratio réel/bot (R) | Temps bot requis | Facteur vs bot actuel (47.75s) |
|---|---|---|
| R = 1.2 (comme aujourd'hui — pas de vraie friction) | ~475s | ×10 |
| R = 1.5 (friction légère) | ~380s | ×8 |
| R = 2.0 (friction modérée, raisonnable pour un platformer bien lisible) | ~285s | ×6 |
| R = 3.0 (friction marquée) | ~190s | ×4 |
| R = 5.0 (friction très forte — optimiste) | ~114s | ×2.4 |

Même dans l'hypothèse la plus optimiste (R=5), il faut encore ~2.4× le contenu bot actuel — et cette fois, un ×2.4 qui doit venir de vraie densité de décision, pas de distance, puisque le ×2.2 déjà fait par distance seule n'a produit qu'un R≈1.14. Dans les hypothèses les plus réalistes (R=2 à 3), c'est ×4 à ×6 de contenu effectif qu'il faut ajouter. C'est un chantier d'une tout autre ampleur que les 7 segments déjà faits — pas un ajustement, une refonte de la densité de contenu du niveau.

## Leviers de friction compatibles avec la règle 8.5

La règle 8.5 interdit toute attente forcée > 5s et toute zone sans décision de mouvement > 7s — donc la solution n'est pas de ralentir le joueur passivement (temps mort), mais d'augmenter la fréquence des décisions qu'il doit prendre :

1. **Densité d'obstacles, pas longueur de trajet** — remplacer les longs tronçons rectilignes (visibles dans le tableau ci-dessus : plusieurs segments cruisent à 850 u/s sans interruption) par une alternance plus fréquente de timing, d'angle, de type d'obstacle. Le bot devrait ralentir *à cause du contenu*, pas seulement parcourir plus de distance à vitesse de pointe.
2. **Fréquence de rencontre plus élevée, dans le même plafond de familles** — la règle 10.1 plafonne à 2 familles actives / 3 menaces simultanées, mais rien n'empêche plus d'occurrences successives d'Onibi et de Bakeneko dans les segments qui leur sont dédiés. Actuellement zéro mort sur les deux sessions réelles : les ennemis existants ne menacent quasiment pas.
3. **Vrais embranchements sur le chemin principal** — pas seulement des sceaux optionnels (qui n'affectent pas la vitesse d'un joueur qui va droit au but), mais des choix de route visibles où les deux options semblent plausibles, qui forcent une évaluation avant de s'engager.
4. **Fenêtres de timing resserrées** — sans casser la lisibilité (règle 8.4 : signal sonore + visuel avant tout danger létal), réduire légèrement la marge de certains passages (tuiles, enseignes, éclairs) pour qu'ils exigent une vraie lecture au lieu d'un passage en confiance.
5. **Revoir le motif d'extension lui-même** — « élargir avec point d'entrée fixe » doit être remplacé, pour toute future passe, par « insérer du contenu supplémentaire dans l'espace existant » plutôt que « repousser le bord loin d'une plateforme ». Concrètement : plus d'éléments plus rapprochés, pas les mêmes éléments plus espacés.

## Métrique de suivi proposée

Le temps de traversée bot total (utilisé jusqu'ici) masque le problème — un ×2.2 en distance ressemblait à un progrès alors qu'il n'en était pas un pour le vrai rythme. Proposition : suivre la **vitesse moyenne du bot par segment**, comme dans le tableau ci-dessus. Un segment qui reste proche de 850 u/s (vitesse de course max) sur toute sa longueur est un signal direct de manque de friction, indépendamment de sa durée totale — c'est un signal actionnable dès le graybox, sans attendre de vrais testeurs.

## Ce que ce document ne fait pas

Aucune modification de `.umap` ni de `KyokaiGameMode.cpp` n'a été faite pour produire cette analyse — c'est une investigation de diagnostic et de direction, pas encore un plan de construction segment par segment. Avant de construire quoi que ce soit, il reste à décider : viser une refonte complète de la densité de contenu (chantier ×4 à ×6), ou reconsidérer la cible de 9-10 minutes elle-même pour ce type de niveau (plateforme d'évitement pur, sans boucle de combat).

## Notes ouvertes

- Il manque toujours 3 des 5 vrais testeurs visés — les deux sessions disponibles donnent un signal déjà net (R≈1.14 dans les deux cas, cohérent), mais un troisième point de données réel confirmerait si ce ratio est stable ou si ces deux joueurs sont simplement rapides.
- Deux bugs indépendants repérés pendant cette analyse restent à corriger séparément (non traités ici) : un checkpoint fantôme à x≈-4557 déclenché ~8s après le spawn dans les deux sessions réelles, et une zone jouable après `level_completed` où les deux joueurs sont tombés (x≈31500-32400, au-delà de la ligne d'arrivée).
