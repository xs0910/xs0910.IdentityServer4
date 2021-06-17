var userId = '';

$('.viewUser').on("click", function () {
    userId = $(this).data('id');
    console.log(userId);
});

$('#delUser').on("click", function () {
    console.log('删除按钮触发事件');
    if (userId) {
        $.post('/UserInfo/Delete/' + userId, null, function (data) {
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
                $('DeleteUser').modal('hide');
                ShowFailure("删除失败，无权限！");
            });
    }
});