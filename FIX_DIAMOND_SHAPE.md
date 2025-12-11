# How to Fix the Diamond Shape - Quick Guide

## The Problem
Your screenshot shows the tiles are arranged in a diamond/rotated square pattern instead of an axis-aligned rectangle.

## The Solution
Try each of these 4 layout options in the Inspector until you get a proper rectangle:

### 1. OddROffset (Current Default)
- Creates horizontal rectangle
- Odd rows shift right
- **Try this first**

### 2. EvenROffset
- Creates horizontal rectangle (alternative)
- Even rows shift right
- **Try if OddROffset doesn't work**

### 3. OddQOffset ⭐ RECOMMENDED
- Creates vertical rectangle
- Odd columns shift down
- **Most likely to fix your diamond issue**

### 4. EvenQOffset ⭐ RECOMMENDED
- Creates vertical rectangle (alternative)
- Even columns shift down
- **Also likely to work**

## Steps to Test:

1. **Stop the scene** if it's running
2. **Select MapGenerator** GameObject
3. **Change "Layout Type"** dropdown to try each option:
   - Start with **OddQOffset** 
   - Then try **EvenQOffset**
   - If those don't work, try **EvenROffset**
4. **Run the scene** to see the result
5. **Repeat** until you get a proper rectangle

## What You Should See:

❌ **Diamond (Wrong)**:
```
    ⬢
   ⬢ ⬢
  ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢
⬢ ⬢ ⬢ ⬢ ⬢
```

✅ **Rectangle (Correct)**:
```
⬢ ⬢ ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢ ⬢
⬢ ⬢ ⬢ ⬢ ⬢
 ⬢ ⬢ ⬢ ⬢ ⬢
```

## Pro Tip:
Based on your screenshot showing a 45° rotated diamond, **try OddQOffset or EvenQOffset first** - these create rectangles oriented differently and should fix your issue!

