using System;

namespace mtn
{
    public interface ITestObject
    {
        string TestMethod(string aName);
    }
    public class TestObject : ITestObject
    {
        public int var1 { get; set; }
        public float var2 { get; set; }
        public string var3 { get; set; }
        public CustomDictionary<string, object> aDictionary {get; set; }
        public TestObject():base()
        {
            init();
        }
        public void init()
        {
            var1 = 10;
            var2 = 289.01f;
            var3 = "Random String";
            aDictionary = new CustomDictionary<string, object>();
            aDictionary.Add("first", var1);
            aDictionary.Add("second", var2);
            aDictionary.Add("third", var3);
        }
      
        public string TestMethod(string aName)
        {
            Console.WriteLine("in TestObject.TestMethod Hello --> " + aName);
            return ("Returned value from TestObject.TestMethod(string aName): " + aName);
        }

    }
}
