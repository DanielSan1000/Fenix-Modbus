	//Events
	$(document).ready(function fcEvents(){
		
		//Create Table
		if( !$('.Events table').length )
		{
			$('.Events').html("<table> <thead><th>Lp</th> <th>Time</th><th>Message</th></th></thead> <tbody></tbody></table>");
			
			$.each(Events,function(indx,obj ){
				$('.Events table tbody').append("<tr>"+ "<td>"+indx+"</td>" + "<td>"+obj.frDateTime+"</td>" + "<td>"+obj.Mess+"</td></tr>");
			});
		}
		else
		{
			
			var rowsAmount = $('.Events table tr').length;
			var eventsAmount = $(Events).length;
			
			//rebuild table
			if((rowsAmount - 1) != eventsAmount)
			{
				$('.Events table tbody').html("");
				
					$.each(Events,function(indx,obj ){
						$('.Events table tbody').append("<tr>"+ "<td>"+indx+"</td>" + "<td>"+obj.frDateTime+"</td>" + "<td>"+obj.Mess+"</td></tr>");
					});	
			}
		}
			  
		setTimeout(fcEvents,2000); 
	});

	//Connections
	$(document).ready(function fcConnections(){
		

        if(!$('.Connections table').length)
		{
			$('.Connections').html("<table><thead> <th>Lp</th> <th>Connection Name</th>  <th>Live</th> <th>Parameters</th></thead><tbody></tbody></table>");
			
			var pos = 0;
			$.each(Connections,function(indx,obj ){
    
				//Add Connections
				$('.Connections table tbody').append("<tr>"+ "<td>"+pos+"</td>" + "<td>"+obj.connectionName+"</td>" + "<td>"+obj.isLive+"</td>" + "<td></td>"+"</tr>");
				pos= pos + 1;
    
				//Add parameters
				var td = $('td:last');
				$.each(obj.driverParam,function(idd,os){
					td.append(idd +": " + os + "<br/>");
				});
			});
		}
		else
		{
			var rowsAmount1 = $('.Connections table tr').length;
			var connAmount1 = Connections.length;
				
			if((rowsAmount1-1) != connAmount1)
			{
				var pos = 0;
				$('.Connections table tbody').html("");
				$.each(Connections,function(indx,obj ){
    
					//Add Connections
					$('.Connections table tbody').append("<tr>"+ "<td>"+pos+"</td>" + "<td>"+obj.connectionName+"</td>" + "<td>"+obj.isLive+"</td>" + "<td></td>"+"</tr>");
					pos= pos+1;
    
					//Add parameters
					var td = $('.Connections td:last');
					$.each(obj.driverParam,function(idd,os){
						td.append(idd +": " + os + "<br/>");
					});
				});
			}
		}
		
		setTimeout(fcConnections,2000); 
	});	
	
	//Table Style
	$(function fcStyle(){
		
		$('.Table table').css('border','2');
		$('.Table table tbody tr:even').css('background','#dedede');
		$('.Table table tbody tr:odd').css('background','#ffffff');
		$('.Table table td').css('width','120');
		$('.Table table tr td:nth-child(7)').css("font-weight", "bold");
		$('.Table table tr td:nth-child(7)').css("text-align", "center");
		
		
		$('.Events table').css('border','2');
		$('.Events table tbody tr:even').css('background','#dedede');
		$('.Events table tbody tr:odd').css('background','#ffffff');
		$('.Events table td').css('width','400');
		
		$('.Connections table').css('border','2');
		$('.Connections table tbody tr:even').css('background','#dedede');
		$('.Connections table tbody tr:odd').css('background','#ffffff');
		$('.Connections table td').css('width','300');
		
		$('.Header table').css('border','2');
		$('.Header table td').css('width','300');
		$('.Header table td').css("text-align", "center");
		
		setTimeout(fcStyle,2000); 

	});
			
	//TABLE
	$(document).ready(function pollTable(){

		$('div.Table').each(function(index,obj){
  
        //Wait to refresh Tags
			if ($.isEmptyObject(Tags))
				return;

			//all inputs
			var inputs = {}; 

		//logic for detect tags changes 
		var TagsNamesAct="";
		$.each(Tags,function(names){
			TagsNamesAct = TagsNamesAct + names;
		});
		
		//Change Configuration
		if(TagsNamesAct != TagsNamesPrev || $('td input').length == 0)
		{
		     //clean div
			 $(obj).html("");
		     
			//create new inputs of not exist
			$.each(Tags,function(indx, obj){
				inputs[indx] = "<input type='text' class='Tag' id='"+indx+"'/>";
			});
		
			//Add Table
			$(obj).append("<table><thead></thead><tbody></tbody></table>");
  
			//Go trought Tags
			var table = $(obj).children('table');
			var thead = table.children('thead');
			//Add Th
			thead.append('<th> Name         </th>')
				 .append('<th> Device Adress</th>')
				 .append('<th> Start        </th>')
				 .append('<th> Bit/Byte     </th>')
				 .append('<th> Data Type    </th>')
				 .append('<th> Area Data    </th>')
				 .append('<th> Value        </th>')
				 .append('<th> Description  </th>')
				 .append('<th> Set Value    </th>')
				 .append('<th> Send Value    </th>');				 
  
			//Tags iternation
			var tbody = table.children('tbody');
			$.each(Tags,function(indx, obj){
  
				//add row before
				tbody.append("<tr></tr>")
				var td = tbody.children("tr:last-child");
      
				//add columens
				td.append("<td>" +indx+               "</td>")
				  .append("<td>" +obj.deviceAdress+   "</td>")
				  .append("<td>" +obj.startData+      "</td>")
				  .append("<td>" +obj.scAdres+        "</td>")
				  .append("<td>" +obj.typeData+       "</td>")
				  .append("<td>" +obj.areaData+       "</td>")
				  .append("<td>" +obj.formattedValue+ "</td>")
				  .append("<td>" +obj.description+    "</td>")
				  .append("<td>" +inputs[indx]+       "</td>")
				  .append("<td align='center'> <button id=bt"+ obj.tagName +">Send Value</button>   </td>");			  
			});
	
			//searching for "undefined"
			$(trs).children('td:contains(undefined)').text("n/a");
			
			//Connect button events
			var inpValues = $('.Table input');
			var btValues = $('.Table button');		
			$.each(btValues,function(indx,obj){
			
				$(obj).on("click",function(){
				    
				    var ob = $(inpValues[indx]);
						var req = "Tag/" + ob.attr("id") + "/Value/" + ob.val();
						//Feedback from server
						$.post(req,function(data){			     
						});										
				});
			
			});
						
			//attach Events
			attachEventy();
		}
		else
		{
			
			//Get Table
			var table = $(obj).children('table');
			var tbody = table.children('tbody');
			var trs = tbody.children('tr');
			var tds = trs.children('td');
			
			//pobranie tylko ts
			var sel = tds.not(':has(input)');

			//iteration
			var jump=0;
			$.each(Tags,function(ine,obb){
				sel.eq(jump).html(String(Tags[ine].tagName));
				sel.eq(jump+1).html(String(Tags[ine].deviceAdress));
				sel.eq(jump+2).html(String(Tags[ine].startData));
				sel.eq(jump+3).html(String(Tags[ine].scAdres));
				sel.eq(jump+4).html(String(Tags[ine].typeData));
				sel.eq(jump+5).html(String(Tags[ine].areaData));
				sel.eq(jump+6).html(String(Tags[ine].formattedValue));
				sel.eq(jump+7).html(String(Tags[ine].description));
				
				jump = jump + 9;
			})
			
			//searching for "undefined"
			$(trs).children('td:contains(undefined)').text("n/a");
		}

		//refresh name
		TagsNamesPrev = TagsNamesAct;

	});

	setTimeout(pollTable,1000);

});
	
	//ATTACHED EVENT
	function attachEventy(){ 
 
		$(".Tag").on("keypress",function(e){ 
	
	    	//Enter button
			if (e.which == 13) {
			//Connect to field -> Tag/TagName/Param/Value : Optional
				var req = "Tag/" + $(e.target).attr("id") + "/Value/" + $(e.target).val();
			//Feedback from server
				$.post(req,function(data){
			     
			});
			return false;    
		}   
	});	
};
	
	//BASIC SETTINGS
	$(document).ready(function(){
	                  	
    	//Bacground change
		$('html').css('background','#b2b2b2');
		
			//Init for controls
		$('.ConnectionsContainer').hide();
		$('.EventContainer').hide();
	    
		//Buffor
		$.ajax({
			method: "POST",
			url: "Server/Buffor/Get",
			data:null
		})
		.done(function( msg ) {			
			$('#tbBuffor').val(msg);
		});
		
		$("#btSetBuffor").on('click',function(){		                     		                 
		    $.ajax({
		      method: "POST",
		      url: "Server/Buffor/Set/"+$('#tbBuffor').val(),
		      data: null
		    }).done(function(msg){
		          alert("Data Changed: " + msg);        		        	
		  });
		});				
		
		$('#btClrAlarms').on('click',function(){
		   $.ajax({
				method: "POST",
				url: "Server/Alarms/Clr/*",
				data: {reguest: "empty"}
			})
			.done(function( msg ) {
			  alert(msg);
			});
			
		});
		
		$('#btShowTable').on('click',function(){		
			$('.TableContainer').toggle( "fold", 1000 );
		});
		
		$('#btShowChart').on('click',function(){		
			$('.ChartContainer').toggle( "fold", 1000 );
		});
		
		$('#btShowCon').on('click',function(){		
			$('.ConnectionsContainer').toggle( "fold", 1000 );
		});
			
		$('#btShowEv').on('click',function(){		
			$('.EventContainer').toggle("fold", 1000);
		});
	
		//Add click events to inputs
    	attachEventy();
	});