using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avae.DAL
{
    //[MessagePackObject]
    public class Result
    {
        //[Key(0)]
        public string? Exception { get; set; }

        //[Key(1)]
        public bool Success { get; set; }
    }
}
