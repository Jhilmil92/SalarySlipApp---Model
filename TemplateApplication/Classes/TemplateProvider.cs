using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TemplateApplication.Interfaces;

namespace TemplateApplication.Classes
{
    public class TemplateProvider:ITemplateProvider
    {
        private StreamReader _stream;
        public StreamReader SupplyTemplateStream()
        {
            var currentAssembly = typeof(ITemplateProvider).Assembly;
           // _stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApplication.Templates.DefaultTemplate.html"));
            //_stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApplication.Templates.TrialTemplate.html"));
            //_stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApplication.Templates.DummyTemplate.html"));
            //_stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApplication.Templates.SampleTemplate.html"));
            _stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApplication.Templates.FinalTemplate.html"));

            return _stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
