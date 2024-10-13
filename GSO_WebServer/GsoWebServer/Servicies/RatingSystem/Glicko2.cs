using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Matching
{
    /// <summary>
    /// Glicko2 매치메이킹 알고리즘
    /// http://www.glicko.net/glicko/glicko2.pdf
    /// 
    /// 점수 계산기
    /// https://docs.google.com/spreadsheets/d/1E-Tq8eq5zx1t6U4R81rYf2sN-d7kuzv7RSchauXSQgE/edit?gid=0#gid=0
    /// </summary>
    public class Glicko2
    {

        private const double mGlickoScale = 173.7178;
        private const double mGlickoSystemConst = 0.7; //시스템 상수 0.3 ~ 1.2 (작을수록 변동성 측정값이 크게 변경되는 것을 방지)

        /// <summary>
        /// 매치 결과에 따른 보너스 포인트 값 (추후 수정)
        /// 1.00 - 탈출 O 킬 O 파밍 O
        /// 0.75 - 탈출 O 킬 ∆ 파밍 O OR 탈출 O 킬 O 파밍 ∆
        /// 0.50 - 탈출 O 킬 ∆ 파밍 ∆
        /// 0.25 - 탈출 X 킬 ∆ 파밍 ∆
        /// 0.00 - 탈출 X 킬 X 파밍 X
        /// </summary>
        private long mKillPoint = +5;       //킬
        private long mDeathPoint = -100;      //데스
        private long mDamagePoint = +3;       //체력100 = 1대미지
        private long mFarmingPoint = +4;       //레어급 파밍
        private long mEscapePoint = +20;      //탈출
        private long mSurvialPoint = +2;       //1분 = 1생존

        /// <summary>
        /// 플레이어의 매치 결과에 따른 퍼포먼스 값
        /// </summary>
        private double GetPlayerPerformance(MatchOutcome outcome)
        {
            long kill = outcome.kills * mKillPoint;
            long death = outcome.death * mDeathPoint;
            long damage = (outcome.damage == 0) ? 0 : outcome.damage / 100 * mDamagePoint;
            long farming = outcome.farming * mFarmingPoint;
            long escape = outcome.escape * mEscapePoint;
            long survial = outcome.survival_time * mSurvialPoint;


            double performance = kill + death + damage + farming + escape + survial;

            performance = Math.Clamp(performance, 0, 100); //퍼포먼스 점수는 0 ~ 100까지

            return performance / 100;
        }

        private double GetRatingGlicko2UpScale(double rating)
        {
            return rating * mGlickoScale + 1500.0;
        }

        private double GetRatingDeviationGlicko2UpScale(double ratingDeviation)
        {
            return ratingDeviation * mGlickoScale;
        }

        private double GetRatingGlicko2DownScale(double rating)
        {
            return (rating - 1500.0) / mGlickoScale;
        }

        private double GetRatingDeviationGlicko2DownScale(double ratingDeviation)
        {
            return ratingDeviation / mGlickoScale;
        }

        private double G(double rd)
        {
            return 1.0 / Math.Sqrt(1.0 + (3.0 * Math.Pow(rd, 2)) / Math.Pow(Math.PI, 2));
        }

        private double E(double rating, double opponentRating, double opponentRD)
        {
            return 1.0 / (1.0 + Math.Exp(-G(opponentRD) * (rating - opponentRating)));
        }

        private double CalculateVariance(Dictionary<int, FUserSkill> skills, double rating)
        {
            double varianceInverseSum = 0.0;
            foreach ((int uid, FUserSkill skill) in skills)
            {
                double mu = GetRatingGlicko2DownScale(skill.rating);
                double phi = GetRatingDeviationGlicko2DownScale(skill.deviation);

                double g = G(phi);
                double e = E(rating, mu, phi);
                varianceInverseSum += Math.Pow(g, 2) * e * (1.0 - e);
            }
            return 1.0 / varianceInverseSum;
        }

        private double CalculateDelta(Dictionary<int, FUserSkill> skills, Dictionary<int, MatchOutcome> outcoms, double rating, double variance)
        {
            double deltaSum = 0.0;
            foreach ((int uid, MatchOutcome outcome) in outcoms)
            {
                skills.TryGetValue(uid, out var skill);
                if (skill == null)
                {
                    return 0.0;
                }
                double mu = GetRatingGlicko2DownScale(skill.rating);
                double phi = GetRatingDeviationGlicko2DownScale(skill.deviation);

                double g = G(phi);
                double e = E(rating, mu, phi);
                deltaSum += g * (GetPlayerPerformance(outcome) - e);
            }
            return variance * deltaSum;
        }

        private double F(double x, double a, double delta, double rd, double variance, double tau)
        {
            double expX = Math.Exp(x);
            return expX * (Math.Pow(delta, 2) - Math.Pow(rd, 2) - variance - expX) / (2 * Math.Pow(Math.Pow(rd, 2) + variance + expX, 2)) - (x - a) / Math.Pow(tau, 2);
        }

        /// <summary>
        /// 결과에 따른 플레이어 경험치 계산
        /// </summary>
        public int CalculateExperience(MatchOutcome outcome)
        {
            double exp = GetPlayerPerformance(outcome) * 120;
            return Math.Min(0, (int)exp);
        }

        /// <summary>
        /// 플레이어의 스킬에 관한 업데이트
        /// </summary>
        /// <param name="player">  업데이트할 플레이어</param>
        /// <param name="matches"> 게임에 참여한 플레이어들의 실력과 결과 </param>
        public FUserSkill UpdatePlayerRating(int uid, Dictionary<int, FUserSkill> skills, Dictionary<int, MatchOutcome> outcoms)
        {
            double rating = GetRatingGlicko2DownScale(skills[uid].rating);       // r
            double deviation = GetRatingDeviationGlicko2DownScale(skills[uid].deviation);          // φ
            double volatility = skills[uid].volatility;                         // σ

            double variance = CalculateVariance(skills, rating);                // v
            double delta = CalculateDelta(skills, outcoms, rating, variance);   // ∆

            double a = Math.Log(Math.Pow(volatility, 2));                       // ln(σ^2)
            double tau = mGlickoSystemConst;                                    // τ
            double epsilon = 0.000001;                                          // ε

            double A = a;
            double B = 0.0;
            if (Math.Pow(delta, 2) > Math.Pow(deviation, 2) + variance)
            {
                B = Math.Log(Math.Pow(delta, 2) - Math.Pow(deviation, 2) - variance);
            }
            else //if(Math.Pow(delta, 2) <= Math.Pow(deviation, 2) + variance)
            {
                double k = 1;
                while (true)
                {
                    if (F(a - k * tau, a, delta, deviation, variance, tau) < 0)
                    {
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                B = a - k * tau;
            }

            double fA = F(A, a, delta, deviation, variance, tau);
            double fB = F(B, a, delta, deviation, variance, tau);

            while (Math.Abs(B - A) > epsilon)
            {
                double C = A + (A - B) * fA / (fB - fA);
                double fC = F(C, a, delta, deviation, variance, tau);
                if (fC * fB <= 0)
                {
                    A = B;
                    fA = fB;
                }
                else
                {
                    fA = fA / 2.0;
                }
                B = C;
                fB = fC;
            }

            double newVolatility = Math.Max(0.04, Math.Exp(A / 2));

            double preRatingRD = Math.Sqrt(Math.Pow(deviation, 2) + Math.Pow(newVolatility, 2));

            double newRD = 1.0 / Math.Sqrt(1.0 / Math.Pow(preRatingRD, 2) + 1.0 / variance);

            double newRating = rating + Math.Pow(newRD, 2) * delta;

            var newSkillInfo = new FUserSkill
            {
                rating = Math.Clamp(GetRatingGlicko2UpScale(newRating), 500, 3000),
                deviation = Math.Clamp(GetRatingDeviationGlicko2UpScale(newRD), 30, 350),
                volatility = newVolatility,
            };

            return newSkillInfo;
        }

    }
}
