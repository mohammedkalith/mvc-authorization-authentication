using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCAuthenticationSample
{

    public enum Modules : byte
    {      
        Administration = 1,
        Reporting = 2,
        Invoice = 3
    }

    [Flags]
    public enum Functions
    {
        Access = 1,
        ManageUsers = 2,
        ManageInvoices = 4,
        GenerateReports = 8
       
    }
}
