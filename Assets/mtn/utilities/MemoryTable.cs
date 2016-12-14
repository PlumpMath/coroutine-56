using System.Collections.Generic;
using System.Linq;

namespace mtn
{

    public class TableRow
    {
        private Dictionary<string, int> columnNames;
        private List<string> columns; 
        public string getColValue(string colName)
        {
            string colvalue = null;
            if (columnNames.ContainsKey(colName))
            {
                int colnum = columnNames[colName];
                colvalue = columns[colnum];
            }
            return colvalue;
        }
        public TableRow(Dictionary<string, int> colNames, List<string> cols)
        {
            this.columnNames = colNames;
            this.columns = cols;
        }
    }
    public class MemoryTable
    {
        public bool encrypted { get; set; }
        public string tableName { get; set; }
        public Dictionary<string, List<string>> tableData { get; set; }
        public string csvfile { get; set; }
        public Dictionary<string, int> hdrColumns { get; set; }
        public List<string> headers { get; set; }
        public int NumRows { get { return tableData.Count; } }

        public MemoryTable()  {}
        public MemoryTable(string aFile, bool encryptopt=false) : base()
        {
            encrypted = encryptopt;
            csvfile = aFile;
            // Default Table Name to file Name
            string[] pathArr = csvfile.Split('\\');
            string[] fileArr = pathArr.Last().Split('.');
            tableName = fileArr.Last().ToString();
            empty();
        }

        public void empty()
        {
            tableData = new Dictionary<string, List<string>>();
            hdrColumns = new Dictionary<string, int>();
            headers = new List<string>();
        }

       // public Dictionary<string, List<string>> TableData { get { return tableData; } }
       // public Dictionary<string, int> HeaderData { get { return hdrColumns; } }
       // public List<string> Headers { get { return headers; } }
        public List<TableRow> getAllRows()
        {
            List<TableRow> allRows = new List<TableRow>();
            foreach (List<string> row in tableData.Values)
            {
                allRows.Add(new TableRow(hdrColumns, row));
            }
            return allRows;
        }
        public List<string> getRow(string key)
        {
            if (tableData.ContainsKey(key))
                return tableData[key];
            else
                return null;
        }
        public TableRow getTableRow(string key)
        {
            List<string> row = getRow(key);
            TableRow tableRow = null;
            if (row != null)
            {
                tableRow = new TableRow(hdrColumns, row);
            }
            return tableRow;
        }
        public bool updateRow(string key, string col, string colvalue)
        {
            bool updated = false;
            List<string> row = getRow(key);
            if (row != null)
            {
                if (hdrColumns.ContainsKey(col))
                {
                    int colno = hdrColumns[col];
                    row[colno] = colvalue;
                    updated = true;
                }
            }
            return updated;
        }
        public void updateRow(string[] arg)
        {
            bool rowExists = tableData.ContainsKey(arg[0]);
            if (rowExists)
            {
                List<string> columns = new List<string>();
                foreach (var col in arg) columns.Add(col);
                tableData[columns[0]] = columns;
            }
            else
            {
                addRow(arg);
            }

        }
        public void addRow(string[] arg)
        {
            bool isHeader = (headers.Count == 0);
            List<string> columns = null;
            if (!isHeader) columns = new List<string>();
            int colnum = 0;
            foreach (var col in arg)
            {
                if (isHeader)
                {
                    hdrColumns.Add(col, colnum);
                    headers.Add(col);
                }
                else
                {
                    columns.Add(col);
                }
                ++colnum;
            }
            if (!isHeader)
            {
                tableData.Add(columns[0], columns);
            }
        }
        public void write()
        {
            using (CsvFileWriter writer = new CsvFileWriter(csvfile,encrypted))
            {
                CsvRow hdrow = new CsvRow();
                foreach (var hdrcol in headers)
                {
                    hdrow.Add(hdrcol);
                }
                writer.WriteRow(hdrow);
                foreach (var item in tableData)
                {
                    List<string> row = item.Value;
                    CsvRow csvrow = new CsvRow();
                    foreach (var col in row)
                    {
                        csvrow.Add(col);
                    }
                    writer.WriteRow(csvrow);
                }

            }
        }
        public void read()
        {
            bool hdrow = true;
            // Read sample data from CSV file
            List<string> columns = new List<string>();

            using (CsvFileReader reader = new CsvFileReader(csvfile,encrypted))
            {
                CsvRow row = new CsvRow();
                int colnum = 0;
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        if (hdrow)
                        {
                            hdrColumns.Add(s, colnum);
                            headers.Add(s);
                            ++colnum;
                        }
                        else {
                            columns.Add(s);
                        }
                    }
                    if (hdrow)
                    {
                        hdrow = false;
                    }
                    else {
                        tableData.Add(columns[0], columns);
                 
                    }

                    columns = new List<string>();
                }
            }
        }
    }
}
