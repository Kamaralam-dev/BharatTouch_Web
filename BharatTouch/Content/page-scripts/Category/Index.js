var listCategory = function () {
    var me = this;

    var upsertCategoryForm = function () {
        $("#CategoryInsertForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                CategoryName: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: f.serializeArray(),
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);                        
                        bindCategories();
                        $("#CategoryName").val('');
                        $("#CategoryDescription").val('');
                        $("#IsActive").prop("checked", false);
                    },
                    error: handleAjaxError()
                });
            }
        });
    }

    var upsertSubCategoryForm = function () {
        $("#upsertSubCategoryForm").validate({
            errorPlacement: function (error, element) {
                error.appendTo(element.parent());
            },
            rules: {
                CategoryName: { required: true },
            },
            submitHandler: function (form) {
                var f = $(form);
                $.ajax({
                    type: 'POST',
                    url: f[0].action,
                    data: f.serializeArray(),
                    dataType: 'json',
                    success: function (data, strStatus) {
                        showMessage(data.Message, data.Data, data.Type);
                        bindCategories();
                        $("#CategoryName").val('');
                        $("#CategoryDescription").val('');
                        $("#IsActive").prop("checked", false);
                        CloseModal();
                    },
                    error: handleAjaxError()
                });
            }
        });


    }

    var bindCategories = function () {
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/Category/GetCategories',
            dataType: 'html',
            success: function (data, strStatus) {
                $("#categoryListContainer").html(data);
            },
            error: handleAjaxError()
        });
    }

    var openSubcategoryModel = function (catId,pid,pCategory) {
        $.ajax({
            type: 'GET',
            cache: false,
            data: { categoryId: catId, parentCategoryId: pid, parentCategoryName: pCategory },
            url: '/Category/Upsert',
            dataType: 'html',
            success: function (data, strStatus) {               
                $("body").append(data);               
                OpenModel("CategoryModal");
                upsertSubCategoryForm();               
            },
            error: handleAjaxError()
        });
    }

    var deleteCategory = function (catId) {
        var r = confirm("Are you sure to delete this!");
        if (r == true) {
            $.ajax({
                type: 'POST',
                cache: false,
                data: { catId: catId},
                url: '/Category/DeleteCategory',
                dataType: 'json',
                success: function (data, strStatus) {
                    showMessage(data.Message, data.Data, data.Type);
                    bindCategories();
                },
                error: handleAjaxError()
            });
        }
    }

    this.init = function () {

        $("#categoryListContainer").on("click", "a,button", function () {
            var action = $(this).data("action");
          
            switch (action) {
                case "add-subcat":
                    var parentid = $(this).data('parentid');
                    var category = $(this).data('category');
                    openSubcategoryModel(0,parentid, category);
                    break;
                case "edit":
                    var catId = $(this).data('id');
                    var category = $(this).data('category');
                    openSubcategoryModel(catId,0, category);
                    break;
                case "delete":
                    var catId = $(this).data('id');
                    deleteCategory(catId);
                    break;
                case "expand_l1":
                    var catId = $(this).data('id');
                    $("#btnExpand_l1_" + catId).hide();
                    $("#btnCollapse_l1_" + catId).show();
                    $("#tblSubCategory_l1_" + catId).show();
                    break;
                case "collapse_l1":
                    var catId = $(this).data('id');
                    $("#btnExpand_l1_" + catId).show();
                    $("#btnCollapse_l1_" + catId).hide();                  
                    $("#tblSubCategory_l1_" + catId).hide();
                    break;
                case "expand_l2":
                    var catId = $(this).data('id');
                    $("#btnExpand_l2_" + catId).hide();
                    $("#btnCollapse_l2_" + catId).show();
                    $("#tblSubCategory_l2_" + catId).show();
                    break;
                case "collapse_l2":
                    var catId = $(this).data('id');
                    $("#btnExpand_l2_" + catId).show();
                    $("#btnCollapse_l2_" + catId).hide();
                    $("#tblSubCategory_l2_" + catId).hide();
                    break;
            }
        });

        $("#btnAddCategory").on("click", function () {
            openSubcategoryModel(0, 0, "");
        });

        upsertCategoryForm();
    }
}

