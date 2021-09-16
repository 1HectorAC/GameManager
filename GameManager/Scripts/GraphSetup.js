function BarGraphSetup(canvasId,graphTitle, xValues, yValues) {
    var g1 = '#38ef7d';
    var g2 = '#11998e';
    var g3 = '#05665b';
    var color1 = [g1, g2, g1, g2, g1, g2, g1, g2, g1, g2, g1, g2];
    var color2 = [g3, g3, g3, g3, g3, g3, g3, g3, g3, g3, g3, g3,];

    new Chart(canvasId, {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: color1,
                data: yValues,
                borderColor: color2,
                borderWidth: 2
            }]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: graphTitle,
                fontSize: 25
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}
function PieGraphSetup(canvasId, graphTitle, xValues, yValues) {
    var g3 = '#05665b';
    var color1 = ['#38ef7d', '#18AD94', '#12E8DD', '#2E96E6', '#315AA6', '#656EE6', '#7DE843', '#6ADB00', '#63C757', '#59A6CF' ];
    var color2 = [g3, g3, g3, g3, g3, g3, g3, g3, g3, g3, g3, g3,];


    new Chart(canvasId, {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: color1,
                data: yValues,
                borderColor: color2
            }]
        },
        options: {
            title: {
                display: true,
                text: graphTitle,
                fontSize: 25
            },
        }
    });
}