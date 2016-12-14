using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NUnit.Framework;

namespace mtn
{
    
    public class TestUtilitiesStub
    {
        public static void Main()
        {
        }
    }

    [TestFixture]
    public class TestUtilities
    {

        TestObject tstobj = null;
        MemoryTable outputTable = null;
        string EncryptedMemoryTableFile = "c:/temp3/encryptedMemoryTable.csv";

        string UETableFile = "c:/temp3/UnEncryptedMemoryTable.csv";
        string UETableName = "UnEncryptedMemoryTable";

        string EncryptedSerializedTestObject = "c:/temp3/encryptedTstobj.txt";
        string serializedTestObject  = "c:/temp3/tstobj.xml";

        [SetUp]
        public void Initialize() {
            tstobj = new TestObject();
            tstobj.aDictionary.Add("Fourth", "Fourth String");
            outputTable = new MemoryTable(EncryptedMemoryTableFile, true);
            outputTable.tableName = "Initial Encrypted Table: Written to: " + outputTable.csvfile;
            string[] hdr = { "Product", "Description", "Quantity", "Cost", "Total" };
            string[] data1 = { "Honda", "Honda Civic", "2000", "200", "1000" };
            string[] data2 = { "Acura", "Acura Integra", "5000", "450", "8000" };

            string[] insertData = { "BMW", "Bonn Motor Works", "5000", "450", "8000" };
            outputTable.addRow(hdr);
            outputTable.addRow(data1);
            outputTable.addRow(data2);
            outputTable.updateRow("Honda", "Description", "Honda Accord");
            outputTable.updateRow(insertData);
            outputTable.write();
            Console.WriteLine("Output Table written to file: " + outputTable.csvfile);
        }
        [Test] 
        public void TestEncryption()
        {
            string teststr = "Random String";
            string encrypted = CryptoUtils.EncryptStringAES(teststr);
            Console.WriteLine(teststr + " -- Encrypted with key=" + CryptoUtils.getKey() + " : " + encrypted);
        }
        [Test]
        public void TestEncryptedMemoryTable()
        { 
            dumpTable(outputTable);
            // read encrypted table back
            MemoryTable encryptedTable = new MemoryTable(EncryptedMemoryTableFile,true);
            encryptedTable.tableName = "Read Encrypted MemoryTable From " + EncryptedMemoryTableFile;
            encryptedTable.read();
            dumpTable(encryptedTable);

        }
        [Test]
        public void TestNonEncryptedMemoryTable()
        {
            outputTable.encrypted = false;
            outputTable.csvfile = UETableFile;
            outputTable.tableName = UETableName;
            outputTable.write();

            MemoryTable readTable = new MemoryTable(UETableFile);
            readTable.read();
            dumpTable(readTable);
        }

        [Test]
        public void TestSerialization()
        {
            Assert.AreEqual(10, tstobj.var1);
            Assert.AreEqual(289.01f, tstobj.var2);
            Assert.AreEqual("Random String", tstobj.var3);

            SerializationUtility.SerializeObject(tstobj, serializedTestObject);
            TestObject obj2 = SerializationUtility.DeSerializeObject<TestObject>(serializedTestObject);

            Assert.AreEqual(tstobj.var1, obj2.var1);
            Assert.AreEqual(tstobj.var2, obj2.var2);
            Assert.AreEqual(tstobj.var3, obj2.var3);
            Assert.AreEqual(tstobj.aDictionary["Fourth"], "Fourth String");
            Assert.AreEqual(tstobj.aDictionary["Fourth"], obj2.aDictionary["Fourth"]);

        }
        [Test]
        public void TestEncryptedSerialization()
        {
            Assert.AreEqual(10, tstobj.var1);
            Assert.AreEqual(289.01f, tstobj.var2);
            Assert.AreEqual("Random String", tstobj.var3);

            SerializationUtility.SerializeObject(tstobj, EncryptedSerializedTestObject,true);
            TestObject obj3 = SerializationUtility.DeSerializeObject<TestObject>(EncryptedSerializedTestObject,true);

            Assert.AreEqual(tstobj.var1, obj3.var1);
            Assert.AreEqual(tstobj.var2, obj3.var2);
            Assert.AreEqual(tstobj.var3, obj3.var3);
            Assert.AreEqual(tstobj.aDictionary["Fourth"], "Fourth String");
            Assert.AreEqual(tstobj.aDictionary["Fourth"], obj3.aDictionary["Fourth"]);

        }
        public void dumpTable(MemoryTable testTable)
        {
            // dump table using TableRow.get(key) syntax
            List<string> headers = testTable.headers;
            Console.WriteLine(testTable.tableName + " has Number of Rows = " + testTable.NumRows);
            List<TableRow> allRows = testTable.getAllRows();
            int rownum = 0;
            foreach (TableRow row in allRows)
            {
                ++rownum;
                 Console.Write("--> Row: " + rownum);
                 foreach(string colname in headers)
                {
                    string keyval = row.getColValue(colname);
                    Console.Write(" :" + colname + "=" + keyval);
                }
                Console.WriteLine();
            }
           
            // iterate and display all entries in the memory table using array syntax
            Dictionary<string, List<string>> tableData = testTable.tableData;
            Console.WriteLine("Number of header columns=" + headers.Count);
            StringBuilder builder = new StringBuilder();
            builder.Append("Table Data for: " + testTable.csvfile + " ---> \n");
            foreach (var item in tableData)
            {
                var key = item.Key;
                builder.Append("Row Key: " + key);
                List<string> row = item.Value;
                int colnum = 0;
                foreach (var col in row)
                {
                    builder.Append(" : " + headers[colnum] + "=" + col);
                    ++colnum;
                }
                builder.Append("\n");
            }
            Console.WriteLine(builder.ToString());
        }
    }
}
