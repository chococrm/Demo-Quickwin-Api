using BCRM_App.Constants;
using System;
using System.Globalization;

namespace BCRM_App.Helpers
{
	public class DateTimeHelper
    {
		public static DateTime ParseDateTime(string dateTime)
		{
			try
			{
				string[] array = dateTime.Split("/");

				int year = int.Parse(array[2]);

				if (year >= 2400)
				{
					year -= 543;
				}

				string newDateTime = $"{array[0]}/{array[1]}/{year}";

				DateTime resp = DateTime.ParseExact(newDateTime, CCConstant.Common.DateOfBirthFormat, CultureInfo.InvariantCulture);

				return resp;
			}
			catch (Exception)
			{
				throw new Exception("Invalid DateTime");
			}
		}
	}
}