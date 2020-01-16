$("#apiCallButton").click(function () {
    $.ajax({
        url: "api/ApiWeb",
        method: "post"
    }).done(function (data) {
        $("#displayTxt").text(data);
    });
})