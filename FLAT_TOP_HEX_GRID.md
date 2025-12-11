# Flat-Top Hexagon Grid Generator ✅

## Fixed for Flat-Top Hexagons!

The MapGenerator is now correctly configured for **flat-top hexagon tiles** (hexagons with a flat edge on top/bottom).

## How to Use

1. **In the Inspector**, select your MapGenerator GameObject
2. **Set the Layout Type** to one of these options:

### 🔹 OffsetRows (Default - Recommended)
- **Best for**: Rectangular grid aligned horizontally
- **Description**: Each row is offset from the previous one
- **Result**: Creates a proper rectangle that extends horizontally
- **Formula**: `q = col - row/2`, `r = row`

### 🔹 OffsetColumns
- **Best for**: Rectangular grid aligned vertically  
- **Description**: Each column is offset from the previous one
- **Result**: Creates a proper rectangle that extends vertically
- **Formula**: `q = col`, `r = row - col/2`

### 🔹 Simple (For Comparison)
- **Result**: Creates a diamond/rhombus shape (the "wrong" one)
- **Use this**: To see the difference and confirm the others work correctly

## Visual Representation

### OffsetRows (Horizontal Rectangle):
```
⬢ ⬢ ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢ ⬢
⬢ ⬢ ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢ ⬢
```
Rows alternate left/right - creates horizontal rectangle

### OffsetColumns (Vertical Rectangle):
```
⬢  ⬢  ⬢
⬢  ⬢  ⬢
 ⬢  ⬢  ⬢
 ⬢  ⬢  ⬢
⬢  ⬢  ⬢
```
Columns alternate up/down - creates vertical rectangle

### Simple (Diamond - Wrong):
```
    ⬢
   ⬢ ⬢
  ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢
⬢ ⬢ ⬢ ⬢ ⬢
```
No offset - creates diamond shape

## Quick Test

1. **Run your scene** (auto-generates on Start)
2. **See a proper rectangle?** ✅ You're done!
3. **Still looks rotated?** Try switching between `OffsetRows` and `OffsetColumns`

## Technical Details

For **flat-top hexagons**, the coordinate system works like this:
- Moving in **q-direction** → moves along world X-axis
- Moving in **r-direction** → moves diagonally (X and Z)
- **OffsetRows** formula subtracts `row/2` from q to compensate for the diagonal movement
- **OffsetColumns** formula subtracts `col/2` from r to compensate for the diagonal movement

This creates the rectangular "brick pattern" you expect from hexagonal grids.

## Current Settings
- **Width**: 10 tiles (adjust in Inspector)
- **Height**: 10 tiles (adjust in Inspector)
- **Layout**: OffsetRows (change if needed)
- **Height Level**: 0 (all tiles at same height)

**The grid should now be a proper rectangle, not a diamond!** 🎯

