function renderPreview(boardSize, totalShips, containerID) {
    console.log("renderPreview: " + boardSize + " - " + totalShips + " - " + containerID);
    $.ajax({
        type: "get",
        url: "/handlers/preview.ashx?b=" + boardSize + "&s=" + totalShips,
        contentType: "application/x-www-form-urlencoded",
        success: function (responseData, textStatus, jqXHR) {
            $("#" + containerID).html("");
            $("#" + containerID).html(responseData);
            console.log("renderPreview Complete");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
    return false;
} 