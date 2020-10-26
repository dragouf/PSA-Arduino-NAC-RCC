using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSA_Arduino_NAC
{
    class SeedKeyGenerator
    {
		private static Int32 transform(int data, int[] sec)
		{
			Int32 result = ((data % sec[0]) * sec[2]) - ((data / sec[0]) * sec[1]);
			if (result < 0)
				result += (sec[0] * sec[2]) + sec[1];
			return result;
		}

		public static string getKey(string seedTXT, string appKeyTXT)
		{
			Int32 result;

			string[] seed = { seedTXT.Substring(0, 2), seedTXT.Substring(2, 2), seedTXT.Substring(4, 2), seedTXT.Substring(6, 2) };
			string[] appKey = { appKeyTXT.Substring(0, 2), appKeyTXT.Substring(2, 2) };

			// secrets
			int[] sec_1 = { 0xB2, 0x3F, 0xAA };
			int[] sec_2 = { 0xB1, 0x02, 0xAB };

			// Compute each 16b part of the response
			Int32 res_msb = transform(Int16.Parse(appKey[0] + appKey[1], System.Globalization.NumberStyles.HexNumber), sec_1) | transform(Int16.Parse(seed[0] + seed[3], System.Globalization.NumberStyles.HexNumber), sec_2);
			Int32 res_lsb = transform(Int16.Parse(seed[1] + seed[2], System.Globalization.NumberStyles.HexNumber), sec_1) | transform(res_msb, sec_2);
			result = (res_msb << 16) | res_lsb;
			
			return result.ToString("X8");
		}
	}
}
