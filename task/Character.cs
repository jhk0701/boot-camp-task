using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class Character
    {
        public string Name { get; protected set; }

        public EClass Class { get; protected set; }

        public int Gold { get; set; }

        #region ### STATUS ###

        int _health;
        public int Health
        {
            get { return _health; }
            protected set
            {
                _health = value;

                if (_health <= 0) // dead
                {
                    _health = 0;
                }
                else if (_health > MaxHealth)
                {
                    _health = MaxHealth;
                }
            }
        }
        public int MaxHealth { get; protected set; }

        /// <summary>
        /// 기본 공격력
        /// </summary>
        public int BaseAttack { get; protected set; }
        /// <summary>
        /// 장비 적용 공격력
        /// </summary>
        public int EquipAttack { get; protected set; }
        /// <summary>
        /// 총합 공격력
        /// </summary>
        public int Attack 
        {
            get { return BaseAttack + EquipAttack; }
        }

        /// <summary>
        /// 기본 방어력
        /// </summary>
        public int BaseDefense { get; protected set; }
        /// <summary>
        /// 장비 적용 방어력
        /// </summary>
        public int EquipDefense { get; protected set; }
        /// <summary>
        /// 총합 방어력
        /// </summary>
        public int Defense 
        { 
            get { return BaseDefense + EquipDefense; }
        }

        int _lv = 1;
        public int Level
        {
            get { return _lv; }
            protected set
            {
                if (value <= 0) return;
                _lv = value;
            }
        }

        int _exp = 0;
        public int Exp
        {
            get { return _exp; }
            set
            {
                if (value <= 0) return;
                _exp = value;

                if (_exp >= Level)
                {
                    Level++;
                    _exp -= Level;
                }
            }
        }

        #endregion


        #region ### INVENTORY ###
        
        protected Item[] _ownnedItems = new Item[0];
        // id - isEquipped
        protected Dictionary<int, bool> _isEquipped = new Dictionary<int, bool>();
        // type - id
        //protected Dictionary<EItemType, int> _equippedItems; // 장비별 1개 제약
        
        #endregion


        public virtual void GetDamage(int val)
        {
            Health -= val;
        }
        
        public void GetItem(out Item[] owned, out Dictionary<int, bool> equipped)
        {
            owned = _ownnedItems;
            equipped = _isEquipped;
        }

        public void AddItem(Item item)
        {
            Item[] temp = new Item[_ownnedItems.Length + 1];
            for (int i = 0; i < _ownnedItems.Length; i++)
                temp[i] = _ownnedItems[i];

            temp[_ownnedItems.Length] = item;

            _ownnedItems = temp;
        }

        public void RemoveItem(Item item)
        {
            Item[] temp = new Item[_ownnedItems.Length - 1];
            int offset = 0;
            for (int i = 0; i < _ownnedItems.Length; i++)
            {
                if(item.Equals(_ownnedItems[i]))
                {
                    offset++;
                    continue;
                }

                temp[i - offset] = _ownnedItems[i];
            }

            _ownnedItems = temp;
        }

        public void EquipItem(Item item) 
        {
            if (_isEquipped.ContainsKey(item.id))
                _isEquipped[item.id] = !_isEquipped[item.id];
            else
                _isEquipped[item.id] = true;

            switch (item.type)
            {
                case EItemType.Weapon:
                    EquipAttack += _isEquipped[item.id] ? item.value : -item.value;
                    break;
                case EItemType.Armor:
                    EquipDefense += _isEquipped[item.id] ? item.value : -item.value;
                    break;
            }

        }
    }
}
