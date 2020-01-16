
	google.charts.load('current', {packages: ["orgchart"] });
    google.charts.setOnLoadCallback(drawChart);

    function drawChart() {
		$.ajax({
			type: "POST",
			url: 'Organization/getOrgData',
			data: '{}',
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			success: OnSuccess_getOrgData,
			error: OnErrorCall_getOrgData
		});

        function OnSuccess_getOrgData(repo) {

		    var data = new google.visualization.DataTable();
			data.addColumn('string', 'Entity');
			data.addColumn('string', 'ParentEntity');
			data.addColumn('string', 'ToolTip');

			var response = repo;
			for (var i = 0; i < response.length; i++) {
			
				var empName = response[i].name;
				var empID = response[i].id.toString();
				var reportingManager = response[i].reportingManager != undefined ? response[i].reportingManager.toString() : '';

				var designation = response[i].designation;
				var imgFile = '~/Img/Emp/' + response[i].id + '.jpg';

				data.addRows([[{
				v: empID,
					f: empName + '<div>(<span>' + designation + '</span>)</div> <div><img src="@Url.Content("~/Img/Emp/1.jpg")" class="img-thumbnail" alt="automate logo" style="width:50px;" /></div>'
				}, reportingManager, designation]]);

				
			}
			
			
			var chart = new google.visualization.OrgChart(document.getElementById('chart_div'));
						chart.draw(data, {allowHtml: true });
		}

	    function OnErrorCall_getOrgData() {
		console.log("Whoops something went wrong :( ");
		}
	}