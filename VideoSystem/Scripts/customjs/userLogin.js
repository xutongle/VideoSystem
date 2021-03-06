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

//登录
function login() {
    var code = jQuery("#code").val();
    var phone = jQuery("#phone").val();
    if (phone.length <= 0) {
        jQuery("#logininfo").text("请输入手机号!");
        return;
    }
    if (code.length <= 0)
    {
        jQuery("#logininfo").text("请输入验证码!");
        return;
    }

    jQuery.post(
        "/Login/Main",
        {
            "phone": phone,
            "code": code,
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

//获取验证码
function getCode()
{
    var phone = jQuery("#phone").val();
    if (phone.length <= 0) {
        jQuery("#logininfo").text("请输入手机号!");
        return;
    }
    
    if (!checkPhone(phone)) {
        jQuery("#logininfo").text("手机号格式不正确!");
        return;
    }

    jQuery.post(
        "/PhoneCode/Index",
        {
            "phone": phone
        },
        function (data) {
            if (data == "success") {
                jQuery("#logininfo").text("验证码已发送至您的手机!");
            }
            else{
                jQuery("#logininfo").text("验证码获取失败!");
            }
        },
        "text"
    );
}

/*
    检查手机号格式
*/
function checkPhone(phone) {
    if (!(/^1(3|4|5|7|8)\d{9}$/.test(phone))) {
        return false;
    }
    return true;
}