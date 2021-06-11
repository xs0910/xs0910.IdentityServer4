var roleId = '';

$('.viewRole').on("click", function () {
    roleId = $(this).data('id');
    console.log(roleId);
});

$('#delRole').on("click", function () {
    console.log('删除按钮触发事件');
    if (roleId) {
        $.post('/RoleInfo/Delete/' + roleId, null, function (data) {
            if (data.length > 0) {
                console.log(data);
                ShowFailure(data);
            } else {
                history.go(0);
            }
        })
            .fail(function () {
                $('DeleteUser').modal('hide');
                console.log("删除失败");
                ShowFailure("删除失败，无权限！");
            });
    }
});