using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace ABBDataManagerSystem.Report
{
    internal class ExcelConverter
    {
        public static void ConvertToXLSX(string inputFilePath, string outputFilePath)
        {
            // 读取 Excel 97-2003 文件
            using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook hssfWorkbook = new HSSFWorkbook(fileStream);

                // 创建新的 Excel 2007+ 文件
                XSSFWorkbook xssfWorkbook = new XSSFWorkbook();

                // 复制数据
                for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
                {
                    ISheet sourceSheet = hssfWorkbook.GetSheetAt(i);
                    ISheet targetSheet = xssfWorkbook.CreateSheet(sourceSheet.SheetName);

                    for (int j = 0; j <= sourceSheet.LastRowNum; j++)
                    {
                        IRow sourceRow = sourceSheet.GetRow(j);
                        IRow targetRow = targetSheet.CreateRow(j);

                        if (sourceRow != null)
                        {
                            for (int k = 0; k < sourceRow.LastCellNum; k++)
                            {
                                ICell sourceCell = sourceRow.GetCell(k);
                                ICell targetCell = targetRow.CreateCell(k);

                                if (sourceCell != null)
                                {
                                    // 复制单元格类型和值
                                    targetCell.SetCellType(sourceCell.CellType);
                                    switch (sourceCell.CellType)
                                    {
                                        case CellType.Numeric:
                                            targetCell.SetCellValue(sourceCell.NumericCellValue);
                                            break;
                                        case CellType.String:
                                            targetCell.SetCellValue(sourceCell.StringCellValue);
                                            break;
                                            // 其他类型可以根据需要添加
                                    }
                                }
                            }
                        }
                    }
                }

                // 保存到新文件
                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    xssfWorkbook.Write(outputFileStream);
                }
            }

            Console.WriteLine("转换完成，输出文件路径：" + outputFilePath);
        }
    }
}
