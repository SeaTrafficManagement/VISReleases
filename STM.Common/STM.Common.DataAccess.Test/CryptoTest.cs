using Microsoft.VisualStudio.TestTools.UnitTesting;
using STM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Test
{
    [TestClass]
    public class CryptoTest
    {
        [TestMethod]
        public void PassingTest()
        {
            var input = "Hemligt!";
            var password = "A_vErry secret paSsw0rd#that is not easy to Re2MeMbeR!";
            var encrypted = Encryption.EncryptString(input, password);
            var decrypted = Encryption.DecryptString(encrypted, password);

            Assert.AreEqual(input, decrypted);
        }
    }
}
