# Run Fenix Server

There are several ways to start the program. The first method is to find a shortcut on the desktop. Shortcut indicates

actual version of the program **Fenix**. Double-click on the shortcut then the application will start. Shortcuts looks like picture below

FENIX SERVER IS USED ONLY FOR PROJECT **READ ONLY** AND TEMPORARY CHANGES!!! ALL CHANGES MUST BE DONE IN FENIX MANAGER\!\!\!

![Image](<lib/NewItem36.png>)


The next way is find shortcut on Menu Start and double click on it then application will start. Picture below represent apparent in **\[Menu Start\]**.

![Image](<lib/NewItem37.png>)


Other way is go directly to installation location on hard drive and double click on the **FenixServer.exe**


**Windows 7, 8, 8.1, 10**

\<System Hard Drive\>\\Program Files (x86)\\D.G\\FenixServer.exe

When you start application you should see window below:

![Image](<lib/s\_MainWindow.png>)

## Open Project

Choose **\[Open\]** from menu and select created in Fenix Manger file **\*.psf** when you finished project will be load to window

![Image](<lib/s\_LoadedProject.png>)

# File Hierarchy

Structure below is created during project creating.

![Image](<lib/NewItem45.png>)

**auth.html** - Server will show this page when user will not have authorization

**favicon.ico** - Icon displayed by WebBrowser near to URL.

**fenixlib.js** - Code related with data request

**index.html** - Main page with all content

**jquery-2.2.0** - JQuery library

**flot-0.8.3 -** Flot library (Chart functionality)

**jquery-ui-1.11.4 -** JQuery user interface library

**script.js** - Place where you can create script and place for PageUI script.

## Start Communication

Before you start you must check few parameters. Firstly you must choose with access you prefer (with password or not password) **\[02 Authentication\]**. We will use **\[Basic\]** with password.

We must add user ant their password for access to do this follow procedure below:

![Image](<lib/s\_AddUser.png>)

Everything you can do from Fenix Manager.


Window **Connections** show all connection:

[**Fenix Server\]** Connection between Server and WebBrowser

**[pl001]** Connection between Server and Remote Modbus Device

![Image](<lib/s\_startComm.png>)

To start communication click to button start:

![Image](<lib/s\_BtStart.png>)

Communication started, go to WebBrowser in URL write **http://127.0.0.1** or **http://localhost** typed password and you should see screen below:

![Image](<lib/s\_Webbrowser.png>)

# Autorun

Fenix have possibility to run and load project during windows start-up. To do this you must select option from **Project-\>Autostart**

![Image](<lib/NewItem42.png>)

Fenix will add Task to windows scheduler.

![Image](<lib/NewItem44.png>)


During start windows Fenix will start and will load last project. Project path is stored in registry **HKEY\_CURRENT\_USER\\Software\\Fenix\\LastPath.**

Path is updated every time when user open project by Fenix Server or Fenix Manager.

# Connections

**Request**

```javascript
// REFRESH CONNECTIONS

$(document).ready(function ConnectionsReadPoll() {
    $.ajax({
        method: "POST",
        url: "Connections/All/All",
        data: Connections
    })
    .done(function(msg) {
        // Get JSON Data
        var obj1 = jQuery.parseJSON(msg);
        Connections = obj1;
    });

    setTimeout(ConnectionsReadPoll, 2000);
});
```

&nbsp;

**Refresh Connections**

``` javascript
$(document).ready(function ConnectionsReadPoll() {
    $.ajax({
        method: "POST",
        url: "Connections/All/All",
        data: Connections
    })
    .done(function(msg) {
        // Get JSON Data
        var obj1 = jQuery.parseJSON(msg);
        Connections = obj1;
    });

    setTimeout(ConnectionsReadPoll, 2000);
});
```

**Response**

``` json
[
  {
    "driverParam": {
      "Ip": "127.0.0.1",
      "Port": 502,
      "Timeout": 1000,
      "ReplyTime": 1500
    },
    "connectionName": "pl001",
    "DriverName": "ModbusMasterTCP",
    "isLive": true
  }
]
```

**Example (JQuery / JavaScript usage):**

