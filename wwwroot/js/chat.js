
"use strict";


var loginToken = document.getElementById("tokenData").value;
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => this.loginToken })
    .withAutomaticReconnect()
    .build();

//Disable send button until connection is established
document.getElementById("startRealtime").disabled = true;
document.getElementById("stopRealtime").disabled = true;

connection.on("ReceiveData", function (userMsg, msg) {
    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg;
    //var li = document.createElement("li");
    //li.textContent = encodedMsg;
    //document.getElementById("messagesList").appendChild(li);
    var myObj = JSON.parse(msg);
    document.getElementById("demo").innerHTML = myObj.Price;
    //console.log("user: " + userMsg + " message: " + msg);
});

connection.on("ReceiveRealTimeData", data => OnReceiveRealTimeData(data));
connection.on("ReceiveStatusData", status => OnReceiveStatusData(status));

function OnReceiveRealTimeData(data) {
    var myObj = JSON.parse(data);

	//document.getElementById("inputCount").innerHTML = myObj.input;
	//document.getElementById("passCount").innerHTML = myObj.pass;

	if (myObj.input > 0 && myObj.pass > 0) {
		var d = new Date();
		//d.setSeconds(d.getSeconds() + (i * selectTimeDuration));
		//d += i;
		var temp = new Object();
		temp["times"] = d.toLocaleTimeString();
		temp["input"] = myObj.input;
		temp["pass"] = myObj.pass;

		dataFromClient.push(temp);
		if (dataFromClient.length > 100) {
			dataFromClient.shift();
		}
		updateInput();
	}
	
	updateChartRealtime(parseFloat(myObj.idletimes), parseFloat(myObj.settingtimes), parseFloat(myObj.runningtimes), parseFloat(myObj.downTimetimes));
}
function OnReceiveStatusData(status) {

	// running
	if (status == 0) {
		document.getElementById("monitoringStatus").style.animation = "blinkRunning 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Running...";
	}
	//downtime
	else if (status == 1) {
		document.getElementById("monitoringStatus").style.animation = "blinkDowntime 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Error...";
	}
	//idle
	else if (status == 2) {
		document.getElementById("monitoringStatus").style.animation = "blinkIdle 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Waiting...";
	}
	//setting
	else if (status== 3) {
		document.getElementById("monitoringStatus").style.animation = "blinkSetting 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Setting...";
	}
	//connecting
	else if (status == -1) {
		document.getElementById("monitoringStatus").style.animation = "blinkConnecting 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Connecting...";
	}
	else {
		document.getElementById("monitoringStatus").style.animation = "blinkDisconnecting 2s infinite";
		document.getElementById("monitoringStatus").innerHTML = "Machine is Disconnect...";
	}

}
var dataFromClient = [];
//	{ "times": 'Q1', "input": 100, "pass": 80 },
//	{ "times": 'Q2', "input": 200, "pass": 180 },
//	{ "times": 'Q3', "input": 300, "pass": 270 },
//	{ "times": 'Q4', "input": 400, "pass": 390 }
//];

