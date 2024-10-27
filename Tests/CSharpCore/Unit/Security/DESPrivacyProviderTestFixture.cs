﻿/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2010/12/5
 * Time: 15:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

#pragma warning disable CS0618 // Type or member is obsolete
namespace Lextm.SharpSnmpLib.Unit.Security
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Lextm.SharpSnmpLib.Security;

    public class DESPrivacyProviderTestFixture
    {
        [Fact]
        public void TestException()
        {
            var provider = new DESPrivacyProvider(new OctetString("longlongago"), new MD5AuthenticationProvider(new OctetString("verylonglongago")));
            Assert.Throws<ArgumentNullException>(() => new DESPrivacyProvider(null, null));
            Assert.Throws<ArgumentNullException>(() => new DESPrivacyProvider(OctetString.Empty, null));
            Assert.Throws<ArgumentNullException>(() => provider.Encrypt(null, null));
            Assert.Throws<ArgumentNullException>(() => provider.Encrypt(OctetString.Empty, null));
            {
                var exception = Assert.Throws<ArgumentException>(() => provider.Encrypt(new Null(), SecurityParameters.Create(OctetString.Empty)));
                Assert.Contains("Invalid data type.", exception.Message);
            }

            Assert.Throws<ArgumentNullException>(() => provider.Decrypt(null, null));
            Assert.Throws<ArgumentNullException>(() => provider.Decrypt(OctetString.Empty, null));
            {
                var exception = Assert.Throws<ArgumentException>(() => provider.Decrypt(new Null(), SecurityParameters.Create(OctetString.Empty)));
                Assert.Contains("Cannot decrypt the scope data: Null.", exception.Message);
            }
        }

        [Fact]
        public void TestIsSupported()
        {
#if NET6_0_OR_GREATER
            Assert.True(DESPrivacyProvider.IsSupported);
#elif NET471
                Assert.True(DESPrivacyProvider.IsSupported);
#endif
        }

        [Fact]
        public void TestDescrypt2()
        {
            byte[] encrypted = ByteTool.Convert("04 38 A4 F9 78 15 2B 14 45 F7 4F C5 B2 1C 82 72 9A 0B D9 EE C1 17 3E E1 26 0D 8B D4 7B 0F D7 35 06 1B E2 14 0D 4A 9B CA BF EF 18 6B 53 B9 FA 70 95 D0 15 38 C5 77 96 85 61 40");
            IPrivacyProvider priv;
            if (DESPrivacyProvider.IsSupported)
            {
                priv = new DESPrivacyProvider(new OctetString("privacyphrase"), new MD5AuthenticationProvider(new OctetString("authentication")));
            }
            else
            {
                return;
            }

            var parameters = new SecurityParameters(
                new OctetString(ByteTool.Convert("80001F8880E9630000D61FF449")),
                Integer32.Zero,
                Integer32.Zero,
                new OctetString("lextm"),
                OctetString.Empty,
                new OctetString(ByteTool.Convert("0000000069D39B2A")));
            var data = priv.Decrypt(DataFactory.CreateSnmpData(encrypted),
                                         parameters);
            Assert.Equal(SnmpType.Sequence, data.TypeCode);

            byte[] net =
                ByteTool.Convert(
                    "04 38 A4 F9 78 15 2B 14 45 F7 4F C5 B2 1C 82 72 9A 0B D9 EE C1 17 3E E1 26 0D 8B D4 7B 0F D7 35 06 1B E2 14 0D 4A 9B CA BF EF 18 6B 53 B9 FA 70 95 D0 5D AF 04 5A 68 B5 DA 73");
            var netData = priv.Decrypt(DataFactory.CreateSnmpData(net), parameters);
            Assert.Equal(SnmpType.Sequence, netData.TypeCode);

            Assert.Equal(ByteTool.Convert(netData.ToBytes()), ByteTool.Convert(data.ToBytes()));
        }

        [Fact]
        public void TestDecrypt()
        {
            if (!DESPrivacyProvider.IsSupported)
            {
                return;
            }

            byte[] encrypted = ByteTool.Convert("4B  4F 10 3B 73  E1 E4 BD 91  32 1B CB 41" +
                 "1B A1 C1 D1  1D 2D B7 84  16 CA 41 BF  B3 62 83 C4" +
                 "29 C5 A4 BC  32 DA 2E C7  65 A5 3D 71  06 3C 5B 56" +
                 "FB 04 A4");
            byte[] real = DESPrivacyProvider.Decrypt(encrypted, new byte[] { 0x37, 0xc6, 0x4c, 0xad, 0x49, 0x37, 0xfe, 0xda, 0x57, 0xc8, 0x48, 0x53, 0x47, 0x2a, 0x2e, 0xc0 },
                new byte[] { 0x00, 0x00, 0x00, 0x01, 0x44, 0x2c, 0xa3, 0xb5 });
            byte[] expected = ByteTool.Convert("30  2D  04 0D 80 00 1F 88 80  E9 63 00 00  D6 1F F4 49 04 00 A0 1A 02 02 3A 25  02 01 00 02  01 00 30 0E  30 0C 06 08 2B 06 01 02  01 01 03 00  05 00 01");
            Assert.Equal(expected, real);
        }

        [Fact]
        public void TestEncrypt()
        {
            if (!DESPrivacyProvider.IsSupported)
            {
                return;
            }

            byte[] decrypted = ByteTool.Convert("30  2D  04 0D 80 00 1F 88 80  E9 63 00 00  D6 1F F4 49 04 00 A0 1A 02 02 3A 25  02 01 00 02  01 00 30 0E  30 0C 06 08 2B 06 01 02  01 01 03 00  05 00 01");
            byte[] fake = DESPrivacyProvider.Encrypt(decrypted, new byte[] { 0x37, 0xc6, 0x4c, 0xad, 0x49, 0x37, 0xfe, 0xda, 0x57, 0xc8, 0x48, 0x53, 0x47, 0x2a, 0x2e, 0xc0 },
                new byte[] { 0x00, 0x00, 0x00, 0x01, 0x44, 0x2c, 0xa3, 0xb5 });
            byte[] expected = ByteTool.Convert("4B  4F 10 3B 73  E1 E4 BD 91  32 1B CB 41" +
                 "1B A1 C1 D1  1D 2D B7 84  16 CA 41 BF  B3 62 83 C4" +
                 "29 C5 A4 BC  32 DA 2E C7  65 A5 3D 71  06 3C 5B 56" +
                 "FB 04 A4");
            Assert.Equal(expected, fake);
        }

        [Fact]
        public void TestEncrypt2()
        {
            byte[] expected = ByteTool.Convert("04 30 4B  4F 10 3B 73  E1 E4 BD 91  32 1B CB 41" +
                "1B A1 C1 D1  1D 2D B7 84  16 CA 41 BF  B3 62 83 C4" +
                "29 C5 A4 BC  32 DA 2E C7  65 A5 3D 71  06 3C 5B 56" +
                "FB 04 A4");
            OctetString engineId = new OctetString(ByteTool.Convert("80 00 1F 88 80  E9 63 00 00  D6 1F F4 49"));
            IPrivacyProvider priv;
            if (DESPrivacyProvider.IsSupported)
            {
                priv = new DESPrivacyProvider(new OctetString("passtest"), new MD5AuthenticationProvider(new OctetString("testpass")));
            }
            else
            {
                return;
            }

            Scope scope = new Scope(engineId, OctetString.Empty, new GetRequestPdu(0x3A25, new List<Variable> { new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.3.0")) }));
            SecurityParameters parameters = new SecurityParameters(engineId, new Integer32(0x14), new Integer32(0x35), new OctetString("lexmark"), new OctetString(new byte[12]), new OctetString(ByteTool.Convert("00 00 00  01 44 2C A3 B5")));
            ISnmpData data = priv.Encrypt(scope.GetData(VersionCode.V3), parameters);
            Assert.Equal(SnmpType.OctetString, data.TypeCode);
            Assert.Equal(ByteTool.Convert(expected), ByteTool.Convert(data.ToBytes()));
        }

#if NET6_0_OR_GREATER
        [Theory]
        [MemberData(nameof(Data))]
        public void CompatibilityTest(int length)
        {
            var generator = new Random();
            var data = new byte[length];
            generator.NextBytes(data);
            var iv = new byte[8];
            generator.NextBytes(iv);
            var key = new byte[8];
            generator.NextBytes(key);

            {
                var encrypted = DESPrivacyProvider.LegacyEncrypt(key, iv, data);
                var decrypted = DESPrivacyProvider.Net6Decrypt(key, iv, encrypted);
                Assert.Equal(data, decrypted);
            }

            {
                var encrypted = DESPrivacyProvider.Net6Encrypt(key, iv, data);
                var decrypted = DESPrivacyProvider.LegacyDecrypt(key, iv, encrypted);
                Assert.Equal(data, decrypted);
            }

            {
                var encrypted1 = DESPrivacyProvider.LegacyEncrypt(key, iv, data);
                var encrypted2 = DESPrivacyProvider.Net6Encrypt(key, iv, data);
                Assert.Equal(encrypted1, encrypted2);
            }
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                for (int start = 1; start <= 256; start++)
                {
                    yield return new object[] { start * 8 };
                }
            }
        }
#endif
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
