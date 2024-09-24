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
        
        protected Item[] _ownedItems = new Item[0];
        // id - isEquipped
        protected Dictionary<int, bool> _isEquipped = new Dictionary<int, bool>();
        // 장비별 1개 제약 type - id 
        protected Dictionary<EItemType, Item?> _equipment = new Dictionary<EItemType, Item?>();
        
        #endregion


        public virtual void GetDamage(int val)
        {
            Health -= val;
        }
        public virtual void Cure()
        {
            Health = MaxHealth;
        }

        public void GetItem(out Item[] owned)
        {
            owned = _ownedItems;
        }
        public void GetItem(out Item[] owned, out Dictionary<int, bool> equipped)
        {
            owned = _ownedItems;
            equipped = _isEquipped;
        }

        public void AddItem(Item item)
        {
            Item[] temp = new Item[_ownedItems.Length + 1];
            for (int i = 0; i < _ownedItems.Length; i++)
                temp[i] = _ownedItems[i];

            temp[_ownedItems.Length] = item;

            _ownedItems = temp;
        }

        public void RemoveItem(Item item)
        {
            Item[] temp = new Item[_ownedItems.Length - 1];
            int offset = 0;
            for (int i = 0; i < _ownedItems.Length; i++)
            {
                if(item.Equals(_ownedItems[i]))
                {
                    offset++;
                    continue;
                }

                temp[i - offset] = _ownedItems[i];
            }

            _ownedItems = temp;

            // 제거하려는 아이템이 이미 장착 중인 경우
            if (_isEquipped.ContainsKey(item.id) && _isEquipped[item.id])
                EquipItem(item); // 재선택을 이용한 해제
        }

        public void EquipItem(Item item)
        {
            // 장착 개선
            if (_equipment.ContainsKey(item.type))
            {
                // 교체
                // 기존 장비 해제
                if(_equipment[item.type] != null)
                {
                    Item prev = (Item)_equipment[item.type];
                    _isEquipped[prev.id] = false;
                    AdjustEquip(prev.type, prev.value, false);

                    // 선택한 장비가 이미 장착한 장비일 경우
                    // 해제까지만 진행
                    if (prev.Equals(item))
                    {
                        _equipment[item.type] = null;
                        return;
                    }
                }

                // 새 장비 반영
                _equipment[item.type] = item;
                _isEquipped[item.id] = true;
                AdjustEquip(item.type, item.value, true);
            }
            else 
            { 
                // 단순 추가
                _equipment.Add(item.type, item);
                // 반영
                _isEquipped[item.id] = true;
                AdjustEquip(item.type, item.value, true);
            }

            //if (_isEquipped.ContainsKey(item.id))
            //    _isEquipped[item.id] = !_isEquipped[item.id];
            //else
            //    _isEquipped[item.id] = true;

            //AdjustEquip(item.type, item.value, _isEquipped[item.id]);
        }

        void AdjustEquip(EItemType t, int val, bool isEquip)
        {
            switch (t)
            {
                case EItemType.Weapon:
                    EquipAttack += isEquip ? val : -val;
                    break;
                case EItemType.Armor:
                    EquipDefense += isEquip ? val : -val;
                    break;
            }
        }
    }
}