connection.start().then(function () {
    document.getElementById("startRealtime").disabled = false;
	document.getElementById("stopRealtime").disabled = false;
	updateChartRealtime(0.0, 0.0, 0.0, 0.0);

	//var HH = new Date().getHours();         // Get the hour (0-23)
	//var MM = new Date().getMinutes();       // Get the minutes (0-59)
	//var SS = new Date().getSeconds();       // Get the seconds (0-59)

	var selectTimeDuration = parseInt(document.getElementById("SelectTiggerTimes").value);

	//for (var i = 1, iLen = 1; i <= iLen; i++) {

	//	var d = new Date();
	//	d.setSeconds(d.getSeconds() + (i * selectTimeDuration));
	//	//d += i;
	//	var temp = new Object();
	//	temp["times"] = d.toLocaleTimeString();
	//	temp["input"] = 0.00;
	//	temp["pass"] = 0.00;

	//	dataFromClient.push(temp);
	//}
	

}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("startRealtime").addEventListener("click", function (event) {
    var company = document.getElementById("companyName").innerHTML.toString();
	var machine = document.getElementById("machineName").value;
	var selectTriggerTime = document.getElementById("SelectTiggerTimes").value;
	connection.invoke("TrigerRealTimeMachine", machine, company, true, parseInt(selectTriggerTime)).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("stopRealtime").addEventListener("click", function (event) {
    var company = document.getElementById("companyName").innerHTML;
    var machine = document.getElementById("machineName").value;
    connection.invoke("TrigerRealTimeMachine", machine, company,false,0).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function updateChartRealtime(idle, setting, running, downtime) {

	var chart = JSC.chart('chartDiv',
		{
			debug: false,
			title: {
				label_text: 'Status Monitoring Timeline',
				position: 'center'
			},
			legend_visible: false,
			yAxis: [
				{ line_visible: true, scale_range: [0, 100] }],
			xAxis: [
				{
					defaultTick_gridLine_width: 0,/*Reduces column size to pad against axis line.*/
					spacingPercentage: 0.15
				}],

			defaultSeries: {
				type: 'gaugeLinearHorizontalColumnRoundCaps',
				shape: { label: [{ text: '%name' }] }
			},
			series: [
				{
					name: 'Setting Time',
					points: [['value', setting]]
				},
				{
					name: 'Down Time',
					points: [['value', downtime]]
				},
				{
					name: 'Idle Time',
					points: [['value', idle]]
				},
				{
					name: 'Running Time',
					points: [['value', running]]
				}]
		});
}

function updateInput() {
	var chart = JSC.chart('chartDiv2', {
	debug: false,
	defaultSeries_type: 'column',
	yAxis_scale_type: 'stacked',
	title_label_text: 'Productivity Monitoring',
	yAxis:[
		{
			id: 'a1',
			label_text: 'Volume of product',
			alternateGridFill: 'none'
		},
		{
			id: 'a2',
			label_text: 'Yield(%)',
			orientation: 'right',
			scale: { range: [0, 120], interval: 10},
			defaultTick_label_text: '%value%',
			alternateGridFill: 'none',
			markers: [{ value: [90, 100], color: '#84F0EC', legendEntry_name: 'Range Target Yield ', line_width: 2 },
			{ value: [95], color: 'green', legendEntry_name: 'Target Yield ', line_width: 2 }]
		}],
	xAxis_label_text: 'Time',
		series: makeSeries(dataFromClient)
},function(c){c.series.add(getSumSeries(c));});
		
	function getSumSeries(c)
	{
		var cat,result = {	name:'Yield',type:'line',
								yAxis: 'a2',
								defaultPoint:{
									tooltip: '{%yValue:n1}%',
									marker: { type: 'circle', outline: { color: 'black', width: 2 } }
								},
								points: []};
		//Get categories of the x axis.
		var cats = c.axes('x').getCategories();
		var addData=[];
		for (var i = 0, iLen = cats.length; i < iLen; i++) {
				cat = cats[i];
			c.series().points({ x: cat }).each(function (p) {
				addData.push(p.options('y'));
			});
			var yieldValue = divideYield(addData[0] * 100, addData[1] * 100);
			addData = [];
			result.points.push({ x: cat, y: yieldValue});
		}
		return result;
	}
	function divideYield(input, pass) {
		return parseFloat((pass / input * 100).toFixed(2));
	}
}

function makeSeries(data) {

	//var series = [
	//	{
	//		name: 'Input',
	//		id: 's1',
	//		//color: '#4981E3',
	//		defaultPoint_label: { visible: true, color: 'blue' },
	//		points: [
	//			{ x: 'Q1', y: 230 },
	//			{ x: 'Q2', y: 240 },
	//			{ x: 'Q3', y: 267 },
	//			{ x: 'Q4', y: 238 }]
	//	},
	//	{
	//		name: 'Pass',
	//		color: '#40E340',
	//		defaultPoint_label: { visible: true, color: 'green' },
	//		points: [
	//			{ x: 'Q1', y: 325 },
	//			{ x: 'Q2', y: 367 },
	//			{ x: 'Q3', y: 382 },
	//			{ x: 'Q4', y: 371 }]
	//	}];

	var key = JSC.nest().key('times');
	var series = [
		{
			name: 'Input',
			defaultPoint_label: { visible: true, color: 'blue' },
			points: key.rollup('input').points(data)
		},
		{
			name: 'Pass',
			color: '#40E340',
			defaultPoint_label: { visible: true, color: 'green' },
			points: key.rollup('pass').points(data)
		}
		//{

		//	name: 'Yield',
		//	type: 'line',
		//	yAxis: 'a2',
		//	defaultPoint: {
		//		tooltip: '{%yValue:n1}%',
		//		marker: { type: 'circle', outline: { color: 'black', width: 2 } }
		//	},
		//	points: []

		//}
	];
	return series;
	//function divideYield(input, pass) {
	//	return parseFloat((pass / input * 100).toFixed(2));
	//}
}

