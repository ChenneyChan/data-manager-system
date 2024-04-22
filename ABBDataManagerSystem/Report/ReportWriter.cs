using ABBDataManagerSystem.Bean.Base;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace ABBDataManagerSystem.Report
{
    public class ReportWriter
    {
        public static void WriterToDryTypeTransformer()
        {

        }

        private static bool WriteFigureNumberInfoToReport(FigureNumberInfo? figInfo, ISheet? sheet)
        {
            if (figInfo == null || sheet == null)
            {
                return false;
            }
            string fieldName = FigureNumberInfo.GetMatchFieldByProductType(figInfo.ProductType);
            if (fieldName == null)
            {
                return false;
            }

            System.Type type = typeof(FigureNoInfoFieldDef);
            var fieldInfo = type.GetField(fieldName);
            if (fieldInfo == null)
            {
                return false;
            }
            foreach (var fieldDef in FigureNumberInfo.Defs)
            {
                string? checkValue = (string?)fieldInfo.GetValue(fieldDef.Value);
                if (checkValue == null || checkValue == "False" || checkValue == "True")
                {
                    continue;
                }
                var value = Utils.GetFieldValue(figInfo, fieldDef.Key);
                ExcelUtils.FillCell(checkValue, sheet, value);
            }

            return true;
        }

        // 干式消弧线圈
        public static bool WriteToArcCoilTest(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("数据导入"); // 假设我们要处理第一个 sheet

                ExcelUtils.FillCell("AM3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AM3", sheet, productInfo.Sequence);
                    ExcelUtils.FillCell("AM4", sheet, productInfo.FigureNumber);
                }

                if (voltageDatas != null && voltageDatas.Count > 0)
                {
                    var voltageData = voltageDatas[0];
                    ExcelUtils.FillCell("B56", sheet, voltageData.u3);
                    ExcelUtils.FillCell("H56", sheet, voltageData.i3);
                    ExcelUtils.FillCell("O56", sheet, voltageData.p3);
                    ExcelUtils.FillCell("Y54", sheet, voltageData.Temperature);
                }

                if (impedenceResistanceInfos != null)
                {
                    int index = 28;
                    int max = 52;
                    foreach (var data in impedenceResistanceInfos)
                    {
                        if (index > max)
                        {
                            break;
                        }
                        ExcelUtils.FillCell($"U{index}", sheet, data.Impedance);
                        ExcelUtils.FillCell($"AL{index}", sheet, data.Resistance);
                        index++;
                    }
                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("报告主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }

        // 干式串联电抗器
        public static bool WriteToDryTypeConnectionTest(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("试验数据导入"); // 假设我们要处理第一个 sheet

                ExcelUtils.FillCell("AM3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AM3", sheet, productInfo.Sequence);
                    ExcelUtils.FillCell("AM4", sheet, productInfo.FigureNumber);
                }

                if (voltageDatas != null && voltageDatas.Count > 0)
                {
                    foreach (var voltageData in voltageDatas)
                    {
                        if (voltageData == null)
                        {
                            continue;
                        }
                        if (voltageData.LoadType == VoltageCurrentLossDataInfo.LoadTypeLoad)
                        {
                            ExcelUtils.FillCell("B50", sheet, voltageData.ua);
                            ExcelUtils.FillCell("F50", sheet, voltageData.ub);
                            ExcelUtils.FillCell("J50", sheet, voltageData.uc);
                            ExcelUtils.FillCell("N50", sheet, (voltageData.ua + voltageData.ub + voltageData.uc) / 3);
                            ExcelUtils.FillCell("R50", sheet, voltageData.ia);
                            ExcelUtils.FillCell("V50", sheet, voltageData.ib);
                            ExcelUtils.FillCell("Z50", sheet, voltageData.ic);
                            ExcelUtils.FillCell("AD50", sheet, (voltageData.ia + voltageData.ib + voltageData.ic) / 3);
                            ExcelUtils.FillCell("AH50", sheet, voltageData.pa);
                            ExcelUtils.FillCell("AL50", sheet, voltageData.pb);
                            ExcelUtils.FillCell("AP50", sheet, voltageData.pc);
                            ExcelUtils.FillCell("AT50", sheet, (voltageData.pa + voltageData.pb + voltageData.pc));
                            ExcelUtils.FillCell("Y47", sheet, voltageData.Temperature);
                        }
                        else if (voltageData.LoadType == VoltageCurrentLossDataInfo.LoadType10IN || voltageData.LoadType == VoltageCurrentLossDataInfo.LoadType18IN)
                        {
                            int startIndex = voltageData.LoadType == VoltageCurrentLossDataInfo.LoadType10IN ? 37 : 41;
                            ExcelUtils.FillCell($"O{startIndex}", sheet, voltageData.ia);
                            ExcelUtils.FillCell($"O{startIndex + 1}", sheet, voltageData.ib);
                            ExcelUtils.FillCell($"O{startIndex + 2}", sheet, voltageData.ic);
                            ExcelUtils.FillCell($"O{startIndex + 3}", sheet, (voltageData.ia + voltageData.ib + voltageData.ic) / 3);
                            ExcelUtils.FillCell($"v{startIndex}", sheet, voltageData.ua);
                            ExcelUtils.FillCell($"v{startIndex + 1}", sheet, voltageData.ub);
                            ExcelUtils.FillCell($"v{startIndex + 2}", sheet, voltageData.uc);
                            ExcelUtils.FillCell($"v{startIndex + 3}", sheet, voltageData.uc);
                            ExcelUtils.FillCell($"v{startIndex + 4}", sheet, (voltageData.ua + voltageData.ub + voltageData.uc) / 3);
                        }
                    }
                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }

        // 干式变压器
        public static bool WriteToDryTypeTransformerTest(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos, List<PartialDischargeInfo>? partialDischargeInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("数据导入"); // 假设我们要处理第一个 sheet

                ExcelUtils.FillCell("AK3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AK3", sheet, productInfo.Sequence);
                    ExcelUtils.FillCell("AK4", sheet, productInfo.FigureNumber);
                }

                if (voltageDatas != null)
                {
                    foreach (var TestValue in voltageDatas)
                    {
                        if (TestValue == null)
                        {
                            continue;
                        }
                        // 电压有效值的线电压
                        var uab = Utils.CalculateLineVoltage(TestValue.ua, TestValue.ub);
                        var ubc = Utils.CalculateLineVoltage(TestValue.ub, TestValue.uc);
                        var uca = Utils.CalculateLineVoltage(TestValue.uc, TestValue.ua);
                        var up = (uab + ubc + uca) / 3;

                        // 电压平均值的线电压
                        var uabp = Utils.CalculateLineVoltage(TestValue.pua, TestValue.pub);
                        var ubcp = Utils.CalculateLineVoltage(TestValue.pub, TestValue.puc);
                        var ucap = Utils.CalculateLineVoltage(TestValue.puc, TestValue.pua);
                        var upp = (uabp + ubcp + ucap) / 3;

                        if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeLoad)
                        {
                            ExcelUtils.FillCell("B62", sheet, uab);
                            ExcelUtils.FillCell("F62", sheet, ubc);
                            ExcelUtils.FillCell("J62", sheet, uca);
                            ExcelUtils.FillCell("N62", sheet, up);

                            ExcelUtils.FillCell("R62", sheet, TestValue.ia);
                            ExcelUtils.FillCell("V62", sheet, TestValue.ib);
                            ExcelUtils.FillCell("Z62", sheet, TestValue.ic);
                            ExcelUtils.FillCell("AD62", sheet, (TestValue.ia + TestValue.ib + TestValue.ic) / 3);

                            ExcelUtils.FillCell("AH62", sheet, TestValue.pa);
                            ExcelUtils.FillCell("AL62", sheet, TestValue.pb);
                            ExcelUtils.FillCell("AP62", sheet, TestValue.pc);
                            ExcelUtils.FillCell("AT62", sheet, (TestValue.pa + TestValue.pb + TestValue.pc));

                            ExcelUtils.FillCell("AD59", sheet, TestValue.Temperature);
                        }
                        else if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeNoLoad)
                        {
                            ExcelUtils.FillCell("B53", sheet, uab);
                            ExcelUtils.FillCell("H53", sheet, ubc);
                            ExcelUtils.FillCell("N53", sheet, uca);
                            ExcelUtils.FillCell("T53", sheet, up);

                            ExcelUtils.FillCell("Z53", sheet, uabp);
                            ExcelUtils.FillCell("AF53", sheet, ubcp);
                            ExcelUtils.FillCell("AL53", sheet, ucap);
                            ExcelUtils.FillCell("AR53", sheet, upp);
                        }
                    }
                }

                if (partialDischargeInfos != null && partialDischargeInfos.Count > 0)
                {
                    foreach (var partialDischargeInfo in partialDischargeInfos)
                    {
                        int index = 75;
                        if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType1)
                        {

                        }
                        else if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType2)
                        {
                            index = 76;
                        }
                        ExcelUtils.FillCell($"N{index}", sheet, partialDischargeInfo.DischargeA);
                        ExcelUtils.FillCell($"Z{index}", sheet, partialDischargeInfo.DischargeB);
                        ExcelUtils.FillCell($"AL{index}", sheet, partialDischargeInfo.DischargeC);
                    }
                }
                if (impedenceResistanceInfos != null)
                {

                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("报告主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }

        // 有低压 高压五分接母本
        public static bool WriteToWithLowVoltageTest(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos, List<PartialDischargeInfo>? partialDischargeInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("数据导入");

                ExcelUtils.FillCell("AK3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AK3", sheet, productInfo.Sequence);
                }

                if (voltageDatas != null && voltageDatas.Count > 0)
                {
                    foreach (var TestValue in voltageDatas)
                    {
                        if (TestValue == null)
                        {
                            continue;
                        }
                        // 电压有效值的线电压
                        var uab = Utils.CalculateLineVoltage(TestValue.ua, TestValue.ub);
                        var ubc = Utils.CalculateLineVoltage(TestValue.ub, TestValue.uc);
                        var uca = Utils.CalculateLineVoltage(TestValue.uc, TestValue.ua);
                        var up = (uab + ubc + uca) / 3;

                        // 电压平均值的线电压
                        var uabp = Utils.CalculateLineVoltage(TestValue.pua, TestValue.pub);
                        var ubcp = Utils.CalculateLineVoltage(TestValue.pub, TestValue.puc);
                        var ucap = Utils.CalculateLineVoltage(TestValue.puc, TestValue.pua);
                        var upp = (uabp + ubcp + ucap) / 3;

                        if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeLoad)
                        {
                            ExcelUtils.FillCell("B65", sheet, uab);
                            ExcelUtils.FillCell("F65", sheet, ubc);
                            ExcelUtils.FillCell("J65", sheet, uca);
                            ExcelUtils.FillCell("N65", sheet, up);

                            ExcelUtils.FillCell("R65", sheet, TestValue.ia);
                            ExcelUtils.FillCell("V65", sheet, TestValue.ib);
                            ExcelUtils.FillCell("Z65", sheet, TestValue.ic);
                            ExcelUtils.FillCell("AD65", sheet, (TestValue.ia + TestValue.ib + TestValue.ic) / 3);

                            ExcelUtils.FillCell("AH65", sheet, TestValue.pa);
                            ExcelUtils.FillCell("AL65", sheet, TestValue.pb);
                            ExcelUtils.FillCell("AP65", sheet, TestValue.pc);
                            ExcelUtils.FillCell("AT65", sheet, (TestValue.pa + TestValue.pb + TestValue.pc));

                            ExcelUtils.FillCell("AD62", sheet, TestValue.Temperature);
                        }
                        else if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeNoLoad)
                        {
                            ExcelUtils.FillCell("B56", sheet, uab);
                            ExcelUtils.FillCell("H56", sheet, ubc);
                            ExcelUtils.FillCell("N56", sheet, uca);
                            ExcelUtils.FillCell("T56", sheet, up);

                            ExcelUtils.FillCell("Z56", sheet, uabp);
                            ExcelUtils.FillCell("AF56", sheet, ubcp);
                            ExcelUtils.FillCell("AL56", sheet, ucap);
                            ExcelUtils.FillCell("AR56", sheet, upp);
                        }
                        else if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeZero)
                        {
                            ExcelUtils.FillCell("B73", sheet, TestValue.ua);
                            ExcelUtils.FillCell("J73", sheet, TestValue.ia);
                            ExcelUtils.FillCell("R73", sheet, TestValue.pa);
                            ExcelUtils.FillCell("AN71", sheet, TestValue.Temperature);
                        }
                    }
                }

                if (partialDischargeInfos != null && partialDischargeInfos.Count > 0)
                {
                    foreach (var partialDischargeInfo in partialDischargeInfos)
                    {
                        int index = 82;
                        if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType1)
                        {

                        }
                        else if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType2)
                        {
                            index = 83;
                        }
                        ExcelUtils.FillCell($"N{index}", sheet, partialDischargeInfo.DischargeA);
                        ExcelUtils.FillCell($"Z{index}", sheet, partialDischargeInfo.DischargeB);
                        ExcelUtils.FillCell($"AL{index}", sheet, partialDischargeInfo.DischargeC);
                    }
                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("报告主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }

        // 无低压 高压一分接和二分接兼容母本
        public static bool WriteToWithoutLowVoltage12Test(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos, List<PartialDischargeInfo>? partialDischargeInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("数据导入"); // 假设我们要处理第一个 sheet

                ExcelUtils.FillCell("AM3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AM3", sheet, productInfo.Sequence);
                    ExcelUtils.FillCell("AM4", sheet, productInfo.FigureNumber);
                }

                if (voltageDatas != null && voltageDatas.Count > 0)
                {
                    foreach (var TestValue in voltageDatas)
                    {
                        if (TestValue == null)
                        {
                            continue;
                        }
                        if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeNoLoad)
                        {
                            // 电压有效值的线电压
                            var uab = Utils.CalculateLineVoltage(TestValue.ua, TestValue.ub);
                            var ubc = Utils.CalculateLineVoltage(TestValue.ub, TestValue.uc);
                            var uca = Utils.CalculateLineVoltage(TestValue.uc, TestValue.ua);
                            var up = (uab + ubc + uca) / 3;

                            // 电压平均值的线电压
                            var uabp = Utils.CalculateLineVoltage(TestValue.pua, TestValue.pub);
                            var ubcp = Utils.CalculateLineVoltage(TestValue.pub, TestValue.puc);
                            var ucap = Utils.CalculateLineVoltage(TestValue.puc, TestValue.pua);
                            var upp = (uabp + ubcp + ucap) / 3;
                            ExcelUtils.FillCell("B36", sheet, uab);
                            ExcelUtils.FillCell("H36", sheet, ubc);
                            ExcelUtils.FillCell("N36", sheet, uca);
                            ExcelUtils.FillCell("T36", sheet, up);

                            ExcelUtils.FillCell("Z36", sheet, uabp);
                            ExcelUtils.FillCell("AF36", sheet, ubcp);
                            ExcelUtils.FillCell("AL36", sheet, ucap);
                            ExcelUtils.FillCell("AR36", sheet, upp);
                        }
                        else if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeZero)
                        {
                            ExcelUtils.FillCell("B44", sheet, TestValue.ua);
                            ExcelUtils.FillCell("J44", sheet, TestValue.ia);
                            ExcelUtils.FillCell("R44", sheet, TestValue.pa);
                            ExcelUtils.FillCell("AN42", sheet, TestValue.Temperature);
                        }
                    }
                }

                if (partialDischargeInfos != null && partialDischargeInfos.Count > 0)
                {
                    foreach (var partialDischargeInfo in partialDischargeInfos)
                    {
                        int index = 52;
                        if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType1)
                        {

                        }
                        else if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType2)
                        {
                            index = 53;
                        }
                        ExcelUtils.FillCell($"N{index}", sheet, partialDischargeInfo.DischargeA);
                        ExcelUtils.FillCell($"Z{index}", sheet, partialDischargeInfo.DischargeB);
                        ExcelUtils.FillCell($"AL{index}", sheet, partialDischargeInfo.DischargeC);
                    }
                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("报告主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }

        //无低压 高压五分接和三分接兼容母本
        public static bool WriteToWithoutLowVoltage53Test(string templatePath, string reportFile, string sequence, ProductInfo? productInfo, FigureNumberInfo? figInfo,
            List<VoltageCurrentLossDataInfo>? voltageDatas, List<ImpedenceResistanceInfo>? impedenceResistanceInfos, List<PartialDischargeInfo>? partialDischargeInfos)
        {
            // 读取 Excel 文件
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("数据导入"); // 假设我们要处理第一个 sheet

                ExcelUtils.FillCell("AM3", sheet, sequence);
                if (productInfo != null)
                {
                    ExcelUtils.FillCell("L3", sheet, productInfo.UserName);
                    ExcelUtils.FillCell("AM3", sheet, productInfo.Sequence);
                    ExcelUtils.FillCell("AM4", sheet, productInfo.FigureNumber);
                }

                if (voltageDatas != null && voltageDatas.Count > 0)
                {
                    foreach (var TestValue in voltageDatas)
                    {
                        if (TestValue == null)
                        {
                            continue;
                        }
                        if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeNoLoad)
                        {
                            // 电压有效值的线电压
                            var uab = Utils.CalculateLineVoltage(TestValue.ua, TestValue.ub);
                            var ubc = Utils.CalculateLineVoltage(TestValue.ub, TestValue.uc);
                            var uca = Utils.CalculateLineVoltage(TestValue.uc, TestValue.ua);
                            var up = (uab + ubc + uca) / 3;

                            // 电压平均值的线电压
                            var uabp = Utils.CalculateLineVoltage(TestValue.pua, TestValue.pub);
                            var ubcp = Utils.CalculateLineVoltage(TestValue.pub, TestValue.puc);
                            var ucap = Utils.CalculateLineVoltage(TestValue.puc, TestValue.pua);
                            var upp = (uabp + ubcp + ucap) / 3;
                            ExcelUtils.FillCell("B39", sheet, uab);
                            ExcelUtils.FillCell("H39", sheet, ubc);
                            ExcelUtils.FillCell("N39", sheet, uca);
                            ExcelUtils.FillCell("T39", sheet, up);

                            ExcelUtils.FillCell("Z39", sheet, uabp);
                            ExcelUtils.FillCell("AF39", sheet, ubcp);
                            ExcelUtils.FillCell("AL39", sheet, ucap);
                            ExcelUtils.FillCell("AR39", sheet, upp);
                        }
                        else if (TestValue.LoadType == VoltageCurrentLossDataInfo.LoadTypeZero)
                        {
                            ExcelUtils.FillCell("B47", sheet, TestValue.ua);
                            ExcelUtils.FillCell("J47", sheet, TestValue.ia);
                            ExcelUtils.FillCell("R47", sheet, TestValue.pa);
                            ExcelUtils.FillCell("AN45", sheet, TestValue.Temperature);
                        }
                    }
                }

                if (partialDischargeInfos != null && partialDischargeInfos.Count > 0)
                {
                    foreach (var partialDischargeInfo in partialDischargeInfos)
                    {
                        int index = 56;
                        if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType1)
                        {

                        }
                        else if (partialDischargeInfo.Voltage == PartialDischargeInfo.VoltageType2)
                        {
                            index = 57;
                        }
                        ExcelUtils.FillCell($"N{index}", sheet, partialDischargeInfo.DischargeA);
                        ExcelUtils.FillCell($"Z{index}", sheet, partialDischargeInfo.DischargeB);
                        ExcelUtils.FillCell($"AL{index}", sheet, partialDischargeInfo.DischargeC);
                    }
                }

                WriteFigureNumberInfoToReport(figInfo, sheet);

                // 刷新公式计算
                sheet.ForceFormulaRecalculation = true;

                // 转到报告页面
                sheet = workbook.GetSheet("报告主页");
                sheet.ForceFormulaRecalculation = true;

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(reportFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(outputFileStream);
                }
                workbook.Close();
            }

            return true;
        }
    }
}
