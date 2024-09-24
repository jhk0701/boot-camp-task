﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using task;

namespace DataDefinition
{
    /// <summary>
    /// 플레이어 직업 클래스
    /// </summary>
    public enum EClass : int
    {
        Warrior = 0,
        Thief,
        Archer
    }

    /// <summary>
    /// 아이템 타입
    /// </summary>
    public enum EItemType : int
    {
        Normal = 0,
        Weapon,
        Armor
    }

    /// <summary>
    /// 아이템 정보
    /// </summary>
    public struct Item
    {
        public int id;
        public string name;
        public string description;
        public EItemType type;
        public float value;
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
    }
    
    /// <summary>
    /// 던전 정보
    /// </summary>
    public struct Dungeon
    {
        public string name;
        public int recommendedDefense;
        public int rewardGold;

        public Dungeon(string n, int recommended, int reward)
        {
            name = n;
            recommendedDefense = recommended;
            rewardGold = reward;
        }
    }

    public struct CharacterInitData
    {
        public string name;
        public float attack;
        public float defense;
        public int maxHealth;

        public CharacterInitData(string n, float atk, float def, int maxHp)
        {
            name = n;
            attack = atk;
            defense = def;
            maxHealth = maxHp;
        }
    }

    public class GameData
    {
        // 플레이어 정보
        public Character Player { get; set; }

        /* 상점 정보가... 굳이 필요한가?

         없는 경우 : 
         게임 상, 아이템은 오로지 1개이며
         상점 열 때마다 서로 비교해줘야 함

         있는 경우 :
         게임 상, 아이템은 1개 이상이 될 수 있으며
         상점 열 때마다 별도로 비교할 필요 없음

         결론 :
         확장 및 편의성을 고려했을 때 넣는게 좋을 듯
         */
        public Dictionary<int, bool> ItemSellingInfo { get; set; } // id (int) - is sold out (bool)
    }

    class DataSet
    {
        static DataSet _instance;

        public Item[] Items { get; private set; }
        public Dungeon[] Dungeons { get; private set; }
        public CharacterInitData[] CharacterInitDatas { get; private set; }

        GameData _gameData;

        const string FILE_PATH = "./game.dat";

        private DataSet() 
        {
            // 초기화
            Items = [
                // 방어구
                new Item(0, "수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", EItemType.Armor, 5, 1000),
                new Item(1, "무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", EItemType.Armor, 9, 2000),
                new Item(2, "스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", EItemType.Armor, 15, 3500),
                // 무기
                new Item(3, "낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", EItemType.Weapon, 2, 600),
                new Item(4, "청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", EItemType.Weapon, 5, 1500),
                new Item(5, "스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", EItemType.Weapon, 7, 2500),
            ];

            Dungeons = [
                new Dungeon("쉬운 던전", 5, 1000),
                new Dungeon("일반 던전", 11, 1700),
                new Dungeon("어려운 던전", 17, 2500)
            ];

            CharacterInitDatas = [
                new CharacterInitData("전사", 10f, 10f, 100),
                new CharacterInitData("도적", 15f, 7f, 80),
            ];

            _gameData = new GameData();
            _gameData.Player = new Character();
        }

        // 싱글톤 적용
        public static DataSet GetInstance() {
            if(_instance == null)
                _instance = new DataSet();

            return _instance;
        }

        public void Save()
        {
            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.IncludeFields = true; // 내부 필드 포함
            opt.WriteIndented = true; // 띄어쓰기

            string data = JsonSerializer.Serialize(_gameData, opt);
            File.WriteAllText(FILE_PATH, data);

            // 문제 1
            // 데이터가 공백으로 출력
            // https://stackoverflow.com/questions/58784499/system-text-json-jsonserializer-serialize-returns-empty-json-object
            // https://stackoverflow.com/questions/58139759/how-to-use-class-fields-with-system-text-json-jsonserializer
            // 해결
            // In .NET Core 3.x 버전에선 클래스 필드를 직열화할 수 없었음.
            // In .NET 5 버전 이후부터 옵션을 추가해서 내부 필드를 직열화할 수 있게 됨.
        }
        public void Save(Character player) 
        { 
            _gameData.Player = player;
            Save();
        }
        public void Save(Character player, Dictionary<int, bool> isSold)
        {
            _gameData.Player = player;
            _gameData.ItemSellingInfo = isSold;
            Save();
        }

        public bool Load()
        {
            if (File.Exists(FILE_PATH))
            {
                //유효성 체크 필요
                string text = File.ReadAllText(FILE_PATH);
                return IsVaild(text, out _gameData);
            }
            else
                return false;
        }
        public Character LoadCharater()
        {
            return _gameData.Player;
        }

        bool IsVaild(string text, out GameData data)
        {
            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.PropertyNameCaseInsensitive = true;

            data = JsonSerializer.Deserialize<GameData>(text, opt);
            // 문제 2
            // class deserialize 시 null
            // 1. 접근성
            // 2. 모델 프로퍼티화

            if (data.Player == null)
                return false;

            CharacterInitData initData = CharacterInitDatas[(int)data.Player.Class];
            // 민감한 사항
            // 공격력, 방어력, 체력 관련 수치 조작
            float expectedAttack = data.Player.Level * 0.5f + initData.attack;
            float expectedDefense = data.Player.Level * 1f + initData.defense;
            if (data.Player.BaseAttack > expectedAttack ||
                data.Player.BaseDefense > expectedDefense || 
                data.Player.MaxHealth != initData.maxHealth)
                return false;

            // 더 높은 수준의 유효성 검사가 필요함.

            return true;
        }

        void DebugDeserialize(GameData data)
        {
            Console.WriteLine($"Player name : {data.Player.Name}");
            Console.WriteLine($"Player BaseAttack : {data.Player.BaseAttack}");
            Console.WriteLine($"Player EquipAttack : {data.Player.EquipAttack}");
            Console.WriteLine($"Player  : {data.Player.EquipAttack}");
            Console.WriteLine($"ItemSellingInfo : {data.ItemSellingInfo.Count}");
        }
    }
}
