# AutoCAD Commands

## Points
| Command | Description |
| --- | ----------- |
| 3DSPtProd | Creates a point at the production of a line, given a distance.
| 3DSPtOffsetLn | Creates a point at the intersection of two offset lines. |
| 3DSPtBrgDist | Creates a point at a given bearing and distance. |
| 3DSPtIntBrg | Creates a point at the intersection of two bearings. |
| 3DSPtIntDist | Creates a point at the intersection of two distances. |
| 3DSPtIntBrd | Creates a point at the intersection of a distance and a bearing. |
| 3DSPtBetweenPts | Creates a point in-between points. |
| 3DSPtOffsetBetweenPts | Creates a point between points at an offset. |
| 3DSPtDistSlope | Creates a point at a bearing and slope. |
| 3DSPtIntFour | Creates a point at the intersection of four points. |
| 3DSPtSlope | Creates a point at a location with the calculated slope. |
| 3DSPtLine | Creates a point between two points at a distance. |

## Lines
| Command | Description |
| --- | ----------- |
| 3DSLnDrawLeg | Draws return legs on a given line. |
| 3DSMidPointsBetweenPolys | Draws a point at the mid point of two polylines given a location. |

## Utils
| Command | Description |
| --- | ----------- |
| 3DSInverse | Display inverse information between two points. |
| 3DSInverseOS | Display inverse information on-screen between two points. |
| 3DSInverseChOff | Displays chainage and offset information, given a line and point location. |
| 3DSPtLabelIns | Creates a point at a text or mtext entity location using the Z position. |
| 3DSPtLabelInsText | Creates a point at a text or mtext entity location using the contents as the Z position. |
| 3DSTraverse | Commandline traverse utility. |

## Text
| Command | Description |
| --- | ----------- |
| 3DSTxtUpper | Converts text or mtext to upper case. |
| 3DSTxtLower | Converts text or mtext to lower case. |
| 3DSTxtSentence | Converts text or mtext to sentence case. |
| 3DSTxtPrefix | Adds a prefix to the selected text. |
| 3DSTxtSuffix | Adds a suffix to the selected text. |
| 3DSTxtRmvAlpha | Removes alpha characters from selected text entities. |
| 3DSTxtMathAdd | If text is numeric, add a number to it. |
| 3DSTxtMathSub | If text is numeric, subtract a number from it. |
| 3DSTxtMathMultiply | If text is numeric, multiply it by a number. |
| 3DSTxtMathDiv | If text is numeric, divide it by a number. |
| 3DSTxtRound | Rounds text numbers to a specified decimal place. |

## Dialogs
| Command | Description |
| --- | ----------- |
| 3DSShowAngleCalculator | Shows a simple bearing calculator. |
| 3DSShowTraverseWindow | Shows the traverse utility window. |
| 3DSShowAngleTraverseWindow | Shows the angle traverse utility. |

## Other
| Command | Description |
| --- | ----------- |
| 3DSShowDebug | Shows the debug logs. |
| 3DSShowHelp | Shows the help file. |

# Civil 3D Commands

## Points
| Command | Description |
| --- | ----------- |
| 3DSCptProd | Creates a point at the production of a line, given a distance.
| 3DSCptOffsetLn | Creates a point at the intersection of two offset lines. |
| 3DSCptBrgDist | Creates a point at a given bearing and distance. |
| 3DSCptIntBrg | Creates a point at the intersection of two bearings. |
| 3DSCptIntDist | Creates a point at the intersection of two distances. |
| 3DSCptIntBrd | Creates a point at the intersection of a distance and a bearing. |
| 3DSCptBetweenPts | Creates a point in-between points. |
| 3DSCptOffsetBetweenPts | Creates a point between points at an offset. |
| 3DSCptDistSlope | Creates a point at a bearing and slope. |
| 3DSCptIntFour | Creates a point at the intersection of four points. |
| 3DSCptLabelIns | Creates a point at a text or mtext entity location using the Z position. |
| 3DSCptLabelInsText | Creates a point at a text or mtext entity location using the contents as the Z position. |
| 3DSCptLabelsRest | Resets the label of multiple selected CogoPoints. |
| ~~3DSCptTruntAtTrees~~ | ~~Runs the Trunk at trees command.~~ |
| 3DSRawDesUpper | Converts the raw description of CogoPoints to upper case. |
| 3DSFullDesUpper | Converts the description format of CogoPoints to upper case. |
| 3DSCptMatchLblRot | Matches the angle of multiple CogoPoint labels to a line. |
| 3DSZoomToCpt | Zooms the window to a specified point number. |
| 3DSCptInverse | Inverses between two CogoPoints given point numbers. |
| 3DSStackLabels | Select multiple CogoPoint labels and this command stacks them. |
| 3DSCptUsedPts | Displays the point numbers of used CogoPoints. |
| 3DSCptSetNext | Sets the number of the next CogoPoint. |
| 3DSCptSlope | Creates a point at a location with the calculated slope. |
| 3DSCptMidBetweenPolys | Draws a CogoPoint at the mid point of two polylines given a location. |
| 3DSCptScaleElevation | Scales the CogoPoints elevation by a given amount. |
| 3DSCptFullDescriptionToText | Copys the CogoPoints full description and creates a text entity. |
| 3DSCptRawDescriptionToText | Copys the CogoPoints raw description and creates a text entity. |

## Surface
| Command | Description |
| --- | ----------- |
| 3DSSurfaceElAtPt | Gets the surface elevation at a given point. |
| 3DSSurfaceAddBreaklines | Adds selected breaklines to a surface. |
| 3DSSurfaceRemoveBreaklines | Removes selected breaklines from a surface. |
| 3DSSurfaceSelAboveBelow | Selected points that are above or below a surface. |

## Dialogs
| Command | Description |
| --- | ----------- |
| 3DSCptLabelsMove | Shows dialog to move multiple labels by specified amount. |
| 3DSShowConnectLineworkWindow | Shows the dialog window to draw linework between CogoPoints. |
| 3DSShowCogoPointEditor | Shows the CogoPoint editor dialog. |
| 3DSShowCogoPointFindReplace | Shows the CogoPoint find/replace dialog. |

## Feature Lines
| Command | Description |
| --- | ----------- |
| 3DSFLATTENFEATURELINE | Flattens selected feature lines. (sets elevation to 0 for each vertex) |

## Labels
| Command | Description |
| --- | ----------- |
| 3DSLabelMaskOff | Turns the label mask off for a selected label on a CogoPoint. |
| 3DSLabelMaskOn | Turns the label mask on for a selected label on a CogoPoint. |
| 3DSLabelLineBreak | Converts {} into a line break inside a CogoPoint description format. |
| 3DSLabelStack | Stacks CogoPoint Labels ontop of each other. |
| 3DSLabelOverride | Asks for user input to override the CogoPoint Label. |
