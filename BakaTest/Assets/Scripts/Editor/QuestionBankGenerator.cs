#nullable enable
using UnityEngine;
using UnityEditor;
using BakaTest.Data.Tests;
using BakaTest.Data.Champions;
using System.IO;
using System.Collections.Generic;

namespace BakaTest.Editor
{
    /// <summary>
    /// QuestionBankDataのScriptableObjectを生成するユーティリティ
    /// </summary>
    /// <remarks>
    /// 各教科・難易度ごとにサンプル問題バンクを作成します。
    /// </remarks>
    public static class QuestionBankGenerator
    {
        private const string OutputFolder = "Assets/Resources/Data/QuestionBanks";

        [MenuItem("BakaTest/Generate/Create Sample Question Banks")]
        public static void GenerateSampleQuestionBanks()
        {
            // フォルダが存在しない場合は作成
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
                AssetDatabase.Refresh();
            }

            Debug.Log("[QuestionBankGenerator] Generating sample question banks...");

            int totalCreated = 0;

            // 各教科・難易度の組み合わせで問題バンクを作成
            foreach (Subject subject in System.Enum.GetValues(typeof(Subject)))
            {
                foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
                {
                    CreateQuestionBank(subject, difficulty);
                    totalCreated++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Question Banks Created",
                $"{totalCreated} question banks created in {OutputFolder}\n\n" +
                "各バンクには10問のサンプル問題が含まれています。\n" +
                "実際の問題に置き換えることをお勧めします。",
                "OK"
            );

            Debug.Log($"[QuestionBankGenerator] {totalCreated} question banks created successfully!");
        }

        private static void CreateQuestionBank(Subject subject, DifficultyLevel difficulty)
        {
            var questionBank = ScriptableObject.CreateInstance<QuestionBankData>();

            questionBank.bankId = $"bank_{subject.ToString().ToLower()}_{difficulty.ToString().ToLower()}";
            questionBank.bankName = $"{subject} - {GetDifficultyDisplayName(difficulty)}";
            questionBank.subject = subject;
            questionBank.difficulty = difficulty;

            // サンプル問題を生成
            questionBank.questions = GenerateSampleQuestions(subject, difficulty);

            string fileName = $"QuestionBank_{subject}_{difficulty}.asset";
            CreateAsset(questionBank, fileName);
        }

        private static List<QuestionBankData.QuestionEntry> GenerateSampleQuestions(Subject subject, DifficultyLevel difficulty)
        {
            var questions = new List<QuestionBankData.QuestionEntry>();

            // 各教科・難易度に応じたサンプル問題を10問生成
            for (int i = 0; i < 10; i++)
            {
                questions.Add(CreateSampleQuestion(subject, difficulty, i + 1));
            }

            return questions;
        }

        private static QuestionBankData.QuestionEntry CreateSampleQuestion(Subject subject, DifficultyLevel difficulty, int questionNumber)
        {
            var question = new QuestionBankData.QuestionEntry();

            switch (subject)
            {
                case Subject.Math:
                    question = CreateMathQuestion(difficulty, questionNumber);
                    break;
                case Subject.Science:
                    question = CreateScienceQuestion(difficulty, questionNumber);
                    break;
                case Subject.English:
                    question = CreateEnglishQuestion(difficulty, questionNumber);
                    break;
                case Subject.History:
                    question = CreateHistoryQuestion(difficulty, questionNumber);
                    break;
            }

            return question;
        }

        #region Math Questions

