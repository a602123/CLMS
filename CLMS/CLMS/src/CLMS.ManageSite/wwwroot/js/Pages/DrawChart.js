function BuildLineChart(DataLabels, DataCounts, elementId) {
    var parent = $("#" + elementId).parent();
    $("#" + elementId).remove();
    $("<canvas id='" + elementId + "'></canvas>").appendTo(parent);
    var ctx = $("#" + elementId);
    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: DataLabels,
            datasets: [
                {
                    label: "告警数量",
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(75,192,192,0.4)",
                    borderColor: "rgba(75,192,192,1)",
                    borderCapStyle: 'butt',
                    borderDash: [],
                    borderDashOffset: 0.0,
                    borderJoinStyle: 'miter',
                    pointBorderColor: "rgba(75,192,192,1)",
                    pointBackgroundColor: "#fff",
                    pointBorderWidth: 1,
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: "rgba(75,192,192,1)",
                    pointHoverBorderColor: "rgba(220,220,220,1)",
                    pointHoverBorderWidth: 2,
                    pointRadius: 1,
                    pointHitRadius: 10,
                    data: DataCounts,
                    spanGaps: false,
                }
            ]
        },
        options: {
        }
    });
}

function BuildPieChart(DataLabels, DataCounts, elementId) {
    var parent = $("#" + elementId).parent();
    $("#" + elementId).remove();
    $("<canvas id='" + elementId + "'></canvas>").appendTo(parent);
    var ctx = $("#" + elementId);
    var myChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: DataLabels,
            datasets: [{
                label: '售出台数',
                data: DataCounts,
                borderWidth: 1,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
            }]
        },
        options: {
        }
    });

}

function BuildDoughnutChart(DataLabels, DataCounts, elementId,text) {
    var parent = $("#" + elementId).parent();
    $("#" + elementId).remove();
    $("<canvas id='" + elementId + "'></canvas>").appendTo(parent);

    var ctx = $("#" + elementId);

    var count = 0;
    for (var i = 0; i < DataCounts.length; i++) {
        count += DataCounts[i];
    }
    var myDoughnutChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: DataLabels,
            datasets: [
                {
                    data: DataCounts,
                    backgroundColor: [
                        "#FF6384",
                        "#36A2EB",
                        "#FFCE56"
                    ],
                    hoverBackgroundColor: [
                        "#FF6384",
                        "#36A2EB",
                        "#FFCE56"
                    ]
                }]
        },
        options: {
            title: {
                display: true,
                text: text +' '+ count,
                fontSize: 13
            }
        }
    });
}