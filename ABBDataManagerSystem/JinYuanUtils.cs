using ABBDataManagerSystem.Pages.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem
{
    internal class JinYuanUtils
    {

        #region 计算最大不平衡差
        private static float[]? GetMaxMin(TappingResistanceFields tpr)
        {
            if (tpr.ValueAB == null || tpr.ValueBC == null || tpr.ValueCA == null)
            {
                return null;
            }
            float max = Math.Max((float)tpr.ValueAB, Math.Max((float)tpr.ValueBC, (float)tpr.ValueCA));
            float min = Math.Min((float)tpr.ValueAB, Math.Min((float)tpr.ValueBC, (float)tpr.ValueCA));

            return new float[] { max, min };
        }

        public static float? CalculateMaxUnbalanceDiff(TappingResistanceFields tpr)
        {
            var maxMin = GetMaxMin(tpr);
            if (maxMin == null || maxMin.Length != 2)
            {
                return null;
            }
            float sum = ((float)tpr.ValueAB + (float)tpr.ValueBC + (float)tpr.ValueCA);
            if (sum == 0)
            {
                return null;
            }
            return ((maxMin[0] - maxMin[1]) / (sum / 3)) * 100;
        }
        #endregion
    }
}
