KATSUHIRO PROTOTYPE v5 — DORYOKU-3 VFX & ATTAQUES
=================================================

Cette version reprend le Assets(2).zip fourni.

AJOUTS
------
1. Animations procédurales d'attaque améliorées
   - armement du bras avant
   - recul du torse
   - extension mécanique du bras + pince
   - mouvement du second bras
   - réaction de la tête / lentille
   - animation de charge et de recul pour l'attaque spéciale

2. Vapeur
   - deux échappements de vapeur permanents
   - burst de vapeur à l'armement et à la frappe

3. Étincelles
   - étincelles sur chaque coup reçu
   - étincelles plus violentes lors de la destruction

4. Mort explosive
   - fumée
   - pluie d'étincelles
   - flash lumineux
   - fragments métalliques physiques projetés dans l'environnement

5. Attaque spéciale Kikai-Yūrei : Kegare Spectral Bolt
   - disponible uniquement lorsque le monde ÉTHÉRIQUE est actif
   - charge violette visible sur le Doryoku-3
   - projectile spectral horizontal
   - 2 dégâts par défaut
   - knockback plus puissant que l'attaque de mêlée
   - le projectile disparaît si Kenjiro désactive le Kikai-Yūrei
   - Kenjiro peut annuler la charge en revenant au monde normal

FICHIERS AJOUTÉS
----------------
Assets/_Game/Scripts/Enemies/Doryoku3FXController.cs
Assets/_Game/Scripts/Enemies/Doryoku3SpectralProjectile.cs
Assets/_Game/Scripts/Enemies/Doryoku3DeathFXLifetime.cs

FICHIERS MODIFIÉS
-----------------
Assets/_Game/Scripts/Enemies/Doryoku3Enemy.cs
Assets/_Game/Scripts/Enemies/Doryoku3VisualController.cs
Assets/_Game/Editor/Doryoku3PrototypeBuilder.cs
Assets/_Game/Editor/FoundryPrototypeSceneBuilder.cs

IMPORTANT : MISE À NIVEAU AUTOMATIQUE
-------------------------------------
Le Builder détecte automatiquement si le prefab Doryoku-3 présent dans
le projet est encore une version v4 sans Doryoku3FXController.

Dans ce cas, Unity :
1. supprime l'ancien Doryoku3_Possessed.prefab ;
2. génère le nouveau prefab v5 ;
3. reconstruit Foundry_Prototype.

Si nécessaire :
Tools > Katsuhiro > Create or Rebuild Foundry Prototype

ou :
Tools > Katsuhiro > Rebuild Doryoku-3 Possessed Prefab

TEST RAPIDE
-----------
- Lancer Foundry_Prototype.
- Approcher Doryoku3_Unit07.
- Observer la patrouille et l'attaque de mêlée.
- Appuyer sur K pour activer le monde ÉTHÉRIQUE.
- Garder une distance de quelques mètres : après le cooldown,
  le Doryoku-3 charge puis lance Kegare_Spectral_Bolt.
- Désactiver K pendant la charge : la spéciale est annulée.
- Désactiver K pendant le vol du projectile : le projectile disparaît.
- Frapper l'automate jusqu'à 0 PV : explosion + débris.

PARAMÈTRES PAR DÉFAUT
---------------------
Mêlée :
- dégâts : 1
- portée : 1.75
- wind-up : 0.48 s

Spéciale :
- dégâts : 2
- portée : 2.8 à 9.5
- cooldown : 4.8 s
- wind-up : 0.90 s
- vitesse projectile : 7.5
