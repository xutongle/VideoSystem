using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoSystem.Abstract
{
    public interface IVerifyCode
    {
        //生成验证码
        string CreateValidateCode(int codeLength);
        //创建验证码的图片
        byte[] CreateValidateGraphic(string validateCode);
    }
}
