
// JS
var chart, orgList;
var openedNodeColor = '#e0e0e0', closedNodeColor = '#efefef';

JSC.fetch('csv/orgData.csv').then(function (response) { return response.text(); }).then(function (text) {
	var data = JSC.csv2Json(text);
	orgList = makePoints(data);
	chart = renderChart(orgList);

	//chart = renderChart(makeSeries(data));
});

function renderChart(orgList) {
	return JSC.chart('chartDiv', {
		debug: false,
		//title: {
		//	label: {
		//		text: 'Automate Organizational Chart',
		//		style_fontSize: 16
		//	},
		//	position: 'center'
		//},
		type: 'organization down',
		animation: false,
		defaultSeries: {
			line: { width: 1, color: '#00f' }
		},
		defaultTooltip: {
			asHTML: true,
			outline: 'none',
			zIndex: 10
		},
		defaultPoint: {
			focusGlow_width: 0,
			tooltip: '<div class="tooltipBox">Phone: <b>%phone</b><br>Email: <b>%email</b><br>Address: <b>%address</b></div>',
			annotation: {
				margin: 5,
				label: {
					style_fontSize: 10,
					text: '<span style="font-size:11px;"><span style="align:center;"><span style="font-size:12px;"><b>%position</b></span><br/>' +
						'<img width=50 height=50 align=center margin_bottom=4 margin_top=4 src=%photo><br/>' +
						'%name<br/><br/></span>' +
						'%quality<br/>' +
						'%initiative<br/>' +
						'%cooperativee</span>',
					maxWidth: 140,
					style_fontWeight: 'normal',
					align: 'left'
				}
			},
			outline_width: 0,
			color: closedNodeColor
		},
		series: [{ points: orgList.slice(0, 6) }]
	});
}

function pointClick() {
	var point = this;
	if (point.options('color') == closedNodeColor) {
		point.options({
			label_text: '<b>%position</b><br/>%name<br/>',
			color: openedNodeColor
		});
		orgList.forEach(function (val, i) {
			if (val.parent == point.id) { chart.series(0).points.add(val); }
		})
		point.zoomTo()
	} else {
		point.options({
			label_text: '<b>%position</b><br/>%name<br/>',
			color: closedNodeColor
		});
		if (point.options('id') == 'md') {
			chart.series(0).remove();
			chart.series.add({ points: [orgList[0]] });
		}
		else {
			var childrenID = getAllChildren(point).reverse()
			childrenID.forEach(function (val, i) {
				chart.series(0).points(function (p) { return p.options('id') === val }).remove();
			})
		}
	}

	function getAllChildren(point) {
		var childrenID = chart.series(0).points(function (p) { return p.options('parent') === point.id; }).map(function (a) { return a.id });

		childrenID.forEach(function (val) {
			childrenID = childrenID.concat(getAllChildren(chart.series(0).points(function (p) { return p.options('id') === val; }).items[0]));
		})
		return childrenID;
	}
}

function makePoints(data) {

	var barColors = [
		{ value: 3, color: '#ffca28' },
		{ value: 4, color: '#d4e157' },
		{ value: 5, color: '#66bb6a' }
	];

	function getColor(val) {
		for (var i = 0; i < barColors.length; i++) {
			if (val == barColors[i].value)
				return barColors[i].color;
		}
	}
	var points = JSC.nest().key('name').pointRollup(function (key, val) {
		var result = {
			name: key,
			id: val[0].id,
			parent: val[0].parent,
			attributes: {
				position: val[0].position,
				phone: val[0].phone,
				address: val[0].address,
				email: val[0].email,
				photo: 'Img/Emp/' + val[0].photo + '.jpg',
				quality: '<chart width=28 height=10 color=' + getColor(val[0].work_quality) + ' type=bar data=' + val[0].work_quality + ' max=5> - Quality of work',
				initiative: '<chart width=28 height=10 color=' + getColor(val[0].initiative) + ' type=bar data=' + val[0].initiative + ' max=5> - Initiative',
				cooperative: '<chart width=28 height=10 color=' + getColor(val[0].cooperative) + ' type=bar data=' + val[0].cooperative + ' max=5> - Cooperative'
			}
		};
		if (result.id == 'md') {
			result.annotation_label_text = '<span style="align:center;font-size:13px;"><img width=70 height=70 align=center margin_bottom=4 src=%photo><br/><span style="font-size:14px;"><b>%position</b></span><br/>%name<br/></span>';
		}
		return result;
	}).points(data);

	points.forEach(function (val1, i, arr) {
		if (i > 0) {
			arr.forEach(function (val2, j) {
				if (j > 0 && val1.id == val2.parent) {
					val1.label_text = '<b>%position</b><br/>%name<br/>';
					val1.events_click = pointClick;
				}
			})
		}
	})
	return points;
	
}