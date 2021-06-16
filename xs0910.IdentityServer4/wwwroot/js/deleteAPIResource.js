var Id = '';

$('.viewAPIResource').on("click", function () {
    Id = $(this).data('id');
    console.log(Id);
});

$('#delAPI').on("click", function () {
    console.log('删除按钮触发事件');
    if (Id) {
        $.post('/ApiResources/Delete/' + Id, null, function (data) {
            console.log(data);
            if (data !== null && data.success === true) {
                ShowSuccess(data.msg);

                setTimeout(function () { history.go(0); }, 2000)
            }
            else {
                ShowFailure(data.msg);
            }
        })
            .fail(function () {
                $('DeleteClient').modal('hide');
                ShowFailure("删除失败，无权限！");
            });
    }
});