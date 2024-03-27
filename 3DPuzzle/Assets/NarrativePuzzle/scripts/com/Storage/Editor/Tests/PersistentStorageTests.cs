using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace com
{
    [TestFixture]
    public class PersistentStorageTests
    {

        [Test]
        public static void Test_GetSetObjects()
        {
            //PersistentStorage.Clear();
            //var testObj = new TestClass { aFloat = 0.5f, aString = "test", anInt = 4, anIntList = new List<int> { 1, 2, 3 } };
            //PersistentStorage.SetObject( "test object", testObj );
            //Assert.IsTrue( testObj.Equals( PersistentStorage.GetObject<TestClass>( "test object" ) ), "The object returned from the storage should be equal to the one passed to SetObject method" );
            //PersistentStorage.Save();
            //Assert.IsTrue( testObj.Equals( PersistentStorage.GetObject<TestClass>( "test object" ) ), "The object returned from the storage should still be equal after saving" );
            //PersistentStorage.Load();
            //Assert.IsTrue( testObj.Equals( PersistentStorage.GetObject<TestClass>( "test object" ) ), "The object returned from the storage should still be equal after loading" );
        }

        [Test]
        public static void Test_GetNonExistentObject()
        {
            //PersistentStorage.Clear();
            //var obj = PersistentStorage.GetObject<TestClass>( "does not exist" );
            //Assert.IsNull( obj, "The returned object should be null and the GetObject method should not throw an exception" );
        }

        [Test]
        public static void Test_SaveByReference()
        {
            //var testObj = new TestClass { aFloat = 0.5f, aString = "test", anInt = 4, anIntList = new List<int> { 1, 2, 3 } };
            //PersistentStorage.Clear();
            //PersistentStorage.SetObject( "test object", testObj);
            //testObj.aFloat = 2f;
            //PersistentStorage.Save();
            //PersistentStorage.Load();
            //var testCopy = PersistentStorage.GetObject<TestClass>("test object");
            //Assert.IsTrue( testCopy.aFloat == 2f, "The object should be saved by reference (no explicit second SetObject is required)" );
        }

        [System.Serializable]
        public class TestClass
        {
            public float aFloat;
            public string aString;
            public int anInt;
            public List<int> anIntList;

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                var testObj = obj as TestClass;
                if (testObj == null) return false;

                return aFloat.Equals(testObj.aFloat) &&
                    aString.Equals(testObj.aString) &&
                    anInt.Equals(testObj.anInt) &&
                    (anIntList == null && testObj.anIntList == null || anIntList.SequenceEqual(testObj.anIntList));
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + aFloat.GetHashCode();
                hash = (hash * 7) + aString.GetHashCode();
                hash = (hash * 7) + anInt.GetHashCode();
                hash = (hash * 7) + anIntList.GetHashCode();
                return hash;
            }
        }
    }
}
