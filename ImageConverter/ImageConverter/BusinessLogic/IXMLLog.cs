    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.BusinessLogic
{
    public interface IXMLLog
    {
        void Info(string message);
        void Debug(string message);
        void Error(string message);
    }
}
