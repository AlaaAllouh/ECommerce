
ffunction getAllData() {
    $.ajax({
        url: "https://localhost:7094/api/APIManageProducts/getProducts",
        type: "get",
        contentType: "application/json",
        success: function (result, status, xhr) {
            console.log(result);
            $("tbody").html("");
            $.each(result.$values, function (index, value) {
                $("tbody").append($("<tr>"));
                let appendElement = $("tbody tr").last();
                appendElement.append($("<td>").html(value["id"]));
                appendElement.append($("<td>").html(value["name"]));
                appendElement.append($("<td>").html(value["price"]));
                appendElement.append($("<td>").html(value["catname"]));
                appendElement.append($("<td>").html(value["qty"]));

                // إضافة زر الحذف
                let deleteButton = $("<button>")
                    .text("حذف")
                    .addClass("btn btn-danger deleteProduct")
                    .attr("data-id", value["id"]);

                appendElement.append($("<td>").append(deleteButton));
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr);
        }
    });
}


getAllData();

$("#saveProducts").click(function (e) {

    $.ajax({
        url: "https://localhost:7094/api/APIManageProducts/saveProducts",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify({
            name: $("#name").val(),
            price: $("#price").val(),
            qty: $("#qty").val(),
            catname: $("#catname").val()

        }),
        success: function (result, status, xhr) {
            console.log(result);
            alert("success");
            getAllData();
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            alert("failure");
        }
      
    });
});

$(document).on("click", ".deleteProduct", function () {
    let productId = $(this).attr("data-id");

    if (confirm("هل أنت متأكد من حذف هذا المنتج؟")) {
        $.ajax({
            url: "https://localhost:7094/api/APIManageProducts/deleteProduct/" + productId,
            type: "DELETE",
            success: function (result, status, xhr) {
                alert("تم الحذف بنجاح!");
                getAllData(); // تحديث البيانات بعد الحذف
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                alert("فشل في الحذف!");
            }
        });
    }
});