```javascript
Connections["pl001"].connectionName

Connections["pl001"].DriverName

Connections["pl001"].isLive

Connections["pl001"].driverParam.Ip

Connections["pl001"].driverParam.Port

Connections["pl001"].driverParam.Timeout

Connections["pl001"].driverParam.ReplyTime

```

# Graph

Functionality is responsible to pool data from server for Graph. Function is invoke cyclically every 2000 ms. Fenix use FLOT library.Max. 10 lasts point can be displayed on Web Graph

### Request

```js
$(document).ready(function pollGraph(){

    var data = {};

    $.ajax({
        method: "POST",
        url: "Graph/All/All",
        data: null
    })
    .done(function( msg ) {
        data = jQuery.parseJSON(msg);

        $.plot("#ChartHolder", data, {
            xaxis: {
                mode: "time"
            }
        });
    });

    setTimeout(pollGraph, 2000);

});
```

### Response Server Response:

```json
[
  {"label":"Tag0", "data":[[1442354471942,0.0],[1442354473518,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag1", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag2", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag3", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag4", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag5", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag6", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag7", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag8", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]},
  {"label":"Tag9", "data":[[1442354471942,0.0],[1442354473533,0.0],[1442354475094,0.0],[1442354476654,0.0],[1442354478214,0.0],[1442354479774,0.0],[1442354481334,0.0],[1442354482894,0.0],[1442354484454,0.0],[1442354486014,0.0]]}
]

```

# Events

The code below is using to fetch data form Fenix Server relented with Events.

**Server Request:**

```javascript
// REFRESH EVENTS

$(document).ready(function EventsReadPoll() {
    $.ajax({
        method: "POST",
        url: "Events/All/All",
        data: Events
    })
    .done(function(msg) {
        // Get JSON Data
        Events = jQuery.parseJSON(msg);
    });

    setTimeout(EventsReadPoll, 2000);
});

```

**Server Response:**

```json
[
  {
    "Tm": "2015-09-04T22:27:53.1200686+02:00",
    "Mess": "Warning: Device not respond. COILS.READ.- Timeout",
    "frDateTime": "2015-09-04 22:27"
  },
  {
    "Tm": "2015-09-04T22:27:54.6331551+02:00",
    "Mess": "Error: ModbusMasterRTU.bWorker_DoWork :Unable to write data to the transport connection:",
    "frDateTime": "2015-09-04 22:27"
  },
  {
    "Tm": "2015-09-04T22:27:56.3812551+02:00",
    "Mess": "Error: ModbusMasterTCP.bWorker_RunWorkerCompleted :The operation is not allowed on non-connected sockets.",
    "frDateTime": "2015-09-04 22:27"
  }
]

```

**Example (JavaScript Usage):**

```javascript
$.each(Events, function (indx, obj) {
    $('div').append(indx + " " + obj.Tm + " " + obj.Mess + " " + obj.frDateTime);
});

```

**Reset Alarms from Html/ JavaScript**

```javascript
// Button Events
$(function() {
    $('#btClrAlarms').on('click', function() {
        $.ajax({
            method: "POST",
            url: "Server/Alarms/Clr/*",
            data: { request: "empty" }
        })
        .done(function(msg) {
            alert(msg);
        });
    });
});

```

# Single Write / Read Request

## Logic Behind

Fenix use logic behind created in JQuery to get/send data from/to server. This chapter described single request (one request for single object). Code1 is invoked during first page loading and is responsible to find all \<input class="Tag" id="???"\> and make relation with "Tag". This code attach event handler for ENTER button. Simply is used for writing tag value. Code2 is responsible for reading objects and is invoked cyclically. Time depend on value type in function setTimeout&nbsp; in \[ms\].

**Code 1**

```javascipt
// ATTACHED EVENT
function attachEventy() {
    $(".Tag").on("keypress", function (e) {

        // Enter button
        if (e.which == 13) {
            // Connect to field -> Tag/TagName/Param/Value : Optional
            var req = "Tag/" + $(e.target).attr("id") + "/Value/" + $(e.target).val();

            // Feedback from server
            $.post(req, function (data) {
                alert("DATA CHANGED!");
            });

            return false;
        }
    });
};

```

**Code2**

