# TODO

* Error detection method.
* Add line break to text.
* Overwrite Text Command.
* Copy raw description or full description to text entity
* Add load/save to traverse palette
* Add Curves to traverse?
* Selected objects to traverse window
* XYZ point label (for autocad mainly)
* Insert picture(s) tool
* IMessageBoxService
* Abstract PaletteSet
* Settings (Properties.Settings.Default.MyConnection)
* Linework tool to replace SmartDraft -- in progress
* point report /w alingments
* Rename ShowCogoPointViewer command
* Typing a palette command when visible hides it.
* Improve angle calculator
* Think about making it so palettes open up individually. might solve a lot of issues with events.
> Generates detailed point reports with user controlled columns.  Includes Lat/Lon and alignment station/offset fields. 
Save entire report as HTML or data tabel only to CSV, DBF, XML, etc.  Also include extended point data columns (Civil3D UDP)!

* Create point at extension distance on grade between 2 points.
* Quickly add/remove points by description match, elevation range, inside linear object or window, number range, etc.

> The SPPointElevationsFromSurfaces command allows the user to show point tables with the elevations from 2 surfaces, in addition to the point elevation.
After starting the SPPointElevationsFromSurfaces command, you will be presented with a form from which you select the points, or PointGroups, to compare, 
select the 2 surfaces to use, and the 2 UserDefinedProperties (these must be pre-defined as elevation types).
Once the selection is complete the selected points will have the respective UDP's assigned the surface elevations. You can now assign a label style to 
the points which displays those UDP's, use the DisplayPoints Sincpac tool to create a report, or export the points out to a text file. If you need to 
also include station/offset information, use the DL_Points tool to link the points to alignment(s).


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
some basic C# learning materials, and try to become more familar with the basic
concepts that underly the tools you're working with.

> As can be seen, it is not necessary to commit transactions for object reading access, 
but for performance sake, we had better commit such transactions as well. In addition, 
as can be seen again from the output, it took about twice long for the second command 
to get the same work done. It is clear now that committing transactions is more efficient 
than aborting them even for reading operations.

> Therefore, another good practice comes out, always committing transactions after they started.
