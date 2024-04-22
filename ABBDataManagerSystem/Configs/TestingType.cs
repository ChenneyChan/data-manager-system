namespace ABBDataManagerSystem.Configs
{
    public enum TestingType
    {
        // 干式变压器试验
        DryTypeTransformerTesting = 0,

        // 消弧试验
        ArcSuppressionCoilTesting,

        // 高阻抗
        HighImpedanceTesting,

        // 接地变
        GroudingTesting,

        // 消弧试验
        DrySeriesConnectionTesting,

        // 功率分析仪
        PowerAnalyzer,

        // 产品信息页面
        ProductInfo,

        // 横河温升试验
        TemperatureTestHengHe,

        // 虹润温升试验
        TemperatureTestHongRun,

        // 局放试验
        PartialDischargeTesting,

        // 报告查看打印
        RepotPrinter,

        // 干式变压器试验-WT300
        DryTypeTransformerTestingWT300,
    }
}
