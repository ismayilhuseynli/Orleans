using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.GrainClasses;

public class RobotState
{
    public Queue<string> Instructions { get; set; }
        = new Queue<string>();
}

