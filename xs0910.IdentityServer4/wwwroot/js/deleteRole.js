var roleId = '';

$('.viewRole').on("click", function () {
    roleId = $(this).data('id');
    console.log(roleId);
});

$('#delRole').on("click", function () {
    console.log('删除按钮触发事件');
    if (roleId) {
        $.post('/RoleInfo/Delete/' + roleId, null, function (data) {
            console.log(data);
            if (data !== null && data.success === true) {
                ShowSuccess(data.msg);
                setTimeout(function () { history.go(0) }, 2000);
            }
            else {
                ShowFailure(data.msg);
            }
        })
            .fail(function () {
                $('DeleteRole').modal('hide');
                console.log("删除失败");
                ShowFailure("删除失败，无权限！");
            });
    }
});