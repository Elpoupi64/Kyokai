# Kyokai — Prototype du contrôleur v0.1

Le projet contient le premier socle C++ du contrôleur 2.5D : course, saut variable, coyote time, buffer de saut, glissade, ruée, caméra latérale et affichage de debug.

## Démarrage rapide

1. Régénérer les fichiers de projet depuis `Kyokai.uproject`.
2. Compiler la cible `KyokaiEditor` en Development Editor / Win64.
3. Ouvrir le projet dans Unreal Engine 5.8.
4. Créer une map Basic et y placer un `PlayerStart` et quelques cubes de sol.
5. Vérifier que le GameMode est `KyokaiGameMode`.
6. Lancer Play In Editor.

Commandes : A/D ou flèches pour bouger, Espace pour sauter, Ctrl pour glisser, Maj pour la ruée et F1 pour masquer le debug.

La procédure détaillée et les valeurs de départ sont dans `Docs/TDD_Controller_Prototype_v0.1.md`.
