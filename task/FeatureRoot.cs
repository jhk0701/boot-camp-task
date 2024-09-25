using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    /// <summary>
    /// 기능 기본 구현 클래스
    /// </summary>
    class FeatureRoot : IFeature
    {
        public string Name { get; set; }
        public IScene Parent { get; set; }

        public FeatureRoot(string name, IScene parent)
        {
            Name = name;
            Parent = parent;
        }

        public virtual void Start() {
            Console.Clear();
            Set();
            Act();
        }
        public virtual void Set() { }
        public virtual void Act() { }
    }
}
