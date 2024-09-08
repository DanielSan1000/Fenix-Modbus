
## Open Fenix Manager

There are several ways to start the program. The first method is to find a shortcut on the desktop. Shortcut indicates actual version of the program Fenix. Application will start after the double-click on the shortcut . Shortcuts looks like picture below

***Desktop***

![Image](<lib/NewItem36.png>)

The next way is find shortcut on Menu Start and double click on it then application will start. Picture below represent apparent in 

***Menu Start***.

![Image](<lib/NewItem37.png>)

Other way is go directly to installation location on hard drive and double click on the Fenix.exe Windows 7, 8, 8.1, 10

***\<System>\\Program Files (x86)\\D.G\\Fenix.exe***

After start of the application, you will see window shown below. On start Fenix will check updates on server. If your application won't be actual

Fenix will tell you about it. If there are any updates on Status Strip you will see label Completed. There is a possibility check updates manually, you must click to Help->Check For Updates. To download you must go to Website: **http://sourceforge.net/projects/fenixmodbus/.**

![Image](<lib/NewItem38.png>)

Window below represent manual updates checking.

![Image](<lib/NewItem39.png>)

### Add New Project

Click **New**, you will see window below. Fill all fields. Fields with **[*]** are required. If you want to use HttpServer,you need to leave c option ***[Http Template -> Add files]*** checked,then click save.

![Image](<lib/m\_AddProject.png>)

Fenix will ask you to give path location for project. Fenix will create appropriate files structure (if you selected <Add Files> option). Software has representation of this structure in 

***[Solution Explorer]***

![Image](<lib/NewItem40.png>)

If you just created project,and you want to change any parameters, you can easily change them using 

***[Properties]***

![Image](<lib/m\_ProjectProp.png>)


### Add Connection

Before you start you must check drivers are installed in your system. Click ***[Tools -\> Driver Configuration]*** and you will see the window below:

You will find there information about&nbsp; already installed drivers in the system. Name, Version, Path. You can use this window to mange your drivers. Button reset is used&nbsp;

to force Fenix to&nbsp; search for driver in path location during startup and rewrite configuration file for driver. Fenix use file&nbsp; \***GlobalConfiguration.xml\*** for&nbsp;

loading driver to memory located in installation folder.

![Image](<lib/NewItem32.png>)

If required driver is installed in the system you can add new connection to system. Select **\<Project01\>** from **\*Solution Explorer\*,** click the right mouse button and select **\[Add Connection\]. Then** fill all fields and parameters and click save

![Image](<lib/m\_AddConnection.png>)


### Add Tags

Select **\<Device\>** node click on the right button of the mouse and click **[Add Tag\]**. You will see Window (picture below). Checked **[Add Range]** to add few tags and additionally fill how many you need.

![Image](<lib/m\_AddTags.png>)

We change name of the tags. To do this you need to click twice every tags and go to **Properties** and there change the name. New names (Temp1, Temp2, Temp3, Temp4, Temp5)

![Image](<lib/m\_TagsChangeNames.png>)


### Add InternalTag

Internal Tags has been created for additional work. These tags are not related with communication, but only exists in Fenix memory. Only scripts can access to this tags. To create it select node **<Internal Tags>** and follow picture below:

![Image](<lib/m\_AddIntTags.png>)


### Use VBScript script for InternalTag

Before you start to create script, you must add timer, which will be invoke script to certain period cyclically **[02 Time [ms]].** To add timer double click to **<Internal Tags>** node and go to **Properties** and follow picture below:

![Image](<lib/m\_AddTimerToScript.png>)

Add script **(GetTag("Temp1") +GetTag("Temp2")+GetTag("Temp3")+GetTag("Temp4")+GetTag("Temp5")) / 5** you must double click to node **\<Average\>** and go to **\*Properties\*** and follow picture below:

![Image](<lib/m\_AddIntTagScript.png>)


### C# Logging Script

At the begging you must check, which triggers are installed in system. Follow the procedure below. You have possibility to define your own timer.

![Image](<lib/m\_CTimers.png>)

Next step is bind you timer to script file. Timer will invoke Cycle() method according to defined time. Method Start() and Stop() are not related with the timer.Follow on the procedure below:

![Image](<lib/m\_CTimerSelect.png>)

If you selected right Timer, you can define logging script.To temporary blocking script you can use option **[Enable : false]** in ***Properties***. To open script file follow procedure below:

![Image](<lib/m\_CEditScriot.png>)


```csharp
using System;
using ProjectDataLib;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Script : ScriptModel
{
    StreamWriter file;

    public override void Start()
    {
        // Open stream
        file = new StreamWriter("C://logg.csv");

        // Output Windows
        Write("Logg Started: " + DateTime.Now.ToString());

        // Create Header
        file.WriteLine(@"Date;Tag10;Tag11");
    }

    // Invoke cyclically (timer setup)
    public override void Cycle()
    {
        // Output Window
        Write("Cycle Record: " + DateTime.Now.ToString());

        // Save data to file
        file.WriteLine(DateTime.Now.ToString() + ";" + GetTag("Tag10") + ";" + GetTag("Tag11"));
    }

    // Invoke one time during driver stop
    public override void Stop()
    {
        // Output Windows
        Write("Logg Stopped: " + DateTime.Now.ToString());

        // Save and Close stream
        file.Close();
        file = null;
    }
}

```

This script will be logging all Tags int to **C://logg.csv.** Script will start when you start any editor (TableView, ChartView).


![Image](<lib/m\_ScriptViz.png>)

Model script has implemented Write Method. This method is created due to communicate script with the user. All information you can watch in **\*Output\*** window.

### Start Fetching Data

1. Select node to show observation&nbsp; level
2. Choose appropriate editor. Editor should open
3. Start / Stop Communication (selected node)
4. Start / Stop Communication (all drivers)
5. TableView
6. ChartView

![Image](<lib/NewItem41.png>)