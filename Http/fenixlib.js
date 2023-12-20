//TAGS BUFFOR
var TagsNamesPrev="";
var Tags = {};
var Connections ={};
var Events = {};

//SEARCHING FOR / TIMER / USER / MACHINE / TAG
$(document).ready(function allReadPoll(){
	
	//Every timers handling
	$('div.Timer, div.User, div.Tag, div.Machine').each(function( index ) {
		
		//request
		var req = $(this).attr('class')+"/" + $(this).attr('id') + "/Value";
		var id = $(this).attr('id');
		
		//request to server
		$.post(req,function(data){
			//server response
			$('div#'+id).html(data);
		});
	});
	
	setTimeout(allReadPoll,1000);
});

//REFRESH TAGS
$(document).ready(function TagsReadPoll(){	
		$.ajax({
			method: "POST",
			url: "Tags/All/All",
			data: {reguest: "empty"}
		})
		.done(function( msg ) {
			
			//Get JSON Data
			var obj1 = jQuery.parseJSON(msg);
			
			//JSON table element iteration
			Tags = {};
			$.each(obj1, function(idx, obj) {
				//element prop. connection to "TAGS"
				Tags[obj.tagName] = obj
			});
		});
	
	setTimeout(TagsReadPoll,2000);
});

//REFRESH CONNECTIONS
$(document).ready(function ConnectionsReadPoll(){	
		$.ajax({
			method: "POST",
			url: "Connections/All/All",
			data: Connections
		})
		.done(function( msg ) {
			
			//Get JSON Data
			var obj1 = jQuery.parseJSON(msg);
			Connections = obj1;
		});
	
	setTimeout(ConnectionsReadPoll,2000);
});

//REFRESH EVENTS
$(document).ready(function EventsReadPoll(){	
		$.ajax({
			method: "POST",
			url: "Events/All/All",
			data: Events
		})
		.done(function( msg ) {
			
			//Get JSON Data
			Events = jQuery.parseJSON(msg);
		});
	
	setTimeout(EventsReadPoll,2000);
});

//GRAPH
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
			xaxis: 
			{ 
				mode: "time",
				timeformat: "%H:%M:%S",
				minTickSize: [1, "second"]
			},
		       	
			points: 
			{ 
				show: true 
			},
			
			lines: 
			{ 
				show: true 
			},
		       	
		    grid: 
		    {
				hoverable: true
			},
		       	
		    tooltip: {
        		show: true,
        		content: "%s | x: %x; y: %y"
      		}
		       					       			 		       			    
		});
				
	});
	
	setTimeout(pollGraph,2000);
});












