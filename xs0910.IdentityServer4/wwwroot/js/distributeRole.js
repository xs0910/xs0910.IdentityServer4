$('#distributeRole').on('click', function () {
    var check_ids = [];
    $('input').each(function () {
        if (this.checked) {
            check_ids.push($(this).data('id'));
        }
    })

    if (check_ids.length === 0) {
        ShowWarn("至少应该选中一个角色")
        return;
    }
    var userId = $('#UserId').val();
    var postData = {
        strLists: check_ids.join(','),
        id: userId
    };

    $.post('/UserInfo/Distribute', postData, function (data) {
        console.log(data);
        ShowSuccess(data);
    })
        .fail(function () {
            ShowFailure("删除失败，无权限！");
        });
});