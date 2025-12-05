# Guide d'intégration du Pathfinding 3D

## Étapes rapides

### Étape 1: Vérifier les fichiers créés
- ✅ `Assets/Scripts/Pathfinding/HexPathfinding3D.cs` - Nouveau pathfinding 3D
- ✅ `Assets/Scripts/Entities/GhostIA_3D.cs` - Version mise à jour de GhostIA

### Étape 2: Remplacer le système existant

**Option A - Remplacement complet (recommandé):**

1. Supprimer `HexPathfinding2D.cs`
2. Renommer `HexPathfinding3D.cs` → `HexPathfinding2D.cs`
3. Supprimer `GhostIA.cs`
4. Renommer `GhostIA_3D.cs` → `GhostIA.cs`

**Option B - Coexistence:**

1. Garder les deux systèmes
2. Utiliser `HexPathfinding3D` pour les nouveaux pathfindings
3. Mettre à jour les appels: `HexPathfinding3D.instance.FindPath(...)`

### Étape 3: Mettre à jour les appels au pathfinding

**Ancien code (2D):**
```csharp
List<Vector2> path = HexPathfinding2D.instance.FindPath(startPos, goalPos);
```

**Nouveau code (3D):**
```csharp
int startHeight = HeightManager.instance.GetHeightIndex(tilemapManager.GetTile(startCell));
int goalHeight = HeightManager.instance.GetHeightIndex(tilemapManager.GetTile(goalCell));
List<Vector2> path = HexPathfinding3D.instance.FindPath(startPos, goalPos, startHeight, goalHeight);
```

## Vérification de l'intégration

### Checklist
- [ ] Les fichiers sont créés dans les bons répertoires
- [ ] Pas d'erreurs de compilation
- [ ] Les instances singleton sont correctement initialisées
- [ ] Les chemins sont calculés avec les hauteurs correctes
- [ ] Les fantômes se déplacent correctement en 3D

### Tests recommandés

1. **Test basique:**
   - Placer un fantôme sur une hauteur
   - Lui demander d'aller à une autre hauteur
   - Vérifier que le chemin monte/descend correctement

2. **Test de performance:**
   - Créer plusieurs fantômes
   - Vérifier que le pathfinding ne ralentit pas le jeu

3. **Test d'obstacles:**
   - Placer des obstacles à différentes hauteurs
   - Vérifier que les chemins les contournent

## Dépannage

### Erreur: "HexPathfinding3D not found"
- Vérifier que `HexPathfinding3D.cs` est dans `Assets/Scripts/Pathfinding/`
- Vérifier que la classe a `public static HexPathfinding3D instance`

### Erreur: "GetHeightIndex returns null"
- Vérifier que `HeightManager.instance` est initialisé
- Vérifier que les tiles ont des hauteurs assignées

### Les chemins ne montent pas/descendent pas
- Vérifier que `startHeight` et `goalHeight` sont corrects
- Vérifier que `IsHeightWalkable()` permet les transitions

### Performance dégradée
- Réduire la fréquence des recalculs de chemin
- Implémenter un cache de chemins
- Utiliser des waypoints au lieu de recalculer à chaque tick

## Différences clés avec le système 2D

| Aspect | 2D | 3D |
|--------|----|----|
| Dimensions | X, Y | X, Y, Z (hauteur) |
| Voisins | 6 | 8 |
| Coût horizontal | 1.0 | 1.0 |
| Coût vertical | N/A | 1.5+ |
| Heuristique | Manhattan 2D | Manhattan 2D + hauteur |
| Paramètres FindPath | (start, goal) | (start, goal, startH, goalH) |

## Rollback

Si vous devez revenir au système 2D:

1. Supprimer `HexPathfinding3D.cs`
2. Supprimer `GhostIA_3D.cs`
3. Restaurer `HexPathfinding2D.cs` et `GhostIA.cs` depuis Git

```bash
git checkout Assets/Scripts/Pathfinding/HexPathfinding2D.cs
git checkout Assets/Scripts/Entities/GhostIA.cs
```
