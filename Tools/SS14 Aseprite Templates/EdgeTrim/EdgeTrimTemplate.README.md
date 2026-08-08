# README
These are templates by zergologist that greatly simplify working with EdgeTrim and IconSmooth sprites that have either regular and irreguluar grids.

Adapted from the IconSmooth template, where a video on how to use that can be found at the link:
https://github.com/space-wizards/space-station-14/pull/32210

If you do not feel like watching a video and/or need some extra guidance in using this tool, please refer to the guide below.

## GUIDE

### IMPORT
If you're importing old IconSmoothing sprites, paste the pieces in the 'Export' layer of the Smoothing group, making sure to use the 'Manual' editing mode for tilemaps.

### EXPORT
Copy and paste the pieces at the top under the 'Export' layer into their own 64 pixel by 64 pixel images.
    For the Smoothing pieces, follow the naming convention of 'base-smooth#' - say you're copying from the '0' square and making a carpet, the sprite's name would be 'carpet-smooth0'
    For the Trimming pieces, follow the naming convention of 'base-edge#' - say you're copying from the '4' square and making a wall, the sprite's name would be 'wall-edge4'

### EDITING
**WHEN EDITING TILEMAPS, ONLY USE MANUAL MODE; THAT ENSURES ANY CHANGES MADE GET PROPERLY APPLIED**
My advice is to make a "working layer" and start working on your sprite from there, testing against the top and bottom piece layers to see if you need to adjust where spritework gets cut.
The FunkyWall version of the template is already prepared for working on the IconSmoothing and EdgeTrimming sprites that Funky's slanted walls use, and can be referenced if need be.
While you could work directly on the Piece layers, just be careful to clean up the pieces for the final export.

### EdeTrimEx LAYER
This layer explains why there isn't a grand total of 19 additional sprites to make (and export) when working with EdgeTrimming. Simple as.