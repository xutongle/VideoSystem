var pass = jQuery("#UserPassword");
var confirmPass = jQuery("#UserConfirmPassword");
var accountElement = jQuery("#UserAccount");
var verifycode = jQuery("#verifycode");

/*
   检查账号是否存在
*/
function isAccountExist() {
    if (accountElement.val().length <= 0) {
        jQuery("#info").text("账号不能为空!");
        return;
    }

    jQuery.post(
        "/Login/IsRegistAccountExist",
        { "account": accountElement.val() },
        function (data, statusText, xhr) {
            if (data == "erro") {
                jQuery("#info").text("此账号已存在，请重新输入!");
                accountElement.val("");
                return;
            }
        },
        "text"
    );
}

/*
   检查密码长度
*/
function passLenth() {
    if (pass.val().length <= 0) {
        jQuery("#info").text("密码不能为空！");
        return;
    }
    if (pass.val().length < 8) {
        jQuery("#info").text("密码长度不能小于8位！");
        return;
    }
}

/*
    检查密码是否一致
*/
function isPassSame() {

    if (pass.val().length > 0) {
        if (confirmPass.val() != pass.val()) {
            jQuery("#info").text("两次密码不一致，请重新输入!");
            confirmPass.val("");
            return;
        }
    }
}

//设定倒数秒数  
var t = 5;
//显示倒数秒数  
function showTime() {
    t -= 1;
    jQuery("#info").text("注册成功，将在" + t + "秒后跳转登陆页面");
    if (t == 0) {
        location.href = '/Login';
    }
    //每秒执行一次,showTime()  
    setTimeout("showTime()", 1000);
}

//加密密码
function regist() {
    var password = jQuery("#UserPassword").val();
    var UserConfirmPassword = jQuery("#UserConfirmPassword").val();

    if (jQuery("#UserAccount").val().length<=0)
    {
        jQuery("#info").text("请填写帐号");
        return;
    }
    if (password.length<=0)
    {
        jQuery("#info").text("请填写密码");
        return;
    }
    if (UserConfirmPassword.length <= 0) {
        jQuery("#info").text("请确认密码");
        return;
    }
    if (jQuery("#email").val().length <= 0) {
        jQuery("#info").text("请填写邮箱");
        return;
    }
    if (jQuery("#phone").val().length <= 0) {
        jQuery("#info").text("请填写电话");
        return;
    }
    if (jQuery("#code").val().length <= 0) {
        jQuery("#info").text("请填写验证码");
        return;
    }

    var passMD5 = $.md5(password).toLocaleUpperCase();
    var passConfirmMD5 = $.md5(UserConfirmPassword).toLocaleUpperCase();

    jQuery.post(
        "/Login/Regist",
        {
            "UserAccount": jQuery("#UserAccount").val(),
            "UserPassword": passMD5,
            "UserConfirmPassword": passConfirmMD5,
            "UserEmail": jQuery("#email").val(),
            "UserPhone": jQuery("#phone").val(),
            "UserBrowser1": "no",
            "UserBrowser2": "no",
            "UserBrowser3": "no",
            "Uniq": "no",
            "From": "注册"
        },
        function (data) {
            //注册成功
            if (data == "success") {
                showTime();
            }
            //注册失败
            else {
                jQuery("#info").text("注册失败");
            }
        },
        "text");
}