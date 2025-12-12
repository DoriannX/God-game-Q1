# Modifications du Pathfinding pour la 3D

## Résumé
Le système de pathfinding a été modifié pour supporter la hauteur comme une vraie dimension 3D, au lieu de simplement la vérifier comme contrainte.

## Fichiers créés

### 1. `HexPathfinding3D.cs`
Nouvelle classe de pathfinding qui traite la hauteur comme une dimension:

**Changements clés:**
- Utilise `Vector3Int` avec Z comme hauteur (au lieu de Z=0)
- Méthode `GetNeighbors3D()`: Retourne 8 voisins (6 horizontaux + 2 verticaux)
- Méthode `GetMovementCost()`: Calcule le coût en fonction du type de mouvement
  - Mouvement horizontal: coût = 1
  - Mouvement vertical: coût = 1 + (hauteur_diff * 0.5)
- Méthode `Heuristic3D()`: Heuristique Manhattan 2D + distance verticale pondérée
- Signature: `FindPath(Vector2 startWorld, Vector2 goalWorld, int startHeight, int goalHeight)`

**Avantages:**
- Pathfinding vrai 3D avec optimisation des chemins verticaux
- Coûts de mouvement réalistes (escalade plus coûteuse)
- Meilleure gestion des changements d'altitude

### 2. `GhostIA_3D.cs`
Version mise à jour de GhostIA pour utiliser le nouveau pathfinding 3D:

**Changements:**
- Méthode `ComputePath()` modifiée pour:
  - Récupérer la hauteur actuelle du fantôme
  - Récupérer la hauteur de la destination
  - Appeler `HexPathfinding3D.instance.FindPath()` avec les hauteurs

```csharp
private void ComputePath()
{
    Vector3Int currentCell = TilemapManager.instance.WorldToHexAxial(transform.position);
    Vector3Int targetCell = TilemapManager.instance.WorldToHexAxial(targetPosition);
    
    int startHeight = HeightManager.instance.GetHeightIndex(TilemapManager.instance.GetTile(currentCell));
    int goalHeight = HeightManager.instance.GetHeightIndex(TilemapManager.instance.GetTile(targetCell));
    
    currentPath = HexPathfinding3D.instance.FindPath(transform.position, targetPosition, startHeight, goalHeight);
}
```

## Migration

Pour utiliser le nouveau système:

1. **Option 1 - Remplacer l'existant:**
   - Supprimer `HexPathfinding2D.cs`
   - Renommer `HexPathfinding3D.cs` en `HexPathfinding2D.cs`
   - Remplacer `GhostIA.cs` par `GhostIA_3D.cs`

2. **Option 2 - Coexistence:**
   - Garder les deux systèmes
   - Utiliser `HexPathfinding3D` pour les nouveaux pathfindings
   - Utiliser `HexPathfinding2D` pour l'héritage

## Comportement du pathfinding 3D

### Exploration des voisins
- **6 voisins horizontaux**: Les 6 hexagones adjacents à la même hauteur
- **2 voisins verticaux**: Montée/descente d'un niveau de hauteur

### Coûts de mouvement
```
Horizontal (même hauteur): 1.0
Vertical (±1 hauteur): 1.5
Vertical (±2 hauteur): 2.0
```

### Heuristique
```
h = Manhattan_distance_XY + (hauteur_diff * 0.5)
```

## Considérations de performance

- Buffer de voisins augmenté de 6 à 8 éléments
- Calcul de coût légèrement plus complexe
- Heuristique 3D plus précise = exploration moins large

## Prochaines étapes optionnelles

1. **Optimisations:**
   - Ajouter un cache pour les calculs de hauteur
   - Implémenter un système de "jump points" pour les hexagones
   - Ajouter des zones interdites (obstacles 3D)

2. **Améliorations:**
   - Supporter les pentes (transitions de hauteur progressives)
   - Ajouter des coûts différents par type de terrain
   - Implémenter des tunnels/ponts

3. **Debugging:**
   - Visualiser les chemins 3D en Gizmos
   - Afficher les coûts de chaque segment
   - Logger les explorations pour profiling
