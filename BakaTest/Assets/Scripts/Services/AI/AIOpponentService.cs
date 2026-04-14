#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Champions;

namespace BakaTest.Services.AI
{
    /// <summary>
    /// AI対戦相手管理サービスの実装
    /// </summary>
    public class AIOpponentService : IAIOpponentService
    {
        private readonly System.Random _random = new System.Random();

        // 難易度別の総ポイント数
        private readonly Dictionary<AIDifficulty, int> _difficultyPoints = new Dictionary<AIDifficulty, int>
        {
            { AIDifficulty.Easy, 800 },     // 簡単：800ポイント
            { AIDifficulty.Medium, 1600 },  // 普通：1600ポイント
            { AIDifficulty.Hard, 2400 }     // 難しい：2400ポイント
        };

        // 難易度別のAI名前リスト
        private readonly Dictionary<AIDifficulty, List<string>> _aiNames = new Dictionary<AIDifficulty, List<string>>
        {
            {
                AIDifficulty.Easy, new List<string>
                {
                    "初心者", "新米", "ルーキー", "ビギナー", "見習い",
                    "駆け出し", "初級者", "新参者", "新人", "未熟者"
                }
            },
            {
                AIDifficulty.Medium, new List<string>
                {
                    "中級者", "エキスパート", "戦士", "挑戦者", "勇者",
                    "闘士", "熟練者", "ベテラン", "達人", "プロ"
                }
            },
            {
                AIDifficulty.Hard, new List<string>
                {
                    "マスター", "覇者", "チャンピオン", "帝王", "猛者",
                    "伝説", "最強", "究極", "神", "無敵"
                }
            }
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AIOpponentService()
        {
            Debug.Log("[AIOpponentService] Initialized.");
        }

        /// <summary>
        /// 難易度に基づく総ポイント数を取得します
        /// </summary>
        public int GetTotalPointsForDifficulty(AIDifficulty difficulty)
        {
            return _difficultyPoints[difficulty];
        }

        /// <summary>
        /// AIのポイント配分を生成します
        /// </summary>
        public Dictionary<Subject, int> AllocatePoints(AIDifficulty difficulty, AIPersonality personality, ChampionData champion)
        {
            int totalPoints = GetTotalPointsForDifficulty(difficulty);
            
            Debug.Log($"[AIOpponentService] Allocating {totalPoints} points for {difficulty} difficulty, {personality} personality");

            return personality switch
            {
                AIPersonality.Balanced => AllocateBalanced(totalPoints),
                AIPersonality.Aggressive => AllocateAggressive(totalPoints),
                AIPersonality.Defensive => AllocateDefensive(totalPoints),
                AIPersonality.Tactical => AllocateTactical(totalPoints, champion),
                AIPersonality.Random => AllocateRandom(totalPoints),
                _ => AllocateBalanced(totalPoints)
            };
        }

        /// <summary>
        /// バランス型：全ステータスに均等配分
        /// </summary>
        private Dictionary<Subject, int> AllocateBalanced(int totalPoints)
        {
            int pointsPerSubject = totalPoints / 4;
            int remainder = totalPoints % 4;

            var allocation = new Dictionary<Subject, int>
            {
                { Subject.Math, pointsPerSubject },
                { Subject.Science, pointsPerSubject },
                { Subject.English, pointsPerSubject },
                { Subject.History, pointsPerSubject }
            };

            // 余りをランダムに配分
            var subjects = new List<Subject> { Subject.Math, Subject.Science, Subject.English, Subject.History };
            for (int i = 0; i < remainder; i++)
            {
                var subject = subjects[_random.Next(subjects.Count)];
                allocation[subject]++;
            }

            Debug.Log($"[AIOpponentService] Balanced allocation: Math={allocation[Subject.Math]}, Science={allocation[Subject.Science]}, English={allocation[Subject.English]}, History={allocation[Subject.History]}");
            return allocation;
        }

        /// <summary>
        /// 攻撃型：攻撃力重視（Math, Englishに多め）
        /// </summary>
        private Dictionary<Subject, int> AllocateAggressive(int totalPoints)
        {
            // Math(Attack) 40%, English(Speed) 30%, Science(Defense) 15%, History(HP) 15%
            var allocation = new Dictionary<Subject, int>
            {
                { Subject.Math, (int)(totalPoints * 0.40f) },
                { Subject.English, (int)(totalPoints * 0.30f) },
                { Subject.Science, (int)(totalPoints * 0.15f) },
                { Subject.History, (int)(totalPoints * 0.15f) }
            };

            // 端数調整
            int allocated = allocation.Values.Sum();
            int remainder = totalPoints - allocated;
            allocation[Subject.Math] += remainder; // 余りは攻撃力に

            Debug.Log($"[AIOpponentService] Aggressive allocation: Math={allocation[Subject.Math]}, Science={allocation[Subject.Science]}, English={allocation[Subject.English]}, History={allocation[Subject.History]}");
            return allocation;
        }

        /// <summary>
        /// 防御型：耐久力重視（Science, Historyに多め）
        /// </summary>
        private Dictionary<Subject, int> AllocateDefensive(int totalPoints)
        {
            // History(HP) 35%, Science(Defense) 35%, Math(Attack) 15%, English(Speed) 15%
            var allocation = new Dictionary<Subject, int>
            {
                { Subject.History, (int)(totalPoints * 0.35f) },
                { Subject.Science, (int)(totalPoints * 0.35f) },
                { Subject.Math, (int)(totalPoints * 0.15f) },
                { Subject.English, (int)(totalPoints * 0.15f) }
            };

            // 端数調整
            int allocated = allocation.Values.Sum();
            int remainder = totalPoints - allocated;
            allocation[Subject.History] += remainder; // 余りはHPに

            Debug.Log($"[AIOpponentService] Defensive allocation: Math={allocation[Subject.Math]}, Science={allocation[Subject.Science]}, English={allocation[Subject.English]}, History={allocation[Subject.History]}");
            return allocation;
        }

        /// <summary>
        /// 戦術型：チャンピオンの親和性に最適化
        /// </summary>
        private Dictionary<Subject, int> AllocateTactical(int totalPoints, ChampionData champion)
        {
            // チャンピオンの親和性比率を取得
            float mathRatio = champion.subjectAffinity.MathToAttackRatio;
            float scienceRatio = champion.subjectAffinity.ScienceToDefenseRatio;
            float englishRatio = champion.subjectAffinity.EnglishToSpeedRatio;
            float historyRatio = champion.subjectAffinity.HistoryToHPRatio;

            float totalRatio = mathRatio + scienceRatio + englishRatio + historyRatio;

            // 親和性に基づいてポイント配分
            var allocation = new Dictionary<Subject, int>
            {
                { Subject.Math, (int)(totalPoints * (mathRatio / totalRatio)) },
                { Subject.Science, (int)(totalPoints * (scienceRatio / totalRatio)) },
                { Subject.English, (int)(totalPoints * (englishRatio / totalRatio)) },
                { Subject.History, (int)(totalPoints * (historyRatio / totalRatio)) }
            };

            // 端数調整（最大親和性の教科に追加）
            int allocated = allocation.Values.Sum();
            int remainder = totalPoints - allocated;
            
            float maxRatio = Math.Max(Math.Max(mathRatio, scienceRatio), Math.Max(englishRatio, historyRatio));
            Subject maxSubject = Subject.Math;
            if (scienceRatio == maxRatio) maxSubject = Subject.Science;
            else if (englishRatio == maxRatio) maxSubject = Subject.English;
            else if (historyRatio == maxRatio) maxSubject = Subject.History;
            
            allocation[maxSubject] += remainder;

            Debug.Log($"[AIOpponentService] Tactical allocation for {champion.championName}: Math={allocation[Subject.Math]}, Science={allocation[Subject.Science]}, English={allocation[Subject.English]}, History={allocation[Subject.History]}");
            return allocation;
        }

        /// <summary>
        /// ランダム型：完全ランダム配分
        /// </summary>
        private Dictionary<Subject, int> AllocateRandom(int totalPoints)
        {
            var allocation = new Dictionary<Subject, int>
            {
                { Subject.Math, 0 },
                { Subject.Science, 0 },
                { Subject.English, 0 },
                { Subject.History, 0 }
            };

            var subjects = new List<Subject> { Subject.Math, Subject.Science, Subject.English, Subject.History };

            // ランダムに配分（最低でも各教科に5%は配分）
            int minPerSubject = totalPoints / 20; // 5%
            foreach (var subject in subjects)
            {
                allocation[subject] = minPerSubject;
            }

            int remainingPoints = totalPoints - (minPerSubject * 4);

            // 残りをランダムに配分
            while (remainingPoints > 0)
            {
                var randomSubject = subjects[_random.Next(subjects.Count)];
                int pointsToAdd = Math.Min(remainingPoints, _random.Next(1, 51)); // 1-50ポイント
                allocation[randomSubject] += pointsToAdd;
                remainingPoints -= pointsToAdd;
            }

            Debug.Log($"[AIOpponentService] Random allocation: Math={allocation[Subject.Math]}, Science={allocation[Subject.Science]}, English={allocation[Subject.English]}, History={allocation[Subject.History]}");
            return allocation;
        }

        /// <summary>
        /// チャンピオンを選択します
        /// </summary>
        public ChampionData? SelectChampion(List<ChampionData> availableChampions, AIDifficulty difficulty)
        {
            if (availableChampions == null || availableChampions.Count == 0)
            {
                Debug.LogWarning("[AIOpponentService] No champions available for selection!");
                return null;
            }

            ChampionData? selected = null;

            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    // 簡単：ランダムに選択
                    selected = availableChampions[_random.Next(availableChampions.Count)];
                    break;

                case AIDifficulty.Medium:
                    // 普通：ロール別にバランスよく選択
                    selected = SelectByRole(availableChampions);
                    break;

                case AIDifficulty.Hard:
                    // 難しい：最強のチャンピオンを選択（総合ステータス重視）
                    selected = SelectStrongest(availableChampions);
                    break;
            }

            Debug.Log($"[AIOpponentService] Selected champion: {selected?.championName} for {difficulty} difficulty");
            return selected;
        }

