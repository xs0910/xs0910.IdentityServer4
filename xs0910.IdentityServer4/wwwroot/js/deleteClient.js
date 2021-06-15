var Id = '';

$('.viewClient').on("click", function () {
    Id = $(this).data('id');
    console.log(Id);
});

$('#delClient').on("click", function () {
    console.log('删除按钮触发事件');
    if (Id) {
        $.post('/Clients/Delete/' + Id, null, function (data) {
            console.log(data);
            if (data !== null && data.Success) {
                ShowSuccess(data.Msg);
                // history.go(0);
            }
            else {
                ShowFailure(data.Msg);
            }
        })
            .fail(function () {
                $('DeleteClient').modal('hide');
                ShowFailure("删除失败，无权限！");
            });
    }
});