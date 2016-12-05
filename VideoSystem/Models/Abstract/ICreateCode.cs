using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSystem.Models;

namespace VideoSystem.Abstract
{
    public interface ICreateCode
    {
        List<string> createCode(int count, Video v);
    }
}
