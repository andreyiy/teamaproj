using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class ChatUserVM
    {
        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}
