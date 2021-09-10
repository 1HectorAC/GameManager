$("form[name=type_Form]").submit(function (e) {
    
    console.log("Form Submited to js");
    var $form = $(this);
    var data = $form.serialize();
    
    $.ajax({
        url: "/Games/GameTypeList",
        type: "POST",
        data: data,
        dataType: "json",
        success: AddGamesList,
        error: DetectError
        
    });
    

    e.preventDefault();
});

// Add table with some game data to page.
function AddGamesList(data) {
    $("#searchType").hide();
    

    var title = data['Title']
    var titleList = data['TitleList']
    var dateList = data['DateList']
    var systemList = data['SystemList']

    $("#tableTitle").text(title + " List");

    for (i = 0; i < titleList.length; i++) {
        var dataRow = $('<tr>');
        dataRow.append("<td>" + titleList[i] + "</td>");
        dataRow.append("<td>" + dateList[i] + "</td>");
        dataRow.append("<td>" + systemList[i] + "</td>");
        
        $("#displayType").append(dataRow);
    }

    $("#typeTable").fadeIn();
}

function DetectError() {
    console.log("error");
}

function ShowInput() {
    $("#typeTable").hide();
    $("#displayType").empty();
    $("#searchType").fadeIn();
}


// Fade console section in or out depending on type selected. 
$("#consoleButton").click(function () {
    $('#consoleSection').fadeIn();
});
$(".notConsoleButton").click(function () {
    $('#consoleSection').fadeOut();
});
