﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDefinition
{
    public enum EClass : int
    {
        None = 0,
        Warrior,
        Thief,
        Archer
    }
    public enum EItemType : int
    {
        Normal = 0,
        Weapon,
        Armor
    }

    public struct Item
    {
        public int id;
        public string name;
        public string description;
        public EItemType type;
        public int value;
        public int price;

        public Item (int i, string n, string desc, EItemType t, int val, int p)
        {
            id = i;
            name = n;
            description = desc;
            type = t;
            value = val;
            price = p;
        }

        public string GetDesc()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{name, -10}\t");
            sb.Append($"| {(type.Equals(EItemType.Weapon) ? "공격력" : "방어력")} +{value}\t");
            sb.Append($"| {description, -30}\t");

            return sb.ToString();
        }

        public string GetDesc(bool isSoldOut)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDesc());
            sb.Append($"| {(isSoldOut ? "구매완료" : price + " G")}\t");

            return sb.ToString();
        }
    }
    

    class DataSet
    {
        static DataSet _instance;

        Item[] _item = {
            // 방어구
            new Item(0, "수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", EItemType.Armor, 5, 1000),
            new Item(1, "무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", EItemType.Armor, 9, 2000),
            new Item(2, "스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", EItemType.Armor, 15, 3500),
            // 무기
            new Item(3, "낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", EItemType.Weapon, 2, 600),
            new Item(4, "청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", EItemType.Weapon, 5, 1500),
            new Item(5, "스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", EItemType.Weapon, 7, 2500),
        };

        public Item[] Items
        {
            get { return _item; }
        }

        private DataSet() { }
        public static DataSet GetInstance() {
            if(_instance != null) 
                return _instance;
            else
            {
                _instance = new DataSet();
                return _instance;
            }
        }

    }
}
