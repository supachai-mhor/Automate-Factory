
"use strict";

var startsRealTimeFlag = false;
var dataFromClient = [];

var loginToken = document.getElementById("tokenData").value;
var connection = new signalR.HubConnectionBuilder()
	.withUrl("/AutomateHub", { accessTokenFactory: () => this.loginToken })
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
connection.on("ReceiveRealTimeErrorData", (data, mc) => OnReceiveRealTimeErrorData(data,mc));
connection.on("ReceiveStatusData", (status,mc) => OnReceiveStatusData(status,mc));

function OnReceiveRealTimeData(data) {

	if (startsRealTimeFlag) {
		
		var myObj = JSON.parse(data);
		var machineSelect = document.getElementById("machineName").value;
		if (machineSelect == myObj.machineName) {
			if (myObj.input > 0 && myObj.pass > 0) {
				var d = new Date();
				//d.setSeconds(d.getSeconds() + (i * selectTimeDuration));
				//d += i;
				var temp = new Object();
				temp["times"] = d.toLocaleTimeString();
				temp["input"] = myObj.input;
				temp["pass"] = myObj.pass;

				dataFromClient.push(temp);
				if (dataFromClient.length > 15) {
					dataFromClient.shift();
				}
				updateInput();
			}
			updateChartYieldAndOEE(myObj.totalInput, myObj.totalPass, myObj.yield, myObj.oee);
			updateChartRealtime(myObj.jobNumber, myObj.operatorName, myObj.supervisorName,parseFloat(myObj.idletimes), parseFloat(myObj.settingtimes), parseFloat(myObj.runningtimes), parseFloat(myObj.downTimetimes))
		}
	}
}
function OnReceiveRealTimeErrorData(data, mc) {

	if (startsRealTimeFlag) {
		var machineSelect = document.getElementById("machineName").value;
		if (machineSelect == mc) {
			var encodedMsg = data;
			var li = document.createElement("li");
			li.textContent = encodedMsg;
			document.getElementById("ErrorMessage").appendChild(li);
		}
	}
}
function OnReceiveStatusData(status,mc) {

	if (startsRealTimeFlag) {
		var machineSelect = document.getElementById("machineName").value;
		if (machineSelect == mc) {

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
			else if (status == 3) {
				document.getElementById("monitoringStatus").style.animation = "blinkSetting 2s infinite";
				document.getElementById("monitoringStatus").innerHTML = "Machine is Setting...";
			}
			//connecting
			else if (status == -1) {
				document.getElementById("monitoringStatus").style.animation = "blinkConnecting 2s infinite";
				document.getElementById("monitoringStatus").innerHTML = "Machine is Connecting...";
			}
			else {
				startsRealTimeFlag = false;
				document.getElementById("monitoringStatus").style.animation = "blinkDisconnecting 2s infinite";
				document.getElementById("monitoringStatus").innerHTML = "Machine is Disconnect...";
			}
		}
	}
}

