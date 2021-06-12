var userId = '';

$('.viewUser').on("click", function () {
    userId = $(this).data('id');
    console.log(userId);
});

$('#delUser').on("click", function () {
    console.log('删除按钮触发事件');
    if (userId) {
        $.post('/UserInfo/Delete/' + userId, null, function (data) {
            if (data.length > 0) {
                console.log(data);
                ShowFailure(data);
            }
            else {
                history.go(0);
            }
        })
            .fail(function () {
                $('DeleteUser').modal('hide');
                ShowFailure("删除失败，无权限！");
            });
    }
});