// دالة لجلب جميع المنتجات وعرضها في الجدول
function getAllData() {
    $.ajax({
        url: "https://localhost:7094/api/APIManageProducts/getProducts",
        type: "get",
        contentType: "application/json",
        success: function (result, status, xhr) {
            console.log("البيانات الراجعة:", result);

            $("tbody").html(""); // تفريغ الجدول أولاً

            let products = result.$values ?? result; // يدعم إذا كانت البيانات داخل $values أو مباشرةً

            $.each(products, function (index, value) {
                $("tbody").append($("<tr>"));
                let appendElement = $("tbody tr").last();

                appendElement.append($("<td>").html(value["id"]));
                appendElement.append($("<td>").html(value["name"]));
                appendElement.append($("<td>").html(value["price"]));
                appendElement.append($("<td>").html(value["catname"]));
                appendElement.append($("<td>").html(value["qty"]));

                let deleteButton = $("<button>")
                    .text("حذف")
                    .addClass("btn btn-danger deleteProduct")
                    .attr("data-id", value["id"]);

                appendElement.append($("<td>").append(deleteButton));
            });
        },
        error: function (xhr, status, error) {
            console.log("خطأ في تحميل البيانات:", xhr);
        }
    });
}

// تشغيل تحميل البيانات عند فتح الصفحة
getAllData();


// عند الضغط على زر الإضافة
$("#saveProducts").click(function (e) {
    e.preventDefault(); // منع إعادة تحميل الصفحة

    $.ajax({
        url: "https://localhost:7094/api/APIManageProducts/saveProducts",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify({
            name: $("#name").val(),
            price: $("#price").val(),
            qty: $("#qty").val(),
            catname: $("#cat").val() // لازم الـ id في الـ HTML يكون "cat"
        }),
        success: function (result, status, xhr) {
            console.log("تمت الإضافة:", result);
            alert("تمت الإضافة بنجاح!");
            getAllData(); // إعادة تحميل الجدول بعد الإضافة
        },
        error: function (xhr, status, error) {
            console.log("فشل في الإضافة:", xhr);
            alert("فشل في الإضافة!");
        }
    });
});


// عند الضغط على زر الحذف
$(document).on("click", ".deleteProduct", function () {
    let productId = $(this).attr("data-id");

    if (confirm("هل أنت متأكد من حذف هذا المنتج؟")) {
        $.ajax({
            url: "https://localhost:7094/api/APIManageProducts/deleteProduct/" + productId,
            type: "DELETE",
            success: function (result, status, xhr) {
                alert("تم الحذف بنجاح!");
                getAllData(); // تحديث الجدول بعد الحذف
            },
            error: function (xhr, status, error) {
                console.log("فشل في الحذف:", xhr);
                alert("فشل في الحذف!");
            }
        });
    }
});
