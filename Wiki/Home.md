# Welcome

The Fenix is a software that has been designed to communicate with the industry devices. Currently Fenix supports the following drivers and features:

* Modbus Master RTU
* Modbus Master ASCII
* Modbus Master TCP / IP
* Siemens S7300/400
* WWW Server (HTML / JSON)

# Links

**[FenixManager Documentation](https://github.com/DanielSan1000/Fenix-Modbus/wiki/FenixManager)**<br>
**[FenixServer Documentation](https://github.com/DanielSan1000/Fenix-Modbus/wiki/FenixServer)**<br>
**[Editors Documentation](https://github.com/DanielSan1000/Fenix-Modbus/wiki/FenixServer)**

# History

Fenix Modbus 2.6.3
- Fixed multi-window monitoring. Tags are better managed. Communication is related to the number of open windows.
- Communication is optimized in relation to multi-window monitoring.
- Added handling of deleting project elements through WebServer.
- Added buttons in TableView for Boolean.
- Possibility of editing in TableView.

Fenix Modbus 2.6.4
- Detected an issue with communication stopping after entering SetValue. Fixed.

Fenix Modbus 2.6.5
- After changing the value in TableView, the cell selection indicator goes to the first cell. Fixed.
- Fixed the inability to change secAddress in TableView.
- TableView added the ability to parameterize devices.

Fenix 2.6.6
- Added docking of windows.
- Removed TrayIcon.
- Removed Alignment Window.
- Removed Forms (tableView, viewLogger, chartView) and replaced with new parent-type windows.
- Fenix Manager is an MDI window.
- WebServer changes its name and will be an external application that opens .psf files using WPF technology.
- Menu stack in sub-windows will have smaller icons.
- Added current version label to the window.

Fenix 2.6.7
- Introduced global names for tags. Better selection for HttpServer.
- Added code. When closing the Properties Manager and double-clicking on the treeView, there was an error. Added code to open propManager.
- Added option to reset assemblyPath in AutoSearchDriver.
- Changed Icon for ServerHttp.

Fenix 2.6.8
- Removed from the interfaceFolder structure.

Fenix 2.6.8
- Fixed scripts.
- Added internals Tag.
- Formatting displayed numbers for WebServer.
- Tag - added linear scaling.

Fenix 2.6.9
- Added file handling through WebServer.
- InternalsTag.

Fenix 3.0.0
- Output driver is an independent window.

Fenix 3.0.1
- Added the ability to format the tag value.
- Option to enable output driver.
- Fixed various errors.

Fenix 3.0.2
- Fixed problems with creating a new file.
- Added renaming of the file when changing data in the program.
- Added appropriate icons for files.
- Script engine.

Fenix 3.0.3
- Optimization.

Fenix 3.0.4
- Added a new ZedGraph chart.
- Code refactoring.

Fenix 3.0.5
- Fixed errors.
- Work on the Scintilla editor.
- Improved parameter work.

Fenix 3.0.6
- Work on the web server.

Fenix 3.0.7
- Change name description for Device Object - "Folder Name" to "Device Name".
- Changes inside drivers (TCP/ RTU / ASCII) for better management of requests.
- Change Window Management (Window starts at the bottom when opened).
- Changed Code editor to AvalonEdit.
- Possibility to start another editor during communication.
- Added reConfig() method to driver to cover changing Tags parameters online.
- Deleted Stack button from Output.
- Added Help file.

Fenix 3.0.8
- Changed Framework version to .NET 4.6.
- Changed algorithm for searching double Tags names during adding.
- Repaired problems with sbyte type.
- Code Editor added the possibility to save selected text to clipboard as HTML.
- Code editor during startup chooses the right highlighting (JavaScript).
- ASCII formatting.

Fenix 3.0.9
- Repaired bugs (During adding range tags, the name was not assigned).
- Added new Siemens S7300/400 driver.
- New design for TableView (WPF).
- Simplified interface (removed some features).
- Moved Start / Stop to Fenix Manager.

Fenix 3.1.0
- Added saving windows layout.
- Problems with reading data higher than 16,000.
- Repaired bugs.

Fenix 3.1.1
- Everything is WPF.
- Lots of new things.

Fenix 3.1.2
- Bugs.
- Database for Chart.
- Bugs.

Fenix 3.1.6
- Used Metro UI.

Fenix 3.1.7
- Removed problem with names in Tag and InTag windows.
- XML file and start removing *.psf.
- Removed automatic save during close.
- Removed bugs related to ChartView and saving parameters.
- TableView did not show value for Tag when the row was selected. Repaired.
- Added a small rectangle in the TreeView with color to simplify identification.
- Problem with the database during Tag script usage. Repaired.
- Update OxyPlot 1.0.0.2175 -> 1.0.0.2176.
- Update SC-Script.bin 3.12.2.1 -> 3.13.2.
- Update Newtonsoft.Json 9.0.1-beta -> 9.0.1.
- Update System.Data.Sql 1.0.101 -> 1.0.102.

Fenix 3.1.8
- Database thread strongly overloaded the main thread. UI was hanging. Repaired.
- Psf files not supported.
- ZedGraph removed from the project.

Fenix 3.1.9
- Fenix didn't want to write data in S7 DB blocks. Repaired.
- During creating Connection default parameters were given (not from windows). Repaired.
- Local Help file was replaced by Website help.
- Siemens S7 driver, all events were connected to CommunicationView.
- CommunicationView added a driver name column.
- CommunicationView added the amount of records in GridView.
- CommunicationView added save to CSV possibility.
- CommunicationView added save to clipboard possibility.
- CommunicationView filtering data possibility (basic).
- Database CSV export problem with a comma for values. Used dot for values only.
- Update OxyPlot 1.0.0.2176 -> 1.0.0.2182.
- Update MahApps.Metro 1.3.0.166 -> 1.3.0.188.
- Update SC-Script.bin 3.13.2 -> 3.14.
- Update TaskScheduler 2.5.20 -> 2.5.21.

Fenix 3.2.0
- CommunicationView for Diff Time used "Integer greater than" filter.
- Update DataGridExtension 1.0.33 -> 1.0.44.
- Update MahApps.Metro 1.3.0 -> 1.4.0.
- Update SQLite 1.0.102 -> 1.0.103.
- Update OxyPlot 1.0.0.2182 -> 2.0.0.0933.
- Update Newtonsoft.Json 9.0.1 -> 9.0.2.
- Added a folder with logs for catching information about unhandled exceptions [Fenix Directory]\Logs.

Fenix 3.3.0
- Move to .NET framework 4.8
- Update all libraries.
- Add better colorizing selecting in tags range.