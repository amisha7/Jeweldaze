$(document).ready(function () {
    $("#ddlst").change(function () {
        $.ajax({
            type: 'post',
            url: "/User_new/getcity",
            data: { stid: $('#ddlst').val() },
            datatype: 'json',
            traditional: true,
            success: (function (ls) {
                var d = "<select id='ddlct'>";
                d = d + "<option value=''>--Select City--</option>";

                for (var i = 0; i < ls.length; i++) {

                    d = d + '<option value=' + ls[i].Value + '>' + ls[i].Text + '</option>';
                }
                d = d + '</select>';
                $('#ddlct').html(d);
            })

        })
    })
})