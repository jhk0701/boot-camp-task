using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    interface IFeature
    {
        string Name { get; set; }
        IScene Parent { get; set; }

        void Start();
        void Set();
        void Act();
    }
}
