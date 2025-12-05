# Guide Complet d'Intégration 3D

## Vue d'ensemble
Le système de jeu a été converti de 2D à 3D avec support complet de la profondeur (axe Z). Ce guide couvre toutes les modifications nécessaires.

## Modifications effectuées

### 1. Pathfinding 3D ✅
**Fichier:** `HexPathfinding3D.cs`

- Traite la hauteur comme dimension Z
- 8 voisins (6 horizontaux + 2 verticaux)
- Coûts de mouvement réalistes
- Heuristique 3D optimisée

**Utilisation:**
```csharp
int startHeight = HeightManager.instance.GetHeightIndex(startTile);
int goalHeight = HeightManager.instance.GetHeightIndex(goalTile);
List<Vector2> path = HexPathfinding3D.instance.FindPath(start, goal, startHeight, goalHeight);
```

### 2. Mouvement 3D ✅
**Fichier:** `GhostMovement.cs` (modifié)

- Utilise `Vector3` au lieu de `Vector2`
- Mouvement fluide sur tous les axes
- Distance 3D correctement calculée

**Changements:**
- `Vector2 currentTarget` → `Vector3 currentTarget`
- `Vector2.Distance()` → `Vector3.Distance()`
- `Vector2.MoveTowards()` → `Vector3.MoveTowards()`

### 3. Contrôle IA 3D ✅
**Fichier:** `GhostIA_3D.cs` (optionnel)

- Récupère les hauteurs de départ/destination
- Utilise `HexPathfinding3D`
- Passe les hauteurs au mouvement

## Étapes d'intégration

### Étape 1: Vérifier les fichiers
```
Assets/Scripts/Pathfinding/
├── HexPathfinding2D.cs (ancien, peut être gardé)
├── HexPathfinding3D.cs ✅ (nouveau)
└── HexPathfinding3D_Examples.cs (exemples)

Assets/Scripts/Entities/
├── GhostMovement.cs ✅ (modifié pour 3D)
├── GhostIA.cs (ancien, à adapter)
└── GhostIA_3D.cs (optionnel, nouveau)
```

### Étape 2: Adapter GhostIA.cs

Si vous gardez `GhostIA.cs`, modifiez la méthode `ComputePath()`:

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

### Étape 3: Tester le mouvement 3D

1. Ouvrir une scène avec un fantôme
2. Placer le fantôme sur une hauteur
3. Lui donner un ordre de déplacement à une autre hauteur
4. Vérifier que:
   - ✅ Il se déplace horizontalement ET verticalement
   - ✅ Le mouvement est fluide
   - ✅ La vitesse est cohérente
   - ✅ Pas de saccades ou de corrections Z

### Étape 4: Vérifier les autres systèmes

Chercher tous les appels à:
- `Vector2.Distance()` → Vérifier si doit être `Vector3.Distance()`
- `Vector2.MoveTowards()` → Vérifier si doit être `Vector3.MoveTowards()`
- `HexPathfinding2D.instance.FindPath()` → Adapter pour `HexPathfinding3D`

## Checklist de migration

### Pathfinding
- [ ] `HexPathfinding3D.cs` créé et fonctionnel
- [ ] `GhostIA.cs` ou `GhostIA_3D.cs` utilise `HexPathfinding3D`
- [ ] Les hauteurs sont correctement récupérées
- [ ] Les chemins incluent les mouvements verticaux

### Mouvement
- [ ] `GhostMovement.cs` utilise `Vector3`
- [ ] `Vector3.Distance()` utilisé partout
- [ ] `Vector3.MoveTowards()` utilisé partout
- [ ] Pas de corrections Z après le mouvement

### Gameplay
- [ ] Les fantômes se déplacent correctement en 3D
- [ ] Les animations fonctionnent avec les 3 axes
- [ ] Les collisions sont correctes
- [ ] Les performances sont acceptables

## Optimisations optionnelles

### 1. Cache de hauteurs
```csharp
private Dictionary<Vector3Int, int> heightCache = new();

int GetCachedHeight(Vector3Int cell)
{
    if (!heightCache.TryGetValue(cell, out int height))
    {
        height = HeightManager.instance.GetHeightIndex(TilemapManager.instance.GetTile(cell));
        heightCache[cell] = height;
    }
    return height;
}
```

### 2. Pathfinding asynchrone
```csharp
private Coroutine pathfindingCoroutine;

private IEnumerator ComputePathAsync()
{
    // Calcul du chemin sur plusieurs frames
    yield return null;
    currentPath = HexPathfinding3D.instance.FindPath(...);
}
```

### 3. Interpolation de hauteur
```csharp
private void Update()
{
    Vector3 currentPosition = transform.position;
    Vector3 targetPos = new Vector3(currentTarget.x, currentTarget.y, currentTarget.z);
    
    // Interpolation 3D complète
    transform.position = Vector3.Lerp(currentPosition, targetPos, Time.deltaTime * speed);
}
```

## Dépannage

### Problème: Le fantôme ne monte/descend pas
**Solution:** Vérifier que `GhostMovement.GoTo()` reçoit la bonne hauteur

### Problème: Mouvement saccadé
**Solution:** Vérifier que `TickSystem.instance.tickInterval` est correct

### Problème: Pathfinding ne trouve pas de chemin
**Solution:** Vérifier que les hauteurs de départ/destination sont correctes

### Problème: Performance dégradée
**Solution:** Implémenter un cache de hauteurs ou réduire la fréquence des pathfindings

## Fichiers de référence

- `HexPathfinding3D_Examples.cs` - 8 exemples d'utilisation
- `PATHFINDING_3D_MODIFICATIONS.md` - Documentation technique
- `MOVEMENT_3D_CHANGES.md` - Détails des modifications de mouvement

## Prochaines étapes

1. ✅ Pathfinding 3D implémenté
2. ✅ Mouvement 3D implémenté
3. ⏳ Adapter tous les appels au pathfinding
4. ⏳ Tester complètement le système
5. ⏳ Optimisations si nécessaire
6. ⏳ Documenter les changements pour l'équipe
