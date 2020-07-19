using Atdi.Modules.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
{
    class Program
    {
	    private static Dictionary<string, string> LicenseNumbersDb;
	    private static Dictionary<string, string> InstanceNumbersDb;
	    private static string LicenseNumbersDbPath = @"C:\Projects\Repos\atdi.ua\Dev\Modules\Atdi.Modules.LicenseGenerator\DB\LicenseNumbers.db";
	    private static string InstanceNumbersDbPath = @"C:\Projects\Repos\atdi.ua\Dev\Modules\Atdi.Modules.LicenseGenerator\DB\InstancesNumbers.db";
		static void InitDBs()
		{
			if (File.Exists(LicenseNumbersDbPath))
			{
				var buffer = File.ReadAllBytes(LicenseNumbersDbPath);
				LicenseNumbersDb = buffer.Deserialize<Dictionary<string, string>>();
			}
			else
			{
				LicenseNumbersDb = new Dictionary<string, string>();
				//уником девайсы
				LicenseNumbersDb.Add("LIC-DBD13-G65-055", "LIC-DBD13-G65-055");
				LicenseNumbersDb.Add("LIC-DBD13-G65-269", "LIC-DBD13-G65-269");
				LicenseNumbersDb.Add("LIC-DBD13-G65-332", "LIC-DBD13-G65-332");
				LicenseNumbersDb.Add("LIC-DBD13-G65-482", "LIC-DBD13-G65-482");
				LicenseNumbersDb.Add("LIC-DBD13-G65-494", "LIC-DBD13-G65-494");
				LicenseNumbersDb.Add("LIC-DBD13-G65-691", "LIC-DBD13-G65-691");
				LicenseNumbersDb.Add("LIC-DBD13-G65-702", "LIC-DBD13-G65-702");
				LicenseNumbersDb.Add("LIC-DBD13-G65-773", "LIC-DBD13-G65-773");
				LicenseNumbersDb.Add("LIC-DBD13-G65-866", "LIC-DBD13-G65-866");
				LicenseNumbersDb.Add("LIC-DBD13-G65-925", "LIC-DBD13-G65-925");
				//уником сервера
				LicenseNumbersDb.Add("LIC-SBD13-G65-686", "LIC-SBD13-G65-686");
				LicenseNumbersDb.Add("LIC-SBD13-G65-840", "LIC-SBD13-G65-840");
				//уником мониторинг
				LicenseNumbersDb.Add("LIC-CBD13-G65-050", "LIC-CBD13-G65-050");
				LicenseNumbersDb.Add("LIC-CBD13-G65-206", "LIC-CBD13-G65-206");
				LicenseNumbersDb.Add("LIC-CBD13-G65-837", "LIC-CBD13-G65-837");
				LicenseNumbersDb.Add("LIC-CBD13-G65-962", "LIC-CBD13-G65-962");

				// УДЦР устрйоства
				LicenseNumbersDb.Add("LIC-DBD13-G65-067", "LIC-DBD13-G65-067");
				LicenseNumbersDb.Add("LIC-DBD13-G65-130", "LIC-DBD13-G65-130");
				LicenseNumbersDb.Add("LIC-DBD13-G65-131", "LIC-DBD13-G65-131");
				LicenseNumbersDb.Add("LIC-DBD13-G65-159", "LIC-DBD13-G65-159");
				LicenseNumbersDb.Add("LIC-DBD13-G65-252", "LIC-DBD13-G65-252");
				LicenseNumbersDb.Add("LIC-DBD13-G65-266", "LIC-DBD13-G65-266");
				LicenseNumbersDb.Add("LIC-DBD13-G65-321", "LIC-DBD13-G65-321");
				LicenseNumbersDb.Add("LIC-DBD13-G65-356", "LIC-DBD13-G65-356");
				LicenseNumbersDb.Add("LIC-DBD13-G65-515", "LIC-DBD13-G65-515");
				LicenseNumbersDb.Add("LIC-DBD13-G65-557", "LIC-DBD13-G65-557");
				LicenseNumbersDb.Add("LIC-DBD13-G65-599", "LIC-DBD13-G65-599");
				LicenseNumbersDb.Add("LIC-DBD13-G65-620", "LIC-DBD13-G65-620");
				LicenseNumbersDb.Add("LIC-DBD13-G65-629", "LIC-DBD13-G65-629");
				LicenseNumbersDb.Add("LIC-DBD13-G65-680", "LIC-DBD13-G65-680");
				LicenseNumbersDb.Add("LIC-DBD13-G65-786", "LIC-DBD13-G65-786");
				LicenseNumbersDb.Add("LIC-DBD13-G65-804", "LIC-DBD13-G65-804");
				LicenseNumbersDb.Add("LIC-DBD13-G65-847", "LIC-DBD13-G65-847");
				LicenseNumbersDb.Add("LIC-DBD13-G65-889", "LIC-DBD13-G65-889");
				LicenseNumbersDb.Add("LIC-DBD13-G65-898", "LIC-DBD13-G65-898");
				LicenseNumbersDb.Add("LIC-DBD13-G65-973", "LIC-DBD13-G65-973");
				// УДЦР СДРН сервер
				LicenseNumbersDb.Add("LIC-SBD13-G65-607", "LIC-SBD13-G65-607");
				// вебквери
				LicenseNumbersDb.Add("LIC-WQWPBD13-G65-550", "LIC-WQWPBD13-G65-550");
				LicenseNumbersDb.Add("LIC-WQASBD13-G65-605", "LIC-WQASBD13-G65-605");
				// Босян вебквери
				LicenseNumbersDb.Add("LIC-WQWPCA10-B00-718", "LIC-WQWPCA10-B00-718");
				LicenseNumbersDb.Add("LIC-WQASCA10-B00-857", "LIC-WQASCA10-B00-857");
				// Казахстан вебквери
				LicenseNumbersDb.Add("LIC-WQWPBD71-F23-889", "LIC-WQWPBD71-F23-889");
				LicenseNumbersDb.Add("LIC-WQASBD71-F23-733", "LIC-WQASBD71-F23-733");

				var buffer1 = LicenseNumbersDb.Serialize();
				File.WriteAllBytes(LicenseNumbersDbPath, buffer1);
			}

			if (File.Exists(InstanceNumbersDbPath))
			{
				var buffer = File.ReadAllBytes(InstanceNumbersDbPath);
				InstanceNumbersDb = buffer.Deserialize<Dictionary<string, string>>();
			}
			else
			{
				InstanceNumbersDb = new Dictionary<string, string>();

				//уником девайсы
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8206", "SENSOR-DBD13-G65-8206");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-5691", "SENSOR-DBD13-G65-5691");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-9001", "SENSOR-DBD13-G65-9001");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-6334", "SENSOR-DBD13-G65-6334");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-2828", "SENSOR-DBD13-G65-2828");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-1110", "SENSOR-DBD13-G65-1110");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8653", "SENSOR-DBD13-G65-8653");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8416", "SENSOR-DBD13-G65-8416");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-0244", "SENSOR-DBD13-G65-0244");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8900", "SENSOR-DBD13-G65-8900");

				InstanceNumbersDb.Add("SDRNSV-SBD13-G65-9594", "SDRNSV-SBD13-G65-9594");
				InstanceNumbersDb.Add("SDRNSV-SBD13-G65-6077", "SDRNSV-SBD13-G65-6077");
				//уником мониторинг
				InstanceNumbersDb.Add("CLIENT-CBD13-G65-1455", "CLIENT-CBD13-G65-1455");
				InstanceNumbersDb.Add("CLIENT-CBD13-G65-8379", "CLIENT-CBD13-G65-8379");
				InstanceNumbersDb.Add("CLIENT-CBD13-G65-0495", "CLIENT-CBD13-G65-0495");
				InstanceNumbersDb.Add("CLIENT-CBD13-G65-9778", "CLIENT-CBD13-G65-9778");

				// УДЦР устрйоства
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-3668", "SENSOR-DBD13-G65-3668");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-5854", "SENSOR-DBD13-G65-5854");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-7613", "SENSOR-DBD13-G65-7613");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-4850", "SENSOR-DBD13-G65-4850");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-1000", "SENSOR-DBD13-G65-1000");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-3206", "SENSOR-DBD13-G65-3206");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8938", "SENSOR-DBD13-G65-8938");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-9832", "SENSOR-DBD13-G65-9832");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-4036", "SENSOR-DBD13-G65-4036");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-2516", "SENSOR-DBD13-G65-2516");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-6781", "SENSOR-DBD13-G65-6781");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-2314", "SENSOR-DBD13-G65-2314");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-5768", "SENSOR-DBD13-G65-5768");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-3716", "SENSOR-DBD13-G65-3716");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-2440", "SENSOR-DBD13-G65-2440");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-0561", "SENSOR-DBD13-G65-0561");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-6554", "SENSOR-DBD13-G65-6554");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-8386", "SENSOR-DBD13-G65-8386");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-6214", "SENSOR-DBD13-G65-6214");
				InstanceNumbersDb.Add("SENSOR-DBD13-G65-7870", "SENSOR-DBD13-G65-7870");
				// УДЦР СДРН сервер
				InstanceNumbersDb.Add("SDRNSV-SBD13-G65-3690", "SDRNSV-SBD13-G65-3690");
				// вебквери
				InstanceNumbersDb.Add("WBP-WQBD13-G65-9714", "WBP-WQBD13-G65-9714");
				InstanceNumbersDb.Add("APPSRV-WQBD13-G65-8673", "APPSRV-WQBD13-G65-8673");
				// Босян вебквери
				InstanceNumbersDb.Add("WBP-WQCA10-B00-1437", "WBP-WQCA10-B00-1437");
				InstanceNumbersDb.Add("APPSRV-WQCA10-B00-1782", "APPSRV-WQCA10-B00-1782");
				// Казахстан вебквери
				InstanceNumbersDb.Add("WBP-WQBD71-F23-9623", "WBP-WQBD71-F23-9623");
				InstanceNumbersDb.Add("APPSRV-WQBD71-F23-7515", "APPSRV-WQBD71-F23-7515");

				var buffer2 = InstanceNumbersDb.Serialize();
				File.WriteAllBytes(InstanceNumbersDbPath, buffer2);
			}
		}

		static void SaveDBs()
		{
			var buffer1 = LicenseNumbersDb.Serialize();
			File.WriteAllBytes(LicenseNumbersDbPath, buffer1);
			var buffer2 = InstanceNumbersDb.Serialize();
			File.WriteAllBytes(InstanceNumbersDbPath, buffer2);
		}

		static void Main(string[] args)
        {
			InitDBs();
			//var path = "C:\\Projects\\Licensing\\UDCR\\WebQuery\\AppServer";
			//WebQueryAppServer_ForUDCR(path);
			//path = "C:\\Projects\\Licensing\\UDCR\\WebQuery\\WebPortal";
			//WebQueryWebPortal_ForUDCR(path);

			//UpdatePeriod_ICSControl_ForUDCR(@"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2018");

			//UpdatePeriod_ICSControl_ForTest(@"C:\Projects\Licensing\Test\Sdrn\Licenses_2018");

			//ICSControl_ForTesting_ClusterServers(@"C:\Projects\Licensing\Test\Sdrn\Licenses_2019");

			//ICSControl_ForUDCR(@"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019\УНИКОМ", 2, 10, 4);

			//ICSControl_ForTesting_ClientMonitoring(@"C:\Projects\Licensing\Test\Sdrn\Licenses_2019", 1);

			//ICSControl_ForTesting_ClusterServers(@"C:\Projects\Licensing\Test\Sdrn\Licenses_2020");

			//UpdatePeriod_ICSControl_ForUDCR_2020(@"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019");

			//UpdatePeriod_WebQueryAppServer_ForBosny_version_1_2020();
			//UpdatePeriod_WebQueryAppServer_ForBosny_version_2_2020();

			//UpdatePeriod_ICSControl_ForTest_2020();

			//ICSControl_ForTesting_CalcServer(@"C:\Projects\Repos\atdi.ua\Dev\Delivery\Licenses\Test\Sdrn\CalcServerLicense", 1);

			//WebQuery_ForKazahstan(@"C:\Projects\Repos\atdi.ua\Dev\Delivery\Licenses\Kzh\WebQuery");

			//ICSControl_ForTesting_Infocentr(@"C:\Projects\Repos\atdi.ua\Dev\Delivery\Licenses\Test\Sdrn\InfocentrLicense", 1);

			//выпуск 90 сенсоров
			//ICSControl_ForUDCR_UNICOM_2020();

			// для тестирования плагинов
			// ICSMPlugin_ForTesting_Calc_2020();

			// УДЦР, 8+8 для плагинов , 1 сервер расчетов, 1 инфоцентер
			//ICSMPlugin_ForUDCR_CalcTasks_2020();

			// Поставка Июль УДЦР 50 лицензий
			//SdrnDevice_ForUDCR_2020_50p();

			//20200716
			// GE06 - start 
			//SdrnGN06CalcPlugin_ForTest_2020();
			//SdrnGN06CalcPlugin_ForATDI_SA_2020_1p();

			//20200716
			//CalcServer_ForATDI_SA_2020_1p();

			//20200717
			WebQuery_for_CRA_Lithuania_2020_1p();

			Console.WriteLine("Process was finished");

			Console.ReadKey();
        }

        static void UpdatePeriod_ICSControl_ForUDCR_2019(string path)
        {
            var startDate = new DateTime(2018, 12, 25);
            var stopDate = new DateTime(2020, 1, 1);
            var outPath = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019";
            var ownerId = "OID-BD13-G65-N00";
            var ownerKey = "BD13-G65";

            // server
            UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "ServerLicense\\LIC-SBD13-G65-607.SDRNSV-SBD13-G65-3690.lic"),
                productKey: "MGDM-RD0E-ER0I-6GJR-0DCS",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);

            var data = new string[][]
            {
                new string[] { "LIC-DBD13-G65-067.SENSOR-DBD13-G65-3668", "MGIN-1J63-6SAD-EE6P-CDJI" },
                new string[] { "LIC-DBD13-G65-130.SENSOR-DBD13-G65-5854", "FY3I-0CBG-3G1V-DNA1-DBI3" },
                new string[] { "LIC-DBD13-G65-131.SENSOR-DBD13-G65-7613", "J81L-DO1R-EC73-3ELP-8DI0" },
                new string[] { "LIC-DBD13-G65-159.SENSOR-DBD13-G65-4850", "DEOO-TTCL-6S1V-9R3V-S4RN" },
                new string[] { "LIC-DBD13-G65-252.SENSOR-DBD13-G65-1000", "IKBO-SSCS-D2YB-BMLL-ECTF" },
                new string[] { "LIC-DBD13-G65-266.SENSOR-DBD13-G65-3206", "E5MD-35PO-YETY-CIEN-EZ0T" },
                new string[] { "LIC-DBD13-G65-321.SENSOR-DBD13-G65-8938", "H0LZ-DR5E-NZVT-JCVV-DSSS" },
                new string[] { "LIC-DBD13-G65-356.SENSOR-DBD13-G65-9832", "8EIN-KTNZ-UYS8-3NR6-NT09" },
                new string[] { "LIC-DBD13-G65-515.SENSOR-DBD13-G65-4036", "SEE8-ONGD-4VC0-8NS2-HYN0" },
                new string[] { "LIC-DBD13-G65-557.SENSOR-DBD13-G65-2516", "MNOS-7CDE-Q1XM-9Q1H-N3IE" },
                new string[] { "LIC-DBD13-G65-599.SENSOR-DBD13-G65-6781", "DIVV-CWIC-GGOE-D5DD-OT9F" },
                new string[] { "LIC-DBD13-G65-620.SENSOR-DBD13-G65-2314", "BQ35-02C7-ZBC6-DLCE-XG2C" },
                new string[] { "LIC-DBD13-G65-629.SENSOR-DBD13-G65-5768", "CEXS-DL10-D3GD-ETB1-CIEC" },
                new string[] { "LIC-DBD13-G65-680.SENSOR-DBD13-G65-3716", "EC6E-0DGD-5RDN-C6D5-KCCC" },
                new string[] { "LIC-DBD13-G65-786.SENSOR-DBD13-G65-2440", "S715-OC0I-4DLR-E2DG-PIOG" },
                new string[] { "LIC-DBD13-G65-804.SENSOR-DBD13-G65-0561", "EN0O-ISNC-9S1E-CCRZ-QDRD" },
                new string[] { "LIC-DBD13-G65-847.SENSOR-DBD13-G65-6554", "NRD2-C1IS-DEGA-D4RL-ELCO" },
                new string[] { "LIC-DBD13-G65-889.SENSOR-DBD13-G65-8386", "3B53-ELE1-QO02-BI4V-CT8E" },
                new string[] { "LIC-DBD13-G65-898.SENSOR-DBD13-G65-6214", "NDDV-3N2O-L9NC-GTEG-5SCV" },
                new string[] { "LIC-DBD13-G65-973.SENSOR-DBD13-G65-7870", "2JO3-N6I5-SLCG-RI5C-3VH5" },
            };

            foreach (var item in data)
            {
                UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "DeviceLicense\\" + item[0] +  ".lic"),
                productKey: item[1],
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);
            }
            // dev 1
            
        }

        static void UpdatePeriod_ICSControl_ForUDCR_2020(string path)
        {
            var startDate = new DateTime(2018, 12, 25);
            var stopDate = new DateTime(2021, 1, 1);
            
            var ownerId = "OID-BD13-G65-N00";
            var ownerKey = "BD13-G65";
            var year = (ushort) 2020;

            // WebQuery Web Portal
            var outPath = @"C:\Projects\Licensing\UDCR\WebQuery\AppServer\Licenses_2020";

            UpdateLicesePeriod2(
                sourcefileName: "C:\\Projects\\Licensing\\UDCR\\WebQuery\\AppServer\\BD13-G65\\ServerLicense\\LIC-WQASBD13-G65-605.APPSRV-WQBD13-G65-8673.lic",
                productKey: "IV0O-6R1S-NU0P-6GWS-EWRX",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year: year);

            // WebQuery Web Portal
            outPath = @"C:\Projects\Licensing\UDCR\WebQuery\WebPortal\Licenses_2020";

            UpdateLicesePeriod2(
                sourcefileName: "C:\\Projects\\Licensing\\UDCR\\WebQuery\\WebPortal\\BD13-G65\\ServerLicense\\LIC-WQWPBD13-G65-550.WBP-WQBD13-G65-9714.lic",
                productKey: "NP0W-Q6B4-U0W3-9ZW1-IE7R",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year: year);

            // SDRN
            outPath = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020";
            // server
            UpdateLicesePeriod2(
                sourcefileName: @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019\BD13-G65\ServerLicense\LIC-SBD13-G65-607.SDRNSV-SBD13-G65-3690.lic",
                productKey: "MGDM-RD0E-ER0I-6GJR-0DCS",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year: year);

            var data = new string[][]
            {
                new string[] { "LIC-DBD13-G65-067.SENSOR-DBD13-G65-3668", "MGIN-1J63-6SAD-EE6P-CDJI" },
                new string[] { "LIC-DBD13-G65-130.SENSOR-DBD13-G65-5854", "FY3I-0CBG-3G1V-DNA1-DBI3" },
                new string[] { "LIC-DBD13-G65-131.SENSOR-DBD13-G65-7613", "J81L-DO1R-EC73-3ELP-8DI0" },
                new string[] { "LIC-DBD13-G65-159.SENSOR-DBD13-G65-4850", "DEOO-TTCL-6S1V-9R3V-S4RN" },
                new string[] { "LIC-DBD13-G65-252.SENSOR-DBD13-G65-1000", "IKBO-SSCS-D2YB-BMLL-ECTF" },
                new string[] { "LIC-DBD13-G65-266.SENSOR-DBD13-G65-3206", "E5MD-35PO-YETY-CIEN-EZ0T" },
                new string[] { "LIC-DBD13-G65-321.SENSOR-DBD13-G65-8938", "H0LZ-DR5E-NZVT-JCVV-DSSS" },
                new string[] { "LIC-DBD13-G65-356.SENSOR-DBD13-G65-9832", "8EIN-KTNZ-UYS8-3NR6-NT09" },
                new string[] { "LIC-DBD13-G65-515.SENSOR-DBD13-G65-4036", "SEE8-ONGD-4VC0-8NS2-HYN0" },
                new string[] { "LIC-DBD13-G65-557.SENSOR-DBD13-G65-2516", "MNOS-7CDE-Q1XM-9Q1H-N3IE" },
                new string[] { "LIC-DBD13-G65-599.SENSOR-DBD13-G65-6781", "DIVV-CWIC-GGOE-D5DD-OT9F" },
                new string[] { "LIC-DBD13-G65-620.SENSOR-DBD13-G65-2314", "BQ35-02C7-ZBC6-DLCE-XG2C" },
                new string[] { "LIC-DBD13-G65-629.SENSOR-DBD13-G65-5768", "CEXS-DL10-D3GD-ETB1-CIEC" },
                new string[] { "LIC-DBD13-G65-680.SENSOR-DBD13-G65-3716", "EC6E-0DGD-5RDN-C6D5-KCCC" },
                new string[] { "LIC-DBD13-G65-786.SENSOR-DBD13-G65-2440", "S715-OC0I-4DLR-E2DG-PIOG" },
                new string[] { "LIC-DBD13-G65-804.SENSOR-DBD13-G65-0561", "EN0O-ISNC-9S1E-CCRZ-QDRD" },
                new string[] { "LIC-DBD13-G65-847.SENSOR-DBD13-G65-6554", "NRD2-C1IS-DEGA-D4RL-ELCO" },
                new string[] { "LIC-DBD13-G65-889.SENSOR-DBD13-G65-8386", "3B53-ELE1-QO02-BI4V-CT8E" },
                new string[] { "LIC-DBD13-G65-898.SENSOR-DBD13-G65-6214", "NDDV-3N2O-L9NC-GTEG-5SCV" },
                new string[] { "LIC-DBD13-G65-973.SENSOR-DBD13-G65-7870", "2JO3-N6I5-SLCG-RI5C-3VH5" },
            };

            foreach (var item in data)
            {
                UpdateLicesePeriod2(
                sourcefileName: @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019\BD13-G65\DeviceLicense\" + item[0] + ".lic",
                productKey: item[1],
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year:year);
            }
            // dev 1

        }

		static void UpdatePeriod_ICSControl_ForTest_2020()
		{
			var startDate = new DateTime(2018, 12, 25);
			var stopDate = new DateTime(2022, 1, 1);

			var ownerId = "OID-BD12-A00-N00";
			var ownerKey = "BD12-A00";
			var year = (ushort)2021;

			// WebQuery Web Portal
			//var outPath = @"C:\Projects\Licensing\UDCR\WebQuery\AppServer\Licenses_2020";

			//UpdateLicesePeriod2(
			//	sourcefileName: "C:\\Projects\\Licensing\\UDCR\\WebQuery\\AppServer\\BD13-G65\\ServerLicense\\LIC-WQASBD13-G65-605.APPSRV-WQBD13-G65-8673.lic",
			//	productKey: "IV0O-6R1S-NU0P-6GWS-EWRX",
			//	outPath: outPath,
			//	ownerId: ownerId,
			//	ownerKey: ownerKey,
			//	startDate: startDate,
			//	stopDate: stopDate,
			//	year: year);

			// WebQuery Web Portal
			//outPath = @"C:\Projects\Licensing\UDCR\WebQuery\WebPortal\Licenses_2020";

			//UpdateLicesePeriod2(
			//	sourcefileName: "C:\\Projects\\Licensing\\UDCR\\WebQuery\\WebPortal\\BD13-G65\\ServerLicense\\LIC-WQWPBD13-G65-550.WBP-WQBD13-G65-9714.lic",
			//	productKey: "NP0W-Q6B4-U0W3-9ZW1-IE7R",
			//	outPath: outPath,
			//	ownerId: ownerId,
			//	ownerKey: ownerKey,
			//	startDate: startDate,
			//	stopDate: stopDate,
			//	year: year);

			//// SDRN
			var outPath = @"C:\Projects\Licensing\Test\Sdrn\Licenses_2020";
			//// server
			//UpdateLicesePeriod2(
			//	sourcefileName: @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019\BD13-G65\ServerLicense\LIC-SBD13-G65-607.SDRNSV-SBD13-G65-3690.lic",
			//	productKey: "MGDM-RD0E-ER0I-6GJR-0DCS",
			//	outPath: outPath,
			//	ownerId: ownerId,
			//	ownerKey: ownerKey,
			//	startDate: startDate,
			//	stopDate: stopDate,
			//	year: year);

			var data = new string[][]
			{
				new string[] { "LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280", "0ZB0-DVZR-ATI1-WIHB-NC1B" },
				new string[] { "LIC-DBD12-A00-192.SENSOR-DBD12-A00-0618", "BCBC-ECA1-L9EC-IR7E-V1AD" },
				new string[] { "LIC-DBD12-A00-333.SENSOR-DBD12-A00-5140", "CCMR-RBZN-2C1D-OE40-EC23" },
				new string[] { "LIC-DBD12-A00-442.SENSOR-DBD12-A00-5169", "ADL6-EDCV-NC1I-CE1N-DLH0" },
				new string[] { "LIC-DBD12-A00-511.SENSOR-DBD12-A00-8429", "020B-CJHO-TDNI-ZEEO-2KDI" },
				new string[] { "LIC-DBD12-A00-542.SENSOR-DBD12-A00-9792", "N8XE-HLRQ-DERB-NLIV-0DX2" },
				new string[] { "LIC-DBD12-A00-668.SENSOR-DBD12-A00-8485", "ROL9-XSMC-WDCT-62ST-R5ER" },

				new string[] { "LIC-DBD12-A00-722.SENSOR-DBD12-A00-1692", "BE1D-RLNN-S0S6-EN42-0028" },
				new string[] { "LIC-DBD12-A00-787.SENSOR-DBD12-A00-3828", "NTC0-IODJ-7SC0-01EB-DVRV" },
				new string[] { "LIC-DBD12-A00-878.SENSOR-DBD12-A00-8918", "0VE1-OCOL-S4S0-C1D1-SEXB" },
			};

			foreach (var item in data)
			{
				UpdateLicesePeriod2(
				sourcefileName: @"C:\Projects\Licensing\Test\Sdrn\Licenses_2019\BD12-A00\DeviceLicense\" + item[0] + ".lic",
				productKey: item[1],
				outPath: outPath,
				ownerId: ownerId,
				ownerKey: ownerKey,
				startDate: startDate,
				stopDate: stopDate,
				year: year);
			}
			// dev 1

		}
		static void UpdatePeriod_ICSControl_ForTest(string path)
        {
            var startDate = new DateTime(2018, 12, 25);
            var stopDate = new DateTime(2020, 1, 1);
            var outPath = @"C:\Projects\Licensing\Test\Sdrn\Licenses_2019";
            var ownerId = "OID-BD12-A00-N00";
            var ownerKey = "BD12-A00";

            // server
            UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "ServerLicense\\LIC-SBD12-A00-613.SDRNSV-SBD12-A00-8591.lic"),
                productKey: "XC0R-EEVL-0AT5-LETT-VEUO",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);

            var data = new string[][]
            {
                new string[] { "LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280", "0ZB0-DVZR-ATI1-WIHB-NC1B" },
                new string[] { "LIC-DBD12-A00-192.SENSOR-DBD12-A00-0618", "BCBC-ECA1-L9EC-IR7E-V1AD" },
                new string[] { "LIC-DBD12-A00-333.SENSOR-DBD12-A00-5140", "CCMR-RBZN-2C1D-OE40-EC23" },
                new string[] { "LIC-DBD12-A00-442.SENSOR-DBD12-A00-5169", "ADL6-EDCV-NC1I-CE1N-DLH0" },
                new string[] { "LIC-DBD12-A00-511.SENSOR-DBD12-A00-8429", "020B-CJHO-TDNI-ZEEO-2KDI" },
                new string[] { "LIC-DBD12-A00-542.SENSOR-DBD12-A00-9792", "N8XE-HLRQ-DERB-NLIV-0DX2" },
                new string[] { "LIC-DBD12-A00-668.SENSOR-DBD12-A00-8485", "ROL9-XSMC-WDCT-62ST-R5ER" },
                new string[] { "LIC-DBD12-A00-722.SENSOR-DBD12-A00-1692", "BE1D-RLNN-S0S6-EN42-0028" },
                new string[] { "LIC-DBD12-A00-787.SENSOR-DBD12-A00-3828", "NTC0-IODJ-7SC0-01EB-DVRV" },
                new string[] { "LIC-DBD12-A00-878.SENSOR-DBD12-A00-8918", "0VE1-OCOL-S4S0-C1D1-SEXB" },
            };

            foreach (var item in data)
            {
                UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "DeviceLicense\\" + item[0] + ".lic"),
                productKey: item[1],
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);
            }
            // dev 1

        }

        static void UpdateLicesePeriod(string sourcefileName, string productKey, string ownerKey, string ownerId, string outPath, DateTime startDate, DateTime stopDate)
        {
            var licBody = File.ReadAllBytes(sourcefileName);
            var lic = LicenseVerifier.GetLicenseInfo(ownerId, productKey, licBody);

            //var licenseIndex = GetUniqueIntegerKey(3);

            lic.Created = DateTime.Now;
            lic.StartDate = startDate;
            lic.StopDate = stopDate;
            //lic.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";

            var creator = new LicenseCreator();
            var result = creator.Create(new LicenseData[] { lic });

            //var directory = $"{outPath}\\{ownerKey}\\{lic.LicenseType}";
            Directory.CreateDirectory(outPath);
            var newfileName = $"{outPath}\\{lic.LicenseNumber}.{lic.Instance}.lic";

            File.WriteAllBytes(newfileName, result.Body);

            CreateLicenseDescriptionFile(lic, newfileName);

            var testLicBody = File.ReadAllBytes(newfileName);

            var vd = new VerificationData
            {
                OwnerId = lic.OwnerId,
                ProductName = lic.ProductName,
                ProductKey = lic.ProductKey,
                LicenseType = lic.LicenseType,
                Date = startDate
            };

            var cc = LicenseVerifier.Verify(vd, testLicBody);

            Console.WriteLine($"Update license: '{productKey}' >>> {newfileName}");

        }

        static void UpdateLicesePeriod2(string sourcefileName, string productKey, string ownerKey, string ownerId, string outPath, DateTime startDate, DateTime stopDate, ushort year)
        {
            var licBody = File.ReadAllBytes(sourcefileName);
            var lic = LicenseVerifier.GetLicenseInfo(ownerId, productKey, licBody);
            var lic2 = new LicenseData2
            {
                Year = year,
                LimitationTerms = LicenseLimitationTerms.Year,
                Created = DateTime.Now,
                Company = lic.Company,
                Copyright = lic.Copyright,
                Count = lic.Count,
                Instance = lic.Instance,
                LicenseNumber = lic.LicenseNumber,
                LicenseType = lic.LicenseType,
                OwnerId = lic.OwnerId,
                OwnerName = lic.OwnerName,
                ProductKey = lic.ProductKey,
                ProductName = lic.ProductName,
                StartDate = lic.StartDate,
                StopDate = stopDate,
                
            };
            //var licenseIndex = GetUnieIntegerKey(3);

            //lic.Created = DateTime.Now;
            //lic.StartDate = startDate;
            //lic.StopDate = stopDate;
            //lic.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";

            var creator = new LicenseCreator();
            var result = creator.Create(new LicenseData2[] { lic2 });

            //var directory = $"{outPath}\\{ownerKey}\\{lic2.LicenseType}";
            Directory.CreateDirectory(outPath);
            var newfileName = $"{outPath}\\{lic2.LicenseNumber}.{lic2.Instance}.lic";

            File.WriteAllBytes(newfileName, result.Body);

            CreateLicenseDescriptionFile(lic2, newfileName);

            var testLicBody = File.ReadAllBytes(newfileName);

            var vd = new VerificationData2
            {
                OwnerId = lic2.OwnerId,
                ProductName = lic2.ProductName,
                ProductKey = lic2.ProductKey,
                LicenseType = lic2.LicenseType,
                Date = startDate,
                YearHash = LicenseVerifier.EncodeYear(year)
            };

            var cc = LicenseVerifier.Verify(vd, testLicBody);

            Console.WriteLine($"Update license: '{productKey}' >>> {newfileName}");

        }

        static void WebQueryAppServer_ForUDCR(string path)
        {
            var ownerKey = "BD13-G65";
            var ownerId = "OID-BD13-G65-N00";
            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";

            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForUDCR(string path)
        {
            var ownerKey = "BD13-G65";
            var ownerId = "OID-BD13-G65-N00";
            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";

            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryAppServer_ForBosny(string path)
        {
            var ownerKey = "CA10-B00";
            var ownerId = "OID-CA10-B00-N00";
            var ownerName = "Regulatorna agencija za komunikacije";
            var company = "ATDI Ukraine";
            
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForBosny(string path)
        {
            var ownerKey = "CA10-B00";
            var ownerId = "OID-CA10-B00-N00";
            var ownerName = "Regulatorna agencija za komunikacije";
            var company = "ATDI Ukraine";
            
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }


        static void UpdatePeriod_WebQueryAppServer_ForBosny_version_1_2020()
        {
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2021, 1, 1);
            
            var ownerId = "OID-CA10-B00-N00";
            var ownerKey = "CA10-B00";

            // server
            var outPath = @"C:\Projects\Licensing\Bosny\WebQuery\Licenses_2020\Version_Current\AppServer";
            UpdateLicesePeriod(
                sourcefileName: @"C:\Projects\Licensing\Bosny\WebQuery\AppServer\CA10-B00\ServerLicense\LIC-WQASCA10-B00-857.APPSRV-WQCA10-B00-1782.lic",
                productKey: "EDRA-9WNB-E8DC-VIEC-QYND",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);

            // WebPortal
            outPath = @"C:\Projects\Licensing\Bosny\WebQuery\Licenses_2020\Version_Current\WebPortal";
            UpdateLicesePeriod(
                sourcefileName: @"C:\Projects\Licensing\Bosny\WebQuery\WebPortal\CA10-B00\ServerLicense\LIC-WQWPCA10-B00-718.WBP-WQCA10-B00-1437.lic",
                productKey: "PAWE-ANNE-YC0I-R306-EICC",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);

        }

        static void UpdatePeriod_WebQueryAppServer_ForBosny_version_2_2020()
        {
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2021, 1, 1);

            var ownerId = "OID-CA10-B00-N00";
            var ownerKey = "CA10-B00";

            var year = (ushort)2020;

            // server
            var outPath = @"C:\Projects\Licensing\Bosny\WebQuery\Licenses_2020\Version_3.2.0\AppServer";
            UpdateLicesePeriod2(
                sourcefileName: @"C:\Projects\Licensing\Bosny\WebQuery\AppServer\CA10-B00\ServerLicense\LIC-WQASCA10-B00-857.APPSRV-WQCA10-B00-1782.lic",
                productKey: "EDRA-9WNB-E8DC-VIEC-QYND",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year: year);

            // WebPortal
            outPath = @"C:\Projects\Licensing\Bosny\WebQuery\Licenses_2020\Version_3.2.0\WebPortal";
            UpdateLicesePeriod2(
                sourcefileName: @"C:\Projects\Licensing\Bosny\WebQuery\WebPortal\CA10-B00\ServerLicense\LIC-WQWPCA10-B00-718.WBP-WQCA10-B00-1437.lic",
                productKey: "PAWE-ANNE-YC0I-R306-EICC",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate,
                year: year);

            

        }






        static void WebQueryAppServer_ForTesting(string path)
        {
            var ownerKey = "BD12-A00";
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            
            var startDate = new DateTime(2018, 12, 5);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForTesting(string path)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2018, 12, 5);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void ICSControl_ForTesting(string path)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2018, 9, 12);
            var stopDate = new DateTime(2019, 1, 1);

            //MakeServerLicense();
            for (int i = 0; i < 10; i++)
            {
                var licenseIndex = GetUniqueIntegerKey(3);
                var deviceIndex = GetUniqueIntegerKey(4);
                var licPrefix = "LIC-D";
                var instancePrefix = "SENSOR-D";
                MakeLicense(path, licPrefix, instancePrefix, "DeviceLicense", "ICS Control Device", licenseIndex, deviceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);
            var srvLicPrefix = "LIC-S";
            var srvInstancePrefix = "SDRNSV-S";

            MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "ICS Control Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void ICSControl_ForTesting_ClusterServers(string path,  int count = 2)
        {
            const string ownerId = "OID-BD12-A00-N00";
            const string ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            const string company = "ТОВ 'Лабораторія інформаційних систем'";
            const string ownerKey = "BD12-A00";
            var startDate = new DateTime(2019, 8, 20);
            var stopDate = new DateTime(2025, 8, 20);
            const ushort year = 2020;
            //MakeServerLicense();
            for (int i = 0; i < count; i++)
            {
                var srvLicenseIndex = GetUniqueIntegerKey(3);
                var instanceIndex = GetUniqueIntegerKey(4);
                var srvLicPrefix = "LIC-S";
                var srvInstancePrefix = "SDRNSV-S";

                MakeLicense2(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "ICS Control Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate, year);
            }

            
        }

        static void ICSControl_ForTesting_CalcServer(string path, int count = 2)
        {
	        const string ownerId = "OID-BD12-A00-N00";
	        const string ownerName = "ТОВ 'Лабораторія інформаційних систем'";
	        const string company = "ТОВ 'Лабораторія інформаційних систем'";
	        const string ownerKey = "BD12-A00";
	        var startDate = new DateTime(2020, 2, 5);
	        var stopDate = new DateTime(2025, 8, 20);
	        const ushort year = 2020;
	        //MakeServerLicense();
	        for (int i = 0; i < count; i++)
	        {
		        var srvLicenseIndex = GetUniqueIntegerKey(3);
		        var instanceIndex = GetUniqueIntegerKey(4);
		        var srvLicPrefix = "LIC-C";
		        var srvInstancePrefix = "SDRNSV-C";

		        MakeLicense2(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "SDRN Calc Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate, year);
	        }


        }

        static void ICSControl_ForTesting_Infocentr(string path, int count = 2)
        {
	        const string ownerId = "OID-BD12-A00-N00";
	        const string ownerName = "ТОВ 'Лабораторія інформаційних систем'";
	        const string company = "ТОВ 'Лабораторія інформаційних систем'";
	        const string ownerKey = "BD12-A00";
	        var startDate = new DateTime(2020, 2, 5);
	        var stopDate = new DateTime(2025, 8, 20);
	        const ushort year = 2020;
	        //MakeServerLicense();
	        for (int i = 0; i < count; i++)
	        {
		        var srvLicenseIndex = GetUniqueIntegerKey(3);
		        var instanceIndex = GetUniqueIntegerKey(4);
		        var srvLicPrefix = "LIC-I";
		        var srvInstancePrefix = "SDRNSV-I";

		        MakeLicense2(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "SDRN Infocenter Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate, year);
	        }


        }

		static void WebQuery_ForKazahstan(string path)
        {
			var ownerKey = "BD71-F23";
			var ownerId = "OID-BD71-F23-N00";
			var ownerName = "Республиканское государственное предприятие на праве хозяйственного ведения(РГП) «Государственная радиочастотная служба» Министерства цифрового развития, инноваций и аэрокосмической промышленности Республики Казахстан";
			var company = "ТОВ 'Лабораторія інформаційних систем'";

			var startDate = new DateTime(2020, 02, 06);
			var stopDate = new DateTime(2020, 03, 31);
			var productName = "WebQuery Web Portal";
			var licenseType = "ServerLicense";

			var srvLicenseIndex = GetUniqueIntegerKey(3);
			var instanceIndex = GetUniqueIntegerKey(4);

			var licPrefix = "LIC-WQWP";
			var instancePrefix = "WBP-WQ";

			MakeLicense3(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);




			 productName = "WebQuery Application Server";
			 licenseType = "ServerLicense";

			 srvLicenseIndex = GetUniqueIntegerKey(3);
			 instanceIndex = GetUniqueIntegerKey(4);

			 licPrefix = "LIC-WQAS";
			 instancePrefix = "APPSRV-WQ";

			MakeLicense3(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
		}

		static void ICSControl_ForUDCR(string path, int serverCount, int deviceCount, int clientCount)
        {
            //var ownerId = "OID-BD13-G65-N00"; //  Сам УДЦР
            var ownerId = "OID-BD13-G65-N01"; //  лицензии для УДЦР выданы УНИКОМ

            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD13-G65";
            var startDate = new DateTime(2019, 9, 9);
            var stopDate = new DateTime(2020, 12, 31);

            //MakeServerLicense();
            for (int i = 0; i < deviceCount; i++)
            {
                var licenseIndex = GetUniqueIntegerKey(3);
                var deviceIndex = GetUniqueIntegerKey(4);
                var licPrefix = "LIC-D";
                var instancePrefix = "SENSOR-D";
                MakeLicense(path, licPrefix, instancePrefix, "DeviceLicense", "ICS Control Device", licenseIndex, deviceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }

            for (int i = 0; i < serverCount; i++)
            {
                var srvLicenseIndex = GetUniqueIntegerKey(3);
                var instanceIndex = GetUniqueIntegerKey(4);
                var srvLicPrefix = "LIC-S";
                var srvInstancePrefix = "SDRNSV-S";

                MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "ICS Control Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }

            for (int i = 0; i < clientCount; i++)
            {
                var srvLicenseIndex = GetUniqueIntegerKey(3);
                var instanceIndex = GetUniqueIntegerKey(4);
                var srvLicPrefix = "LIC-C";
                var srvInstancePrefix = "CLIENT-C";

                MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ClientLicense", "ICS Control Monitoring Client", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }
        }

        static void ICSControl_ForTesting_ClientMonitoring(string path, int clientCount)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2019, 9, 9);
            var stopDate = new DateTime(2021, 12, 31);

            for (int i = 0; i < clientCount; i++)
            {
                var srvLicenseIndex = GetUniqueIntegerKey(3);
                var instanceIndex = GetUniqueIntegerKey(4);
                var srvLicPrefix = "LIC-C";
                var srvInstancePrefix = "CLIENT-C";

                MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ClientLicense", "ICS Control Monitoring Client", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }
        }

        public static string GetProductKey(string productName, string licenseType, string instance, string ownerId, string number)
        {
            var source = productName + "ZXCV158BNM" + licenseType + "ASD290FGHJKL" + instance + "QWE346RTYU7IOP" + ownerId + number;
            var data = new Stack<string>();
            for (int i = 0; i < 5; i++)
            {
                data.Push(GetUniqueKey(source, 4));
            }

            return string.Join("-", data.ToArray());
        }

        public static string GetUniqueKey(int maxSize)
        {
            var chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ2451678390".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueKey(string source, int maxSize)
        {
            var chars =
           source.ToUpper().Replace(" ", "").Replace("-", "").ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueIntegerKey(int maxSize)
        {
            var chars =
            "2146389507".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        

        private static string MakeLicense(string path, string licPrefix, string instancePrefix, string licenseType, string productName, string licenseIndex, string instanceIndex, string ownerName, string ownerId, string ownerKey, string company, DateTime startDate, DateTime stopDate)
        {
            var productKey = string.Empty;

            var c = new LicenseCreator();

            var l = new LicenseData
            {
                //LicenseNumber = $"LIC-D{ownerKey}-{licenseIndex}",
                LicenseType = licenseType, //"DeviceLicense",
                Company = company,
                Copyright = "",
                OwnerId = ownerId,
                OwnerName = ownerName,
                Created = DateTime.Now,
                StartDate = startDate,
                StopDate = stopDate,
                ProductKey = productKey,
                ProductName = productName,
                Count = 1,
                //Instance = $"SENSOR-D{ownerKey}-{deviceIndex}"
            };

            if ("DeviceLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else if ("ServerLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else if ("ClientLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else
            {

                throw new InvalidOperationException($"Invalid the license type '{licenseType}'");
            }

            productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
            l.ProductKey = productKey;

            var result = c.Create(new LicenseData[] { l });

            var directory = $"{path}\\{ownerKey}\\{licenseType}";
            Directory.CreateDirectory(directory);

            var fileName = $"{directory}\\{l.LicenseNumber}.{l.Instance}.lic";

            File.WriteAllBytes(fileName, result.Body);
            CreateLicenseDescriptionFile(l, fileName);

            var licBody = File.ReadAllBytes(fileName);

            var vd = new VerificationData
            {
                OwnerId = l.OwnerId,
                ProductName = l.ProductName,
                ProductKey = l.ProductKey,
                LicenseType = l.LicenseType,
                Date = startDate
            };

            var cc = LicenseVerifier.Verify(vd, licBody);

            Console.WriteLine($"Made license: '{productKey}' >>> {fileName}");
            return productKey;
        }

        

		private static string MakeLicense2(
			string path, 
			string licPrefix, 
			string instancePrefix, 
			string licenseType, 
			string productName, 
			string licenseIndex, 
			string instanceIndex, 
			string ownerName, 
			string ownerId, 
			string ownerKey, 
			string company, 
			DateTime startDate, 
			DateTime stopDate, 
			ushort year, LicenseLimitationTerms limitationTerms = LicenseLimitationTerms.Year | LicenseLimitationTerms.TimePeriod)
        {
            var productKey = string.Empty;

            var c = new LicenseCreator();

            var l = new LicenseData2()
            {
                //LicenseNumber = $"LIC-D{ownerKey}-{licenseIndex}",
                LicenseType = licenseType, //"DeviceLicense",
                Company = company,
                Copyright = "",
                OwnerId = ownerId,
                OwnerName = ownerName,
                Created = DateTime.Now,
                StartDate = startDate,
                StopDate = stopDate,
                ProductKey = productKey,
                ProductName = productName,
                Count = 1,
                LimitationTerms = limitationTerms,
                Year =  year
                //Instance = $"SENSOR-D{ownerKey}-{deviceIndex}"
            };

            if ("DeviceLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else if ("ServerLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else if ("ClientLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else
            {

                throw new InvalidOperationException($"Invalid the license type '{licenseType}'");
            }

            productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
            l.ProductKey = productKey;

            var result = c.Create(new LicenseData2[] { l });

            var directory = $"{path}\\{ownerKey}\\{licenseType}";
            Directory.CreateDirectory(directory);

            var fileName = $"{directory}\\{l.LicenseNumber}.{l.Instance}.lic";

            File.WriteAllBytes(fileName, result.Body);
            CreateLicenseDescriptionFile(l, fileName);

            var licBody = File.ReadAllBytes(fileName);

            var vd = new VerificationData2
            {
                OwnerId = l.OwnerId,
                ProductName = l.ProductName,
                ProductKey = l.ProductKey,
                LicenseType = l.LicenseType,
                Date = startDate,
                YearHash = LicenseVerifier.EncodeYear(year)
            };

            var cc = LicenseVerifier.Verify(vd, licBody);

            Console.WriteLine($"Made license: '{productKey}' >>> {fileName}");
            return productKey;
        }

		private static string MakeLicense3(string path, string licPrefix, string instancePrefix, string licenseType, string productName, string licenseIndex, string instanceIndex, string ownerName, string ownerId, string ownerKey, string company, DateTime startDate, DateTime stopDate)
		{
			var productKey = string.Empty;

			var c = new LicenseCreator();

			var l = new LicenseData2()
			{
				//LicenseNumber = $"LIC-D{ownerKey}-{licenseIndex}",
				LicenseType = licenseType, //"DeviceLicense",
				Company = company,
				Copyright = "",
				OwnerId = ownerId,
				OwnerName = ownerName,
				Created = DateTime.Now,
				StartDate = startDate,
				StopDate = stopDate,
				ProductKey = productKey,
				ProductName = productName,
				Count = 1,
				LimitationTerms = LicenseLimitationTerms.TimePeriod,
				Year = 2020
				//Instance = $"SENSOR-D{ownerKey}-{deviceIndex}"
			};

			if ("DeviceLicense".Equals(licenseType))
			{
				l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
				l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
			}
			else if ("ServerLicense".Equals(licenseType))
			{
				l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
				l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
			}
			else if ("ClientLicense".Equals(licenseType))
			{
				l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
				l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
			}
			else
			{

				throw new InvalidOperationException($"Invalid the license type '{licenseType}'");
			}

			productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
			l.ProductKey = productKey;

			var result = c.Create(new LicenseData2[] { l });

			var directory = $"{path}\\{ownerKey}\\{licenseType}";
			Directory.CreateDirectory(directory);

			var fileName = $"{directory}\\{l.LicenseNumber}.{l.Instance}.lic";

			File.WriteAllBytes(fileName, result.Body);
			CreateLicenseDescriptionFile(l, fileName);

			var licBody = File.ReadAllBytes(fileName);

			var vd = new VerificationData2
			{
				OwnerId = l.OwnerId,
				ProductName = l.ProductName,
				ProductKey = l.ProductKey,
				LicenseType = l.LicenseType,
				Date = startDate,
				YearHash = LicenseVerifier.EncodeYear(2020)
			};

			var cc = LicenseVerifier.Verify(vd, licBody);

			Console.WriteLine($"Made license: '{productKey}' >>> {fileName}");
			return productKey;
		}

		private static void CreateLicenseDescriptionFile(LicenseData l, string fileName)
        {
            var l2 = l as LicenseData2;
            var l4 = l as LicenseData4;

			var verFileData = new StringBuilder();

            verFileData.AppendLine();
            verFileData.AppendLine("  -- License Data -- ");
            verFileData.AppendLine($"License Number    : '{l.LicenseNumber}'");
            verFileData.AppendLine($"License Type      : {l.LicenseType}");
            verFileData.AppendLine($"Issued by Company : {l.Company}");
            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Owner Id     : '{l.OwnerId}'");
            verFileData.AppendLine($"Owner Name   : '{l.OwnerName}'");
            verFileData.AppendLine($"Product Name : '{l.ProductName}'");
            verFileData.AppendLine($"Product Key  : '{l.ProductKey}'");
            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Created/Renewal : {l.Created}");

            if (l2 != null)
            {
                verFileData.AppendLine($"Limitation Terms: {l2.LimitationTerms}");
            }

            verFileData.AppendLine($"Start Date      : {l.StartDate}");
            verFileData.AppendLine($"Stop Date       : {l.StopDate}");

            if (l2 != null)
            {
                verFileData.AppendLine($"Year            : {l2.Year}");
                verFileData.AppendLine($"Version         : {l2.Version}");
                verFileData.AppendLine($"AssemblyFullName: {l2.AssemblyFullName}");
            }

            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Instance : '{l.Instance}'");
            verFileData.AppendLine("  ------------------ ");

            if (l4?.ExternalServices != null)
            {
	            verFileData.AppendLine("  ------------------ ");
	            verFileData.AppendLine($"  External Services ({l4.ExternalServices.Length}): ");
	            var index = 0;
	            foreach (var serviceDescriptor in l4.ExternalServices)
	            {
		            ++index;
					verFileData.AppendLine($"   - {index:D3} SID='{serviceDescriptor.Id}'; Name='{serviceDescriptor.Name}'");
				}
				verFileData.AppendLine("  ------------------ ");
			}

            File.WriteAllText(fileName + ".txt", verFileData.ToString(), Encoding.UTF8);
        }


		static void ICSControl_ForUDCR_UNICOM_2020()
		{
			//var ownerId = "OID-BD13-G65-N00"; //  Сам УДЦР
			var ownerId = "OID-BD13-G65-N01"; //  лицензии для УДЦР выданы УНИКОМ

			var ownerName = "Державне підприємство «Український державний центр радіочастот»";
			var company = "ТОВ 'Лабораторія інформаційних систем'";
			var ownerKey = "BD13-G65";
			var startDate = new DateTime(2020, 1, 1);
			var stopDate = new DateTime(2030, 12, 31);
			var path = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\УНИКОМ\BD13-G65\DeviceLicense";
			
			for (int i = 0; i < 90; i++)
			{
				var licPrefix = "LIC-D";
				var instancePrefix = "SENSOR-D";
				BuilProductLicense(
					path, 
					licPrefix, 
					instancePrefix, 
					"DeviceLicense", 
					"ICS Control Device", 
					ownerName, 
					ownerId, 
					ownerKey, 
					company, 
					startDate, 
					stopDate, 2020, LicenseLimitationTerms.Year);
			}

			
		}

		static void ICSMPlugin_ForTesting_Calc_2020()
		{
			const string ownerId = "OID-BD12-A00-N00";
			const string ownerName = "ТОВ 'Лабораторія інформаційних систем'";
			const string company = "ТОВ 'Лабораторія інформаційних систем'";
			const string ownerKey = "BD12-A00";
			var startDate = new DateTime(2020, 6, 1);
			var stopDate = new DateTime(2025, 6, 1);

			var licPrefix = "LIC-P";
			var instancePrefix = "ICSMP-C";

			var path1 = @"C:\Projects\Licensing\Test\ICSMPlugins\SdrnCalcServerClient";

			

			BuilProductLicense(
				path1,
				licPrefix,
				instancePrefix,
				"ClientLicense",
				"ICSM Plugin - SDRN Calc Server Client",
				ownerName,
				ownerId,
				ownerKey,
				company,
				startDate,
				stopDate, 2020, LicenseLimitationTerms.TimePeriod);


			var path2 = @"C:\Projects\Licensing\Test\ICSMPlugins\SdrnStationCalibrationCalc";
			BuilProductLicense(
				path2,
				licPrefix,
				instancePrefix,
				"ClientLicense",
				"ICSM Plugin - SDRN Station Calibration Calc",
				ownerName,
				ownerId,
				ownerKey,
				company,
				startDate,
				stopDate, 2020, LicenseLimitationTerms.TimePeriod);


		}

		static void ICSMPlugin_ForUDCR_CalcTasks_2020()
		{

			const string ownerKey = "BD13-G65";
			const string ownerId = "OID-BD13-G65-N00";
			const string ownerName = "Державне підприємство «Український державний центр радіочастот»";
			const string company = "ТОВ 'Лабораторія інформаційних систем'";
			
			var startDate = new DateTime(2020, 6, 17);
			var stopDate = new DateTime(2025, 6, 17);

			var licPrefix = "LIC-P";
			var instancePrefix = "ICSMP-C";

			var path1 = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\BD13-G65\Plugins\XICSM_SdrnCalcServerClient";
			for (int i = 0; i < 8; i++)
			{
				BuilProductLicense(
					path1,
					licPrefix,
					instancePrefix,
					"ClientLicense",
					"ICSM Plugin - SDRN Calc Server Client",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod);
			}
			


			var path2 = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\BD13-G65\Plugins\XICSM_SdrnStationCalibrationCalc";
			for (int i = 0; i < 8; i++)
			{
				BuilProductLicense(
					path2,
					licPrefix,
					instancePrefix,
					"ClientLicense",
					"ICSM Plugin - SDRN Station Calibration Calc",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod);
			}

			//const ushort year = 2020;

			startDate = new DateTime(2020, 6, 17);
			stopDate = new DateTime(2021, 1, 1);
			var path3 = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\BD13-G65\CalcServer";
			for (int i = 0; i < 1; i++)
			{
				BuilProductLicense(
					path3,
					"LIC-C",
					"SDRNSV-C",
					"ServerLicense",
					"SDRN Calc Server",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.Year);
			}

			startDate = new DateTime(2020, 6, 17);
			stopDate = new DateTime(2021, 1, 1);
			var path4 = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\BD13-G65\InfocenterServer";
			for (int i = 0; i < 1; i++)
			{
				BuilProductLicense(
					path4,
					"LIC-I",
					"SDRNSV-I",
					"ServerLicense",
					"SDRN Infocenter Server",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.Year);
			}

		}

		static void SdrnDevice_ForUDCR_2020_50p()
		{

			var ownerKey = Storage.Clients.UDCR.OwnerKey; // "BD13-G65";
			var ownerId = Storage.Clients.UDCR.OwnerId;  // "OID-BD13-G65-N00";
			var ownerName = Storage.Clients.UDCR.OwnerName; // "Державне підприємство «Український державний центр радіочастот»";
			var company = Storage.Companies.LIS_Uk; // "ТОВ 'Лабораторія інформаційних систем'";

			var startDate = new DateTime(2020, 7, 15);
			var stopDate = new DateTime(2025, 7, 15);

			//var licPrefix = "LIC-P";
			//var instancePrefix = "ICSMP-C";

			var licPrefix = "LIC-D";
			var instancePrefix = "SENSOR-D";

			var path1 = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2020\BD13-G65\DeviceLicense\Июль";
			for (int i = 0; i < 50; i++)
			{
				BuilProductLicense(
					path1,
					licPrefix,
					instancePrefix,
					Storage.LicenseTypes.DeviceLicense, // "DeviceLicense",
					Storage.Products.ICS_Control_Device, // "ICS Control Device",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.Year);
			}

		}

		static void SdrnGN06CalcPlugin_ForATDI_SA_2020_1p()
		{

			var ownerKey = Storage.Clients.ATDI_SA.OwnerKey; // "BD13-G65";
			var ownerId = Storage.Clients.ATDI_SA.OwnerId;  // "OID-BD13-G65-N00";
			var ownerName = Storage.Clients.ATDI_SA.OwnerName; // "Державне підприємство «Український державний центр радіочастот»";
			var company = Storage.Companies.ATDI_Ukraine_EN; // "ТОВ 'Лабораторія інформаційних систем'";

			var startDate = new DateTime(2020, 7, 16);
			var stopDate = new DateTime(2020, 8, 20);

			var licPrefix = "LIC-P";
			var instancePrefix = "ICSMP-C";

			//var licPrefix = "LIC-D";
			//var instancePrefix = "SENSOR-D";

			var path1 = @"C:\Projects\Licensing\ATDI_SA\ICSM_Plugin_GE06_Calc\20200716";
			for (int i = 0; i < 1; i++)
			{
				BuilProductLicense(
					path1,
					licPrefix,
					instancePrefix,
					Storage.LicenseTypes.ClientLicense, // "DeviceLicense",
					Storage.Products.ICSM_Plugin_GE06_Calc, // "ICS Control Device",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod);
			}

		}

		static void SdrnGN06CalcPlugin_ForTest_2020()
		{

			var ownerId = Storage.Clients.LIS_ForTest.OwnerId; // "OID-BD12-A00-N00";
			var ownerName = Storage.Clients.LIS_ForTest.OwnerName; // "ТОВ 'Лабораторія інформаційних систем'";
			var company = Storage.Companies.ATDI_Ukraine_EN;
			var ownerKey = Storage.Clients.LIS_ForTest.OwnerKey; // "BD12-A00";
			var startDate = new DateTime(2020, 7, 16);
			var stopDate = new DateTime(2025, 7, 16);

			var licPrefix = "LIC-P";
			var instancePrefix = "ICSMP-C";

			var path1 = @"C:\Projects\Licensing\Test\ICSMPlugins\SdrnGe06Calc";

			for (int i = 0; i < 1; i++)
			{
				BuilProductLicense(
					path1,
					licPrefix,
					instancePrefix,
					Storage.LicenseTypes.ClientLicense, 
					Storage.Products.ICSM_Plugin_GE06_Calc,
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod);
			}

		}

		static void CalcServer_ForATDI_SA_2020_1p()
		{

			var ownerKey = Storage.Clients.ATDI_SA.OwnerKey; // "BD13-G65";
			var ownerId = Storage.Clients.ATDI_SA.OwnerId;  // "OID-BD13-G65-N00";
			var ownerName = Storage.Clients.ATDI_SA.OwnerName; // "Державне підприємство «Український державний центр радіочастот»";
			var company = Storage.Companies.ATDI_Ukraine_EN; // "ТОВ 'Лабораторія інформаційних систем'";

			var startDate = new DateTime(2020, 7, 16);
			var stopDate = new DateTime(2020, 8, 20);

			var licPrefix = "LIC-C";
			var instancePrefix = "SDRNSV-C";

			

			var path1 = @"C:\Projects\Licensing\ATDI_SA\CalcServer\20200716";
			for (int i = 0; i < 1; i++)
			{
				BuilProductLicense(
					path1,
					licPrefix,
					instancePrefix,
					Storage.LicenseTypes.ServerLicense, // "DeviceLicense",
					Storage.Products.SDRN_Calc_Server, // "ICS Control Device",
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod);
			}

		}

		static void WebQuery_for_CRA_Lithuania_2020_1p()
		{

			var ownerKey = Storage.Clients.CRA_Lithuania.OwnerKey; 
			var ownerId = Storage.Clients.CRA_Lithuania.OwnerId;  
			var ownerName = Storage.Clients.CRA_Lithuania.OwnerName; 
			var company = Storage.Companies.ATDI_Ukraine_EN; 

			var startDate = new DateTime(2020, 7, 17);
			var stopDate = new DateTime(2021, 1, 1);
			
			// WebPortal 
			var webPortalLicPrefix = "LIC-WQWP";
			var webPortalInstancePrefix = "WBP-WQ";

			var webPortalPath = @"C:\Projects\Licensing\CRA_Lithuania\WebQuery\WebPortal\20200717";
			for (int i = 0; i < 1; i++)
			{
				BuildProductLicenseAsVer4(
					webPortalPath,
					webPortalLicPrefix,
					webPortalInstancePrefix,
					Storage.LicenseTypes.ServerLicense, 
					Storage.Products.WebQuery_Web_Portal, 
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod,
					null);
			}

			var appServerLicPrefix = "LIC-WQAS";
			var appServerInstancePrefix = "APPSRV-WQ";

			var appServerPath = @"C:\Projects\Licensing\CRA_Lithuania\WebQuery\AppServer\20200717";
			for (int i = 0; i < 1; i++)
			{
				BuildProductLicenseAsVer4(
					appServerPath,
					appServerLicPrefix,
					appServerInstancePrefix,
					Storage.LicenseTypes.ServerLicense, 
					Storage.Products.WebQuery_Application_Server,
					ownerName,
					ownerId,
					ownerKey,
					company,
					startDate,
					stopDate, 2020, LicenseLimitationTerms.TimePeriod,
					new ExternalServiceDescriptor[]
					{
						new ExternalServiceDescriptor
						{
							Id = "152B48BE-4CA9-478A-B412-C7CF5D64962E",
							Name = "Public WEB Portal"
						}, 
					});
			}
		}

		private static string BuildNextLicenseNumber(string licPrefix, string ownerKey, int numMaxSize = 3)
		{
			var number = string.Empty;
			var count = 0;

			while (++count < 100_000)
			{
				var licenseIndex = GetUniqueIntegerKey(numMaxSize);
				number = $"{licPrefix}{ownerKey}-{licenseIndex}";

				if (!LicenseNumbersDb.ContainsKey(number))
				{
					LicenseNumbersDb.Add(number, number);
					return number;
				}
			}
			throw new InvalidOperationException("Couldn't build next number of license");
		}

		private static string BuildNextInstanceNumber(string instancePrefix, string ownerKey, int numMaxSize = 4)
		{
			var number = string.Empty;
			var count = 0;

			while (++count < 100_000)
			{
				var instanceIndex = GetUniqueIntegerKey(numMaxSize);
				number = $"{instancePrefix}{ownerKey}-{instanceIndex}";

				if (!InstanceNumbersDb.ContainsKey(number))
				{
					InstanceNumbersDb.Add(number, number);
					return number;
				}
			}
			throw new InvalidOperationException("Couldn't build next number of instance");
		}

		private static string BuilProductLicense(
			string path,
			string licPrefix,
			string instancePrefix,
			string licenseType,
			string productName,
			string ownerName,
			string ownerId,
			string ownerKey,
			string company,
			DateTime startDate,
			DateTime stopDate,
			ushort year, LicenseLimitationTerms limitationTerms = LicenseLimitationTerms.Year | LicenseLimitationTerms.TimePeriod)
		{
			if (!"DeviceLicense".Equals(licenseType)
			    && !"ServerLicense".Equals(licenseType)
			    && !"ClientLicense".Equals(licenseType))
			{
				throw new InvalidOperationException($"Invalid the license type '{licenseType}'");
			}

			var c = new LicenseCreator();
			var l = new LicenseData2()
			{
				LicenseType = licenseType,
				Company = company,
				Copyright = "",
				OwnerId = ownerId,
				OwnerName = ownerName,
				Created = DateTime.Now,
				StartDate = startDate,
				StopDate = stopDate,
				ProductName = productName,
				Count = 1,
				LimitationTerms = limitationTerms,
				Year = year,
				LicenseNumber = BuildNextLicenseNumber(licPrefix, ownerKey),
				Instance = BuildNextInstanceNumber(instancePrefix, ownerKey)
			};

			var productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
			l.ProductKey = productKey;

			var result = c.Create(new LicenseData2[] { l });

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			var fileName = $"{path}\\{l.LicenseNumber}.{l.Instance}.lic";

			File.WriteAllBytes(fileName, result.Body);
			CreateLicenseDescriptionFile(l, fileName);

			var licBody = File.ReadAllBytes(fileName);

			var vd = new VerificationData2
			{
				OwnerId = l.OwnerId,
				ProductName = l.ProductName,
				ProductKey = l.ProductKey,
				LicenseType = l.LicenseType,
				Date = startDate,
				YearHash = LicenseVerifier.EncodeYear(year)
			};

			var cc = LicenseVerifier.Verify(vd, licBody);
			SaveDBs();

			Console.WriteLine($"Build next license: '{productKey}' >>> {fileName}");
			return productKey;
		}

		private static string BuildProductLicenseAsVer4(
			string path,
			string licPrefix,
			string instancePrefix,
			string licenseType,
			string productName,
			string ownerName,
			string ownerId,
			string ownerKey,
			string company,
			DateTime startDate,
			DateTime stopDate,
			ushort year, LicenseLimitationTerms limitationTerms = LicenseLimitationTerms.Year | LicenseLimitationTerms.TimePeriod,
			ExternalServiceDescriptor[] externalServices = null)
		{
			if (!"DeviceLicense".Equals(licenseType)
				&& !"ServerLicense".Equals(licenseType)
				&& !"ClientLicense".Equals(licenseType))
			{
				throw new InvalidOperationException($"Invalid the license type '{licenseType}'");
			}

			var c = new LicenseCreator();
			var l = new LicenseData4()
			{
				LicenseType = licenseType,
				Company = company,
				Copyright = "",
				OwnerId = ownerId,
				OwnerName = ownerName,
				Created = DateTime.Now,
				StartDate = startDate,
				StopDate = stopDate,
				ProductName = productName,
				Count = 1,
				LimitationTerms = limitationTerms,
				Year = year,
				LicenseNumber = BuildNextLicenseNumber(licPrefix, ownerKey),
				Instance = BuildNextInstanceNumber(instancePrefix, ownerKey),
				ExternalServices = externalServices
			};

			var productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
			l.ProductKey = productKey;

			var result = c.Create(new LicenseData4[] { l });

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			var fileName = $"{path}\\{l.LicenseNumber}.{l.Instance}.lic";

			File.WriteAllBytes(fileName, result.Body);
			CreateLicenseDescriptionFile(l, fileName);

			var licBody = File.ReadAllBytes(fileName);

			var vd = new VerificationData2
			{
				OwnerId = l.OwnerId,
				ProductName = l.ProductName,
				ProductKey = l.ProductKey,
				LicenseType = l.LicenseType,
				Date = startDate,
				YearHash = LicenseVerifier.EncodeYear(year)
			};

			var cc = LicenseVerifier.Verify(vd, licBody);
			SaveDBs();

			Console.WriteLine($"Build next license: '{productKey}' >>> {fileName}");
			return productKey;
		}
	}
}
