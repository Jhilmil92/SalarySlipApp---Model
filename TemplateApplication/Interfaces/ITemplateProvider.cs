using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateApplication.Interfaces
{
    public interface ITemplateProvider:IDisposable
    {
        StreamReader SupplyTemplateStream();
    }
}
