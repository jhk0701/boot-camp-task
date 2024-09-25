using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    interface IScene
    {
        /// <summary>
        /// 마을 이름
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 마을에 온 플레이어
        /// </summary>
        Character Player { get; set; }

        /// <summary>
        /// 마을에서 할 수 있는 기능들
        /// </summary>
        IFeature[] Features { get; set; }

        void Start();
        void ArriveScene(Character player);
        void LeaveScene();

        void Act();
    }
}