connection.start().then(function () {
    document.getElementById("startRealtime").disabled = false;
	document.getElementById("stopRealtime").disabled = false;
	updateChartRealtime("","","",0.0, 0.0, 0.0, 0.0);
	updateChartYieldAndOEE(0, 0, 0, 0, 30000);

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
	startsRealTimeFlag = true;
	var machine = document.getElementById("machineName").value;
	var selectTriggerTime = document.getElementById("SelectTiggerTimes").value;
	dataFromClient = [];
	//document.getElementById("JobNumber").innerHTML = '';
	//document.getElementById("Operator").innerHTML = '';
	//document.getElementById("Supervisor").innerHTML = '';
	connection.invoke("TrigerRealTimeMachine", machine, true, parseInt(selectTriggerTime)).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("stopRealtime").addEventListener("click", function (event) {
	startsRealTimeFlag = false;
	document.getElementById("monitoringStatus").style.animation = "blinkDisconnecting 2s infinite";
	document.getElementById("monitoringStatus").innerHTML = "Stop connect to machine...";
    //var company = document.getElementById("companyName").innerHTML;
    //var machine = document.getElementById("machineName").value;
    //connection.invoke("TrigerRealTimeMachine", machine, company,false,0).catch(function (err) {
    //    return console.error(err.toString());
    //});
    event.preventDefault();
});

var template = {
	title: {
		label_style: {
			fontSize: '24',
			fontFamily: 'Arial',
			color: '#616161',
			fontWeight: 'bold'
		},
	},
	defaultAxis_defaultTick_label_color: 'green',//#757575',
	// box_fill:'none',
	legend_visible: false,
	toolbar_visible: false
};

function updateChartYieldAndOEE(inputV, passV, yieldV, oeeV, MaxInput = 10000) {

	var chart1 = new JSC.Chart('cc1', {
		debug: false,
		template: template,
		title_label: { text: 'Today', margin_left: 20 },
		legend_visible: false,
		yAxis: [
			{
				id: 'y1',
				line: { width: 5, color: 'smartPalette:pal2' },
				defaultTick_enabled: false,
				scale_range: [0, MaxInput]
			},
			{
				id: 'y2',
				line: { width: 5, color: 'smartPalette:pal3' },
				defaultTick_enabled: false,
				scale_range: [0, MaxInput]
			},
			{
				id: 'y3',
				defaultTick: { padding: 1, enabled: false },
				customTicks: [0, 25, 50, 75, 100],
				line: {
					width: 5,
					breaks: {},
					color: 'smartPalette:pal1'
				},
				scale_range: [0, 100]
			},
			{
				id: 'y4',
				defaultTick: { padding: 1, enabled: false },
				customTicks: [0, 25, 50, 75, 100],
				line: {
					width: 5,
					breaks: {},
					color: 'smartPalette:pal1'
				},
				scale_range: [0, 100]
			}
		],
		xAxis: {
			spacingPercentage: .15,
			defaultTick: { label_color: 'none' }
		},
		defaultPoint_tooltip: '',
		defaultSeries: {
			type: 'gauge column roundCaps',
			angle_orientation: 0,
			//palette: ['#00838f'],
			shape: {
				offset: '0,5',
				innerSize: '70%',
				label: [
					{
						text: '<span style=\'font-size:15px\'><b>%name</b></span><br><span style=\'font-size:18px\'>%value</span>',
						width: 55,
						verticalAlign: 'middle',
						offset: '-90,0',
						align: 'right',
						color: '#757575'
					},
					{
						verticalAlign: 'middle',
						align: 'center',
						offset: '0,0',
						text: '<icon name=\'%icon\' size=36 fill=#757575>',
					}
				]
			}
		},
		series: [
			{
				name: 'Input',
				yAxis: 'y1',
				palette: {
					id: 'pal2',
					pointValue: '%yValue',
					defaultRange: { legendEntry: { value: '{%min:n0}–{%max:n0}' } },
					colors: ['#FF5E5E', 'yellow', 'green'],
					ranges: { min: 0, max: 30000, interval: 100 }
				},
				points: [['value', inputV]],
				attributes_icon: ''//material/maps/directions-walk'
			},
			{
				name: 'Pass',
				yAxis: 'y2',
				palette: {
					id: 'pal3',
					pointValue: '%yValue',
					defaultRange: { legendEntry: { value: '{%min:n0}–{%max:n0}' } },
					colors: ['#FF5E5E', 'yellow', 'green'],
					ranges: { min: 0, max: 30000, interval: 100 }
				},
				points: [['value', passV]],
				shape_label: [{ text: '<span style=\'font-size:15px\'><b>%name</b></span><br><span style=\'font-size:18px\'>%value</span>' }, {}],
				attributes_icon: ''//material/device/access-time'
			},
			{
				name: 'Yield',
				yAxis: 'y3',
				palette: {
					id: 'pal1',
					pointValue: '%yValue',
					ranges: [
						{ value: 25, color: '#FF5353' },
						{ value: 50, color: '#FFD221' },
						{ value: 75, color: '#77E6B4' },
						{ value: [90, 100], color: 'green' }
					]
				},
				points: [['value', yieldV]],
				shape_label: [{ text: '<span style=\'font-size:15px\'><b>%name</b></span><br><span style=\'font-size:18px\'>%value%</span>' }, {}],
				attributes_icon: ''//material/social/whatshot'
			},
			{
				name: 'OEE',
				yAxis: 'y4',
				palette: {
					id: 'pal1',
					pointValue: '%yValue',
					ranges: [
						{ value: 25, color: '#FF5353' },
						{ value: 50, color: '#FFD221' },
						{ value: 75, color: '#77E6B4' },
						{ value: [90, 100], color: 'green' }
					]
				},
				points: [['value', oeeV]],
				shape_label: [{ text: '<span style=\'font-size:15px\'><b>%name</b></span><br><span style=\'font-size:18px\'>%value%</span>' }, {}],
				attributes_icon: ''//material/maps/place'
			}]
	});



}

function updateChartRealtime(JobNumber, operator,supervisor,idle, setting, running, downtime) {

	var chart = JSC.chart('chartDiv',
		{
			debug: false,
			title: {
				label_text: '',
				position: 'center'
			},
			annotations: [
				{
					position: 'inside top left',
					margin: 5,
					fill: 'rgb(230, 55, 230)',
					label_text: 'Job Number: ' + JobNumber + '<br/>' + 'Operator: ' + operator + '<br/>' + 'Supervisor: ' + supervisor
				}
			],
			legend_visible: false,
			yAxis: [
				{
					line_visible: true,
					scale_range: [0, 100]
				}],
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
					defaultPoint: { tooltip: '{%yValue:n2}%' },
					points: [['value', setting]]
				},
				{
					name: 'Down Time',
					defaultPoint: { tooltip: '{%yValue:n2}%' },
					points: [['value', downtime]]
				},
				{
					name: 'Idle Time',
					defaultPoint: { tooltip: '{%yValue:n2}%' },
					points: [['value', idle]]
				},
				{
					name: 'Running Time',
					defaultPoint: { tooltip: '{%yValue:n2}%' },
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
	];
	return series;
}

