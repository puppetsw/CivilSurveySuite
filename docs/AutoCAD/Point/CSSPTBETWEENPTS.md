# CSSPTBETWEENPTS

## Description

Creates a point between two points at a given distance.

### Remarks

The main difference between this command and CSSPTLINE is that this command displays a chainage on screen between the picked points.

Distances longer than the distance between the two points can be added. For example, the distance between the two points is 50m. A distance of 55m can be entered.

Negative distances can also be entered.

Press `ENTER` or `ESC` to end the command.

## Usage

* Run command (CSSPTBETWEENPTS)
* Pick first point
* Pick second point
* Enter distance(s)

## Example Output

```
Command: CSSPTBETWEENPTS
3DS> First point:
3DS> Second point:
3DS> Total distance: 77.064
3DS> Enter distance: 5
3DS> Enter distance: 10
3DS> Enter distance: 15
3DS> Enter distance: 40
3DS> Enter distance: 100
3DS> Enter distance: -5
3DS> Enter distance:
```