```javascript
// SEARCHING FOR / TIMER / USER / MACHINE / TAG
$(document).ready(function allReadPoll() {
    
    // Every timer handling
    $('div.Timer, div.User, div.Tag, div.Machine').each(function(index) {
        
        // Request
        var req = $(this).attr('class') + "/" + $(this).attr('id') + "/Value";
        var id = $(this).attr('id');
        
        // Request to server
        $.post(req, function(data) {
            // Server response
            $('div#' + id).html(data);
        });
    });

    setTimeout(allReadPoll, 1000);
});

```

## HTML

&nbsp;

**Write Tag**

In HTML you are able to create code which will be responsible to write Tag value. To do this you must use following code in HTML:

```html
<input type="text" class="Tag" id="Tag0" />. 
```

"Tag0" this the tag name in Fenix and must be unique. In HTML will appear input element and when you write data and push enter data will be send to server.

This functionality is usable only for Tag object.


**Read Tag / Timer / User / Machine**

Reading functionality is usable for following elements:


Tag - Basic communication element from Fenix 
```html
<div class="Tag" id="Tag0"></div>
```

Timer - Actual Fenix Server time 
```html
<div class="Timer"></div>
```

User - Current logged user in Fenix Server
```html
<div class="User"></div\>
```

Machine &nbsp; &nbsp; - Machine name where the Fenix Server is installed
```html
<div class="Machine"></div>
```

# Tags

Inside **fenixlib.js** was implemented mechanism for refreshing Tags objects.

**Request**

```javascript
// REFRESH TAGS
$(document).ready(function TagsReadPoll() {
    $.ajax({
        method: "POST",
        url: "Tags/All/All",
        data: { request: "empty" }
    })
    .done(function(msg) {
        // Get JSON Data
        var obj1 = jQuery.parseJSON(msg);

        // Initialize or reset the Tags object
        Tags = {};

        // Iterate through JSON table elements
        $.each(obj1, function(idx, obj) {
            // Connect element properties to "TAGS"
            Tags[obj.tagName] = obj;
        });
    });

    setTimeout(TagsReadPoll, 2000);
});
```

**JSON Response form Fenix Server**

```json
[
  {
    "tagName": "Tag0",
    "areaData": "Coils",
    "startData": 0,
    "deviceAdress": 1,
    "scAdres": 0,
    "value": true,
    "formattedValue": "True",
    "typeData": "BIT",
    "description": ""
  },
  // ... (similar objects for Tag1 to Tag9)
]

```

**Example (JavaScript Usage):**

Tags\["Tag0"\].tagName

Tags\["Tag0"\].areaData

Tags\["Tag0"\].startData

Tags\["Tag0"\].deviceAdress

Tags\["Tag0"\].scAdres

Tags\["Tag0"\].value

Tags\["Tag0"\].formattedValue

Tags\["Tag0"\].typeData

Tags\["Tag0"\].description

# Table

Functionality is responsible to pool data from server for Table. Function is invoke cyclically every 1000 ms.

