//用户意见反馈
function submitSuggest() {
    var suggestText = jQuery("#suggestText");
    if (suggestText.val().length <= 0) {
        $.jBox.tip('反馈内容不能为空', 'error', { focusId: "suggestText" });
        return;
    }
    jQuery.post(
        "/Home/UserSuggest",
        { "suggestText": suggestText.val() },
        function (data) {
            if (data == "反馈成功") {
                suggestText.val("");
            }
            $.jBox.tip(data, "提示");
        },
            "text"
        );
}