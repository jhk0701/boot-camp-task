using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    /// <summary>
    /// 휴식 기능
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
