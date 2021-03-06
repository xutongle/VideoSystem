jQuery(document).ready(function () {
    //默认设置
    var fp1 = new Fingerprint();
    //canvas设置
    var fp2 = new Fingerprint({ canvas: true });
    //插件设置
    var fp3 = new Fingerprint({ ie_activex: true });
    //屏幕设置
    var fp4 = new Fingerprint({ screen_resolution: true });

    var str = "" + fp1.get() + fp2.get() + fp3.get() + fp4.get();

    jQuery("#userLoginForm").append("<input type='hidden' id='UserBrowser' name='UserBrowser' value='" + $.md5(str) + "'/>");
    jQuery.get(
        "/Login/GetUserBrowser",
        { "UserBrowser": $.md5(str) },
        function (data) {
            //alert(data);
        },
        "text");
});

function login() {
    var password = jQuery("#password").val();
    var account = jQuery("#account").val();
    if (password.length <= 0)
    {
        jQuery("#logininfo").text("请输入密码!");
        return;
    }
    if (account.length<=0)
    {
        jQuery("#logininfo").text("请输入账号!");
        return;
    }

    jQuery.post(
        "/Login/Main",
        {
            "account": account,
            "password": $.md5(password).toLocaleUpperCase(),
            "UserBrowser": jQuery("#UserBrowser").val()
        },
        function (data) {
            if (data == "success") {
                location.href = "/Home";
            }
            else if (data == "error") {
                jQuery("#logininfo").text("账号或密码错误!");
            }
            else {
                jQuery("#logininfo").text("您无权在此电脑登陆!");
            }
        },
        "text"
    );
}