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
            var password = "## Change tins to your private passphrase ##";
            var encrypted = Encryption.EncryptString(input, password);
            var decrypted = Encryption.DecryptString(encrypted, password);

            Assert.AreEqual(input, decrypted);
        }
    }
}
