# Kurogane : Les Esprits de Vapeur — Protocole de playtest

Version 0.1 — préparé pour l'étape 7 du build order du Niveau 02 ("Tester avec cinq joueurs") (2026-08-31)

## Pourquoi ce document

Cette étape nécessite de vrais testeurs humains — ni moi ni aucun outil automatisé ne peut la remplacer. Ce protocole et l'instrumentation qui l'accompagne (voir plus bas) sont là pour que, quand vous aurez vos cinq joueurs, le test produise des données exploitables directement contre les critères d'acceptation de la brief, sans dépouillement manuel fastidieux.

## Identité

| Champ | Valeur |
| --- | --- |
| Niveau testé | 02 — Les Toits sous la pluie |
| Build | tag/commit à préciser au moment du test (voir `git log` sur `master`) |
| Nombre de testeurs visé | 5, chacun en premier contact avec le niveau |
| Durée par session | ~15-20 minutes (jeu + debrief court) |

## Ce que ce test doit produire

Les huit critères d'acceptation de la brief, chacun rattaché à sa source de donnée :

| Critère | Source |
| --- | --- |
| Durée médiane 8-12 minutes | Automatique (`level_completed.total_time_s`) |
| 80 % des testeurs finissent en moins de 15 minutes | Automatique |
| Aucun obstacle responsable de plus de 20 % des échecs | Automatique (`death.cause`) |
| Chemin principal compris sans indication | **Observation manuelle** — rien à capturer automatiquement pour "compris sans aide" |
| Aucune mort causée par un danger hors caméra | **Observation manuelle** + recoupement avec `death.location_x` |
| Les trois checkpoints fonctionnent | Automatique (présence de 3× `checkpoint_activated`) |
| 60 images/seconde | Automatique (`fps_sample`), avec réserve — voir Limites connues |
| Route experte terminable sans interruption forcée | Automatique (`expert_route_used`) + **observation manuelle** de la traversée si un testeur l'emprunte |

## Préparation (avant l'arrivée des testeurs)

1. Compiler et lancer le projet normalement (Play In Editor ou un build empaqueté) sur la carte `/Game/Levels/A01_L02/L_ToitsSousLaPluie`.
2. **Ne pas** lancer avec un des flags `-Kyokai*Test` (`-KyokaiLevel02Timing`, `-KyokaiExpertRouteTest`, etc.) — ce sont les bots de vérification automatisée de ce projet, ils désactivent volontairement la capture de playtest pour ne pas polluer les vraies données. Une session de test humain n'a besoin d'aucun flag.
3. Vérifier que `Saved/Playtests/` est accessible en écriture (créé automatiquement au premier lancement s'il n'existe pas).
4. Contrôles à rappeler au testeur juste avant de commencer (mappings par défaut, `DefaultInput.ini`) :
   - **A/D** : déplacement
   - **Espace** : saut
   - **Ctrl (gauche)** : glissade (au sol, en mouvement)
   - **Shift (gauche)** : Ruée vapeur / dash
   - **F1** : affichage debug (à ne pas activer pendant le test, réservé au dev)

## Consignes à donner au testeur

À lire ou paraphraser avant de lancer, sans plus de détail que ça — trop d'explication fausse le critère "compris sans indication" :

> « Tu vas traverser un niveau de plateforme en 2.5D. Le but est d'arriver au bout. Il n'y a pas d'autre objectif ni de combat. Prends ton temps pour comprendre les contrôles au début, ils sont simples. Si tu meurs ou tombes, tu repars du dernier point que tu as touché au sol. Vas-y. »

Ne pas :
- expliquer les mécaniques d'obstacles (rebonds, vent, éclairs, ennemis) à l'avance — c'est justement ce que le critère "compris sans indication" doit vérifier ;
- intervenir pendant la partie sauf si le testeur reste bloqué plus de ~2 minutes sans aucune progression (noter l'endroit si ça arrive, c'est une donnée précieuse) ;
- révéler l'existence de la route experte, sauf si le testeur la trouve seul et pose une question.

## Rôle de l'observateur pendant la session

