
var chart, orgList;
var openedNodeColor = '#e0e0e0', closedNodeColor = '#efefef';

JSC.fetch('csv/orgData.csv').then(function (response) { return response.text(); }).then(function (text) {
	var data = JSC.csv2Json(text);
	orgList = makePoints(data);
	chart = renderChart(orgList);
});
function renderChart(orgList) {
	return JSC.chart('chartDiv', {
		debug: false,
		//title: {
		//	label: {
		//		text: 'Police Department Organizational Chart',
		//		style_fontSize: 16
		//	},
		//	position: 'center'
		//},
		type: 'organization down',
		animation: false,
		defaultSeries: {
			line: { width: 1, color: '#e0e0e0' }
		},
		defaultPoint: {
			focusGlow_width: 0,
			tooltip: '',
			label: {
				text: '<b>%department</b><br/>%name<br/>%position',
				autoWrap: false,
				align: 'center'
			},
			annotation: { padding: 5, margin: 2 },
			outline_width: 0,
			color: closedNodeColor
		},
		series: [{ points: orgList.slice(0, 5) }]
	});
}

function pointClick() {
	var point = this;
	if (point.options('color') == closedNodeColor) {
		point.options({
			label_text: '<b>%department</b><br/>%name<br/>%position<icon name=material/navigation/arrow-drop-up size=14 color=gray>',
			color: openedNodeColor
		});
		orgList.forEach(function (val, i) {
			if (val.parent == point.id) { chart.series(0).points.add(val); }
		})
		point.zoomTo()
	} else {
		point.options({
			label_text: '<b>%department</b><br/>%name<br/>%position<icon name=material/navigation/arrow-drop-down size=14 color=gray>',
			color: closedNodeColor
		});
		if (point.options('parent') == '') {
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
	var points = JSC.nest().key('name').pointRollup(function (key, val) {
		var result = {
			name: key,
			id: val[0].id,
			parent: val[0].parent,
			attributes: {
				department: '<span style="font-size:13px">' + val[0].department + '</span>',
				position: '<i>' + val[0].position + '</i><br/>'
			}
		};
		if (val[0].parent == '') {
			result.attributes.position = '';
			result.name = '<span style="font-size:14px">' + key + '</span>';
			result.attributes.department = '<span style="font-size:15px">' + val[0].department + '</span>';
		}
		return result;
	}).points(data);
	points.forEach(function (val1, i, arr) {
		if (i > 0) {
			arr.forEach(function (val2, j) {
				if (j > 0 && val1.id == val2.parent) {
					val1.label_text = '<b>%department</b><br/>%name<br/>%position<icon name=material/navigation/arrow-drop-down size=14 color=gray>';
					val1.events_click = pointClick;
				}
			})
		}

	})
	return points;
}