```javascript
// SEARCHING FOR TABLE

$(document).ready(function pollTable() {

    // Iterate through each div with class "Table"
    $('div.Table').each(function(index, obj) {

        // Wait for the Tags to be refreshed
        if ($.isEmptyObject(Tags))
            return;

        // Object to store input elements
        var inputs = {};

        // Logic to detect changes in tags
        var TagsNamesAct = "";
        $.each(Tags, function(names) {
            TagsNamesAct = TagsNamesAct + names;
        });

        // Change Configuration if tags have changed or the table is not created
        if (TagsNamesAct != TagsNamesPrev || $('td input').length == 0) {

            // Clear the div content
            $(obj).html("");

            // Create new inputs if they don't exist
            $.each(Tags, function(indx, obj) {
                inputs[indx] = "<input type='text' class='Tag' id='" + indx + "'/>";
            });

            // Add Table structure
            $(obj).append("<table><thead></thead><tbody></tbody></table>");

            // Traverse Tags
            var table = $(obj).children('table');
            var thead = table.children('thead');
            thead.append('<th> Name &nbsp; &nbsp; &nbsp; &nbsp; </th>')
                .append('<th> Device Address</th>')
                .append('<th> Start&nbsp; &nbsp; &nbsp; &nbsp; </th>')
                .append('<th> Bit/Byte &nbsp; &nbsp; </th>')
                .append('<th> Data Type&nbsp; &nbsp; </th>')
                .append('<th> Area Data&nbsp; &nbsp; </th>')
                .append('<th> Value&nbsp; &nbsp; &nbsp; &nbsp; </th>')
                .append('<th> Description&nbsp; </th>')
                .append('<th> Set Value&nbsp; &nbsp; </th>');

            var tbody = table.children('tbody');

            // Iterate through Tags
            $.each(Tags, function(indx, obj) {

                // Add row
                tbody.append("<tr></tr>");
                var td = tbody.children("tr:last-child");

                // Add columns
                td.append("<td>" + indx + " </td>")
                    .append("<td>" + obj.deviceAdress + "</td>")
                    .append("<td>" + obj.startData + " </td>")
                    .append("<td>" + obj.scAdres + "</td>")
                    .append("<td>" + obj.typeData + " </td>")
                    .append("<td>" + obj.areaData + " </td>")
                    .append("<td>" + obj.formattedValue + "</td>")
                    .append("<td>" + obj.description + " </td>")
                    .append("<td>" + inputs[indx] + " </td>");
            });

            // Replace "undefined" with "n/a"
            $(trs).children('td:contains(undefined)').text("n/a");

            // Attach Events
            attachEventy();
        } else {

            // Get existing Table
            var table = $(obj).children('table');
            var tbody = table.children('tbody');
            var trs = tbody.children('tr');
            var tds = trs.children('td');

            // Get cells without input elements
            var sel = tds.not(':has(input)');

            // Iterate through Tags
            var jump = 0;
            $.each(Tags, function(ine, obb) {
                sel.eq(jump).html(String(Tags[ine].tagName));
                sel.eq(jump + 1).html(String(Tags[ine].deviceAdress));
                sel.eq(jump + 2).html(String(Tags[ine].startData));
                sel.eq(jump + 3).html(String(Tags[ine].scAdres));
                sel.eq(jump + 4).html(String(Tags[ine].typeData));
                sel.eq(jump + 5).html(String(Tags[ine].areaData));
                sel.eq(jump + 6).html(String(Tags[ine].formattedValue));
                sel.eq(jump + 7).html(String(Tags[ine].description));

                jump = jump + 8;
            });

            // Replace "undefined" with "n/a"
            $(trs).children('td:contains(undefined)').text("n/a");
        }

        // Refresh name
        TagsNamesPrev = TagsNamesAct;
    });

    // Set timeout for polling
    setTimeout(pollTable, 1000);
});

```

**Server Response:**

```json
[
  {
    "tagName": "Tag0",
    "areaData": "Coils",
    "startData": 0,
    "deviceAdress": 1,
    "scAdres": 0,
    "value": true,
    "formattedValue": "True",
    "typeData": "BIT",
    "description": ""
  },
  {
    "tagName": "Tag1",
    "areaData": "Coils",
    "startData": 1,
    "deviceAdress": 1,
    "scAdres": 0,
    "value": false,
    "formattedValue": "False",
    "typeData": "BIT",
    "description": ""
  },
  // ... (similar objects for Tag2, Tag3, and so on)
]

```

# Custom Request

If the proposition from fenix is not appropriate for you you can crate your own solution based on few simple request shown below.

## Read

\[Object\] / \[Object Name\] / \[Field\]


Example:

Tag/Tag0/Value

```javascript
// Read request to Fenix Server
$.post("Tag/Tag0/Value", function(response) {
  // Response contains data from Fenix Server
  alert(response);
});

```

## Write

\[Object\] / \[Object Name\] / \[Field\] / \[Value\]

Example:

Tag/Tag0/Value/20
```javascript
// Write request to Fenix Server
$.post("Tag/Tag0/Value/20", function(response) {
  // Success callback - Response contains data from Fenix Server
  console.log("Write request successful:", response);
}).fail(function(error) {
  // Error callback - Handle errors, if any
  console.error("Error in write request:", error);
});

```

**Allowed operation**

Timer {read}

Machine {read}

User {read}

Tag {read , write}