Trois choses à noter manuellement (rien de tout ça n'est capturé automatiquement) :

1. **Moments de confusion apparente** — le testeur hésite, tourne en rond, ou répète la même erreur sans comprendre pourquoi. Noter le segment approximatif (1 à 7, voir la fiche de niveau) et une phrase de description.
2. **Toute mort qui semble venir d'un danger hors caméra** — le testeur se fait toucher sans avoir eu le temps de voir venir le danger. C'est un des critères d'acceptation directs.
3. **Comportement sur la route experte**, si le testeur l'emprunte (entrée au segment 3, au-dessus des enseignes — elle court désormais sur toute la longueur du niveau jusqu'à l'arrivée, mise à jour 2026-09-02) : est-ce que la traversée s'est faite sans blocage forcé ? A-t-il dû abandonner et redescendre ?

Une grille simple suffit :

| Testeur | Confusion (segment / description) | Mort hors caméra ? | Route experte tentée ? | Notes libres |
| --- | --- | --- | --- | --- |
| 1 | | | | |
| 2 | | | | |
| 3 | | | | |
| 4 | | | | |
| 5 | | | | |

## Données capturées automatiquement

Chaque session écrit un fichier `Saved/Playtests/Playtest_<horodatage>.jsonl` — un événement JSON par ligne, écrit au fil de l'eau (pas seulement à la fin, pour ne rien perdre si la session est interrompue). Exemple de contenu :

```jsonl
{"event": "session_start", "session_id": "20260831_165237"}
{"event": "fps_sample", "elapsed_s": 2.00, "fps": 61.2, "min_fps_so_far": 58.4}
{"event": "checkpoint_activated", "elapsed_s": 118.4, "location_x": 7200.00}
{"event": "death", "cause": "onibi", "elapsed_s": 245.1, "location_x": 10820.3, "location_z": 460.1}
{"event": "checkpoint_activated", "elapsed_s": 310.2, "location_x": 12300.00}
{"event": "expert_route_used", "elapsed_s": 340.0}
{"event": "level_completed", "total_time_s": 612.4}
```

Types d'événements :

| `event` | Champs | Sens |
| --- | --- | --- |
| `session_start` | `session_id` | Début de la session |
| `fps_sample` | `elapsed_s`, `fps`, `min_fps_so_far` | Échantillon toutes les 2s (voir limites) |
| `checkpoint_activated` | `elapsed_s`, `location_x` | Un des 3 checkpoints touché (x=11900 / 20000 / 26300 — mis à jour 2026-09-02 après l'extension de rythme en 7 segments, commit `c08f874` ; les valeurs d'origine 7200/12300/15600 ne sont plus valables sur le niveau actuel) |
| `death` | `cause`, `elapsed_s`, `location_x`, `location_z` | Renvoi au dernier checkpoint. `cause` ∈ {`fall`, `lightning`, `onibi`, `bakeneko`} |
| `expert_route_used` | `elapsed_s` | Le joueur a foulé la route experte (segment 3) |
| `level_completed` | `total_time_s` | Ligne d'arrivée atteinte — fin de session réussie |
| `session_end_incomplete` | `elapsed_s` | La session s'est terminée (fermeture du jeu) sans avoir atteint la ligne d'arrivée |

Après chaque session, renommer ou déplacer le fichier avec le numéro du testeur (ex. `Playtest_testeur3_20260831_165237.jsonl`) pour ne pas les mélanger.

## Dépouillement (après les 5 sessions)

Pour chaque critère automatique :

- **Durée médiane** : trier les 5 `total_time_s` (uniquement les sessions avec `level_completed`), prendre la valeur du milieu.
- **80 % sous 15 minutes** : sur 5 testeurs, ça veut dire au moins 4 sessions avec `total_time_s <= 900`.
- **Aucun obstacle > 20 % des échecs** : compter les `death` par `cause` sur l'ensemble des 5 sessions, diviser par le nombre total de morts. Si une seule `cause` dépasse 20 % du total, c'est un obstacle à revoir.
- **Les trois checkpoints fonctionnent** : chaque session devrait montrer 3 `checkpoint_activated` (aux mêmes `location_x`). Une session qui en montre moins signale un souci de trajet, pas forcément un bug — à recouper avec les notes d'observation.
- **60 fps** : regarder `min_fps_so_far` en fin de session. Une valeur qui reste proche de 60 (ou de la cible de la machine de test) sur toute la session est un bon signe ; des creux ponctuels correspondant à un `elapsed_s` précis valent la peine d'être recoupés avec ce qui se passait à ce moment (changement de segment, beaucoup d'acteurs à l'écran, etc.).
- **Route experte sans interruption forcée** : croiser `expert_route_used` avec les notes d'observation — l'automatique dit juste "il y est allé", pas "il a réussi sans blocage".

Les deux critères purement qualitatifs (compréhension sans indication, aucune mort hors caméra) se lisent uniquement dans la grille d'observation.

## Limites connues de l'instrumentation

- **`fps_sample` est un instantané, pas une vraie moyenne** — chaque échantillon lit `1 / DeltaSeconds` de la frame en cours au moment du timer (toutes les 2s), pas un FPS moyenné sur la fenêtre. Un pic ponctuel de lag entre deux échantillons peut ne jamais être capturé. Pour une mesure de performance plus rigoureuse, un futur passage pourra accumuler un vrai FPS moyen/min par fenêtre glissante plutôt qu'un point instantané.
- **`expert_route_used` détecte la présence sur la plateforme, pas la réussite de la traversée** — un testeur qui y monte puis tombe immédiatement déclenche quand même l'événement. C'est voulu (mesurer si les joueurs la *découvrent* est déjà une donnée utile), mais ça ne remplace pas l'observation manuelle pour juger si la traversée s'est faite proprement.
- **La cause `"unknown"`** peut apparaître si `RespawnAtCheckpoint()` est appelée sans cause précisée (ne devrait pas arriver dans le jeu actuel — les quatre points d'appel existants passent tous une cause) ; si elle apparaît dans les données, c'est le signe d'un nouveau point d'appel ajouté sans étiquette, à corriger dans le code plutôt qu'à ignorer dans le dépouillement.
- **Aucune donnée n'est capturée si le testeur joue avec un des flags `-Kyokai*Test`** — c'est intentionnel (voir Préparation), mais à vérifier si un fichier de session manque après un test.
