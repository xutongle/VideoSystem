
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSystem.Models;

namespace VideoSystem.Abstract
{
    public interface IExportExcel
    {
        DataTable MakeDataTable(Code[] codeArray);
        byte[] WriteExcel(Code[] codeArray);
    }
}
