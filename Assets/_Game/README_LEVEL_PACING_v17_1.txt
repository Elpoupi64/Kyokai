KATSUHIRO PROTOTYPE v17.1 — LEVEL PACING 8–10 MINUTES
========================================================

Base cumulative :
- toutes les étapes jusqu'à la v17 External Playtest sont conservées ;
- la v17.1 ajoute uniquement le pacing, une section jouable et la mesure.

NOUVEAUTÉS
----------
- vraie salle "Chaîne 4 possédée" entre premier combat et checkpoint ;
- 3 relais Kikai à alterner Éthérique / Normal / Éthérique ;
- second Doryoku-3 réveillé pendant la salle ;
- porte d'entrée qui empêche de contourner le premier combat ;
- porte de sortie verrouillée jusqu'à résolution complète ;
- poursuite reconstruite avec sol segmenté ;
- conduite rompue ;
- pont spectral de poursuite ;
- deux valves vapeur cycliques ;
- plateformes de rythme ;
- jalons de télémétrie ;
- rapport automatique par section ;
- objectif TOTAL 08:00–10:00 ;
- F4 affiche désormais l'état du pacing.

MENU UNITY
----------
Tools > Katsuhiro > Prepare v17.1 Pacing 8-10 Minutes

Puis :
Tools > Katsuhiro > Run v17.1 Pacing QA

Build :
Tools > Katsuhiro > Build v17.1 Pacing Playtest - Current Platform

RAPPORTS
--------
Assets/_Game/QA/v17_1_PACING_QA_REPORT.txt
Assets/_Game/QA/v17_1_STATIC_QA_REPORT.txt

En jeu :
Application.persistentDataPath/Katsuhiro_Playtest/
v17_1_pacing_..._summary.txt

IMPORTANT
---------
La durée 8–10 minutes ne peut pas être garantie par une analyse statique.
La version est conçue pour être mesurée puis calibrée sur plusieurs runs.

Le rapport indique [RAPIDE], [CIBLE] ou [LONG] pour chaque section et pour
le total, afin que la v17.2 éventuelle ne modifie que les sections réellement
hors cible.
