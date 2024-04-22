using NPOI.SS.UserModel;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Report
{
    internal class ExcelUtils
    {
        public static void FillCellResistance(String cellIndex, ISheet sheet, String value, double unitBase = 1000)
        {
            if (value == null || value.Length == 0)
            {
                return;
            }
            try
            {
                double v = double.Parse(value) * unitBase;
                //Log.Info($"FillCellResistance, cell = {cellIndex} sourceValue = {value} calcValue = {v}");
                FillCell(cellIndex, sheet, v);
            }
            catch (Exception e)
            {
                Log.Error($"fail to fill resistance, {value}, " + e.ToString());
            }
        }

        public static void FillCellAsString(String cellIndex, ISheet sheet, String value)
        {
            FillCell(cellIndex, sheet, value, true);
        }

        public static void FillCell(String cellIndex, ISheet sheet, Object value, bool asString = false)
        {
            if (value == null || (value.GetType() == typeof(String) && ((String)value).Trim().Length == 0))
            {
                return;
            }
            int row, col;
            ConvertCellPosition(cellIndex, out row, out col);

            IRow sheetRow = sheet.GetRow(row);
            ICell cell;
            cell = sheetRow.GetCell(col);
            if (cell == null)
            {
                cell = sheetRow.CreateCell(col, CellType.String);
            }
            if (value.GetType() == typeof(double) || value.GetType() == typeof(float))
            {
                cell.SetCellType(CellType.Numeric);
                double v = Convert.ToDouble(value);
                cell.SetCellValue(v);
            }
            else if (value.GetType() == typeof(string))
            {
                int iValue; double dValue;

                Type type = Utils.ToNumberic((String)value, out dValue, out iValue);
                if (type == typeof(String) || asString)
                    cell.SetCellValue((string)value);
                else if (type == typeof(int))
                {
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(iValue);
                }
                else if (type == typeof(double))
                {
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(dValue);
                }
            }
            else if (value.GetType() == typeof(int))
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((int)value);
            }
            else
                cell.SetCellValue(value.ToString());
        }

        public static void ConvertCellPosition(string cellPosition, out int row, out int column)
        {
            row = 0;
            column = 0;

            string columnStr = string.Empty;
            foreach (char c in cellPosition)
            {
                if (Char.IsLetter(c))
                {
                    columnStr += c;
                }
                else
                {
                    break;
                }
            }

            column = ConvertColumnNameToNumber(columnStr);
            column = Math.Max(column - 1, 0);

            if (int.TryParse(cellPosition.Substring(columnStr.Length), out row))
            {
                row = Math.Max(0, row - 1);
            }
            else
            {
                row = 0;
            }
        }

        private static int ConvertColumnNameToNumber(string columnName)
        {
            int result = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                result *= 26;
                result += columnName[i] - 'A' + 1;
            }
            return result;
        }

        public static void _ParseCellIndex(String cellIndex, out int row, out int col)
        {
            String cellString = cellIndex.ToUpper();
            int splitIndex = 0;
            for (int i = 0; i < cellString.Length; i++)
            {
                if (cellString[i] >= 'A' && cellString[i] <= 'Z')
                {
                    splitIndex++;
                    continue;
                }
                break;
            }

            String rowStr = cellString.Substring(splitIndex);
            String colStr = cellString.Substring(0, splitIndex);

            row = int.Parse(rowStr) - 1;
            if (colStr.Length == 1)
            {
                col = colStr[0] - 'A';
            }
            else
            {
                col = 0;
            }

            //Console.WriteLine($"{cellIndex} {rowStr} {colStr} {row} {col}");
        }

    }
}