        private static QuestionBankData.QuestionEntry CreateMathQuestion(DifficultyLevel difficulty, int num)
        {
            var q = new QuestionBankData.QuestionEntry();

            switch (difficulty)
            {
                case DifficultyLevel.Elementary:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "5 + 8 = ?";
                            q.choice0 = "12";
                            q.choice1 = "13";
                            q.choice2 = "14";
                            q.choice3 = "15";
                            q.choice4 = "16";
                            q.correctAnswerIndex = 1;
                            q.explanation = "5 + 8 = 13です。";
                            break;
                        case 1:
                            q.questionText = "12 - 7 = ?";
                            q.choice0 = "3";
                            q.choice1 = "4";
                            q.choice2 = "5";
                            q.choice3 = "6";
                            q.choice4 = "7";
                            q.correctAnswerIndex = 2;
                            q.explanation = "12 - 7 = 5です。";
                            break;
                        case 2:
                            q.questionText = "4 × 6 = ?";
                            q.choice0 = "20";
                            q.choice1 = "22";
                            q.choice2 = "24";
                            q.choice3 = "26";
                            q.choice4 = "28";
                            q.correctAnswerIndex = 2;
                            q.explanation = "4 × 6 = 24です。";
                            break;
                    }
                    break;

                case DifficultyLevel.MiddleSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "(-3) + 5 = ?";
                            q.choice0 = "-8";
                            q.choice1 = "-2";
                            q.choice2 = "2";
                            q.choice3 = "8";
                            q.choice4 = "15";
                            q.correctAnswerIndex = 2;
                            q.explanation = "負の数と正の数の加算: -3 + 5 = 2";
                            break;
                        case 1:
                            q.questionText = "2x + 5 = 13のとき、xの値は?";
                            q.choice0 = "2";
                            q.choice1 = "3";
                            q.choice2 = "4";
                            q.choice3 = "5";
                            q.choice4 = "6";
                            q.correctAnswerIndex = 2;
                            q.explanation = "2x = 13 - 5 = 8, x = 4";
                            break;
                        case 2:
                            q.questionText = "√64 = ?";
                            q.choice0 = "6";
                            q.choice1 = "7";
                            q.choice2 = "8";
                            q.choice3 = "9";
                            q.choice4 = "10";
                            q.correctAnswerIndex = 2;
                            q.explanation = "8 × 8 = 64なので、√64 = 8";
                            break;
                    }
                    break;

                case DifficultyLevel.HighSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "(x + 2)(x - 3)を展開すると?";
                            q.choice0 = "x² - x - 6";
                            q.choice1 = "x² + x - 6";
                            q.choice2 = "x² - x + 6";
                            q.choice3 = "x² + x + 6";
                            q.choice4 = "x² - 5x - 6";
                            q.correctAnswerIndex = 0;
                            q.explanation = "(x+2)(x-3) = x² - 3x + 2x - 6 = x² - x - 6";
                            break;
                        case 1:
                            q.questionText = "sin 30° = ?";
                            q.choice0 = "0";
                            q.choice1 = "1/2";
                            q.choice2 = "√2/2";
                            q.choice3 = "√3/2";
                            q.choice4 = "1";
                            q.correctAnswerIndex = 1;
                            q.explanation = "30度の正弦は1/2です。";
                            break;
                        case 2:
                            q.questionText = "log₂ 8 = ?";
                            q.choice0 = "1";
                            q.choice1 = "2";
                            q.choice2 = "3";
                            q.choice3 = "4";
                            q.choice4 = "5";
                            q.correctAnswerIndex = 2;
                            q.explanation = "2³ = 8なので、log₂ 8 = 3";
                            break;
                    }
                    break;

