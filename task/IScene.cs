using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    interface IScene
    {
        string Name { get; set; }
        Character Player { get; set; }

        void ArriveScene(Character player);
        void LeaveScene();
        void Act();
    }
}
