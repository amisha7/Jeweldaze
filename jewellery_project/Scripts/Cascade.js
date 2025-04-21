$(document).ready(function () {
    $("#ddlst").change(function () {
        $.ajax({
            type: 'post',
            url: "/admin/getproduct",
            data: { stid: $('#ddlst').val() },
            datatype: 'json',
            traditional: true,
            success: (function (ls) {
                var d = "<select id='ddlct'>";
                d = d + "<option value=''>--Select Product--</option>";

                for (var i = 0; i < ls.length; i++) {

                    d = d + '<option value=' + ls[i].Value + '>' + ls[i].Text + '</option>';
                }
                d = d + '</select>';
                $('#ddlct').html(d);
            })

        })
    })
})


//$(document).ready(function () {
//    $("#ddlct").change(function () {
//        $.ajax({
//            type: 'post',
//            url: "/DropDown/getPincode",
//            data: { ctid: $('#ddlct').val() },
//            datatype: 'json',
//            traditional: true,
//            success: (function (ls) {
//                var d = "<select id='ddlpin'>";
//                d = d + "<option value=''>Select City</option>";

//                for (var i = 0; i < ls.length; i++) {

//                    d = d + '<option value=' + ls[i].Value + '>' + ls[i].Text + '</option>';
//                }
//                d = d + '</select>';
//                $('#ddlpin').html(d);
//            })

//        })
//    })
//})