                case DifficultyLevel.University:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "∫ 2x dx = ?";
                            q.choice0 = "x";
                            q.choice1 = "x² + C";
                            q.choice2 = "2x² + C";
                            q.choice3 = "x²/2 + C";
                            q.choice4 = "2x + C";
                            q.correctAnswerIndex = 1;
                            q.explanation = "xの積分は x²/2なので、2xの積分は x² + C";
                            break;
                        case 1:
                            q.questionText = "lim(x→0) (sin x)/x = ?";
                            q.choice0 = "0";
                            q.choice1 = "1";
                            q.choice2 = "∞";
                            q.choice3 = "-1";
                            q.choice4 = "不定";
                            q.correctAnswerIndex = 1;
                            q.explanation = "有名な極限: lim(x→0) (sin x)/x = 1";
                            break;
                        case 2:
                            q.questionText = "d/dx (e^x) = ?";
                            q.choice0 = "e^x";
                            q.choice1 = "e";
                            q.choice2 = "xe^(x-1)";
                            q.choice3 = "ln x";
                            q.choice4 = "x";
                            q.correctAnswerIndex = 0;
                            q.explanation = "e^xの導関数はe^x自身です。";
                            break;
                    }
                    break;
            }

            return q;
        }

        #endregion

        #region Science Questions

        private static QuestionBankData.QuestionEntry CreateScienceQuestion(DifficultyLevel difficulty, int num)
        {
            var q = new QuestionBankData.QuestionEntry();

            switch (difficulty)
            {
                case DifficultyLevel.Elementary:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "太陽系で一番大きな惑星は?";
                            q.choice0 = "地球";
                            q.choice1 = "火星";
                            q.choice2 = "木星";
                            q.choice3 = "土星";
                            q.choice4 = "天王星";
                            q.correctAnswerIndex = 2;
                            q.explanation = "木星は太陽系最大の惑星です。";
                            break;
                        case 1:
                            q.questionText = "水の沸点は摂氏何度?";
                            q.choice0 = "0度";
                            q.choice1 = "50度";
                            q.choice2 = "100度";
                            q.choice3 = "150度";
                            q.choice4 = "200度";
                            q.correctAnswerIndex = 2;
                            q.explanation = "水は100℃で沸騰します。";
                            break;
                        case 2:
                            q.questionText = "植物が光合成で作り出すのは?";
                            q.choice0 = "二酸化炭素";
                            q.choice1 = "酸素";
                            q.choice2 = "窒素";
                            q.choice3 = "水素";
                            q.choice4 = "メタン";
                            q.correctAnswerIndex = 1;
                            q.explanation = "光合成により酸素が作られます。";
                            break;
                    }
                    break;

                case DifficultyLevel.MiddleSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "化学式H₂Oが表す物質は?";
                            q.choice0 = "水素";
                            q.choice1 = "酸素";
                            q.choice2 = "水";
                            q.choice3 = "過酸化水素";
                            q.choice4 = "二酸化炭素";
                            q.correctAnswerIndex = 2;
                            q.explanation = "H₂Oは水の化学式です。";
                            break;
                        case 1:
                            q.questionText = "電流の単位は?";
                            q.choice0 = "ボルト(V)";
                            q.choice1 = "アンペア(A)";
                            q.choice2 = "オーム(Ω)";
                            q.choice3 = "ワット(W)";
                            q.choice4 = "ジュール(J)";
                            q.correctAnswerIndex = 1;
                            q.explanation = "電流の単位はアンペア(A)です。";
                            break;
                        case 2:
                            q.questionText = "細胞の核に含まれる遺伝情報を持つ物質は?";
                            q.choice0 = "タンパク質";
                            q.choice1 = "脂質";
                            q.choice2 = "炭水化物";
                            q.choice3 = "DNA";
                            q.choice4 = "RNA";
                            q.correctAnswerIndex = 3;
                            q.explanation = "DNAが遺伝情報を保持しています。";
                            break;
                    }
                    break;

                case DifficultyLevel.HighSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "周期表で原子番号6の元素は?";
                            q.choice0 = "窒素";
                            q.choice1 = "炭素";
                            q.choice2 = "酸素";
                            q.choice3 = "ホウ素";
                            q.choice4 = "ベリリウム";
                            q.correctAnswerIndex = 1;
                            q.explanation = "原子番号6は炭素(C)です。";
                            break;
                        case 1:
                            q.questionText = "オームの法則 V = IR の I は何を表す?";
                            q.choice0 = "電圧";
                            q.choice1 = "電流";
                            q.choice2 = "抵抗";
                            q.choice3 = "電力";
                            q.choice4 = "エネルギー";
                            q.correctAnswerIndex = 1;
                            q.explanation = "Iは電流(Current)を表します。";
                            break;
                        case 2:
                            q.questionText = "光合成の反応式で正しいのは?";
                            q.choice0 = "CO₂ + H₂O → C₆H₁₂O₆ + O₂";
                            q.choice1 = "O₂ + H₂O → CO₂ + C₆H₁₂O₆";
                            q.choice2 = "C₆H₁₂O₆ → CO₂ + H₂O + O₂";
                            q.choice3 = "N₂ + O₂ → NO₂";
                            q.choice4 = "H₂ + O₂ → H₂O";
                            q.correctAnswerIndex = 0;
                            q.explanation = "光合成は二酸化炭素と水からグルコースと酸素を作ります。";
                            break;
                    }
                    break;

                case DifficultyLevel.University:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "熱力学第二法則が示すのは?";
                            q.choice0 = "エネルギー保存則";
                            q.choice1 = "エントロピー増大則";
                            q.choice2 = "質量保存則";
                            q.choice3 = "運動量保存則";
                            q.choice4 = "角運動量保存則";
                            q.correctAnswerIndex = 1;
                            q.explanation = "第二法則はエントロピーが増大することを示します。";
                            break;
                        case 1:
                            q.questionText = "電子のスピン量子数は?";
                            q.choice0 = "0";
                            q.choice1 = "1";
                            q.choice2 = "1/2";
                            q.choice3 = "2";
                            q.choice4 = "-1";
                            q.correctAnswerIndex = 2;
                            q.explanation = "電子はスピン1/2のフェルミ粒子です。";
                            break;
                        case 2:
                            q.questionText = "ミトコンドリアの主な機能は?";
                            q.choice0 = "タンパク質合成";
                            q.choice1 = "ATP合成";
                            q.choice2 = "DNA複製";
                            q.choice3 = "光合成";
                            q.choice4 = "解糖系";
                            q.correctAnswerIndex = 1;
                            q.explanation = "ミトコンドリアは細胞のエネルギー通貨ATPを生産します。";
                            break;
                    }
                    break;
            }

            return q;
        }

        #endregion

        #region English Questions

        private static QuestionBankData.QuestionEntry CreateEnglishQuestion(DifficultyLevel difficulty, int num)
        {
            var q = new QuestionBankData.QuestionEntry();

            switch (difficulty)
            {
                case DifficultyLevel.Elementary:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "「りんご」を英語で言うと?";
                            q.choice0 = "Orange";
                            q.choice1 = "Apple";
                            q.choice2 = "Banana";
                            q.choice3 = "Grape";
                            q.choice4 = "Lemon";
                            q.correctAnswerIndex = 1;
                            q.explanation = "Apple = りんご";
                            break;
                        case 1:
                            q.questionText = "I ___ a student. 空欄に入る単語は?";
                            q.choice0 = "is";
                            q.choice1 = "am";
                            q.choice2 = "are";
                            q.choice3 = "be";
                            q.choice4 = "was";
                            q.correctAnswerIndex = 1;
                            q.explanation = "I am a student. (私は学生です)";
                            break;
                        case 2:
                            q.questionText = "「こんにちは」を英語で言うと?";
                            q.choice0 = "Good morning";
                            q.choice1 = "Good night";
                            q.choice2 = "Hello";
                            q.choice3 = "Goodbye";
                            q.choice4 = "Thank you";
                            q.correctAnswerIndex = 2;
                            q.explanation = "Hello = こんにちは";
                            break;
                    }
                    break;

                case DifficultyLevel.MiddleSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "She ___ to school every day. 空欄に入る動詞は?";
                            q.choice0 = "go";
                            q.choice1 = "goes";
                            q.choice2 = "going";
                            q.choice3 = "went";
                            q.choice4 = "gone";
                            q.correctAnswerIndex = 1;
                            q.explanation = "三人称単数現在形: She goes";
                            break;
                        case 1:
                            q.questionText = "「彼女は英語を勉強している」の英訳は?";
                            q.choice0 = "She study English";
                            q.choice1 = "She studied English";
                            q.choice2 = "She is studying English";
                            q.choice3 = "She studies English";
                            q.choice4 = "She will study English";
                            q.correctAnswerIndex = 2;
                            q.explanation = "現在進行形: is studying";
                            break;
                        case 2:
                            q.questionText = "\"Beautiful\"の比較級は?";
                            q.choice0 = "beautifuler";
                            q.choice1 = "more beautiful";
                            q.choice2 = "most beautiful";
                            q.choice3 = "beautifully";
                            q.choice4 = "beautifulest";
                            q.correctAnswerIndex = 1;
                            q.explanation = "長い形容詞: more beautiful";
                            break;
                    }
                    break;

                case DifficultyLevel.HighSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "If I ___ rich, I would buy a house. 空欄に入る動詞は?";
                            q.choice0 = "am";
                            q.choice1 = "was";
                            q.choice2 = "were";
                            q.choice3 = "be";
                            q.choice4 = "been";
                            q.correctAnswerIndex = 2;
                            q.explanation = "仮定法過去: If I were";
                            break;
                        case 1:
                            q.questionText = "The book ___ by many people. 受動態の空欄に入るのは?";
                            q.choice0 = "read";
                            q.choice1 = "reads";
                            q.choice2 = "is read";
                            q.choice3 = "reading";
                            q.choice4 = "was reading";
                            q.correctAnswerIndex = 2;
                            q.explanation = "受動態現在形: is read";
                            break;
                        case 2:
                            q.questionText = "\"Unprecedented\"の意味は?";
                            q.choice0 = "予期された";
                            q.choice1 = "前例のない";
                            q.choice2 = "重要な";
                            q.choice3 = "一般的な";
                            q.choice4 = "古い";
                            q.correctAnswerIndex = 1;
                            q.explanation = "unprecedented = 前例のない、空前の";
                            break;
                    }
                    break;

                case DifficultyLevel.University:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "\"Serendipity\"の意味として最も適切なのは?";
                            q.choice0 = "悲劇";
                            q.choice1 = "思いがけない幸運な発見";
                            q.choice2 = "計画的な成功";
                            q.choice3 = "必然的な結果";
                            q.choice4 = "予測可能な展開";
                            q.correctAnswerIndex = 1;
                            q.explanation = "serendipity = 偶然の幸運な発見";
                            break;
                        case 1:
                            q.questionText = "分詞構文 \"Having finished the work, he went home\" の意味は?";
                            q.choice0 = "仕事を終えている間、彼は家に帰った";
                            q.choice1 = "仕事を終えた後、彼は家に帰った";
                            q.choice2 = "仕事を終える前に、彼は家に帰った";
                            q.choice3 = "仕事を終えながら、彼は家に帰った";
                            q.choice4 = "仕事を終えるために、彼は家に帰った";
                            q.correctAnswerIndex = 1;
                            q.explanation = "完了分詞構文: ~した後";
                            break;
                        case 2:
                            q.questionText = "\"Albeit\"の用法として正しいのは?";
                            q.choice0 = "接続詞（~だけれども）";
                            q.choice1 = "名詞（証拠）";
                            q.choice2 = "動詞（主張する）";
                            q.choice3 = "形容詞（明白な）";
                            q.choice4 = "副詞（常に）";
                            q.correctAnswerIndex = 0;
                            q.explanation = "albeit = although（~だけれども）";
                            break;
                    }
                    break;
            }

            return q;
        }

        #endregion

        #region History Questions

        private static QuestionBankData.QuestionEntry CreateHistoryQuestion(DifficultyLevel difficulty, int num)
        {
            var q = new QuestionBankData.QuestionEntry();

            switch (difficulty)
            {
                case DifficultyLevel.Elementary:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "日本の初代天皇とされるのは?";
                            q.choice0 = "神武天皇";
                            q.choice1 = "推古天皇";
                            q.choice2 = "聖徳太子";
                            q.choice3 = "卑弥呼";
                            q.choice4 = "明治天皇";
                            q.correctAnswerIndex = 0;
                            q.explanation = "神武天皇が日本の初代天皇とされています。";
                            break;
                        case 1:
                            q.questionText = "江戸幕府を開いたのは誰?";
                            q.choice0 = "織田信長";
                            q.choice1 = "豊臣秀吉";
                            q.choice2 = "徳川家康";
                            q.choice3 = "源頼朝";
                            q.choice4 = "足利尊氏";
                            q.correctAnswerIndex = 2;
                            q.explanation = "1603年に徳川家康が江戸幕府を開きました。";
                            break;
                        case 2:
                            q.questionText = "明治維新が起きたのは何年?";
                            q.choice0 = "1868年";
                            q.choice1 = "1858年";
                            q.choice2 = "1878年";
                            q.choice3 = "1888年";
                            q.choice4 = "1898年";
                            q.correctAnswerIndex = 0;
                            q.explanation = "明治維新は1868年です。";
                            break;
                    }
                    break;

                case DifficultyLevel.MiddleSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "鎌倉幕府が成立したのは何年?";
                            q.choice0 = "1185年";
                            q.choice1 = "1192年";
                            q.choice2 = "1333年";
                            q.choice3 = "1467年";
                            q.choice4 = "1603年";
                            q.correctAnswerIndex = 1;
                            q.explanation = "1192年に源頼朝が征夷大将軍に任命され、鎌倉幕府が成立しました。";
                            break;
                        case 1:
                            q.questionText = "第二次世界大戦が終結したのは何年?";
                            q.choice0 = "1943年";
                            q.choice1 = "1944年";
                            q.choice2 = "1945年";
                            q.choice3 = "1946年";
                            q.choice4 = "1947年";
                            q.correctAnswerIndex = 2;
                            q.explanation = "1945年8月15日に日本が降伏しました。";
                            break;
                        case 2:
                            q.questionText = "「大化の改新」が起きたのは何世紀?";
                            q.choice0 = "5世紀";
                            q.choice1 = "6世紀";
                            q.choice2 = "7世紀";
                            q.choice3 = "8世紀";
                            q.choice4 = "9世紀";
                            q.correctAnswerIndex = 2;
                            q.explanation = "大化の改新は645年（7世紀）です。";
                            break;
                    }
                    break;

                case DifficultyLevel.HighSchool:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "「承久の乱」で敗れたのは誰?";
                            q.choice0 = "北条政子";
                            q.choice1 = "源頼朝";
                            q.choice2 = "後鳥羽上皇";
                            q.choice3 = "足利尊氏";
                            q.choice4 = "平清盛";
                            q.correctAnswerIndex = 2;
                            q.explanation = "1221年の承久の乱で後鳥羽上皇が敗北しました。";
                            break;
                        case 1:
                            q.questionText = "「日米和親条約」を締結した人物は?";
                            q.choice0 = "井伊直弼";
                            q.choice1 = "徳川慶喜";
                            q.choice2 = "ペリー";
                            q.choice3 = "阿部正弘";
                            q.choice4 = "勝海舟";
                            q.correctAnswerIndex = 2;
                            q.explanation = "1854年にペリーと幕府が日米和親条約を締結しました。";
                            break;
                        case 2:
                            q.questionText = "「応仁の乱」の期間は?";
                            q.choice0 = "1467-1477年";
                            q.choice1 = "1457-1467年";
                            q.choice2 = "1477-1487年";
                            q.choice3 = "1447-1457年";
                            q.choice4 = "1487-1497年";
                            q.correctAnswerIndex = 0;
                            q.explanation = "応仁の乱は1467年から1477年まで続きました。";
                            break;
                    }
                    break;

                case DifficultyLevel.University:
                    switch (num % 3)
                    {
                        case 0:
                            q.questionText = "「律令制度」で中央政府の最高機関は?";
                            q.choice0 = "太政官";
                            q.choice1 = "神祇官";
                            q.choice2 = "民部省";
                            q.choice3 = "兵部省";
                            q.choice4 = "刑部省";
                            q.correctAnswerIndex = 0;
                            q.explanation = "太政官が律令制の最高行政機関でした。";
                            break;
                        case 1:
                            q.questionText = "「御成敗式目」を制定したのは?";
                            q.choice0 = "源頼朝";
                            q.choice1 = "北条泰時";
                            q.choice2 = "北条時宗";
                            q.choice3 = "足利尊氏";
                            q.choice4 = "徳川家康";
                            q.correctAnswerIndex = 1;
                            q.explanation = "1232年に北条泰時が御成敗式目を制定しました。";
                            break;
                        case 2:
                            q.questionText = "「生類憐みの令」を発布した将軍は?";
                            q.choice0 = "徳川家光";
                            q.choice1 = "徳川綱吉";
                            q.choice2 = "徳川吉宗";
                            q.choice3 = "徳川家斉";
                            q.choice4 = "徳川慶喜";
                            q.correctAnswerIndex = 1;
                            q.explanation = "5代将軍徳川綱吉が生類憐みの令を発布しました。";
                            break;
                    }
                    break;
            }

            return q;
        }

        #endregion

        private static string GetDifficultyDisplayName(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => "小学校",
                DifficultyLevel.MiddleSchool => "中学校",
                DifficultyLevel.HighSchool => "高校",
                DifficultyLevel.University => "大学",
                _ => difficulty.ToString()
            };
        }

        private static void CreateAsset(QuestionBankData questionBank, string fileName)
        {
            string path = Path.Combine(OutputFolder, fileName);

            // 既存のアセットがある場合は上書き確認
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog(
                    "Overwrite Confirmation",
                    $"Question bank '{questionBank.bankName}' already exists. Overwrite?",
                    "Yes",
                    "No"))
                {
                    Debug.Log($"[QuestionBankGenerator] Skipped {questionBank.bankName}");
                    return;
                }
            }

            AssetDatabase.CreateAsset(questionBank, path);
            Debug.Log($"[QuestionBankGenerator] Created {questionBank.bankName} at {path}");
        }
    }
}
