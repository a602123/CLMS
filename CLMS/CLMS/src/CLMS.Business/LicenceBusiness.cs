using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZZBFBY.CommonTools_Core.Cryptography;

namespace CLMS.Business
{
    public class LicenceBusiness
    {
        private static LicenceBusiness _instance;


        private string _machineCode;
        private bool _licenceState;
        private int _licenceNumber;

        public static LicenceBusiness GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LicenceBusiness();
            }
            return _instance;
        }

        private LicenceBusiness()
        {
            _machineCode = CreateMachineCode();
#if DEBUG
            _machineCode = "F786BABFD1B73EA13428ACA18E4A358C";
#endif
            TestLicenceCode();
        }

        public void TestLicenceCode()
        {
            Dictionary<string, string> config = new ConfigBusiness().GetConfigFromDB(" AND `ConfigName`='SysCode' OR `ConfigName`='LicenceNumber'");
            if (config.ContainsKey("SysCode") && config.ContainsKey("LicenceNumber"))
            {
                if (TestLicenceCode(config["LicenceNumber"], config["SysCode"]))
                {
                    _licenceNumber = int.Parse(config["LicenceNumber"]);
                    _licenceState = true;
                }
                else
                {
                    _licenceNumber = 0;
                    _licenceState = false;
                }
            }
            else
            {
                _licenceNumber = 0;
                _licenceState = false;
            }
        }

        private bool TestLicenceCode(string licenceNumber, string licenceCode)
        {
            string aseStr = AESHelper.Encrypt($"<MachineCode>{_machineCode}</MachineCode><Number>{licenceNumber}</Number>", HelperKeys.Key, HelperKeys.IV);
            string temp = MD5Helper.ComputeMD5Hash(aseStr);
            return MD5Helper.ComputeMD5Hash(aseStr) == licenceCode;
        }

        private string CreateMachineCode()
        {
            return MD5Helper.ComputeMD5Hash(AESHelper.Encrypt($"<Mac>{System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault().GetPhysicalAddress().ToString()}</Mac>", HelperKeys.Key, HelperKeys.IV));
        }

        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineCode { get { return _machineCode; } }

        public bool LicenceState { get { return _licenceState; } }

        public int LicenceNumber { get { return _licenceNumber; } }
    }
}
