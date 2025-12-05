# Tile System - Guide d'utilisation

## Vue d'ensemble
Ce système permet de placer, remplacer et empiler différents types de tiles hexagonales avec un système de brush et de preview.

## Composants principaux

### 1. TileSelector
Gère la sélection de la tile actuelle à placer.
- **Scroll molette** : Change de tile (parcourt la liste des tiles disponibles)
- Affiche le nom de la tile sélectionnée en haut à gauche

### 2. TilemapManagerCopy
Gère le placement des tiles de base.
- **Clic gauche** : Place/remplace la tile de base avec la tile sélectionnée
- **Clic droit** : Ajoute une tile en hauteur (ne remplace pas la base)

### 3. TileHeightManager
Gère l'empilement vertical des tiles.
- **Shift + Clic droit** : Baisse la colonne (retire une tile du sommet)
- `maxHeight` : Limite la hauteur maximale d'une colonne

### 4. BrushSizeManager
Gère la taille du brush (zone de placement).
- **Ctrl + Scroll molette** : Change la taille du brush (1-10)
- Permet de placer plusieurs tiles à la fois

### 5. BrushPreview
Affiche une preview transparente de la tile qui sera placée.
- S'adapte automatiquement à la hauteur des colonnes existantes
- Change de forme quand vous changez de tile
- Change de taille avec le brush

### 6. TileOcclusionCulling (DÉSACTIVÉ - Conservé pour usage futur)
~~Optimise le rendu en cachant les tiles bloquées.~~
- ⚠️ **Ce système est actuellement désactivé**
- Le script est conservé dans le projet pour une utilisation future si nécessaire
- Pour l'activer : attacher le script à un GameObject et configurer les paramètres

## Configuration Unity

### Setup de base
1. Créer un GameObject vide et y attacher les managers actifs :
   - TileSelector
   - TilemapManagerCopy
   - TileHeightManager
   - BrushSizeManager
   - BrushPreview
   ~~- TileOcclusionCulling~~ (DÉSACTIVÉ)

2. **TileSelector** :
   - Ajouter les tiles dans le tableau `Available Tiles`
   - Pour chaque tile : définir un nom et assigner le prefab

3. **TilemapManagerCopy** :
   - Assigner la référence vers TileSelector
   - Assigner la référence vers TileHeightManager
   - Assigner la référence vers BrushSizeManager
   - Définir `Hex Size` (largeur de l'hexagone)

4. **TileHeightManager** :
   - Assigner la référence vers TileSelector
   - Assigner la référence vers BrushSizeManager
   - Définir `Tile Height` (hauteur d'une tile)
   - Définir `Max Height` (hauteur max des colonnes)
   - Définir `Hex Size` (doit correspondre à TilemapManagerCopy)

5. **BrushPreview** :
   - Assigner la référence vers TileSelector
   - Assigner la référence vers TileHeightManager
   - Assigner la référence vers BrushSizeManager
   - Assigner un matériau transparent dans `Preview Material Template`
   - Ajuster `Preview Color` (couleur de la preview)
   - Définir `Hex Size` et `Tile Height` (doivent correspondre aux autres scripts)


## Contrôles

| Action | Contrôle |
|--------|----------|
| Changer de tile | **Scroll molette** |
| Placer/remplacer tile de base | **Clic gauche** |
| Ajouter tile en hauteur | **Clic droit** |
| Baisser une colonne | **Shift + Clic droit** |
| Changer taille du brush | **Ctrl + Scroll** |

## Fonctionnalités

### Système de tiles multiples
- Ajoutez autant de tiles que vous voulez dans le TileSelector
- Chaque tile peut avoir son propre modèle 3D
- Le scroll change la tile sélectionnée

### Placement intelligent
- **Clic gauche** : Remplace complètement la tile de base et réinitialise la hauteur
- **Clic droit** : Ajoute uniquement en hauteur sans toucher à la base
- Le système empêche de dépasser `maxHeight`

### Preview en temps réel
- La preview utilise le prefab de la tile sélectionnée
- S'adapte à la hauteur des colonnes existantes
- Change automatiquement quand vous changez de tile ou de taille de brush

## Notes importantes

1. Toutes les valeurs `Hex Size` et `Tile Height` doivent être cohérentes entre les scripts
2. Le prefab de tile doit avoir un collider pour le raycast
3. Les matériaux de preview doivent avoir le mode de rendu transparent
4. Le système utilise des coordonnées hexagonales axiales (flat-top)
5. **TileOcclusionCulling** est désactivé mais le script est conservé pour usage futur

## Dépannage

**La preview ne s'affiche pas ?**
- Vérifiez que le TileSelector a des tiles assignées
- Vérifiez que le matériau de preview est assigné et transparent

**Les tiles ne se placent pas ?**
- Vérifiez que toutes les références sont assignées
- Vérifiez que les prefabs de tiles ont des colliders

**Le scroll ne fonctionne pas ?**
- Le scroll simple change la tile
- Ctrl+Scroll change la taille du brush
- Assurez-vous de ne pas être en conflit avec d'autres scripts

**Vous voulez activer l'occlusion culling ?**
- Le script TileOcclusionCulling est disponible dans le projet
- Attacher le script à un GameObject
- Configurer les paramètres et assigner la caméra
- Appeler RegisterTile() pour chaque tile créée