        /// <summary>
        /// ロール別にバランスよく選択
        /// </summary>
        private ChampionData SelectByRole(List<ChampionData> champions)
        {
            // 各ロールからランダムに選択
            var roles = new List<ChampionRole>
            {
                ChampionRole.Tank,
                ChampionRole.DPS,
                ChampionRole.Support,
                ChampionRole.Mage,
                ChampionRole.Assassin
            };

            var randomRole = roles[_random.Next(roles.Count)];
            var championsInRole = champions.Where(c => c.role == randomRole).ToList();

            if (championsInRole.Count > 0)
            {
                return championsInRole[_random.Next(championsInRole.Count)];
            }

            // 該当ロールがいない場合はランダム
            return champions[_random.Next(champions.Count)];
        }

        /// <summary>
        /// 最強のチャンピオンを選択（総合ステータス）
        /// </summary>
        private ChampionData SelectStrongest(List<ChampionData> champions)
        {
            // 総合力スコアを計算（基礎ステータスの合計）
            return champions.OrderByDescending(c =>
                c.baseStats.HP +
                c.baseStats.Attack * 10 +
                c.baseStats.Defense * 5 +
                c.baseStats.Speed * 3
            ).First();
        }

        /// <summary>
        /// 難易度に基づくAI名を生成します
        /// </summary>
        public string GenerateAIName(AIDifficulty difficulty)
        {
            var names = _aiNames[difficulty];
            string selectedName = names[_random.Next(names.Count)];

            Debug.Log($"[AIOpponentService] Generated AI name: {selectedName} (Difficulty: {difficulty})");
            return selectedName;
        }
    }
}
