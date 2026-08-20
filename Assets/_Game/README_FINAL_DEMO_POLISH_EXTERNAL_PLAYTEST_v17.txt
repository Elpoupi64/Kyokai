KATSUHIRO PROTOTYPE v17 — FINAL DEMO POLISH + EXTERNAL PLAYTEST
=================================================================

Cette version est cumulative et construite depuis le dernier Assets.zip fourni.
La v16.1 RC2 et toutes les étapes précédentes restent incluses.

OBJECTIF
--------
La v17 ne crée pas un nouveau niveau.
Elle prépare la vertical slice pour un premier playtest externe réel.

AJOUTS PRINCIPAUX
-----------------
1. Final polish conservateur :
   - mouvement légèrement plus nerveux ;
   - coyote time / jump buffer plus permissifs ;
   - combo légèrement plus souple ;
   - esquive plus fiable ;
   - caméra un peu plus réactive ;
   - attaques Doryoku-3 légèrement plus lisibles ;
   - mini-boss légèrement moins punitif.

2. Performance HUD F3 amélioré :
   - FPS moyen ;
   - 1% low approximatif ;
   - frame time ;
   - preset qualité ;
   - nombre de particules ;
   - mémoire GC.

3. Playtest HUD F4 :
   - durée ;
   - morts ;
   - respawns ;
   - checkpoints ;
   - tentatives / victoire boss ;
   - activations Kikai ;
   - fin de démo ;
   - copie du chemin du rapport.

4. Télémétrie locale :
   - aucun envoi réseau ;
   - JSON + résumé TXT ;
   - sauvegardés dans Application.persistentDataPath/Katsuhiro_Playtest/.

5. Build playtest :
   Tools > Katsuhiro > Prepare v17 External Playtest
   Tools > Katsuhiro > Run v17 External Playtest QA
   Tools > Katsuhiro > Build v17 External Playtest - Current Platform

6. Documents de test :
   Assets/_Game/Playtest/PLAYTEST_GUIDE_v17.txt
   Assets/_Game/Playtest/EXTERNAL_PLAYTEST_SURVEY_v17.txt

7. Le build externe copie automatiquement :
   - PLAYTEST_GUIDE_v17.txt
   - EXTERNAL_PLAYTEST_SURVEY_v17.txt
   - v17_PLAYTEST_QA_REPORT.txt

PERFORMANCE / LISIBILITÉ
------------------------
Le budget particules devient :
- Performance : 40 %
- Balanced : 64 %
- Cinematic : 95 %

L'objectif est de réduire le bruit visuel sans retirer l'atmosphère.

VALIDATION
----------
La v17 réutilise le préflight v16.1 puis ajoute ses propres checks.
Une build playtest est bloquée si le QA contient un blocker.

IMPORTANT
---------
Cet environnement ne compile pas Unity et ne mesure pas les FPS réels.
La v17 est une préparation structurelle pour le vrai test sur votre machine.

La première information utile après test sera :
- v17_PLAYTEST_QA_REPORT.txt ;
- Console Unity ;
- rapport v17_playtest_..._summary.txt ;
- captures F3 des zones lourdes.
