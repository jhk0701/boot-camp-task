using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    public class Character
    {
        public string Name { get; set; }

        public EClass Class { get; set; }

        public int Gold { get; set; }


        #region ### STATUS ###

        int _health;
        /// <summary>
        /// 현재 생명력
        /// </summary>
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
        /// <summary>
        /// 최대 생명력
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// 기본 공격력
        /// </summary>
        public float BaseAttack { get; set; }
        /// <summary>
        /// 장비 적용 공격력
        /// </summary>
        public float EquipAttack { get; set; }
        /// <summary>
        /// 총합 공격력
        /// </summary>
        public float Attack
        {
            get { return BaseAttack + EquipAttack; }
        }

        /// <summary>
        /// 기본 방어력
        /// </summary>
        public float BaseDefense { get; set; }
        /// <summary>
        /// 장비 적용 방어력
        /// </summary>
        public float EquipDefense { get; set; }
        /// <summary>
        /// 총합 방어력
        /// </summary>
        public float Defense
        {
            get { return BaseDefense + EquipDefense; }
        }

        int _lv = 1;
        /// <summary>
        /// 현재 레벨
        /// </summary>
        public int Level
        {
            get { return _lv; }
            set
            {
                if (value <= 0) return;
                _lv = value;
                OnLevelChanged?.Invoke(_lv);
            }
        }

        int _exp = 0;
        /// <summary>
        /// 현재 경험치
        /// </summary>
        public int Exp
        {
            get { return _exp; }
            set
            {
                if (value <= 0) return;
                _exp = value;

                if (_exp >= Level)
                {
                    _exp -= Level;
                    Level++;
                }
            }
        }

        /// <summary>
        /// 레벨 변경 시 호출
        /// </summary>
        protected Action<int> OnLevelChanged;

        #endregion


        #region ### INVENTORY ###

        public Item[] OwnedItems { get; set; }
        // id - isEquipped
        public Dictionary<int, bool> IsEquipped { get; set; }
        // 장비별 1개 제약 type - Item 
        public Dictionary<EItemType, Item?> Equipment { get; set; }

        #endregion

        public Character()
        {
            // 초기화
            OwnedItems = new Item[0];
            IsEquipped = new Dictionary<int, bool>();
            Equipment = new Dictionary<EItemType, Item?>();
        }

        protected void Initialize(EClass eClass)
        {
            Level = 1;
            CharacterInitData initData = DataSet.GetInstance().CharacterInitDatas[(int)Class];

            MaxHealth = initData.maxHealth;
            Health = MaxHealth;

            BaseAttack = initData.attack;
            BaseDefense = initData.defense;

            OnLevelChanged = (int lv) =>
            {
                Console.WriteLine($"레벨이 올랐습니다!\n현재 레벨 : {lv}\n");
                Console.WriteLine($"기본 공격력과 방어력이 소폭 상승했습니다.\n");
                BaseAttack += 0.5f;
                BaseDefense += 1f;
            };
        }

        public virtual void GetDamage(int val)
        {
            Health -= val;
        }
        public virtual void Cure()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// 인벤토리에 아이템 추가
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            Item[] temp = new Item[OwnedItems.Length + 1];
            for (int i = 0; i < OwnedItems.Length; i++)
                temp[i] = OwnedItems[i];

            temp[OwnedItems.Length] = item;

            OwnedItems = temp;
        }

        /// <summary>
        /// 인벤토리에서 아이템 제거
        /// 장착 아이템일 시, 자동 해제
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(Item item)
        {
            Item[] temp = new Item[OwnedItems.Length - 1];
            int offset = 0;
            for (int i = 0; i < OwnedItems.Length; i++)
            {
                if (item.Equals(OwnedItems[i]))
                {
                    offset++;
                    continue;
                }

                temp[i - offset] = OwnedItems[i];
            }

            OwnedItems = temp;

            // 제거하려는 아이템이 이미 장착 중인 경우
            if (IsEquipped.ContainsKey(item.id) && IsEquipped[item.id])
                EquipItem(item); // 재선택을 이용한 해제
        }

        /// <summary>
        /// 아이템 장착
        /// 장착 개선으로 타입별 1개만 착용 가능
        /// </summary>
        /// <param name="item"></param>
        public void EquipItem(Item item)
        {
            // 장착 개선
            if (Equipment.ContainsKey(item.type))
            {
                // 교체
                // 기존 장비 해제
                if (Equipment[item.type] != null)
                {
                    Item prev = (Item)Equipment[item.type];
                    IsEquipped[prev.id] = false;
                    AdjustEquip(prev.type, prev.value, false);

                    // 선택한 장비가 이미 장착한 장비일 경우
                    // 해제까지만 진행
                    if (prev.Equals(item))
                    {
                        Equipment[item.type] = null;
                        return;
                    }
                }

                // 새 장비 반영
                Equipment[item.type] = item;
                IsEquipped[item.id] = true;
                AdjustEquip(item.type, item.value, true);
            }
            else
            {
                // 단순 추가
                Equipment.Add(item.type, item);
                // 반영
                IsEquipped[item.id] = true;
                AdjustEquip(item.type, item.value, true);
            }

            // 장착 기능
            //if (_isEquipped.ContainsKey(item.id))
            //    _isEquipped[item.id] = !_isEquipped[item.id];
            //else
            //    _isEquipped[item.id] = true;

            //AdjustEquip(item.type, item.value, _isEquipped[item.id]);
        }

        /// <summary>
        /// 장착한 아이템 적용
        /// </summary>
        /// <param name="t"></param>
        /// <param name="val"></param>
        /// <param name="isEquip"></param>
        void AdjustEquip(EItemType t, float val, bool isEquip)
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
