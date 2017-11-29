using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerForTheLogic.ClientObject;

namespace DataAccessLayer.Tests
{
    /// <summary>
    /// MongoDALTests
    /// Team: DB
    /// Unit tests for CRUD functions to DB.
    /// Author: Michael, Sean, Stephanie, Bill 
    /// Date: 2017-11-21
    /// Based on: https://msdn.microsoft.com/en-us/library/hh694602.aspx
    /// </summary>
    [TestClass()]
    public class MongoDALTests
    {
        [TestMethod()]
        public void CreatePersonTest()
        {
            Guid newGuid = Guid.NewGuid();
            Person testPerson = new Person(newGuid, "Fn", "Ln", 100, 200, Guid.NewGuid().ToString(), 1, 1, Guid.NewGuid().ToString(), 2, 2, 100, 50, 7, 9);
            Assert.IsNotNull(testPerson);
            Assert.IsTrue(DALValidator.DALPersonValidator(testPerson));
        }

        [TestMethod()]
        public void InsertPersonTest()
        {
            //Arrange
            MongoDAL db = new MongoDAL();
            Guid newGuid = Guid.NewGuid();
            Person testPerson = new Person(newGuid, "Fn", "Ln", 100, 200, Guid.NewGuid().ToString(), 1, 1, Guid.NewGuid().ToString(), 2, 2, 100, 50, 7, 9);

            //Act
            db.InsertPerson(testPerson);

            //Assert
            Object returnedPerson = db.GetObjectByGuid(newGuid);
            Assert.IsNotNull(returnedPerson);
        }

        [TestMethod()]
        public void InsertProductTest()
        {
            //Arrange
            MongoDAL db = new MongoDAL();
            Product testProduct = new Product("test_product_1", 50);

            //Act
            db.InsertProduct(testProduct);

            //Assert
            Product returnedProduct = db.GetProduct("test_product_1");
            Console.WriteLine(returnedProduct.Name);
            Assert.AreEqual(returnedProduct.GlobalCount, 50);
        }

        [TestMethod()]
        public void InsertClockTest()
        {
            MongoDAL db = new MongoDAL();
            Clock testClock = new Clock(55, 22, 25, 1990);
            db.InsertClock(testClock);

            Clock returnedClock = db.GetClock();
            Assert.AreEqual(55, returnedClock.NetMinutes);
        }

        [TestMethod()]
        public void UpdateClockTest()
        {
            MongoDAL db = new MongoDAL();
            db.UpdateClock(50, 22, 25, 1990);
            Clock returnedClock = db.GetClock();
            Assert.AreEqual(50, returnedClock.NetMinutes);
        }

        [TestMethod()]
        public void UpdateProductByNameTest()
        {
            MongoDAL db = new MongoDAL();
            db.UpdateProductByName("test_product_1", 20);

            MongoDAL db1 = new MongoDAL();
            Product returnedProduct = db1.GetProduct("test_product_1");
            Assert.AreEqual(20, returnedProduct.GlobalCount);

            db.UpdateProductByName("test_product_1", 30);

            MongoDAL db2 = new MongoDAL();
            Product returnedProduct2 = db2.GetProduct("test_product_1");
            Assert.AreEqual(30, returnedProduct2.GlobalCount);
        }
    }
}