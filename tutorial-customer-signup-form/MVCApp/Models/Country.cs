using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class Country
    {
        public static List<string> GetAllCountries()
        {
            List<string> countries = new List<string>();

            try
            {
                CultureInfo[] cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                foreach (CultureInfo cultureInfo in cultureInfos)
                {
                    RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);

                    if (!(countries.Contains(regionInfo.EnglishName)))
                    {
                        countries.Add(regionInfo.EnglishName);
                    }
                }

                countries.Sort();
                countries = Country.SortCountryList(countries);
            }
            catch (Exception)
            {
                throw;
            }

            return countries;
        }

        private static List<string> SortCountryList(List<string> countries)
        {
            try
            {
                int nl = countries.FindIndex(c => c.Contains("Canada"));
                countries = MoveItemAtIndexToFront(countries, nl);
                int at = countries.FindIndex(c => c.Contains("United States"));
                countries = MoveItemAtIndexToFront(countries, at);
                int de = countries.FindIndex(c => c.Contains("Germany"));
                countries = MoveItemAtIndexToFront(countries, de);

                // insert placeholder between most common countries and all the rest
                // custome validation ensures that PlaceHolde isn't a valide country
                string placeholder = "";
                countries.Insert(3, placeholder);

                return countries;
            }
            catch (Exception)
            {
                return countries;
            }
        }

        private static List<string> MoveItemAtIndexToFront(List<string> countries, int i)
        {
            string countryName = countries[i];
            countries.RemoveAt(i);
            countries.Insert(0, countryName);

            return countries;
        }
    }
}