using ExcelDataReader;
using System.Data;

namespace WebScraping.Services;

public static class FileEditor
{
    public static DataSet? GetDataSetFromFile(string directoryPath, string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(fileName))
                return null;

            var filePath = Path.Combine(directoryPath, fileName);
            if (!File.Exists(filePath))
                return null;

            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            excelReader.Close();
            Console.WriteLine($"Data set was readed!");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return null;
        }
    }
    public static DataTable? TransposeDataTable(DataSet dataSet)
    {
        DataTable transposedTable = new DataTable();

        try
        {
            DataTable? data1Table = dataSet.Tables["Data1"];

            if (data1Table == null)
            {
                throw new ArgumentException("Data1 table not found in the DataSet");
            }

            int rowIndex = data1Table.Rows.Cast<DataRow>()
                                            .Select((row, index) => new { Row = row, Index = index })
                                            .FirstOrDefault(x => x.Row[0].ToString() == "Series ID")?.Index ?? -1;

            if (rowIndex == -1)
            {
                throw new ArgumentException("Series ID not found in Data1 table");
            }

            var columnNames = data1Table.Rows.Cast<DataRow>()
                                             .Skip(rowIndex)
                                             .Select(row => row[0].ToString());

            foreach (var columnName in columnNames)
            {
                transposedTable.Columns.Add(columnName, typeof(object));
            }

            for (int i = 0; i < data1Table.Columns.Count; i++)
            {
                DataRow newRow = transposedTable.NewRow();

                for (int j = 0, k = rowIndex; j < data1Table.Rows.Count - rowIndex; j++, k++)
                {
                    var value = data1Table.Rows[k][i];
                    newRow[j] = value;
                }

                transposedTable.Rows.Add(newRow);
            }
            Console.WriteLine($"Transposing table is saccess!");
            return transposedTable;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return null;
        }
    }
    public static void SaveAsCsvFile(string path, string filename, DataTable data)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(filename) || data is null)
            return;
        var filePath = Path.Combine(path, filename);

        Console.WriteLine("Prepare to daving data...");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(string.Join(",", data.Columns));

            foreach (DataRow row in data.Rows)
            {
                writer.WriteLine(string.Join(",", row.ItemArray));
            }
        }
        Console.WriteLine($"Data successfuly saved into the folder: \'{path}\',\nFile name is: \'{filename}\'");
    }
}
