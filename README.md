# TODO

## Things to do before 1.0.0.0
* Finish ribbon and toolbar.
* Update Commands.md

## House-keeping
* Fix up tests
* Combine math commands into one with option.

### General
* Error detection method.
* Toolbar & Menu
* Settings
* Link CogoPoint Labels (for moving)

### Setout Tool
* Create block with point and text entities.
* Export to CSV or Excel.

### Linework Connector
* Linework tool to replace SmartDraft -- in progress

### Point Reports
> Generates detailed point reports with user controlled columns.  Includes Lat/Lon and alignment station/offset fields. 
Save entire report as HTML or data table only to CSV, DBF, XML, etc.  Also include extended point data columns (Civil3D UDP)!
* point report /w alignments
* station format
* edit multiple
* hide non-adjacent
* zoom to point
* option to add cut/fill between surfaces
* Sorting order to report screen
* decimal places on report screen
* export to csv
* Create point at extension distance on grade between 2 points.
* Quickly add/remove points by description match, elevation range, inside linear object or window, number range, etc.

### Compare Two Surfaces
> The SPPointElevationsFromSurfaces command allows the user to show point tables with the elevations from 2 surfaces, in addition to the point elevation.
After starting the SPPointElevationsFromSurfaces command, you will be presented with a form from which you select the points, or PointGroups, to compare,
select the 2 surfaces to use, and the 2 UserDefinedProperties (these must be pre-defined as elevation types).
Once the selection is complete the selected points will have the respective UDP's assigned the surface elevations. You can now assign a label style to
the points which displays those UDP's, use the DisplayPoints Sincpac tool to create a report, or export the points out to a text file. If you need to
also include station/offset information, use the DL_Points tool to link the points to alignment(s).


## Window Visibility States

```
   <StackPanel>
        <StackPanel.Style>
            <Style TargetType="StackPanel">
                <Setter Property="Visibility" Value="Collapsed" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Path=ViewState}" Value="{x:Static local:ViewState.State1}">
                        <Setter Property="Visibility" Value="Visible" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </StackPanel.Style>
    </StackPanel>
```
# Notes

> When you apply the CommandMethod attribute to a non-static method, AutoCAD's
managed runtime will create multiple instances of your class, one for each
document that you invoke the command in. Those instances are not created until
you invoke the command.

> The problem you have is that AutoCAD's managed runtime also creates an instance
of the class with the IExtensionApplication attribute on it as well, so multiple
instances of your class are getting created, which obviously wasn't your
intention.

> For one thing, it's not wise to implement commands in the same class that
implements IExtensionApplication, but if you really wanted to do that, you could
if you make the command handler methods static. If you're not sure about the
difference between static and non-static (or 'instance') methods, get a hold of
some basic C# learning materials, and try to become more familiar with the basic
concepts that underlie the tools you're working with.

> As can be seen, it is not necessary to commit transactions for object reading access, 
but for performance sake, we had better commit such transactions as well. In addition, 
as can be seen again from the output, it took about twice long for the second command 
to get the same work done. It is clear now that committing transactions is more efficient 
than aborting them even for reading operations.

> Therefore, another good practice comes out, always committing transactions after they started.

> For an instance command method, the method's enclosing type is instantiated separately for each open document. 
> This means that each document gets a private copy of the command's instance data. Thus there is no danger 
> of overwriting document-specific data when the user switches documents. If an instance method needs to share 
> data globally, it can do so by declaring static or Shared member variables. For a static command method, 
> the managed wrapper runtime module does not need to instantiate the enclosing type.
> A single copy of the method's data is used, regardless of the document context. Static commands normally do 
> not use per-document data and do not require special consideration for MDI mode.
