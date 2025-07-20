using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNS.Admin.Model
{
    public class ActionModel
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
