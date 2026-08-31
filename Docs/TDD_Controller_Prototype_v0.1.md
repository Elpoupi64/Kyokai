# Kyokai — Technical Design Document du contrôleur

Version 0.1 — Prototype PC Windows — Unreal Engine 5.8

## 1. Objectif du jalon

Produire un contrôleur 2.5D agréable dans une salle de test de 90 secondes avant de fabriquer les vingt niveaux. Le jalon valide la réponse des commandes, la caméra latérale, la lisibilité à vitesse élevée et la cible de 60 images par seconde.

Ce jalon ne contient ni art final, ni combat complet, ni boss, ni narration, ni système de collectibles définitif.

## 2. Décisions techniques

| Domaine | Décision v0.1 |
| --- | --- |
| Plateforme de référence | PC Windows, clavier et manette |
| Architecture | C++ pour le comportement, Blueprint pour l'assemblage et les réglages |
| Déplacement | `ACharacter` avec `UKyokaiMovementComponent` |
| Plan jouable | Axe X/Z, profondeur Y verrouillée par Plane Constraint |
| Caméra | Perspective latérale, spring arm fixe, retard et anticipation horizontale |
| Entrées | Enhanced Input recommandé ; mappings historiques de secours inclus |
| Framerate | 60 fps cible ; aucune mécanique ne dépend du framerate |
| Réseau | Hors périmètre du prototype, jeu solo |

## 3. Architecture livrée

| Classe | Responsabilité |
| --- | --- |
| `AKyokaiCharacter` | Entrées, saut assisté, glissade, ruée, caméra et debug |
| `UKyokaiMovementComponent` | Réglages locomoteurs, contrainte 2.5D et vitesse de glissade |
| `AKyokaiGameMode` | Sélection du personnage prototype par défaut |
| `BP_AikoPrototype` | Futur Blueprint enfant pour mesh, animation, sons et réglages |
| `L_ControllerGym` | Futur niveau de mesure et parcours de 90 secondes |

## 4. Paramètres initiaux

| Paramètre | Valeur de départ |
| --- | ---: |
| Vitesse de course | 850 cm/s |
| Accélération | 6000 cm/s² |
| Freinage au sol | 5200 cm/s² |
| Contrôle aérien | 0,65 |
| Vitesse verticale du saut | 1000 cm/s |
| Maintien maximal du saut | 0,18 s |
| Coyote time | 0,12 s |
| Buffer de saut | 0,12 s |
| Seuil de glissade | 450 cm/s |
| Entrée en glissade | 900 cm/s |
| Vitesse de ruée | 1500 cm/s |
| Durée de ruée | 0,18 s |
| Recharge de ruée | 0,35 s |
| Anticipation caméra | 220 cm |

Ces valeurs sont des hypothèses de test, pas des constantes de production. Elles doivent être évaluées sur un parcours métrique.

## 5. Commandes de secours

| Action | Clavier | Manette |
| --- | --- | --- |
| Déplacement | A/D ou flèches | Stick gauche |
| Saut | Espace | Bouton inférieur |
| Glissade | Ctrl gauche | Bouton droit |
| Ruée | Maj gauche | Gâchette haute droite |
| Debug | F1 | — |

## 6. Création du Blueprint et du niveau

1. Compiler le projet puis ouvrir Unreal Editor.
2. Créer `Content/Blueprints/Characters/BP_AikoPrototype`, enfant de `AKyokaiCharacter`.
3. Conserver la capsule visible pendant les premières mesures ou ajouter un mesh temporaire simple.
4. Créer `Content/Maps/Prototype/L_ControllerGym` avec un niveau Basic vide.
5. Ajouter un sol de 4000 × 400 cm, un `PlayerStart`, des murs de mesure et des plateformes espacées de 300, 500 et 700 cm.
6. Dans World Settings, utiliser `AKyokaiGameMode` ou un Blueprint enfant.
7. Lancer Play In Editor et vérifier le panneau de debug affiché à l'écran.

## 7. Enhanced Input à créer dans l'éditeur

Le code fonctionne avec les mappings de secours. Pour passer à la configuration cible, créer :

- `IA_Move`, valeur Axis1D ;
- `IA_Jump`, valeur Bool ;
- `IA_Slide`, valeur Bool ;
- `IA_Dash`, valeur Bool ;
- `IMC_Gameplay`, avec les touches du tableau précédent.

Assigner ensuite le contexte et les quatre actions dans les propriétés Input de `BP_AikoPrototype`. Le code bascule automatiquement sur Enhanced Input quand les cinq références sont présentes.

## 8. Parcours de test de 90 secondes

Le parcours doit contenir, dans cet ordre :

1. Couloir de 20 mètres pour mesurer accélération, vitesse et freinage.
2. Trois plateformes de hauteurs croissantes pour régler le saut variable.
3. Bord de plateforme permettant de tester le coyote time.
4. Plateforme d'arrivée permettant de tester le buffer de saut.
5. Tunnel bas précédé d'une ligne droite pour la glissade.
6. Fosse large pour tester la ruée au sol puis la ruée aérienne.
7. Ligne finale combinant saut, glissade et ruée sans arrêt forcé.

## 9. Critères d'acceptation

- Le personnage reste sur le plan Y sans dérive visible.
- Les entrées clavier et manette répondent sans délai perceptible.
- Le saut déclenché jusqu'à 120 ms après un bord fonctionne.
- Le saut pressé jusqu'à 120 ms avant l'atterrissage est exécuté à la réception.
- Relâcher rapidement le saut produit un saut sensiblement plus bas.
- La glissade ne démarre qu'au sol et au-dessus du seuil prévu.
- Une seule ruée aérienne est disponible avant l'atterrissage.
- La caméra anticipe la direction sans tremblement ni changement de profondeur.
- Le parcours tient 60 fps sur le PC de référence.
- Cinq testeurs terminent le parcours sans explication orale détaillée.

## 10. Mesures à relever

Pour chaque session : durée totale, nombre de chutes, obstacle de première mort, usages de la ruée, sauts mis en buffer, changements de direction et appréciation du contrôle sur une échelle de 1 à 5.

## 11. Prochaines extensions après validation

1. Saut mural et détection des surfaces autorisées.
2. Rebond sur objet ou ennemi.
3. Ressource de Pression et coût de la ruée.
4. Caméra sur spline avec volumes de cadrage.
5. Checkpoint et réapparition en moins de deux secondes.
6. Graybox complet du niveau 02, « Les Toits sous la pluie ».
