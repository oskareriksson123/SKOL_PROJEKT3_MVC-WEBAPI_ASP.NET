//$("#apiCallButton").click(function () {
//    $.ajax({
//        url: "api/VarApi",
//        method: "post"
//    }).done(function (data) {
//        $("#displayTxt").text(data);
//    });
//})


$("#Skicka").click(function () {

    $.ajax({

        url: '/api/VarApi',

        type: 'POST',

        method: 'Post',

        dataType: 'json',

        data: $('#form2').serialize(),

        success: function (data, textStatus, xhr) {

            console.log(data);

        },

        error: function (xhr, textStatus, errorThrown) {

            console.log(textStatus);

        }

    });

});