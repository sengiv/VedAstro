

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using SwissEphNet;
using static VedAstro.Library.PlanetName;


namespace VedAstro.Library
{

	//█▀▄▀█ █▀▀ ▀▀█▀▀ █░░█ █▀▀█ █▀▀▄ 　 ▀▀█▀▀ █▀▀█ 　 ▀▀█▀▀ █░░█ █▀▀ 　 █▀▄▀█ █▀▀█ █▀▀▄ █▀▀▄ █▀▀ █▀▀ █▀▀ 
	//█░▀░█ █▀▀ ░░█░░ █▀▀█ █░░█ █░░█ 　 ░░█░░ █░░█ 　 ░░█░░ █▀▀█ █▀▀ 　 █░▀░█ █▄▄█ █░░█ █░░█ █▀▀ ▀▀█ ▀▀█ 
	//▀░░░▀ ▀▀▀ ░░▀░░ ▀░░▀ ▀▀▀▀ ▀▀▀░ 　 ░░▀░░ ▀▀▀▀ 　 ░░▀░░ ▀░░▀ ▀▀▀ 　 ▀░░░▀ ▀░░▀ ▀▀▀░ ▀░░▀ ▀▀▀ ▀▀▀ ▀▀▀ 

	//█▀▀█ █▀▀█ █▀▀▄ █▀▀ █▀▀█ 　 ▀▀█▀▀ █▀▀█ 　 ▀▀█▀▀ █░░█ █▀▀ 　 █▀▀ █░░█ █▀▀█ █▀▀█ █▀▀ 
	//█░░█ █▄▄▀ █░░█ █▀▀ █▄▄▀ 　 ░░█░░ █░░█ 　 ░░█░░ █▀▀█ █▀▀ 　 █░░ █▀▀█ █▄▄█ █░░█ ▀▀█ 
	//▀▀▀▀ ▀░▀▀ ▀▀▀░ ▀▀▀ ▀░▀▀ 　 ░░▀░░ ▀▀▀▀ 　 ░░▀░░ ▀░░▀ ▀▀▀ 　 ▀▀▀ ▀░░▀ ▀░░▀ ▀▀▀▀ ▀▀▀

	/// <summary>
	/// Collection of astronomical calculator functions
	/// Note : Many of the functions here use cacheing machanism
	/// </summary>
	public static partial class Calculate
	{
		#region AVASTA



		[API("Gets all the Avastas for a planet, Lajjita, Garvita, Kshudita, etc...")]
		public static List<Avasta> PlanetAvasta(PlanetName planetName, Time time)
		{
			var finalList = new Avasta?[6]; //total 6 avasta

			//add in each avasta that matches
			finalList[0] = IsPlanetInLajjitaAvasta(planetName, time) ? Avasta.LajjitaShamed : null;
			finalList[1] = IsPlanetInGarvitaAvasta(planetName, time) ? Avasta.GarvitaProud : null;
			finalList[2] = IsPlanetInKshuditaAvasta(planetName, time) ? Avasta.KshuditaStarved : null;
			finalList[3] = IsPlanetInTrashitaAvasta(planetName, time) ? Avasta.TrishitaThirst : null;
			finalList[4] = IsPlanetInMuditaAvasta(planetName, time) ? Avasta.MuditaDelighted : null;
			finalList[5] = IsPlanetInKshobhitaAvasta(planetName, time) ? Avasta.KshobitaAgitated : null;

			// Convert array to List<Avasta> and remove nulls
			var resultList = finalList.OfType<Avasta>().ToList();
			return resultList;

		}

		[API("Lajjita / humiliated : Planet in the 5th house in conjunction with rahu or ketu, Saturn or mars.")]
		public static bool IsPlanetInLajjitaAvasta(PlanetName planetName, Time time)
		{
			//check if input planet is in 5th
			var isPlanetIn5thHouse = IsPlanetInHouse(time, planetName, HouseName.House5);

			//check if any negative planets is in 5th (conjunct)
			var planetNames = new List<PlanetName>() { Rahu, Ketu, Saturn, Mars };
			var rahuKetuSaturnMarsIn5th = IsAllPlanetInHouse(time, planetNames, HouseName.House5);

			//check if all conditions are met Lajjita
			var isLajjita = isPlanetIn5thHouse && rahuKetuSaturnMarsIn5th;

			return isLajjita;

		}

		[API("Garvita/proud – Planet in exaltation sign or moolatrikona zone.happiness and gains")]
		public static bool IsPlanetInGarvitaAvasta(PlanetName planetName, Time time)
		{
			//Planet in exaltation sign
			var planetExalted = IsPlanetExalted(planetName, time);

			//moolatrikona zone
			var planetInMoolatrikona = IsPlanetInMoolatrikona(planetName, time);

			//check if all conditions are met for Garvita
			var isGarvita = planetExalted || planetInMoolatrikona;

			return isGarvita;
		}

		[API("Kshudita/hungry – Planet in enemy’s sign or conjoined with enemy or aspected by enemy.Grief")]
		public static bool IsPlanetInKshuditaAvasta(PlanetName planetName, Time time)
		{
			//Planet in enemy’s sign 
			var planetExalted = IsPlanetInEnemyHouse(time, planetName);

			//conjoined with enemy (same house)
			var conjunctWithMalefic = IsPlanetConjunctWithEnemyPlanets(planetName, time);

			//aspected by enemy
			var aspectedByMalefic = IsPlanetAspectedByEnemyPlanets(planetName, time);

			//check if all conditions are met for Kshudita
			var isKshudita = planetExalted || conjunctWithMalefic || aspectedByMalefic;

			return isKshudita;
		}

		/// <summary>
		/// i) The Planet who being conjoined or aspected by a Malefic or his enemy Planet is situated,
		/// without the aspect of a benefic Planet, in the 4th House is Trashita.
		/// 
		/// Another version
		/// 
		/// If the Planet is situated in a watery sign, is aspected by an enemy Planet and
		/// is without the aspect of benefic Planets he is called Trashita.
		///
		/// --------
		/// "A planet in a Water Sign and aspected by an enemy planet,
		/// with no auspiscious Graha aspecting is said to be Trishita Avastha/Thirsty State".
		/// 
		/// This state is in effect whenever a planet is in a Water Sign and it gets
		/// aspected by an enemy planet. But if, a Gentle Planet (Mercury/Venus/Moon) aspects here,
		/// it strengthens the planet in Water Sign. This Avastha is only for the aspecting enemy
		/// planet that will cause Trishita/Thirst. This state shows that a planet in a watery
		/// Rasi can still be productive even when aspected by enemies, though it will not be happy.
		/// As the name “Thirsty State” implies, it indicates the lack of emotional fulfillment that a planet experiences.
		/// </summary>
		[API("Trashita/thirsty – Planet in a watery sign, aspected by a enemy and is without the aspect of benefic Planets")]
		public static bool IsPlanetInTrashitaAvasta(PlanetName planetName, Time time)
		{
			//Planet in a watery sign
			var planetInWater = IsPlanetInWaterySign(planetName, time);

			//aspected by an enemy
			var aspectedByEnemy = IsPlanetAspectedByEnemyPlanets(planetName, time);

			//no benefic planet aspect
			var noBeneficAspect = false == IsPlanetAspectedByBeneficPlanets(planetName, time);

			//check if all conditions are met for Trashita
			var isTrashita = planetInWater && aspectedByEnemy && noBeneficAspect;

			return isTrashita;
		}

		/// <summary>
		/// The Planet who is in his friend’s sign, is in conjunction with Jupiter,
		/// and is together with or is aspected by a friendly Planet is called Mudita
		/// 
		/// Mudita/sated/happy – Planet in a friend’s sign or aspected by a friend and conjoined with Jupiter, Gains
		///
		/// If a planet is in a friend’s sign or joined with a friend or aspected by a friend,
		/// or that joined with Jupiter is called Mudita Avastha/Delighted State
		///
		/// It is clear from explanation itself that a planet will feel delighted when it
		/// is in friendly sign or friendly planet conjuncts/aspects or it is joined by the
		/// biggest benefic planet Jupiter. We can understand planet’s delight in such cases. 
		/// 
		/// Planet in friendly sign - A planet in a friendly sign is productive,
		/// and the stronger that friend planet, the more productive it will be. 
		/// </summary>
		[API("Mudita/sated/happy – Planet in a friend’s sign or aspected by a friend and conjoined with Jupiter, Gains")]
		public static bool IsPlanetInMuditaAvasta(PlanetName planetName, Time time)
		{
			//Planet who is in his friend’s sign
			var isInFriendly = IsPlanetInFriendHouse(time, planetName);

			//is in conjunction with Jupiter
			var isConjunctJupiter = IsPlanetConjunctWithPlanet(planetName, Jupiter, time);

			//is together with or is aspected by a friendly (conjunct or aspect)
			var isConjunctWithFriendly = IsPlanetConjunctWithFriendPlanets(planetName, time);
			var isAspectedByFriendly = IsPlanetAspectedByFriendPlanets(planetName, time);
			var accosiatedWithFriendly = isConjunctWithFriendly || isAspectedByFriendly;

			//check if all conditions are met for Mudita
			var isMudita = isInFriendly || isConjunctJupiter || accosiatedWithFriendly;

			return isMudita;
		}

		/// <summary>
		/// If a planet is conjunct by Sun or it is aspected by Enemy Malefic Planets then
		/// it should always be known as Kshobhita Avastha/Agitated State
		/// 
		/// Kshobhita/guilty/repentant – Planet in conjunction with sun and aspected by malefics and an enemy. Penury
		/// </summary>
		[API("Kshobhita/guilty/repentant – Planet in conjunction with sun and aspected by malefics and an enemy. Penury")]
		public static bool IsPlanetInKshobhitaAvasta(PlanetName planetName, Time time)
		{
			//Planet in conjunction with sun 
			var conjunctWithSun = IsPlanetConjunctWithPlanet(planetName, Sun, time);

			//aspected by an enemy or malefic
			var isAspectedByEnemy = false == IsPlanetAspectedByEnemyPlanets(planetName, time);
			var isAspectedByMalefics = false == IsPlanetAspectedByMaleficPlanets(planetName, time);
			var accosiatedWithBadPlanets = isAspectedByEnemy || isAspectedByMalefics;

			//check if all conditions are met for Kshobhita
			var isKshobhita = conjunctWithSun && accosiatedWithBadPlanets;

			return isKshobhita;
		}

		#endregion

		#region ALL DATA

		/// <summary>
		/// Wrapper function for open API
		/// </summary>
		[API("Gets all possible calculations for a Planet at a given Time", Category.Astronomical)]
		public static List<APIFunctionResult> AllPlanetData(PlanetName planetName, Time time)
		{
			//exclude this method from getting included in "Find" and Execute below
			MethodBase method = MethodBase.GetCurrentMethod();
			MethodInfo methodToExclude = method as MethodInfo;

			//do calculation
			var raw = AutoCalculator.FindAndExecuteFunctions(Category.All, methodToExclude, planetName, time);

			return raw;
		}

		/// <summary>
		/// Wrapper function for open API
		/// </summary>
		[API("All possible calculations for a House at a given Time", Category.Astronomical)]
		public static List<APIFunctionResult> AllHouseData(HouseName houseName, Time time)
		{
			//exclude this method from getting included in "Find" and Execute below
			MethodBase method = MethodBase.GetCurrentMethod();
			MethodInfo methodToExclude = method as MethodInfo;

			//do calculation
			var raw = AutoCalculator.FindAndExecuteFunctions(Category.All, methodToExclude, houseName, time);

			return raw;
		}

		/// <summary>
		/// Wrapper function for open API
		/// </summary>
		[API("All possible calculations for a Planet and House at a given Time", Category.Astronomical)]
		public static List<APIFunctionResult> AllPlanetHouseData(PlanetName planetName, HouseName houseName, Time time)
		{
			//exclude this method from getting included in "Find" and Execute below
			MethodBase method = MethodBase.GetCurrentMethod();
			MethodInfo methodToExclude = method as MethodInfo;

			//do calculation
			var raw = AutoCalculator.FindAndExecuteFunctions(Category.All, methodToExclude, planetName, houseName, time);

			return raw;
		}

		/// <summary>
		/// Wrapper function for open API
		/// </summary>
		[API("All possible calculations for a Zodiac Sign at a given Time", Category.Astronomical)]
		public static List<APIFunctionResult> AllZodiacSignData(ZodiacName zodiacName, Time time)
		{
			//exclude this method from getting included in "Find" and Execute below
			MethodBase method = MethodBase.GetCurrentMethod();
			MethodInfo methodToExclude = method as MethodInfo;

			//do calculation
			var raw = AutoCalculator.FindAndExecuteFunctions(Category.All, methodToExclude, zodiacName, time);

			return raw;
		}


		#endregion

		/// <summary>
		/// SkyChartGIF squeeze the Sky Juice!
		/// </summary>
		[API("Get sky chart as animated GIF. URL can be used like a image source link")]
		public static async Task<byte[]> SkyChartGIF(Time time) => await SkyChartManager.GenerateChartGif(time, 750, 230);

		/// <summary>
		/// SkyChartGIF squeeze the Sky Juice!
		/// </summary>
		[API("Get sky chart at a given time. SVG image file. URL can be used like a image source link")]
		public static async Task<string> SkyChart(Time time) => await SkyChartManager.GenerateChart(time, 750, 230);

		public static bool IsPlanetInWaterySign(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = Calculate.PlanetRasiSign(planetName, time);

			//check if sign is watery
			var isWater = Calculate.IsWaterSign(planetSign.GetSignName());

			return isWater;
		}

		/// <summary>
		/// Wrapper function to make planet name appear "Payload" of API call data, for easier data probing by 3rd party code
		/// </summary>
		[API("English name of planet", Category.Astronomical)]
		public static string PlanetName(PlanetName planetName) => planetName.ToString();

		/// <summary>
		/// Use of Residential Strength --This will
		/// enable us to judge the exact quantity of effect that
		/// a pJanet in a Bhava gives, which may find expression
		/// during its Dasa.Its application and usefulness
		/// will be explained on a subsequent occasion.
		/// This effect will materialize during his Dasa or
		/// Bhukti. This is only a general statement standing
		/// to be modified or qualified in the light of other
		/// important factors such as, the strength or the weakness
		/// of the planets aspecting the Bhavas, the
		/// strength of the Bhava itself and the disposition
		/// of planets towards particular signs, the yogakarakas
		/// and such other factors.
		/// For instance, in the Standard Horoscope Jupiter
		/// gives 0.48 units of the total effects of the 6th Bhava.
		/// </summary>
		[API("Strength to judge the exact quantity of effect planet gives in a house")]
		public static double ResidentialStrength(PlanetName planetName, Time time)
		{
			return 0;

			//todo from PG15 of Bhava and Graha Balas
			throw new NotImplementedException("");
		}

		/// <summary>
		/// Converts time back to longitude, it is the reverse of GetLocalTimeOffset in Time
		/// Exp :  5h. 10m. 20s. E. Long. to 77° 35' E. Long
		/// </summary>
		[API("Converts time back to longitude", Category.Astronomical)]
		public static Angle TimeToLongitude(TimeSpan time)
		{
			//TODO function is a candidate for caching
			//degrees is equivalent to hours
			var totalDegrees = time.TotalHours * 15;

			return Angle.FromDegrees(totalDegrees);
		}

		//NORMAL FUNCTIONS
		//FUNCTIONS THAT CALL OTHER FUNCTIONS IN THIS CLASS

		/// <summary>
		/// Gets the ephemris time that is consumed by Swiss Ephemeris
		/// </summary>
		[API("Converts normal time to Ephemeris time shown as a number")]
		public static double TimeToEphemerisTime(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("TimeToEphemerisTime", time), _timeToEphemerisTime);


			//UNDERLYING FUNCTION
			double _timeToEphemerisTime()
			{
				SwissEph ephemeris = new();

				//set GREGORIAN CALENDAR
				int gregflag = SwissEph.SE_GREG_CAL;

				//get LMT at UTC (+0:00)
				DateTimeOffset utcDate = LmtToUtc(time);

				//extract details of time
				int year = utcDate.Year;
				int month = utcDate.Month;
				int day = utcDate.Day;
				double hour = (utcDate.TimeOfDay).TotalHours;


				double jul_day_UT;
				double jul_day_ET;

				//do conversion to ephemris time
				jul_day_UT = ephemeris.swe_julday(year, month, day, hour, gregflag); //time to Julian Day
				jul_day_ET = jul_day_UT + ephemeris.swe_deltat(jul_day_UT); //Julian Day to ET

				return jul_day_ET;
			}

		}

		/// <summary>
		/// Gets planet longitude used vedic astrology
		/// Nirayana Longitude = Sayana Longitude corrected to Ayanamsa
		/// Number from 0 to 360, represent the degrees in the zodiac as viewed from earth
		/// Note: Since Nirayana is corrected, in actuality 0 degrees will start at Taurus not Aries
		/// </summary>
		[API("Planet longitude that has been corrected with Ayanamsa")]
		public static Angle PlanetNirayanaLongitude(Time time, PlanetName planetName)
		{
			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey(nameof(PlanetNirayanaLongitude), time, planetName, YearOfCoincidence), _getPlanetNirayanaLongitude);


			//UNDERLYING FUNCTION
			Angle _getPlanetNirayanaLongitude()
			{
				//declare return value
				Angle returnValue;


				//Get sayana longitude on day 
				Angle longitude = PlanetSayanaLongitude(time, planetName);


				//3 - Hindu Nirayana Long = Sayana Long — Ayanamsa.
				Angle birthAyanamsa = Ayanamsa(time);

				//if below ayanamsa add 360 before minus
				returnValue = longitude.TotalDegrees < birthAyanamsa.TotalDegrees
					? (longitude + Angle.Degrees360) - birthAyanamsa
					: longitude - birthAyanamsa;


				return returnValue;
			}


		}

		[API("Gets Moon's position or day in lunar calendar", Category.StarsAboveMe)]
		public static LunarDay LunarDay(Time time)
		{
			//get position of sun & moon
			Angle sunLong = PlanetNirayanaLongitude(time, Sun);
			Angle moonLong = PlanetNirayanaLongitude(time, Moon);

			double rawLunarDate;

			if (moonLong.TotalDegrees > sunLong.TotalDegrees)
			{
				rawLunarDate = (moonLong - sunLong).TotalDegrees / 12.0;
			}
			else
			{
				rawLunarDate = ((moonLong + Angle.Degrees360) - sunLong).TotalDegrees / 12.0;
			}

			//round number to next whole number (ceiling)
			int roundedLunarDateNumber = (int)Math.Ceiling(rawLunarDate);

			//use lunar date number to initialize a lunar day instance
			var lunarDay = new LunarDay(roundedLunarDateNumber);

			//return lunar day to caller
			return lunarDay;


		}

		/// <summary>
		/// Gets constellation behind the moon (shortcut function)
		/// </summary>
		[API("Constellation behind the moon at a given time", Category.StarsAboveMe)]
		public static PlanetConstellation MoonConstellation(Time time) => PlanetConstellation(time, Moon);

		/// <summary>
		/// Gets the constellation behind a planet at a given time
		/// </summary>
		[API("Gets the constellation behind a planet at a given time")]
		public static PlanetConstellation PlanetConstellation(Time time, PlanetName planet)
		{
			//get position of planet in longitude
			var planetLongitude = PlanetNirayanaLongitude(time, planet);

			//return the constellation behind the planet
			return ConstellationAtLongitude(planetLongitude);
		}

		[API("Tarabala or birth ruling constellation strength, used for personal muhurtha")]
		public static Tarabala Tarabala(Time time, Person person)
		{
			int dayRulingConstellationNumber = MoonConstellation(time).GetConstellationNumber();

			int birthRulingConstellationNumber = MoonConstellation(person.BirthTime).GetConstellationNumber();

			int counter = 0;

			int cycle;


			//Need to count from birthRulingConstellationNumber to dayRulingConstellationNumber
			//todo upgrade to "ConstellationCounter", double check validity first
			//If birthRulingConstellationNumber is more than dayRulingConstellationNumber
			if (birthRulingConstellationNumber > dayRulingConstellationNumber)
			{
				//count birthRulingConstellationNumber to last constellation (27)
				int countToLastConstellation = (27 - birthRulingConstellationNumber) + 1; //plus 1 to count it self

				//add dayRulingConstellationNumber to countToLastConstellation(difference)
				counter = dayRulingConstellationNumber + countToLastConstellation;
			}
			else if (birthRulingConstellationNumber == dayRulingConstellationNumber)
			{
				counter = 1;
			}
			else if (birthRulingConstellationNumber < dayRulingConstellationNumber)
			{
				//If dayRulingConstellationNumber is more than or equal to birthRulingConstellationNumber
				counter = (dayRulingConstellationNumber - birthRulingConstellationNumber) + 1; //plus 1 to count it self
			}

			//change to double for division and then round up
			cycle = (int)Math.Ceiling(((double)counter / 9.0));


			//divide the number by 9 if divisible. Otherwise
			//keep it as it is.
			if (counter > 9)
			{
				//get modulos of counter
				counter = counter % 9;
				if (counter == 0)
					counter = 9;
			}


			//initialize new tarabala from tarabala number & cycle
			var returnValue = new Tarabala(counter, cycle);

			return returnValue;
		}

		/// <summary>
		/// Chandrabala or lunar strength
		///
		/// Reference:
		/// Chandrabala. - As we have already said above, the consideration of the
		/// Moon and his position are of much importance in Muhurtha. To be at its
		/// best, the Moon should not occupy in the election chart, a position that
		/// happens to represent the 6th, 8th or 12th from the person's Janma Rasi.
		/// </summary>
		[API("Chandrabala or lunar strength, used for personal muhurtha")]
		public static int Chandrabala(Time time, Person person)
		{
			//TODO Needs to be updated with count sign from sign for better consistency
			//     also possible to leave it as is for better decoupling since this is working fine

			//initialize chandrabala number as 0
			int chandrabalaNumber = 0;

			//get zodiac name & convert to its number
			var dayMoonSignNumber = (int)MoonSignName(time);
			var birthMoonSignNumber = (int)MoonSignName(person.BirthTime);


			//Need to count from birthMoonSign to dayMoonSign

			//If birthMoonSign is more than dayMoonSign
			if (birthMoonSignNumber > dayMoonSignNumber)
			{
				//count birthMoonSign to last zodiac (12)
				int countToLastZodiac = (12 - birthMoonSignNumber) + 1; //plus 1 to count it self

				//add dayMoonSign to countToLastZodiac
				chandrabalaNumber = dayMoonSignNumber + countToLastZodiac;

			}
			else if (birthMoonSignNumber == dayMoonSignNumber)
			{
				chandrabalaNumber = 1;
			}
			else if (birthMoonSignNumber < dayMoonSignNumber)
			{
				//If dayMoonSign is more than or equal to birthMoonSign
				chandrabalaNumber = (dayMoonSignNumber - birthMoonSignNumber) + 1; //plus 1 to count it self
			}

			return chandrabalaNumber;

		}

		[API("Zodiac sign behind the Moon at given time", Category.StarsAboveMe)]
		public static ZodiacName MoonSignName(Time time)
		{
			//get zodiac sign behind the moon
			var moonSign = PlanetRasiSign(Moon, time);

			//return name of zodiac sign
			return moonSign.GetSignName();
		}

		[API("Nithya Yoga = (Longitude of Sun + Longitude of Moon) / 13°20' (or 800')", Category.StarsAboveMe)]
		public static NithyaYoga NithyaYoga(Time time)
		{
			//Nithya Yoga = (Longitude of Sun + Longitude of Moon) / 13°20' (or 800')

			//get position of sun & moon in longitude
			Angle sunLongitude = PlanetNirayanaLongitude(time, Sun);
			Angle moonLongitude = PlanetNirayanaLongitude(time, Moon);

			//get joint motion in longitude of the Sun and the Moon
			var jointLongitudeInMinutes = sunLongitude.TotalMinutes + moonLongitude.TotalMinutes;

			//get unrounded nithya yoga number by
			//dividing joint longitude by 800'
			var rawNithyaYogaNumber = jointLongitudeInMinutes / 800;

			//round to ceiling to get whole number
			var nithyaYogaNumber = Math.Ceiling(rawNithyaYogaNumber);

			//convert nithya yoga number to type
			var nithyaYoga = (NithyaYoga)nithyaYogaNumber;

			//return to caller

			return nithyaYoga;
		}

		[API("used for auspicious activities, part Panchang like Tithi, Nakshatra, Yoga, etc.", Category.StarsAboveMe)]
		public static Karana Karana(Time time)
		{
			//declare karana as empty first
			Karana? karanaToReturn = null;

			//get position of sun & moon
			Angle sunLong = PlanetNirayanaLongitude(time, Sun);
			Angle moonLong = PlanetNirayanaLongitude(time, Moon);

			//get raw lunar date
			double rawlunarDate;

			if (moonLong.TotalDegrees > sunLong.TotalDegrees)
			{
				rawlunarDate = (moonLong - sunLong).TotalDegrees / 12.0;
			}
			else
			{
				rawlunarDate = ((moonLong + new Angle(degrees: 360)) - sunLong).TotalDegrees / 12.0;
			}

			//round number to next whole number (ceiling)
			int roundedLunarDateNumber = (int)Math.Ceiling(rawlunarDate);

			//get lunar day already traversed
			var lunarDayAlreadyTraversed = rawlunarDate - Math.Floor(rawlunarDate);

			switch (roundedLunarDateNumber)
			{
				//based on lunar date get karana
				case 1:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Kimstughna : Library.Karana.Bava;
					break;
				case 23:
				case 16:
				case 9:
				case 2:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Balava : Library.Karana.Kaulava;
					break;
				case 24:
				case 17:
				case 10:
				case 3:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Taitula : Library.Karana.Garija;
					break;
				case 25:
				case 18:
				case 11:
				case 4:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Vanija : Library.Karana.Visti;
					break;
				case 26:
				case 19:
				case 12:
				case 5:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Bava : Library.Karana.Balava;
					break;
				case 27:
				case 20:
				case 13:
				case 6:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Kaulava : Library.Karana.Taitula;
					break;
				case 28:
				case 21:
				case 14:
				case 7:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Garija : Library.Karana.Vanija;
					break;
				case 22:
				case 15:
				case 8:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Visti : Library.Karana.Bava;
					break;
				case 29:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Visti : Library.Karana.Sakuna;
					break;
				case 30:
					karanaToReturn = lunarDayAlreadyTraversed <= 0.5 ? Library.Karana.Chatushpada : Library.Karana.Naga;
					break;

			}

			//if karana not found throw error
			if (karanaToReturn == null)
			{
				throw new Exception("Karana could not be found!");
			}

			return (Karana)karanaToReturn;
		}

		[API("Zodiac sign behind the Sun at a time", Category.StarsAboveMe)]
		public static ZodiacSign SunSign(Time time)
		{
			//get zodiac sign behind the sun
			var sunSign = PlanetRasiSign(Sun, time);

			//return zodiac sign behind sun
			return sunSign;
		}

		///<summary>
		///Find time when Sun was in 0.001 degrees
		///in current sign (just entered sign)
		///</summary>
		[API("Find time when Sun in current sign (just entered sign)", Category.StarsAboveMe)]
		public static Time TimeSunEnteredCurrentSign(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey(nameof(TimeSunEnteredCurrentSign), time), _timeSunEnteredCurrentSign);


			//UNDERLYING FUNCTION
			Time _timeSunEnteredCurrentSign()
			{

				//set the maximum accuracy used to calculate time sun will enter the sign
				//once this limit is hit, the previously calculated time will be returned
				double AccuracyLimit = TimePreset.Minute3;

				//set time decrement accuracy at 96 hours (4 days) at first
				double timeDecrementAccuracy = 96;

				//set input time as possible entered time at first
				var possibleEnteredTime = time;
				var previousPossibleEnteredTime = time;


				//get current sun sign
				var currentSunSign = SunSign(time);

				//if entered time not yet found
				while (true)
				{
					//get the sign at possible entered time
					var possibleSunSign = SunSign(possibleEnteredTime);

					//if possible sign name is same as current sign name, then check if sun is about to enter sign
					var signNameIsSame = possibleSunSign.GetSignName() == currentSunSign.GetSignName();
					if (signNameIsSame)
					{
						//if sun sign is less than 0.001 degrees, entered time found
						if (possibleSunSign.GetDegreesInSign().TotalDegrees < 0.001) { break; }

						//else sun not yet torward the start of the sign, so decrement time
						else
						{
							//back up possible entered time before changing
							previousPossibleEnteredTime = possibleEnteredTime;

							//decrement entered time, to check next possible time
							possibleEnteredTime = possibleEnteredTime.SubtractHours(timeDecrementAccuracy);
						}
					}
					//else sun sign is not same, went to far
					else
					{
						//return possible entered time to previous time
						possibleEnteredTime = previousPossibleEnteredTime;

						//if accuracy limit is hit, then use previous time as answer, stop looking
						if (timeDecrementAccuracy <= AccuracyLimit) { break; }

						//decrease time decrement accuracy by half
						timeDecrementAccuracy = timeDecrementAccuracy / 2;

					}
				}

				//return possible entered time
				return possibleEnteredTime;

			}
		}

		///<summary>
		///Find time when Sun was in 29 degrees
		///in current sign (just about to leave sign)
		///
		/// Note:
		/// -2 possible ways leaving time is calculated
		///     1. degrees Sun is in sign is more than 29.999 degrees (very very close to leaving sign)
		///     2. accuracy limit is hit
		///</summary>
		[API("Time when Sun was just about to leave sign (29.99 degree)", Category.StarsAboveMe)]
		public static Time TimeSunLeavesCurrentSign(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetTimeSunLeavesCurrentSign", time), _getTimeSunLeavesCurrentSign);


			//UNDERLYING FUNCTION
			Time _getTimeSunLeavesCurrentSign()
			{

				//set the maximum accuracy used to calculate time sun will leave the sign
				//once this limit is hit, the previously calculated time will be returned
				double AccuracyLimit = TimePreset.Minute3;

				//set time increment accuracy at 96 hours (4 days) at first
				double timeIncrementAccuracy = 96;

				//set input time as possible leaving time at first
				var possibleLeavingTime = time;
				var previousPossibleLeavingTime = time;

				//get current sun sign
				var currentSunSign = SunSign(time);

				//find leaving time
				while (true)
				{
					//get the sign at possible leaving time
					var possibleSunSign = SunSign(possibleLeavingTime);

					//if possible sign name is same as current sign name, then check if sun is about to leave sign
					var signNameIsSame = possibleSunSign.GetSignName() == currentSunSign.GetSignName();
					if (signNameIsSame)
					{
						//if sun sign is more than 29.9 degrees, leaving time found
						if (possibleSunSign.GetDegreesInSign().TotalDegrees > 29.999) { break; }

						//else sun not yet torward the end of the sign, so increment time
						else
						{
							//back up possible leaving time before changing
							previousPossibleLeavingTime = possibleLeavingTime;

							//increment leaving time, to check next possible time
							possibleLeavingTime = possibleLeavingTime.AddHours(timeIncrementAccuracy);
						}
					}
					//else sun sign is not same, went to far, go back a little in time
					else
					{
						//restore possible leaving time to previous time
						possibleLeavingTime = previousPossibleLeavingTime;

						//if accuracy limit is hit, then use previous time as answer, stop looking
						if (timeIncrementAccuracy <= AccuracyLimit) { break; }

						//decrease time increment accuracy by half
						timeIncrementAccuracy = timeIncrementAccuracy / 2;

					}
				}

				//return possible leaving time
				return possibleLeavingTime;

			}

		}

		/// <summary>
		/// Calculates & creates all houses as list
		/// </summary>
		[API("Calculates & creates all houses data as list", Category.Astrology)]
		public static List<House> AllHouseLongitudes(Time time)
		{
			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey(nameof(AllHouseLongitudes), time, YearOfCoincidence), _getHouses);


			//UNDERLYING FUNCTION
			List<House> _getHouses()
			{
				//declare house longitudes
				Angle house1BeginLongitude, house1MiddleLongitude, house1EndLongitude;
				Angle house2BeginLongitude, house2MiddleLongitude, house2EndLongitude;
				Angle house3BeginLongitude, house3MiddleLongitude, house3EndLongitude;
				Angle house4BeginLongitude, house4MiddleLongitude, house4EndLongitude;
				Angle house5BeginLongitude, house5MiddleLongitude, house5EndLongitude;
				Angle house6BeginLongitude, house6MiddleLongitude, house6EndLongitude;
				Angle house7BeginLongitude, house7MiddleLongitude, house7EndLongitude;
				Angle house8BeginLongitude, house8MiddleLongitude, house8EndLongitude;
				Angle house9BeginLongitude, house9MiddleLongitude, house9EndLongitude;
				Angle house10BeginLongitude, house10MiddleLongitude, house10EndLongitude;
				Angle house11BeginLongitude, house11MiddleLongitude, house11EndLongitude;
				Angle house12BeginLongitude, house12MiddleLongitude, house12EndLongitude;


				//1.Get middle longitudes of angular houses

				//1.1 get House 1 & 10

				//Get western 1 & 10 house longitudes
				var cusps = GetHouse1And10Longitudes(time);

				//Get Sayana Long. of cusp of ascend.
				var sayanaCuspOfHouse1 = Angle.FromDegrees(cusps[1]);

				//Get Sayana Long. of cusp of tenth-house
				var sayanaCuspOfHouse10 = Angle.FromDegrees(cusps[10]);

				//Deduct from these two, the Ayanamsa to get the Nirayana longitudes
				// of Udaya Lagna (Ascendant) and the Madhya Lagna (Upper Meridian)
				var ayanamsa = Ayanamsa(time);

				var udayaLagna = sayanaCuspOfHouse1 - ayanamsa;
				var madhyaLagna = sayanaCuspOfHouse10 - ayanamsa;

				//Add 180° to each of these two, to get the Nirayana Asta Lagna (Western Horizon)
				//and the Pathala Lagna (Lower Meridian)
				var astaLagna = udayaLagna + Angle.Degrees180;
				var pathalaLagna = madhyaLagna + Angle.Degrees180;

				//if longitude is more than 360°, expunge 360°
				astaLagna = astaLagna.Expunge360();
				pathalaLagna = pathalaLagna.Expunge360();

				//assign angular house middle longitudes, houses 1,4,7,10
				house1MiddleLongitude = udayaLagna;
				house4MiddleLongitude = pathalaLagna;
				house7MiddleLongitude = astaLagna;
				house10MiddleLongitude = madhyaLagna;

				//2.0 Get middle longitudes of non-angular houses
				//2.1 Calculate arcs
				Angle arcA, arcB, arcC, arcD;

				//calculate Arc A
				if (house4MiddleLongitude < house1MiddleLongitude)
				{
					arcA = ((house4MiddleLongitude + Angle.Degrees360) - house1MiddleLongitude);
				}
				else
				{
					arcA = (house4MiddleLongitude - house1MiddleLongitude);
				}

				//calculate Arc B
				if (house7MiddleLongitude < house4MiddleLongitude)
				{
					arcB = ((house7MiddleLongitude + Angle.Degrees360) - house4MiddleLongitude);
				}
				else
				{
					arcB = (house7MiddleLongitude - house4MiddleLongitude);
				}

				//calculate Arc C
				if (house10MiddleLongitude < house7MiddleLongitude)
				{
					arcC = ((house10MiddleLongitude + Angle.Degrees360) - house7MiddleLongitude);
				}
				else
				{
					arcC = (house10MiddleLongitude - house7MiddleLongitude);
				}

				//calculate Arc D
				if (house1MiddleLongitude < house10MiddleLongitude)
				{
					arcD = ((house1MiddleLongitude + Angle.Degrees360) - house10MiddleLongitude);
				}
				else
				{
					arcD = (house1MiddleLongitude - house10MiddleLongitude);
				}

				//2.2 Trisect each arc
				//Cacl House 2 & 3
				house2MiddleLongitude = house1MiddleLongitude + arcA.Divide(3);
				house2MiddleLongitude = house2MiddleLongitude.Expunge360();
				house3MiddleLongitude = house2MiddleLongitude + arcA.Divide(3);
				house3MiddleLongitude = house3MiddleLongitude.Expunge360();

				//Cacl House 5 & 6
				house5MiddleLongitude = house4MiddleLongitude + arcB.Divide(3);
				house5MiddleLongitude = house5MiddleLongitude.Expunge360();
				house6MiddleLongitude = house5MiddleLongitude + arcB.Divide(3);
				house6MiddleLongitude = house6MiddleLongitude.Expunge360();

				//Cacl House 8 & 9
				house8MiddleLongitude = house7MiddleLongitude + arcC.Divide(3);
				house8MiddleLongitude = house8MiddleLongitude.Expunge360();
				house9MiddleLongitude = house8MiddleLongitude + arcC.Divide(3);
				house9MiddleLongitude = house9MiddleLongitude.Expunge360();

				//Cacl House 11 & 12
				house11MiddleLongitude = house10MiddleLongitude + arcD.Divide(3);
				house11MiddleLongitude = house11MiddleLongitude.Expunge360();
				house12MiddleLongitude = house11MiddleLongitude + arcD.Divide(3);
				house12MiddleLongitude = house12MiddleLongitude.Expunge360();

				//3.0 Calculate house begin & end longitudes

				house1EndLongitude = house2BeginLongitude = HouseJunctionPoint(house1MiddleLongitude, house2MiddleLongitude);
				house2EndLongitude = house3BeginLongitude = HouseJunctionPoint(house2MiddleLongitude, house3MiddleLongitude);
				house3EndLongitude = house4BeginLongitude = HouseJunctionPoint(house3MiddleLongitude, house4MiddleLongitude);
				house4EndLongitude = house5BeginLongitude = HouseJunctionPoint(house4MiddleLongitude, house5MiddleLongitude);
				house5EndLongitude = house6BeginLongitude = HouseJunctionPoint(house5MiddleLongitude, house6MiddleLongitude);
				house6EndLongitude = house7BeginLongitude = HouseJunctionPoint(house6MiddleLongitude, house7MiddleLongitude);
				house7EndLongitude = house8BeginLongitude = HouseJunctionPoint(house7MiddleLongitude, house8MiddleLongitude);
				house8EndLongitude = house9BeginLongitude = HouseJunctionPoint(house8MiddleLongitude, house9MiddleLongitude);
				house9EndLongitude = house10BeginLongitude = HouseJunctionPoint(house9MiddleLongitude, house10MiddleLongitude);
				house10EndLongitude = house11BeginLongitude = HouseJunctionPoint(house10MiddleLongitude, house11MiddleLongitude);
				house11EndLongitude = house12BeginLongitude = HouseJunctionPoint(house11MiddleLongitude, house12MiddleLongitude);
				house12EndLongitude = house1BeginLongitude = HouseJunctionPoint(house12MiddleLongitude, house1MiddleLongitude);

				//4.0 Initialize houses into list
				var houseList = new List<House>();

				houseList.Add(new House(HouseName.House1, house1BeginLongitude, house1MiddleLongitude, house1EndLongitude));
				houseList.Add(new House(HouseName.House2, house2BeginLongitude, house2MiddleLongitude, house2EndLongitude));
				houseList.Add(new House(HouseName.House3, house3BeginLongitude, house3MiddleLongitude, house3EndLongitude));
				houseList.Add(new House(HouseName.House4, house4BeginLongitude, house4MiddleLongitude, house4EndLongitude));
				houseList.Add(new House(HouseName.House5, house5BeginLongitude, house5MiddleLongitude, house5EndLongitude));
				houseList.Add(new House(HouseName.House6, house6BeginLongitude, house6MiddleLongitude, house6EndLongitude));
				houseList.Add(new House(HouseName.House7, house7BeginLongitude, house7MiddleLongitude, house7EndLongitude));
				houseList.Add(new House(HouseName.House8, house8BeginLongitude, house8MiddleLongitude, house8EndLongitude));
				houseList.Add(new House(HouseName.House9, house9BeginLongitude, house9MiddleLongitude, house9EndLongitude));
				houseList.Add(new House(HouseName.House10, house10BeginLongitude, house10MiddleLongitude, house10EndLongitude));
				houseList.Add(new House(HouseName.House11, house11BeginLongitude, house11MiddleLongitude, house11EndLongitude));
				houseList.Add(new House(HouseName.House12, house12BeginLongitude, house12MiddleLongitude, house12EndLongitude));


				return houseList;

			}



		}

		[API("Convert LMT to Julian Days used in Swiss Ephemeris")]
		public static double ConvertLmtToJulian(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey(nameof(ConvertLmtToJulian), time), _convertLmtToJulian);


			//UNDERLYING FUNCTION
			double _convertLmtToJulian()
			{
				//get lmt time
				DateTimeOffset lmtDateTime = time.GetLmtDateTimeOffset();

				//split lmt time to pieces
				int year = lmtDateTime.Year;
				int month = lmtDateTime.Month;
				int day = lmtDateTime.Day;
				double hour = (lmtDateTime.TimeOfDay).TotalHours;

				//set calender type
				int gregflag = SwissEph.SE_GREG_CAL; //GREGORIAN CALENDAR

				//declare output variables
				double localMeanTimeInJulian_UT;

				//initialize ephemeris
				SwissEph ephemeris = new SwissEph();

				//get lmt in julian day in Universal Time (UT)
				localMeanTimeInJulian_UT = ephemeris.swe_julday(year, month, day, hour, gregflag);//time to Julian Day

				return localMeanTimeInJulian_UT;

			}

		}

		/// <summary>
		/// Gets all the planets that are in conjunction with the inputed planet
		///
		/// Note:
		/// 1.The planet inputed is not included in return list
		/// 
		/// 2. Theory behind conjunction
		/// Conjunction :-Two heavenly bodies in the same longitude.
		///
		/// "The effect of an aspect is felt even if the planets are not
		/// exactly in the mutual distances mentioned above. Therefore
		/// a so-called orb of aspect, and this varies in each aspect is allowed."
		/// The orbs of aspects are :
		/// Conjunction = 8° degrees
		///
		/// -Planets can be in same sign but not conjunct :
		/// "There are also other variations
		/// of aspects brought about by two planets remaining in the
		/// same sign and not in conjunction but another planet occupying
		/// a trine in respect of the two."
		/// </summary>
		[API("Gets all the planets that are in conjunction with the inputed planet")]
		public static List<PlanetName> PlanetsInConjuction(Time time, PlanetName inputedPlanetName)
		{
			//set 8° degrees as max space around planet where conjunction occurs
			var conjunctionOrbMax = new Angle(8, 0, 0);

			//get longitude of inputed planet
			var inputedPlanet = PlanetNirayanaLongitude(time, inputedPlanetName);

			//get all planet longitudes
			List<PlanetLongitude> allPlanetLongitudeList = AllPlanetLongitude(time);

			//a place to store conjunct planets 
			var conjunctPlanets = new List<PlanetName>();

			//loop through each planet
			foreach (var planet in allPlanetLongitudeList)
			{
				//skip the inputed planet
				if (planet.GetPlanetName() == inputedPlanetName) { continue; }

				//get the space between the planet in longitude
				var spaceBetween = DistanceBetweenPlanets(inputedPlanet, planet.GetPlanetLongitude());

				//if space between is from 0° to 8°, then it is conjunct
				if (spaceBetween >= Angle.Zero && spaceBetween <= conjunctionOrbMax)
				{
					conjunctPlanets.Add(planet.GetPlanetName());
				}

			}

			//return list
			return conjunctPlanets;
		}

		/// <summary>
		/// Gets longitudinal space between 2 planets
		/// Note :
		/// - Longitude of planet after 360 is 0 degrees,
		///   when calculating difference this needs to be accounted for.
		/// - Calculation in Nirayana longitudes
		/// - Calculates longitudes for you
		/// </summary>
		[API("Gets longitudinal space between 2 planets")]
		public static Angle DistanceBetweenPlanets(PlanetName planet1, PlanetName planet2, Time time)
		{
			var planet1Longitude = PlanetNirayanaLongitude(time, planet1);
			var planet2Longitude = PlanetNirayanaLongitude(time, planet2);

			var distanceBetweenPlanets = planetDistance(planet1Longitude.TotalDegrees, planet2Longitude.TotalDegrees);

			return Angle.FromDegrees(distanceBetweenPlanets);



			//---------------FUNCTION---------------


			double planetDistance(double len1, double len2)
			{
				double d = red_deg(Math.Abs(len2 - len1));

				if (d > 180) return (360 - d);

				return d;
			}

			//Reduces a given double value modulo 360.
			//The return value is between 0 and 360.
			double red_deg(double input) => a_red(input, 360);

			//Reduces a given double value x modulo the double a(should be positive).
			//The return value is between 0 and a.
			double a_red(double x, double a) => (x - Math.Floor(x / a) * a);

		}

		/// <summary>
		/// Gets longitudinal space between 2 planets
		/// Note :
		/// - Longitude of planet after 360 is 0 degrees,
		///   when calculating difference this needs to be accounted for
		/// - Expects you to calculate longitude
		/// </summary>
		[API("Gets longitudinal space between 2 planets")]
		public static Angle DistanceBetweenPlanets(Angle planet1, Angle planet2)
		{

			var distanceBetweenPlanets = planetDistance(planet1.TotalDegrees, planet2.TotalDegrees);

			return Angle.FromDegrees(distanceBetweenPlanets);



			//---------------FUNCTION---------------


			double planetDistance(double len1, double len2)
			{
				double d = red_deg(Math.Abs(len2 - len1));

				if (d > 180) return (360 - d);

				return d;
			}

			//Reduces a given double value modulo 360.
			//The return value is between 0 and 360.
			double red_deg(double input) => a_red(input, 360);

			//Reduces a given double value x modulo the double a(should be positive).
			//The return value is between 0 and a.
			double a_red(double x, double a) => (x - Math.Floor(x / a) * a);

		}

		/// <summary>
		/// Gets the planets in the house
		/// </summary>
		[API("Gets list of planets that's in a house/bhava")]
		public static List<PlanetName> PlanetsInHouse(HouseName houseNumber, Time time)
		{

			//declare empty list of planets
			var listOfPlanetInHouse = new List<PlanetName>();

			//get all houses
			var houseList = AllHouseLongitudes(time);

			//find house that matches input house number
			var house = houseList.Find(h => h.GetHouseName() == houseNumber);

			//get all planet longitudes
			List<PlanetLongitude> allPlanetLongitudeList = AllPlanetLongitude(time);

			//loop through each planet in house
			foreach (var planet in allPlanetLongitudeList)
			{
				//check if planet is in house
				bool planetIsInHouse = house.IsLongitudeInHouseRange(planet.GetPlanetLongitude());

				if (planetIsInHouse)
				{
					//add to list if planet is in house
					listOfPlanetInHouse.Add(planet.GetPlanetName());
				}
			}

			//return list
			return listOfPlanetInHouse;
		}

		/// <summary>
		/// Gets longitude positions of all planets
		/// </summary>
		[API("Gets the Nirayana longitude of all 9 planets")]
		public static List<PlanetLongitude> AllPlanetLongitude(Time time)
		{

			//get longitudes of all planets
			var sunLongitude = PlanetNirayanaLongitude(time, Sun);
			var sun = new PlanetLongitude(Sun, sunLongitude);

			var moonLongitude = PlanetNirayanaLongitude(time, Moon);
			var moon = new PlanetLongitude(Moon, moonLongitude);

			var marsLongitude = PlanetNirayanaLongitude(time, Mars);
			var mars = new PlanetLongitude(Mars, marsLongitude);

			var mercuryLongitude = PlanetNirayanaLongitude(time, Mercury);
			var mercury = new PlanetLongitude(Mercury, mercuryLongitude);

			var jupiterLongitude = PlanetNirayanaLongitude(time, Jupiter);
			var jupiter = new PlanetLongitude(Jupiter, jupiterLongitude);

			var venusLongitude = PlanetNirayanaLongitude(time, Venus);
			var venus = new PlanetLongitude(Venus, venusLongitude);

			var saturnLongitude = PlanetNirayanaLongitude(time, Saturn);
			var saturn = new PlanetLongitude(Saturn, saturnLongitude);

			var rahuLongitude = PlanetNirayanaLongitude(time, Rahu);
			var rahu = new PlanetLongitude(Rahu, rahuLongitude);

			var ketuLongitude = PlanetNirayanaLongitude(time, Ketu);
			var ketu = new PlanetLongitude(Ketu, ketuLongitude);



			//add longitudes to list
			var allPlanetLongitudeList = new List<PlanetLongitude>
			{
				sun, moon, mars, mercury, jupiter, venus, saturn, ketu, rahu
			};


			//return list;
			return allPlanetLongitudeList;
		}

		/// <summary>
		/// Gets longitude positions of all planets Sayana / Fixed zodiac 
		/// </summary>
		[API("Gets longitude positions of all planets Sayana / Fixed zodiac ")]
		public static List<PlanetLongitude> AllPlanetFixedLongitude(Time time)
		{

			//get longitudes of all planets
			var sunLongitude = PlanetSayanaLongitude(time, Sun);
			var sun = new PlanetLongitude(Sun, sunLongitude);

			var moonLongitude = PlanetSayanaLongitude(time, Moon);
			var moon = new PlanetLongitude(Moon, moonLongitude);

			var marsLongitude = PlanetSayanaLongitude(time, Mars);
			var mars = new PlanetLongitude(Mars, marsLongitude);

			var mercuryLongitude = PlanetSayanaLongitude(time, Mercury);
			var mercury = new PlanetLongitude(Mercury, mercuryLongitude);

			var jupiterLongitude = PlanetSayanaLongitude(time, Jupiter);
			var jupiter = new PlanetLongitude(Jupiter, jupiterLongitude);

			var venusLongitude = PlanetSayanaLongitude(time, Venus);
			var venus = new PlanetLongitude(Venus, venusLongitude);

			var saturnLongitude = PlanetSayanaLongitude(time, Saturn);
			var saturn = new PlanetLongitude(Saturn, saturnLongitude);

			var rahuLongitude = PlanetSayanaLongitude(time, Rahu);
			var rahu = new PlanetLongitude(Rahu, rahuLongitude);

			var ketuLongitude = PlanetSayanaLongitude(time, Ketu);
			var ketu = new PlanetLongitude(Ketu, ketuLongitude);



			//add longitudes to list
			var allPlanetLongitudeList = new List<PlanetLongitude>
			{
				sun, moon, mars, mercury, jupiter, venus, saturn, ketu, rahu
			};


			//return list;
			return allPlanetLongitudeList;
		}

		[API("Gets the House number a given planet is in at a time")]
		public static HouseName HousePlanetIsIn(Time time, PlanetName planetName)
		{

			//get the planets longitude
			var planetLongitude = PlanetNirayanaLongitude(time, planetName);

			//get all houses
			var houseList = AllHouseLongitudes(time);

			//loop through all houses
			foreach (var house in houseList)
			{
				//check if planet is in house's range
				var planetIsInHouse = house.IsLongitudeInHouseRange(planetLongitude);

				//if planet is in house
				if (planetIsInHouse)
				{
					//return house's number
					return house.GetHouseName();
				}
			}

			//if planet not found in any house, raise error
			throw new Exception("Planet not in any house, error!");

		}

		/// <summary>
		/// The lord of a bhava is
		/// the Graha (planet) in whose Rasi (sign) the Bhavamadhya falls
		/// </summary>
		[API("Gets planet lord of given house at given time")]
		public static PlanetName LordOfHouse(HouseName houseNumber, Time time)
		{
			//get sign name based on house number //TODO Change to use house name instead of casting to int
			var houseSignName = HouseSignName(houseNumber, time);

			//get the lord of the house sign
			var lordOfHouseSign = LordOfZodiacSign(houseSignName);

			return lordOfHouseSign;
		}

		/// <summary>
		/// The lord of a bhava is
		/// the Graha (planet) in whose Rasi (sign) the Bhavamadhya falls
		/// List overload to GetLordOfHouse (above method)
		/// </summary>
		[API("The lord of a bhava is the Graha (planet) in whose Rasi (sign) the Bhavamadhya falls")]
		public static List<PlanetName> LordOfHouseList(List<HouseName> houseList, Time time)
		{
			var returnList = new List<PlanetName>();
			foreach (var house in houseList)
			{
				var tempLord = LordOfHouse(house, time);
				returnList.Add(tempLord);
			}

			return returnList;
		}

		/// <summary>
		/// Checks if the inputed sign was the sign of the house during the inputed time
		/// </summary>
		[API("Checks if the inputed sign was the sign of the house during the inputed time")]
		public static bool IsHouseSignName(HouseName house, ZodiacName sign, Time time) => HouseSignName(house, time) == sign;

		/// <summary>
		/// Gets the zodiac sign at middle longitude of the house.
		/// </summary>
		[API("zodiac sign at middle longitude of the house")]
		public static ZodiacName HouseSignName(HouseName houseNumber, Time time)
		{

			//get all houses
			var allHouses = AllHouseLongitudes(time);

			//get the house specified 
			var specifiedHouse = allHouses.Find(house => house.GetHouseName() == houseNumber);

			//get sign of the specified house
			//Note :
			//When the middle longitude has just entered a new sign,
			//rounding the longitude shows better accuracy.
			//Example, with middle longitude 90.4694, becomes Cancer (0°28'9"),
			//but predictive results points to Gemini (30°0'0"), so rounding is implemented
			var middleLongitude = specifiedHouse.GetMiddleLongitude();
			//todo is round needed!!
			var roundedMiddleLongitude = Angle.FromDegrees(Math.Round(middleLongitude.TotalDegrees, 4)); //rounded to 5 places for accuracy
			var houseSignName = ZodiacSignAtLongitude(roundedMiddleLongitude).GetSignName();

#if DEBUG_LOG

            //for sake of testing, if sign is changed due to rounding, then log it
            var unroundedSignName = AstronomicalCalculator.GetZodiacSignAtLongitude(middleLongitude).GetSignName();

            if (unroundedSignName != houseSignName)
                {
                    LibLogger.Debug($"Due to rounding sign changed from {unroundedSignName} to {houseSignName}");
                }
#endif


			//return the name of house sign
			return houseSignName;
		}

		/// <summary>
		/// Gets the zodiac sign at middle longitude of the house.
		/// </summary>
		[API("Gets the Constellation at middle longitude of the house")]
		public static PlanetConstellation HouseConstellation(HouseName houseNumber, Time time)
		{

			//get all houses
			var allHouses = AllHouseLongitudes(time);

			//get the house specified 
			var specifiedHouse = allHouses.Find(house => house.GetHouseName() == houseNumber);

			//get sign of the specified house
			//Note :
			//When the middle longitude has just entered a new sign,
			//rounding the longitude shows better accuracy.
			//Example, with middle longitude 90.4694, becomes Cancer (0°28'9"),
			//but predictive results points to Gemini (30°0'0"), so rounding is implemented
			var middleLongitude = specifiedHouse.GetMiddleLongitude();
			var houseConstellation = ConstellationAtLongitude(middleLongitude);

			//return the name of house sign
			return houseConstellation;
		}

		[API("Navamsa Sign Name From Longitude")]
		public static ZodiacName NavamsaSignNameFromLongitude(Angle longitude)
		{
			//1.0 Get ordinary zodiac sign name
			//get ordinary zodiac sign
			var ordinarySign = ZodiacSignAtLongitude(longitude);

			//get name of ordinary sign
			var ordinarySignName = ordinarySign.GetSignName();

			//2.0 Get first navamsa sign
			ZodiacName firstNavamsa;

			switch (ordinarySignName)
			{
				//Aries, Leo, Sagittarius - from Aries.
				case ZodiacName.Aries:
				case ZodiacName.Leo:
				case ZodiacName.Sagittarius:
					firstNavamsa = ZodiacName.Aries;
					break;
				//Taurus, Capricornus, Virgo - from Capricornus.
				case ZodiacName.Taurus:
				case ZodiacName.Capricornus:
				case ZodiacName.Virgo:
					firstNavamsa = ZodiacName.Capricornus;
					break;
				//Gemini, Libra, Aquarius - from Libra.
				case ZodiacName.Gemini:
				case ZodiacName.Libra:
				case ZodiacName.Aquarius:
					firstNavamsa = ZodiacName.Libra;
					break;
				//Cancer, Scorpio, Pisces - from Cancer.
				case ZodiacName.Cancer:
				case ZodiacName.Scorpio:
				case ZodiacName.Pisces:
					firstNavamsa = ZodiacName.Cancer;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			//3.0 Get the number of the navamsa currently in
			//get degrees in ordinary sign
			var degreesInOrdinarySign = ordinarySign.GetDegreesInSign();

			//declare length of a navamsa in the ecliptic arc
			const double navamsaLenghtInDegrees = 3.333333333;

			//divide total degrees in current sign to get raw navamsa number
			var rawNavamsaNumber = degreesInOrdinarySign.TotalDegrees / navamsaLenghtInDegrees;

			//round the raw number to get current navamsa number
			var navamsaNumber = (int)Math.Ceiling(rawNavamsaNumber);

			//4.0 Get navamsa sign
			//count from first navamsa sign
			ZodiacName signAtNavamsa = SignCountedFromInputSign(firstNavamsa, navamsaNumber);

			return signAtNavamsa;

		}

		/// <summary>
		/// Exp : Get 4th sign from Cancer
		/// </summary>
		[API("Exp : Get 4th sign from Cancer")]
		public static ZodiacName SignCountedFromInputSign(ZodiacName inputSign, int countToNextSign)
		{
			//assign counted to same as starting sign at first
			ZodiacName signCountedTo = inputSign;

			//get the next sign the same number as the count to next sign
			for (int i = 1; i < countToNextSign; i++)
			{
				//get the next zodiac sign from the current counted to sign
				signCountedTo = NextZodiacSign(signCountedTo);
			}

			return signCountedTo;

		}

		/// <summary>
		/// Exp : Get 4th house from 5th house (input house)
		/// </summary>
		[API("Exp : Get 4th house from 5th house (input house)")]
		public static int HouseCountedFromInputHouse(int inputHouseNumber, int countToNextHouse)
		{
			//assign count to same as starting house at first
			int houseCountedTo = inputHouseNumber;

			//get the next house the same number as the count to next house
			for (int i = 1; i < countToNextHouse; i++)
			{
				//get the next house number from the current counted to house
				houseCountedTo = NextHouseNumber(houseCountedTo);
			}

			return houseCountedTo;

		}

		/// <summary>
		/// Get zodiac sign planet is in.
		/// </summary>
		[API("Get zodiac sign planet is in.")]
		public static ZodiacSign PlanetRasiSign(PlanetName planetName, Time time)
		{
			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetPlanetRasiSign", planetName, time), _getPlanetRasiSign);


			//UNDERLYING FUNCTION
			ZodiacSign _getPlanetRasiSign()
			{
				//get longitude of planet
				var longitudeOfPlanet = PlanetNirayanaLongitude(time, planetName);

				//get sign planet is in
				var signPlanetIsIn = ZodiacSignAtLongitude(longitudeOfPlanet);

				//return
				return signPlanetIsIn;

			}

		}

		/// <summary>
		/// Checks if a given planet is in a given sign at a given time
		/// </summary>
		[API("IsPlanetInSign")]
		public static bool IsPlanetInSign(PlanetName planetName, ZodiacName signInput, Time time)
		{
			var currentSign = PlanetRasiSign(planetName, time).GetSignName();

			//check if sign match
			return currentSign == signInput;
		}

		/// <summary>
		/// Get navamsa sign of planet
		/// </summary>
		[API("Get navamsa sign of planet")]
		public static ZodiacName PlanetNavamsaSign(PlanetName planetName, Time time)
		{
			//get planets longitude
			var planetLongitude = PlanetNirayanaLongitude(time, planetName);

			//get navamsa sign at longitude
			var navamsaSignOfPlanet = NavamsaSignNameFromLongitude(planetLongitude);

			return navamsaSignOfPlanet;
		}

		/// <summary>
		/// All their location with a quarter sight, the 5th and the
		/// 9th houses with a half sight, the 4th and the 8th houses
		/// with three-quarters of a sight and the 7th house with
		/// a full sight.
		/// 
		/// </summary>
		[API("Gives a list of all zodiac signs a specified planet is aspecting")]
		public static List<ZodiacName> SignsPlanetIsAspecting(PlanetName planetName, Time time)
		{

			//create empty list of signs
			var planetSignList = new List<ZodiacName>();

			//get zodiac sign name which the planet is currently in
			var planetSignName = PlanetRasiSign(planetName, time).GetSignName();

			// Saturn powerfully aspects the 3rd and the 10th houses
			if (planetName == Saturn)
			{
				//get signs planet is aspecting
				var sign3FromSaturn = SignCountedFromInputSign(planetSignName, 3);
				var sign10FromSaturn = SignCountedFromInputSign(planetSignName, 10);

				//add signs to return list
				planetSignList.Add(sign3FromSaturn);
				planetSignList.Add(sign10FromSaturn);

			}

			// Jupiter the 5th and the 9th houses
			if (planetName == Jupiter)
			{
				//get signs planet is aspecting
				var sign5FromJupiter = SignCountedFromInputSign(planetSignName, 5);
				var sign9FromJupiter = SignCountedFromInputSign(planetSignName, 9);

				//add signs to return list
				planetSignList.Add(sign5FromJupiter);
				planetSignList.Add(sign9FromJupiter);

			}

			// Mars, the 4th and the 8th houses
			if (planetName == Mars)
			{
				//get signs planet is aspecting
				var sign4FromMars = SignCountedFromInputSign(planetSignName, 4);
				var sign8FromMars = SignCountedFromInputSign(planetSignName, 8);

				//add signs to return list
				planetSignList.Add(sign4FromMars);
				planetSignList.Add(sign8FromMars);

			}

			//All planets aspect 7th house

			//get signs planet is aspecting
			var sign7FromPlanet = SignCountedFromInputSign(planetSignName, 7);

			//add signs to return list
			planetSignList.Add(sign7FromPlanet);


			return planetSignList;

		}

		/// <summary>
		/// Get navamsa sign of house (mid point)
		/// TODO: Checking for correctness needed
		/// </summary>
		[API("Get navamsa sign of house (mid point)")]
		public static ZodiacName HouseNavamsaSign(HouseName house, Time time)
		{
			//get all houses
			var allHouseList = AllHouseLongitudes(time);

			//get house mid longitude
			var houseMiddleLongitude = allHouseList.Find(hs => hs.GetHouseName() == house).GetMiddleLongitude();

			//get navamsa house sign at house mid longitude
			var navamsaSign = NavamsaSignNameFromLongitude(houseMiddleLongitude);

			return navamsaSign;
		}

		[API("Get Thrimsamsa sign of house (mid point)")]
		public static ZodiacName PlanetThrimsamsaSign(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = PlanetRasiSign(planetName, time);

			//get planet sign name
			var planetSignName = planetSign.GetSignName();

			//get degrees in sign 
			var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

			//declare const number for Thrimsamsa calculation
			const double maxThrimsamsaDegrees = 1; // 30/1
			const double maxSignDegrees = 30.0;

			//get rough Thrimsamsa number
			double roughThrimsamsaNumber = (degreesInSign % maxSignDegrees) / maxThrimsamsaDegrees;

			//get rounded saptamsa number
			var thrimsamsaNumber = (int)Math.Ceiling(roughThrimsamsaNumber);

			//if planet is in odd sign
			if (IsOddSign(planetSignName))
			{
				//1,2,3,4,5 - Mars
				if (thrimsamsaNumber >= 0 && thrimsamsaNumber <= 5)
				{
					//Aries and Scorpio are ruled by Mars
					return ZodiacName.Scorpio;
				}
				//6,7,8,9,10 - saturn
				if (thrimsamsaNumber >= 6 && thrimsamsaNumber <= 10)
				{
					//Capricorn and Aquarius by Saturn.
					return ZodiacName.Capricornus;

				}
				//11,12,13,14,15,16,17,18 - jupiter
				if (thrimsamsaNumber >= 11 && thrimsamsaNumber <= 18)
				{
					//Sagittarius and Pisces by Jupiter
					return ZodiacName.Sagittarius;

				}
				//19,20,21,22,23,24,25 - mercury
				if (thrimsamsaNumber >= 19 && thrimsamsaNumber <= 25)
				{
					//Gemini and Virgo by Mercury
					return ZodiacName.Gemini;
				}
				//26,27,28,29,30 - venus
				if (thrimsamsaNumber >= 26 && thrimsamsaNumber <= 30)
				{
					//Taurus and Libra by Venus;
					return ZodiacName.Taurus;
				}


			}

			//if planet is in even sign
			if (IsEvenSign(planetSignName))
			{
				//1,2,3,4,5 - venus
				if (thrimsamsaNumber >= 0 && thrimsamsaNumber <= 5)
				{
					//Taurus and Libra by Venus;
					return ZodiacName.Taurus;
				}
				//6,7,8,9,10,11,12 - mercury
				if (thrimsamsaNumber >= 6 && thrimsamsaNumber <= 12)
				{
					//Gemini and Virgo by Mercury
					return ZodiacName.Gemini;
				}
				//13,14,15,16,17,18,19,20 - jupiter
				if (thrimsamsaNumber >= 13 && thrimsamsaNumber <= 20)
				{
					//Sagittarius and Pisces by Jupiter
					return ZodiacName.Sagittarius;

				}
				//21,22,23,24,25 - saturn
				if (thrimsamsaNumber >= 21 && thrimsamsaNumber <= 25)
				{
					//Capricorn and Aquarius by Saturn.
					return ZodiacName.Capricornus;

				}
				//26,27,28,29,30 - Mars
				if (thrimsamsaNumber >= 26 && thrimsamsaNumber <= 30)
				{
					//Aries and Scorpio are ruled by Mars
					return ZodiacName.Scorpio;
				}

			}

			throw new Exception("Thrimsamsa not found, error!");
		}

		/// <summary>
		/// When a sign is divided into 12 equal parts each is called a dwadasamsa and measures 2.5 degrees.
		/// The Bhachakra can thus he said to contain 12x12=144 Dwadasamsas. The lords of the 12
		/// Dwadasamsas in a sign are the lords of the 12 signs from it, i.e.,
		/// the lord of the first Dwadasamsa in Mesha is Kuja, that of the second Sukra and so on.
		/// </summary>
		[API("sign is divided into 12 equal parts each is called a dwadasamsa and measures 2.5 degrees")]
		public static ZodiacName PlanetDwadasamsaSign(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = PlanetRasiSign(planetName, time);

			//get planet sign name
			var planetSignName = planetSign.GetSignName();

			//get degrees in sign 
			var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

			//declare const number for Dwadasamsa calculation
			const double maxDwadasamsaDegrees = 2.5; // 30/12
			const double maxSignDegrees = 30.0;

			//get rough Dwadasamsa number
			double roughDwadasamsaNumber = (degreesInSign % maxSignDegrees) / maxDwadasamsaDegrees;

			//get rounded Dwadasamsa number
			var dwadasamsaNumber = (int)Math.Ceiling(roughDwadasamsaNumber);

			//get Dwadasamsa sign from counting with Dwadasamsa number
			var dwadasamsaSign = SignCountedFromInputSign(planetSignName, dwadasamsaNumber);

			return dwadasamsaSign;
		}

		[API("sign is divided into 7 equal parts each is called a Saptamsa and measures 4.28 degrees")]
		public static ZodiacName PlanetSaptamsaSign(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = PlanetRasiSign(planetName, time);

			//get planet sign name
			var planetSignName = planetSign.GetSignName();

			//get degrees in sign 
			var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

			//declare const number for saptamsa calculation
			const double maxSaptamsaDegrees = 4.285714285714286; // 30/7
			const double maxSignDegrees = 30.0;

			//get rough saptamsa number
			double roughSaptamsaNumber = (degreesInSign % maxSignDegrees) / maxSaptamsaDegrees;

			//get rounded saptamsa number
			var saptamsaNumber = (int)Math.Ceiling(roughSaptamsaNumber);

			//2.0 Get even or odd sign

			//if planet is in odd sign
			if (IsOddSign(planetSignName))
			{
				//convert saptamsa number to zodiac name
				return SignCountedFromInputSign(planetSignName, saptamsaNumber);
			}

			//if planet is in even sign
			if (IsEvenSign(planetSignName))
			{
				var countToNextSign = saptamsaNumber + 6;
				return SignCountedFromInputSign(planetSignName, countToNextSign);
			}


			throw new Exception("Saptamsa not found, error!");
		}

		[API("Gets the Drekkana sign the planet is in")]
		public static ZodiacName PlanetDrekkanaSign(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = PlanetRasiSign(planetName, time);

			//get planet sign name
			var planetSignName = planetSign.GetSignName();

			//get degrees in sign 
			var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

			//1.0 get the number of the drekkana the planet is in

			//if planet is in 1st drekkana
			if (degreesInSign >= 0 && degreesInSign <= 10)
			{
				//return planet's current sign
				return planetSignName;
			}

			//if planet is in 2nd drekkana
			if (degreesInSign > 10 && degreesInSign <= 20)
			{
				//return 5th sign from planets current sign
				return SignCountedFromInputSign(planetSignName, 5);
			}

			//if planet is in 3rd drekkana
			if (degreesInSign > 20 && degreesInSign <= 30)
			{
				//return 9th sign from planets current sign
				return SignCountedFromInputSign(planetSignName, 9);
			}

			throw new Exception("Planet drekkana not found, error!");
		}

		/// <summary>
		/// Similar to Exaltation but covers a range not just a point
		/// Moolathrikonas, these are positions similar to exaltation.
		/// NOTE:
		/// - No moolatrikone for Rahu & Ketu, no error will be raised
		/// </summary>
		[API("Similar to Exaltation but covers a range not just a point")]
		public static bool IsPlanetInMoolatrikona(PlanetName planetName, Time time)
		{
			//get sign planet is in
			var planetSign = PlanetRasiSign(planetName, time);

			//Sun's Moola Thrikona is Leo (0°-20°);
			if (planetName == Sun)
			{
				if (planetSign.GetSignName() == ZodiacName.Leo)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 0 && degreesInSign <= 20)
					{
						return true;
					}
				}
			}

			//Moon-Taurus (4°-30°);
			if (planetName == Moon)
			{
				if (planetSign.GetSignName() == ZodiacName.Taurus)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 4 && degreesInSign <= 30)
					{
						return true;
					}
				}
			}

			//Mercury-Virgo (16°-20°);
			if (planetName == Mercury)
			{
				if (planetSign.GetSignName() == ZodiacName.Virgo)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 16 && degreesInSign <= 20)
					{
						return true;
					}
				}
			}

			//Jupiter-Sagittarius (0°-13°);
			if (planetName == Jupiter)
			{
				if (planetSign.GetSignName() == ZodiacName.Sagittarius)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 0 && degreesInSign <= 13)
					{
						return true;
					}
				}
			}

			// Mars-Aries (0°-18°);
			if (planetName == Mars)
			{
				if (planetSign.GetSignName() == ZodiacName.Aries)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 0 && degreesInSign <= 18)
					{
						return true;
					}
				}
			}

			// Venus-Libra (0°-10°)
			if (planetName == Venus)
			{
				if (planetSign.GetSignName() == ZodiacName.Libra)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 0 && degreesInSign <= 10)
					{
						return true;
					}
				}
			}

			// Saturn-Aquarius (0°-20°).
			if (planetName == Saturn)
			{
				if (planetSign.GetSignName() == ZodiacName.Aquarius)
				{
					var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

					if (degreesInSign >= 0 && degreesInSign <= 20)
					{
						return true;
					}
				}
			}

			//if no above conditions met, moolatrikonas not happening 
			return false;
		}

		/// <summary>
		/// Gets a planet's relationship to a sign, based on the relation to the lord
		/// Note :
		/// - Moolatrikona, Debilited & Exalted is not calculated heres
		/// - Rahu & ketu not accounted for
		/// </summary>
		[API("Gets a planets relationship with a sign, friend, enemy, etc.")]
		public static PlanetToSignRelationship PlanetRelationshipWithSign(PlanetName planetName, ZodiacName zodiacSignName, Time time)
		{

			//no calculation for rahu and ketu here
			var isRahu = planetName.Name == PlanetNameEnum.Rahu;
			var isKetu = planetName.Name == PlanetNameEnum.Ketu;
			var isRahuKetu = isRahu || isKetu;
			if (isRahuKetu) { return PlanetToSignRelationship.Empty; }


			//types of relationship
			//Swavarga - own varga
			//Samavarga - neutral's varga
			//Mitravarga - friendly varga
			//Adhi Mitravarga - Intimate friend varga
			//Satruvarga - enemy's varga
			//Adhi Satruvarga - Bitter enemy varga


			//Get lord of zodiac sign
			var lordOfSign = LordOfZodiacSign(zodiacSignName);

			//if lord of sign is same as input planet
			if (planetName == lordOfSign)
			{
				//return own varga, swavarga
				return PlanetToSignRelationship.OwnVarga;
			}

			//else, get relationship between input planet and lord of sign
			PlanetToPlanetRelationship relationshipToLordOfSign = PlanetCombinedRelationshipWithPlanet(planetName, lordOfSign, time);

			//return relation ship with sign based on relationship with lord of sign
			switch (relationshipToLordOfSign)
			{
				case PlanetToPlanetRelationship.BestFriend:
					return PlanetToSignRelationship.BestFriendVarga;
				case PlanetToPlanetRelationship.Friend:
					return PlanetToSignRelationship.FriendVarga;
				case PlanetToPlanetRelationship.BitterEnemy:
					return PlanetToSignRelationship.BitterEnemyVarga;
				case PlanetToPlanetRelationship.Enemy:
					return PlanetToSignRelationship.EnemyVarga;
				case PlanetToPlanetRelationship.Neutral:
					return PlanetToSignRelationship.NeutralVarga;
				default:
					throw new ArgumentOutOfRangeException();
			}

		}

		/// <summary>
		/// In order to find the strengths of planets we have
		/// to mix the temporary relations and the permanent
		/// relations. Thus a temporary enemy plus a permanent
		/// or natural enemy becomes a bitter enemy.
		/// </summary>
		[API("strengths of planets, mix the temporary relations and the permanent")]
		public static PlanetToPlanetRelationship PlanetCombinedRelationshipWithPlanet(PlanetName mainPlanet, PlanetName secondaryPlanet, Time time)
		{

			//no calculation for rahu and ketu here
			var isRahu = mainPlanet.Name == PlanetNameEnum.Rahu;
			var isKetu = mainPlanet.Name == PlanetNameEnum.Ketu;
			var isRahu2 = secondaryPlanet.Name == PlanetNameEnum.Rahu;
			var isKetu2 = secondaryPlanet.Name == PlanetNameEnum.Ketu;
			var isRahuKetu = isRahu || isKetu || isRahu2 || isKetu2;
			if (isRahuKetu) { return PlanetToPlanetRelationship.Empty; }


			//if main planet & secondary planet is same, then it is own plant (same planet), end here
			if (mainPlanet == secondaryPlanet) { return PlanetToPlanetRelationship.SamePlanet; }

			//get planet's permanent relationship
			PlanetToPlanetRelationship planetPermanentRelationship = PlanetPermanentRelationshipWithPlanet(mainPlanet, secondaryPlanet);

			//get planet's temporary relationship
			PlanetToPlanetRelationship planetTemporaryRelationship = PlanetTemporaryRelationshipWithPlanet(mainPlanet, secondaryPlanet, time);

			//Tatkalika Mitra + Naisargika Mitra = Adhi Mitras
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Friend && planetPermanentRelationship == PlanetToPlanetRelationship.Friend)
			{
				//they both become intimate friends (Adhi Mitras).
				return PlanetToPlanetRelationship.BestFriend;
			}

			//Tatkalika Mitra + Naisargika Satru = Sama
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Friend && planetPermanentRelationship == PlanetToPlanetRelationship.Enemy)
			{
				return PlanetToPlanetRelationship.Neutral;
			}

			//Tatkalika Mitra + Naisargika Sama = Mitra
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Friend && planetPermanentRelationship == PlanetToPlanetRelationship.Neutral)
			{
				return PlanetToPlanetRelationship.Friend;
			}

			//Tatkalika Satru + Naisargika Satru = Adhi Satru
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Enemy && planetPermanentRelationship == PlanetToPlanetRelationship.Enemy)
			{
				return PlanetToPlanetRelationship.BitterEnemy;
			}

			//Tatkalika Satru + Naisargika Mitra = Sama
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Enemy && planetPermanentRelationship == PlanetToPlanetRelationship.Friend)
			{
				return PlanetToPlanetRelationship.Neutral;
			}

			//Tatkalika Satru + Naisargika Sama = Satru
			if (planetTemporaryRelationship == PlanetToPlanetRelationship.Enemy && planetPermanentRelationship == PlanetToPlanetRelationship.Neutral)
			{
				return PlanetToPlanetRelationship.Enemy;
			}

			throw new Exception("Combined planet relationship not found, error!");
		}

		/// <summary>
		/// Gets a planets relationship with a house,
		/// Based on the relation between the planet and the lord of the sign of the house
		/// Note : needs verification if this is correct
		/// </summary>
		[API("Relation between the planet and the lord of the sign of the house")]
		public static PlanetToSignRelationship PlanetRelationshipWithHouse(HouseName house, PlanetName planet, Time time)
		{
			//get sign the house is in
			var houseSign = HouseSignName(house, time);

			//get the planet's relationship with the sign
			var relationship = PlanetRelationshipWithSign(planet, houseSign, time);

			return relationship;
		}

		/// <summary>
		/// Temporary Friendship
		/// Planets found in the 2nd, 3rd, 4th, 10th, 11th
		/// and 12th signs from any other planet becomes the
		/// latter's temporary friends. The others are its enemies.
		/// </summary>
		[API("Planets found in the certain signs from any other planet becomes temporary friends")]
		public static PlanetToPlanetRelationship PlanetTemporaryRelationshipWithPlanet(PlanetName mainPlanet, PlanetName secondaryPlanet, Time time)
		{
			//if main planet & secondary planet is same, then it is own plant (same planet), end here
			if (mainPlanet == secondaryPlanet) { return PlanetToPlanetRelationship.SamePlanet; }


			//1.0 get planet's friends
			var friendlyPlanetList = PlanetTemporaryFriendList(mainPlanet, time);

			//check if planet is found in friend list
			var planetFoundInFriendList = friendlyPlanetList.Contains(secondaryPlanet);

			//if found in friend list
			if (planetFoundInFriendList)
			{
				//return relationship as friend
				return PlanetToPlanetRelationship.Friend;
			}

			//if planet is not a friend then it is an enemy
			//return relationship as enemy
			return PlanetToPlanetRelationship.Enemy;
		}

		//public static List<PlanetName> GetPlanetTemporaryEnemyList(PlanetName planetName, Time time)
		//{
		//    //Enemy houses 1,5,6,7,8,9

		//    //get house planet is currently in
		//    var mainPlanetHouseNumber = AstronomicalCalculator.GetHousePlanetIsIn(time, planetName);

		//    //Get houses of enemies of main planet
		//    //get planets in 1
		//    var house1FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 1);
		//    //get planets in 5
		//    var house5FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 5);
		//    //get planets in 6
		//    var house6FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 6);
		//    //get planets in 7
		//    var house7FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 7);
		//    //get planets in 8
		//    var house8FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 8);
		//    //get planets in 9
		//    var house9FromMainPlanet = AstronomicalCalculator.GetHouseCountedFromInputHouse(mainPlanetHouseNumber, 9);

		//    //add houses of friendly planets to a list
		//    var housesOfEnemyPlanet = new List<int>(){house1FromMainPlanet, house5FromMainPlanet, house6FromMainPlanet,
		//                                                house7FromMainPlanet, house8FromMainPlanet, house9FromMainPlanet};

		//    //declare list of enemy planets
		//    var enemyPlanetList = new List<PlanetName>();

		//    //loop through the houses and fill the enemy planet list
		//    foreach (var house in housesOfEnemyPlanet)
		//    {
		//        //get the planets in the current house
		//        var enemyPlanetsInThisHouse = AstronomicalCalculator.GetPlanetsInHouse(house, time);

		//        //add the planets in to the list
		//        enemyPlanetList.AddRange(enemyPlanetsInThisHouse);
		//    }

		//    //remove rahu & ketu from list
		//    enemyPlanetList.Remove(PlanetName.Rahu);
		//    enemyPlanetList.Remove(PlanetName.Ketu);


		//    return enemyPlanetList;

		//}

		//public static List<PlanetName> GetPlanetTemporaryEnemyList(PlanetName planetName, Time time)
		//{
		//    //Signs where enemy planets are located 1,5,6,7,8,9

		//    //get sign planet is currently in
		//    var planetSignName = AstronomicalCalculator.GetPlanetRasiSign(planetName, time).GetSignName();

		//    //Get signs of enemies of main planet
		//    //get planets in 1
		//    var sign1FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 1);
		//    //get planets in 5
		//    var sign5FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 5);
		//    //get planets in 6
		//    var sign6FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 6);
		//    //get planets in 7
		//    var sign7FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 7);
		//    //get planets in 8
		//    var sign8FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 8);
		//    //get planets in 9
		//    var sign9FromMainPlanet = AstronomicalCalculator.GetSignCountedFromInputSign(planetSignName, 9);

		//    //add signs of enemy planets to a list
		//    var signsOfEnemyPlanet = new List<ZodiacName>(){sign1FromMainPlanet, sign5FromMainPlanet, sign6FromMainPlanet,
		//                                                sign7FromMainPlanet, sign8FromMainPlanet, sign9FromMainPlanet};

		//    //declare list of enemy planets
		//    var enemyPlanetList = new List<PlanetName>();

		//    //loop through the signs and fill the enemy planet list
		//    foreach (var sign in signsOfEnemyPlanet)
		//    {
		//        //get the planets in the current sign
		//        var enemyPlanetsInThisSign = AstronomicalCalculator.GetPlanetInSign(sign, time);

		//        //add the planets in to the list
		//        enemyPlanetList.AddRange(enemyPlanetsInThisSign);
		//    }


		//    //remove rahu & ketu from list
		//    enemyPlanetList.Remove(PlanetName.Rahu);
		//    enemyPlanetList.Remove(PlanetName.Ketu);

		//    //remove the main planet from list
		//    enemyPlanetList.Remove(planetName);


		//    return enemyPlanetList;

		//}

		/// <summary>
		/// Gets all the planets in a sign
		/// </summary>
		[API("Gets all the planets in a sign")]
		public static List<PlanetName> PlanetInSign(ZodiacName signName, Time time)
		{
			//get all planets locations in signs
			var sunSignName = PlanetRasiSign(Sun, time).GetSignName();
			var moonSignName = PlanetRasiSign(Moon, time).GetSignName();
			var marsSignName = PlanetRasiSign(Mars, time).GetSignName();
			var mercurySignName = PlanetRasiSign(Mercury, time).GetSignName();
			var jupiterSignName = PlanetRasiSign(Jupiter, time).GetSignName();
			var venusSignName = PlanetRasiSign(Venus, time).GetSignName();
			var saturnSignName = PlanetRasiSign(Saturn, time).GetSignName();
			var rahuSignName = PlanetRasiSign(Rahu, time).GetSignName();
			var ketuSignName = PlanetRasiSign(Ketu, time).GetSignName();


			//create empty list of planet names to return
			var planetFoundInSign = new List<PlanetName>();

			//if planet is in same sign as input sign add planet to list
			if (sunSignName == signName)
			{
				planetFoundInSign.Add(Sun);
			}
			if (moonSignName == signName)
			{
				planetFoundInSign.Add(Moon);
			}
			if (marsSignName == signName)
			{
				planetFoundInSign.Add(Mars);
			}
			if (mercurySignName == signName)
			{
				planetFoundInSign.Add(Mercury);
			}
			if (jupiterSignName == signName)
			{
				planetFoundInSign.Add(Jupiter);
			}
			if (venusSignName == signName)
			{
				planetFoundInSign.Add(Venus);
			}
			if (saturnSignName == signName)
			{
				planetFoundInSign.Add(Saturn);
			}
			if (rahuSignName == signName)
			{
				planetFoundInSign.Add(Rahu);
			}
			if (ketuSignName == signName)
			{
				planetFoundInSign.Add(Ketu);
			}


			return planetFoundInSign;
		}

		/// <summary>
		/// The planets in -the 2nd, 3rd, 4th, 10th, 11th and
		/// 12th signs from any other planet becomes his
		/// (Tatkalika) friend.
		/// </summary>
		[API("Get list of Temporary (Tatkalika) Friend for a planet")]
		public static List<PlanetName> PlanetTemporaryFriendList(PlanetName planetName, Time time)
		{
			//get sign planet is currently in
			var planetSignName = PlanetRasiSign(planetName, time).GetSignName();

			//Get signs of friends of main planet
			//get planets in 2nd
			var sign2FromMainPlanet = SignCountedFromInputSign(planetSignName, 2);
			//get planets in 3rd
			var sign3FromMainPlanet = SignCountedFromInputSign(planetSignName, 3);
			//get planets in 4th
			var sign4FromMainPlanet = SignCountedFromInputSign(planetSignName, 4);
			//get planets in 10th
			var sign10FromMainPlanet = SignCountedFromInputSign(planetSignName, 10);
			//get planets in 11th
			var sign11FromMainPlanet = SignCountedFromInputSign(planetSignName, 11);
			//get planets in 12th
			var sign12FromMainPlanet = SignCountedFromInputSign(planetSignName, 12);

			//add houses of friendly planets to a list
			var signsOfFriendlyPlanet = new List<ZodiacName>(){sign2FromMainPlanet, sign3FromMainPlanet, sign4FromMainPlanet,
														sign10FromMainPlanet, sign11FromMainPlanet, sign12FromMainPlanet};

			//declare list of friendly planets
			var friendlyPlanetList = new List<PlanetName>();

			//loop through the signs and fill the friendly planet list
			foreach (var sign in signsOfFriendlyPlanet)
			{
				//get the planets in the current sign
				var friendlyPlanetsInThisSign = PlanetInSign(sign, time);

				//add the planets in to the list
				friendlyPlanetList.AddRange(friendlyPlanetsInThisSign);
			}

			//remove rahu & ketu from list
			friendlyPlanetList.Remove(Rahu);
			friendlyPlanetList.Remove(Ketu);


			return friendlyPlanetList;

		}

		[API("Greenwich Apparent In Julian Days")]
		public static double GreenwichApparentInJulianDays(Time time)
		{
			//convert lmt to julian days, in universal time (UT)
			var localMeanTimeInJulian_UT = GreenwichLmtInJulianDays(time);

			//get longitude of location
			double longitude = time.GetGeoLocation().Longitude();

			//delcare output variables
			double localApparentTimeInJulian;
			string errorString = "";

			//convert lmt to local apparent time (LAT)
			using SwissEph ephemeris = new();
			ephemeris.swe_lmt_to_lat(localMeanTimeInJulian_UT, longitude, out localApparentTimeInJulian, ref errorString);


			return localApparentTimeInJulian;
		}

		[API("Shows local apparent time from Swiss Eph")]
		public static DateTime LocalApparentTime(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetLocalApparentTime", time), _getLocalApparentTime);


			//UNDERLYING FUNCTION
			DateTime _getLocalApparentTime()
			{
				//convert lmt to julian days, in universal time (UT)
				var localMeanTimeInJulian_UT = ConvertLmtToJulian(time);

				//get longitude of location
				double longitude = time.GetGeoLocation().Longitude();

				//delcare output variables
				double localApparentTimeInJulian;
				string errorString = null;

				//initialize ephemeris
				SwissEph ephemeris = new SwissEph();

				//convert lmt to local apparent time (LAT)
				ephemeris.swe_lmt_to_lat(localMeanTimeInJulian_UT, longitude, out localApparentTimeInJulian, ref errorString);

				var localApparentTime = ConvertJulianTimeToNormalTime(localApparentTimeInJulian);

				return localApparentTime;

			}

		}

		/// <summary>
		/// This method exists mainly for testing internal time calculation of LMT
		/// Important that this method passes the test at all times, so much depends on this
		/// </summary>
		public static DateTimeOffset LocalMeanTime(Time time) => time.GetLmtDateTimeOffset();

		[API("House start middle and end longitudes")]
		public static House House(HouseName houseNumber, Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetHouse", houseNumber, time), _getHouse);


			//UNDERLYING FUNCTION
			House _getHouse()
			{
				//get all house list
				var allHouses = AllHouseLongitudes(time);

				//get required house from list
				var requiredHouse = allHouses.Find(h => h.GetHouseName() == houseNumber);

				return requiredHouse;

			}

		}

		[API("Gets Panchaka at a given time", Category.StarsAboveMe)]
		public static PanchakaName Panchaka(Time time)
		{
			//If the remainder is 1 (mrityu panchakam), it
			// indicates danger; if 2 (agni panchakam), risk from fire; if 4 (raja
			// panchakam), bad results; if 6 (chora panchakam), evil happenings and if
			// 8 (roga panchakam), disease. If the remainder is 3, 5, 7 or zero then it is
			// good.

			//get the number of the lunar day (from the 1st of the month),
			var lunarDateNumber = LunarDay(time).GetLunarDateNumber();

			//get the number of the constellation (from Aswini)
			var rullingConstellationNumber = MoonConstellation(time).GetConstellationNumber();

			//Number of weekday
			var weekdayNumber = (int)DayOfWeek(time);

			//Number of zodiacal sign, number of the Lagna (from Aries).
			var risingSignNumber = (int)HouseSignName(HouseName.House1, time);

			//add all to get total
			double total = lunarDateNumber + rullingConstellationNumber + weekdayNumber + risingSignNumber;

			//get modulos of 9 to get panchaka number (Remainder From Division)
			var panchakaNumber = total % 9.0;

			//convert panchakam number to name
			switch (panchakaNumber)
			{
				//1 (mrityu panchakam)
				case 1:
					return PanchakaName.Mrityu;
				//2 (agni panchakam)
				case 2:
					return PanchakaName.Agni;
				//4 (raja panchakam)
				case 4:
					return PanchakaName.Raja;
				//6 (chora panchakam)
				case 6:
					return PanchakaName.Chora;
				//8 (roga panchakam)
				case 8:
					return PanchakaName.Roga;
				//If the remainder is 3, 5, 7 or 0 then it is good (shubha)
				case 3:
				case 5:
				case 7:
				case 0:
					return PanchakaName.Shubha;
			}

			//if panchaka number did not match above, throw error
			throw new Exception("Panchaka not found, error!");


		}

		[API("Planet lord that governs a weekday", Category.StarsAboveMe)]
		public static PlanetName LordOfWeekday(Time time)
		{
			//Sunday Sun
			//Monday Moon
			//Tuesday Mars
			//Wednesday Mercury
			//Thursday Jupiter
			//Friday Venus
			//Saturday Saturn


			//get the weekday
			var weekday = DayOfWeek(time);

			//based on weekday return the planet lord
			switch (weekday)
			{
				case Library.DayOfWeek.Sunday: return Sun;
				case Library.DayOfWeek.Monday: return Moon;
				case Library.DayOfWeek.Tuesday: return Mars;
				case Library.DayOfWeek.Wednesday: return Mercury;
				case Library.DayOfWeek.Thursday: return Jupiter;
				case Library.DayOfWeek.Friday: return Venus;
				case Library.DayOfWeek.Saturday: return Saturn;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		[API("Planet lord that governs a weekday", Category.StarsAboveMe)]
		public static PlanetName LordOfWeekday(DayOfWeek weekday)
		{
			//Sunday Sun
			//Monday Moon
			//Tuesday Mars
			//Wednesday Mercury
			//Thursday Jupiter
			//Friday Venus
			//Saturday Saturn


			//based on weekday return the planet lord
			switch (weekday)
			{
				case Library.DayOfWeek.Sunday: return Sun;
				case Library.DayOfWeek.Monday: return Moon;
				case Library.DayOfWeek.Tuesday: return Mars;
				case Library.DayOfWeek.Wednesday: return Mercury;
				case Library.DayOfWeek.Thursday: return Jupiter;
				case Library.DayOfWeek.Friday: return Venus;
				case Library.DayOfWeek.Saturday: return Saturn;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static DateTimeOffset LmtToStd(DateTimeOffset lmtDateTime, TimeSpan stdOffset)
		{
			//set lmt to offset
			//var tempTime = new DateTimeOffset(lmtDateTime);

			return lmtDateTime.ToOffset(stdOffset);
		}

		/// <summary>
		/// A hora is equal to 1/24th part of
		/// a day. The Hindu day begins with sunrise and continues till
		/// next sunrise. The first hora on any day will be the
		/// first hour after sunrise and the last hora, the hour
		/// before sunrise the next day.
		/// </summary>
		[API("A hora is equal to 1/24th part of a day", Category.StarsAboveMe)]
		public static int HoraAtBirth(Time time)
		{
			TimeSpan hours;

			var birthTime = time.GetLmtDateTimeOffset();
			var sunriseTime = SunriseTime(time).GetLmtDateTimeOffset();

			//if birth time is after sunrise, then sunrise time is correct 
			if (birthTime >= sunriseTime)
			{
				//get hours (hora) passed since sunrise (start of day)
				hours = birthTime.Subtract(sunriseTime);
			}
			//else birth has occured before sunrise on that day,
			//so have to use sunrise of the previous day
			else
			{
				//get sunrise of the previous day
				var previousDay = new Time(time.GetLmtDateTimeOffset().DateTime.AddDays(-1), time.GetStdDateTimeOffset().Offset, time.GetGeoLocation());
				sunriseTime = SunriseTime(previousDay).GetLmtDateTimeOffset();

				//get hours (hora) passed since sunrise (start of day)
				hours = birthTime.Subtract(sunriseTime);

			}

			//round hours to highest possible (ceiling)
			var hora = Math.Ceiling(hours.TotalHours);

			//if birth time is exactly as sunrise time hora will be zero here, meaning 1st hora
			if (hora == 0) { hora = 1; }


			return (int)hora;

		}

		[API("Gets hora zodiac sign of a planet", Category.StarsAboveMe)]
		public static ZodiacName PlanetHoraSign(PlanetName planetName, Time time)
		{
			//get planet sign
			var planetSign = PlanetRasiSign(planetName, time);

			//get planet sign name
			var planetSignName = planetSign.GetSignName();

			//get planet degrees in sign
			var degreesInSign = planetSign.GetDegreesInSign().TotalDegrees;

			//declare flags
			var planetInFirstHora = false;
			var planetInSecondHora = false;

			//1.0 get which hora planet is in
			//if sign in first hora (0 to 15 degrees)
			if (degreesInSign >= 0 && degreesInSign <= 15)
			{
				planetInFirstHora = true;
			}

			//if sign in second hora (15 to 30 degrees)
			if (degreesInSign > 15 && degreesInSign <= 30)
			{
				planetInSecondHora = true;
			}

			//2.0 check which type of sign the planet is in

			//if planet is in odd sign
			if (IsOddSign(planetSignName))
			{
				//if planet in first hora
				if (planetInFirstHora == true && planetInSecondHora == false)
				{
					//governed by the Sun (Leo)
					return ZodiacName.Leo;
				}

				//if planet in second hora
				if (planetInFirstHora == false && planetInSecondHora == true)
				{
					//governed by the Moon (Cancer)
					return ZodiacName.Cancer;
				}

			}


			//if planet is in even sign
			if (IsEvenSign(planetSignName))
			{
				//if planet in first hora
				if (planetInFirstHora == true && planetInSecondHora == false)
				{
					//governed by the Moon (Cancer)
					return ZodiacName.Cancer;

				}

				//if planet in second hora
				if (planetInFirstHora == false && planetInSecondHora == true)
				{
					//governed by the Sun (Leo)
					return ZodiacName.Leo;
				}

			}

			throw new Exception("Planet hora not found, error!");
		}

		/// <summary>
		/// get sunrise time for that day
		/// </summary>
		[API("get sunrise time for that day at that place")]
		public static Time SunriseTime(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetSunriseTime", time), _getSunriseTime);


			//UNDERLYING FUNCTION
			Time _getSunriseTime()
			{
				//1. Calculate sunrise time

				//prepare data to do calculation
				const int iflag = SwissEph.SEFLG_SWIEPH | SwissEph.SEFLG_SPEED | SwissEph.SEFLG_SIDEREAL;
				const int srflag = SwissEph.SE_BIT_NO_REFRACTION | SwissEph.SE_BIT_DISC_CENTER; //disk is at center of horizon
				var options = SwissEph.SE_CALC_RISE | srflag; //set for sunrise
				var planet = SwissEph.SE_SUN;

				double[] geopos = new Double[3] { time.GetGeoLocation().Longitude(), time.GetGeoLocation().Latitude(), 0 };
				double riseTimeRaw = 0;

				var errorMsg = "";
				const double atpress = 0.0; //pressure
				const double attemp = 0.0;  //temperature

				//create a new time at 12 am on the same day, as calculator searches for sunrise after the inputed time
				var oriLmt = time.GetLmtDateTimeOffset();
				var lmtAt12Am = new DateTime(oriLmt.Year, oriLmt.Month, oriLmt.Day, 0, 0, 0);
				var timeAt12Am = new Time(lmtAt12Am, time.GetStdDateTimeOffset().Offset, time.GetGeoLocation());


				//get LMT at Greenwich in Julian days
				var julianLmtUtcTime = GreenwichLmtInJulianDays(timeAt12Am);

				//do calculation for sunrise time
				using SwissEph ephemeris = new();
				int ret = ephemeris.swe_rise_trans(julianLmtUtcTime, planet, "", iflag, options, geopos, atpress, attemp, ref riseTimeRaw, ref errorMsg);


				//2. Convert raw sun rise time (julian lmt utc) to normal time (std)

				//julian days back to normal time (greenwich)
				var sunriseLmtAtGreenwich = GreenwichTimeFromJulianDays(riseTimeRaw);

				//return sunrise time at orginal location to caller
				var stdOriginal = sunriseLmtAtGreenwich.ToOffset(time.GetStdDateTimeOffset().Offset);
				var sunriseTime = new Time(stdOriginal, time.GetGeoLocation());
				return sunriseTime;

			}

		}

		/// <summary>
		/// Get actual sunset time for that day at that place
		/// </summary>
		[API("actual sunset time for that day at that place")]
		public static Time SunsetTime(Time time)
		{

			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetSunsetTime", time), _getSunsetTime);


			//UNDERLYING FUNCTION
			Time _getSunsetTime()
			{
				//1. Calculate sunset time

				//prepare data to do calculation
				const int iflag = SwissEph.SEFLG_SWIEPH | SwissEph.SEFLG_SPEED | SwissEph.SEFLG_SIDEREAL;
				const int srflag = SwissEph.SE_BIT_NO_REFRACTION | SwissEph.SE_BIT_DISC_CENTER; //disk is at center of horizon
				var options = SwissEph.SE_CALC_SET | srflag; //set for sunset
				var planet = SwissEph.SE_SUN;

				double[] geopos = new Double[3] { time.GetGeoLocation().Longitude(), time.GetGeoLocation().Latitude(), 0 };
				double setTimeRaw = 0;

				var errorMsg = "";
				const double atpress = 0.0; //pressure
				const double attemp = 0.0;  //temperature


				//create a new time at 12 am on the same day, as calculator searches for sunrise after the inputed time
				var oriLmt = time.GetLmtDateTimeOffset();
				var lmtAt12Am = new DateTime(oriLmt.Year, oriLmt.Month, oriLmt.Day, 0, 0, 0);
				var timeAt12Am = new Time(lmtAt12Am, time.GetStdDateTimeOffset().Offset, time.GetGeoLocation());

				//get LMT at Greenwich in Julian days
				var julianLmtUtcTime = GreenwichLmtInJulianDays(timeAt12Am);

				//do calculation for sunset time
				using SwissEph ephemeris = new();
				int ret = ephemeris.swe_rise_trans(julianLmtUtcTime, planet, "", iflag, options, geopos, atpress, attemp, ref setTimeRaw, ref errorMsg);



				//2. Convert raw sun set time (julian lmt utc) to normal time (std)

				//julian days back to normal time (greenwich)
				var sunriseLmtAtGreenwich = GreenwichTimeFromJulianDays(setTimeRaw);

				//return sunset time at orginal location to caller
				var stdOriginal = sunriseLmtAtGreenwich.ToOffset(time.GetStdDateTimeOffset().Offset);
				var sunsetTime = new Time(stdOriginal, time.GetGeoLocation());
				return sunsetTime;

			}

		}

		/// <summary>
		/// Get actual noon time for that day at that place
		/// Returned in apparent time (DateTime)
		/// Note:
		/// This is marked when the centre of the Sun is exactly* on the
		/// meridian of the place. The apparent noon is
		/// almost the same for all places.
		/// *Center of disk is not actually used for now (future implementation)
		/// </summary>
		[API("Sun is exactly overhead at location")]
		public static DateTime NoonTime(Time time)
		{
			//get apparent time
			var localApparentTime = LocalApparentTime(time);
			var apparentNoon = new DateTime(localApparentTime.Year, localApparentTime.Month, localApparentTime.Day, 12, 0, 0);

			return apparentNoon;
		}

		/// <summary>
		/// Checks if planet A is in good aspect to planet B
		///
		/// Note:
		/// A is transmitter, B is receiver
		/// 
		/// An aspect is good or bad according to the relation
		/// between the aspecting and the aspected body
		/// </summary>
		public static bool IsPlanetInGoodAspectToPlanet(PlanetName receivingAspect, PlanetName transmitingAspect, Time time)
		{
			//check if transmitting planet is aspecting receiving planet
			var isAspecting = IsPlanetAspectedByPlanet(receivingAspect, transmitingAspect, time);

			//if not aspecting at all, end here as not occuring
			if (!isAspecting) { return false; }

			//check if it is a good aspect
			var aspectNature = PlanetCombinedRelationshipWithPlanet(receivingAspect, transmitingAspect, time);
			var isGood = aspectNature == PlanetToPlanetRelationship.BestFriend ||
						 aspectNature == PlanetToPlanetRelationship.Friend;

			//if is aspecting and it is good, then occuring (true)
			return isAspecting && isGood;

		}

		/// <summary>
		///Checks if a planet is in good aspect to a house
		///
		/// Note:
		/// An aspect is good or bad according to the relation
		/// between the planet and lord of the house sign
		/// </summary>
		public static bool IsPlanetInGoodAspectToHouse(HouseName receivingAspect, PlanetName transmitingAspect, Time time)
		{
			//check if transmiting planet is aspecting receiving planet
			var isAspecting = IsHouseAspectedByPlanet(receivingAspect, transmitingAspect, time);

			//if not aspecting at all, end here as not occuring
			if (!isAspecting) { return false; }

			//check if it is a good aspect
			var aspectNature = PlanetRelationshipWithHouse(receivingAspect, transmitingAspect, time);

			var isGood = aspectNature == PlanetToSignRelationship.OwnVarga || //Swavarga - own varga
						 aspectNature == PlanetToSignRelationship.FriendVarga || //Mitravarga - friendly varga
						 aspectNature == PlanetToSignRelationship.BestFriendVarga; //Adhi Mitravarga - Intimate friend varga


			//if is aspecting and it is good, then occuring (true)
			return isAspecting && isGood;

		}

		/// <summary>
		/// To determine if sthana bala is indicating good position or bad position
		/// a neutral point is set, anything above is good & below is bad
		///
		/// Note:
		/// Neutral point is derived from all possible sthana bala values across
		/// 25 years (2000-2025), with 1 hour granularity
		///
		/// Formula used = ((max-min)/2)+min
		/// max = hightest possible value
		/// min = lowest possible value
		/// </summary>
		[API("Sthana bala is indicating good position or bad position", Category.Astronomical)]
		public static double PlanetSthanaBalaNeutralPoint(PlanetName planet)
		{
			//no calculation for rahu and ketu here
			var isRahu = planet.Name == PlanetNameEnum.Rahu;
			var isKetu = planet.Name == PlanetNameEnum.Ketu;
			var isRahuKetu = isRahu || isKetu;
			if (isRahuKetu) { return 0; }


			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey("GetPlanetSthanaBalaNeutralPoint", planet), _getPlanetSthanaBalaNeutralPoint);



			double _getPlanetSthanaBalaNeutralPoint()
			{
				int max = 0, min = 0;

				if (planet == Saturn) { max = 297; min = 59; }
				if (planet == Mars) { max = 362; min = 60; }
				if (planet == Jupiter) { max = 296; min = 77; }
				if (planet == Mercury) { max = 295; min = 47; }
				if (planet == Venus) { max = 284; min = 60; }
				if (planet == Sun) { max = 327; min = 52; }
				if (planet == Moon) { max = 311; min = 54; }

				//calculate neutral point
				var neutralPoint = ((max - min) / 2) + min;

				if (neutralPoint <= 0) { throw new Exception("Planet does not have neutral point!"); }

				return neutralPoint;
			}
		}

		/// <summary>
		/// EXPERIMENTAL
		/// To determine if Shadvarga bala is strong or weak
		/// a neutral point is set, anything above is strong & below is weak
		///
		/// Note:
		/// Neutral point is derived from all possible Shadvarga bala values across
		/// 25 years (2000-2025), with 1 hour granularity
		///
		/// Formula used = ((max-min)/2)+min (add min to get exact neutral point from 0 range)
		/// max = hightest possible value
		/// min = lowest possible value
		/// </summary>
		[API("ShadvargaBalaNeutralPoint")]
		public static double PlanetShadvargaBalaNeutralPoint(PlanetName planet)
		{

			//no calculation for rahu and ketu here
			var isRahu = planet.Name == PlanetNameEnum.Rahu;
			var isKetu = planet.Name == PlanetNameEnum.Ketu;
			var isRahuKetu = isRahu || isKetu;
			if (isRahuKetu) { return 0; }


			//CACHE MECHANISM
			return CacheManager.GetCache(new CacheKey(nameof(PlanetShadvargaBalaNeutralPoint), planet), _getPlanetShadvargaBalaNeutralPoint);



			double _getPlanetShadvargaBalaNeutralPoint()
			{
				int max = 0, min = 0;

				if (planet == Saturn) { max = 150; min = 11; }
				if (planet == Mars) { max = 188; min = 21; }
				if (planet == Jupiter) { max = 172; min = 17; }
				if (planet == Mercury) { max = 150; min = 17; }
				if (planet == Venus) { max = 158; min = 15; }
				if (planet == Sun) { max = 180; min = 17; }
				if (planet == Moon) { max = 165; min = 26; }

				//difference between max & min
				var difference = (max - min);

				//divide difference in half to get neutral point
				//add min to get exact neutral point from 0 range
				var neutralPoint = (difference / 2) + min;

				if (neutralPoint <= 0) { throw new Exception("Planet does not have neutral point!"); }


				return neutralPoint;
			}
		}

		/// <summary>
		/// Checks if a planet is in a kendra house (1,4,7,10)
		/// </summary>
		[API("Checks if a planet is in a kendra house (1,4,7,10)")]
		public static bool IsPlanetInKendra(PlanetName planet, Time time)
		{
			//The 4th, the 7th and the 10th are the Kendras
			var planetHouse = HousePlanetIsIn(time, planet);

			//check if planet is in kendra
			var isPlanetInKendra = planetHouse == HouseName.House1 || planetHouse == HouseName.House4 || planetHouse == HouseName.House7 || planetHouse == HouseName.House10;

			return isPlanetInKendra;
		}

		[API("Checks if a planet is in a kendra house (1,4,7,10) from another planet. Exp : Is Jupiter is in a kendra from the Moon")]
		public static bool IsPlanetInKendraFromPlanet(PlanetName kendraFrom, PlanetName kendraTo, Time time)
		{
			//get the number of signs between planets
			var count = Calculate.SignDistanceFromPlanetToPlanet(kendraTo, kendraFrom, time);

			//check if number is a kendra number
			var isKendra = count == 1 ||
						   count == 4 ||
						   count == 7 ||
						   count == 10;

			return isKendra;
		}


		[API("Counts number of sign between 2 planets.")]
		public static int SignDistanceFromPlanetToPlanet(PlanetName startPlanet, PlanetName endPlanet, Time time)
		{
			//get position of "kendra to" planet
			var startSign = Calculate.PlanetRasiSign(startPlanet, time);

			//get position of "kendra from" planet
			var endSign = Calculate.PlanetRasiSign(endPlanet, time);

			//count distance between signs
			var count = Calculate.CountFromSignToSign(startSign.GetSignName(), endSign.GetSignName());

			return count;
		}

		/// <summary>
		/// Checks if the lord of a house is in the specified house.
		/// Example question : Is Lord of 1st house in 2nd house?
		/// </summary>
		[API("Checks if the lord of a house is in the specified house. Exp : Is Lord of 1st house in 2nd house?")]
		public static bool IsHouseLordInHouse(HouseName lordHouse, HouseName occupiedHouse, Time time)
		{
			//get the house lord
			var houseLord = LordOfHouse(lordHouse, time);

			//get house the lord is in
			var houseIsIn = HousePlanetIsIn(time, houseLord);

			//if it matches then occuring
			return houseIsIn == occupiedHouse;



		}

		/// <summary>
		/// Checks if a planet is conjuct with an evil/malefic planet
		/// </summary>
		[API("Checks if a planet is conjuct with an evil/malefic planet")]
		public static bool IsPlanetConjunctWithMaleficPlanets(PlanetName planetName, Time time)
		{
			//get all the planets conjuct with inputed planet
			var planetsInConjunct = PlanetsInConjuction(time, planetName);

			//get all evil planets
			var evilPlanets = MaleficPlanetList(time);

			//check if any conjunct planet is an evil one
			var evilFound = planetsInConjunct.FindAll(planet => evilPlanets.Contains(planet)).Any();
			return evilFound;

		}

		/// <summary>
		/// Checks if a planet is conjunct with an enemy planet by combined relationship
		/// </summary>
		[API("Checks if a planet is conjunct with an enemy planet by combined relationship")]
		public static bool IsPlanetConjunctWithEnemyPlanets(PlanetName inputPlanet, Time time)
		{
			//get all the planets conjunct with inputed planet
			var planetsInConjunct = PlanetsInConjuction(time, inputPlanet);

			//check if any conjunct planet is an enemy
			foreach (var planet in planetsInConjunct)
			{
				//get relationship of the 2 planets
				var aspectNature = PlanetCombinedRelationshipWithPlanet(inputPlanet, planet, time);
				var isEnemy = aspectNature == PlanetToPlanetRelationship.Enemy ||
							 aspectNature == PlanetToPlanetRelationship.BitterEnemy;

				//if enemy than end here as true
				if (isEnemy) { return true; }

			}

			//if control reaches here than no enemy planet found
			return false;

		}

		/// <summary>
		/// Checks if a planet is conjunct with an Friend planet by combined relationship
		/// </summary>
		[API("Checks if a planet is conjunct with a Friend planet by combined relationship")]
		public static bool IsPlanetConjunctWithFriendPlanets(PlanetName inputPlanet, Time time)
		{
			//get all the planets conjunct with inputed planet
			var planetsInConjunct = PlanetsInConjuction(time, inputPlanet);

			//check if any conjunct planet is an Friend
			foreach (var planet in planetsInConjunct)
			{
				//get relationship of the 2 planets
				var conjunctNature = PlanetCombinedRelationshipWithPlanet(inputPlanet, planet, time);
				var isFriend = conjunctNature == PlanetToPlanetRelationship.Friend ||
							   conjunctNature == PlanetToPlanetRelationship.BestFriend;

				//if enemy than end here as true
				if (isFriend) { return true; }

			}

			//if control reaches here than no enemy planet found
			return false;

		}

		/// <summary>
		/// Checks if any evil/malefic planets are in a house
		/// Note : Planet to house relationship not account for
		/// TODO Account for planet to sign relationship, find reference
		/// </summary>
		[API("Checks if any evil/malefic planets are in a house")]
		public static bool IsMaleficPlanetInHouse(HouseName houseNumber, Time time)
		{
			//get all the planets in the house
			var planetsInHouse = PlanetsInHouse(houseNumber, time);

			//get all evil planets
			var evilPlanets = MaleficPlanetList(time);

			//check if any planet in house is an evil one
			var evilFound = planetsInHouse.FindAll(planet => evilPlanets.Contains(planet)).Any();

			return evilFound;

		}

		/// <summary>
		/// Checks if any good/benefic planets are in a house
		/// Note : Planet to house relationship not account for
		/// TODO Account for planet to sign relationship, find reference
		/// </summary>
		[API("Checks if any good/benefic planets are in a house")]
		public static bool IsBeneficPlanetInHouse(HouseName houseNumber, Time time)
		{
			//get all the planets in the house
			var planetsInHouse = PlanetsInHouse(houseNumber, time);

			//get all good planets
			var goodPlanets = BeneficPlanetList(time);

			//check if any planet in house is an good one
			var goodFound = planetsInHouse.FindAll(planet => goodPlanets.Contains(planet)).Any();

			return goodFound;

		}


		/// <summary>
		/// Checks if any evil/malefic planets are in a sign
		/// </summary>
		public static bool IsMaleficPlanetInSign(ZodiacName sign, Time time)
		{
			//get all the planets in the sign
			var planetsInSign = PlanetInSign(sign, time);

			//get all evil planets
			var evilPlanets = MaleficPlanetList(time);

			//check if any planet in sign is an evil one
			var evilFound = planetsInSign.FindAll(planet => evilPlanets.Contains(planet)).Any();

			return evilFound;
		}

		/// <summary>
		/// Gets list of evil/malefic planets in a sign
		/// </summary>
		public static List<PlanetName> MaleficPlanetListInSign(ZodiacName sign, Time time)
		{
			//get all the planets in the sign
			var planetsInSign = PlanetInSign(sign, time);

			//get all evil planets
			var evilPlanets = MaleficPlanetList(time);

			//get evil planets in sign
			var evilFound = planetsInSign.FindAll(planet => evilPlanets.Contains(planet));

			return evilFound;
		}

		/// <summary>
		/// Checks if any good/benefic planets are in a sign
		/// </summary>
		public static bool IsBeneficPlanetInSign(ZodiacName sign, Time time)
		{
			//get all the planets in the sign
			var planetsInSign = PlanetInSign(sign, time);

			//get all good planets
			var goodPlanets = BeneficPlanetList(time);

			//check if any planet in sign is an good one
			var goodFound = planetsInSign.FindAll(planet => goodPlanets.Contains(planet)).Any();

			return goodFound;
		}

		/// <summary>
		/// Gets any good/benefic planets in a sign
		/// </summary>
		public static List<PlanetName> BeneficPlanetListInSign(ZodiacName sign, Time time)
		{
			//get all the planets in the sign
			var planetsInSign = PlanetInSign(sign, time);

			//get all good planets
			var goodPlanets = BeneficPlanetList(time);

			//gets all good planets in this sign
			var goodFound = planetsInSign.FindAll(planet => goodPlanets.Contains(planet));

			return goodFound;
		}

		/// <summary>
		/// Checks if any evil/malefic planet is transmitting aspect to a house
		/// Note: This does NOT account for bad aspects, where relationship with house lord is checked
		/// TODO relationship aspect should be added get reference for it firsts
		/// </summary>
		public static bool IsMaleficPlanetAspectHouse(HouseName house, Time time)
		{
			//get all evil planets
			var evilPlanets = MaleficPlanetList(time);

			//check if any evil planet is aspecting the inputed house
			var evilFound = evilPlanets.FindAll(evilPlanet => IsHouseAspectedByPlanet(house, evilPlanet, time)).Any();

			return evilFound;

		}

		/// <summary>
		/// Checks if any good/benefic planet is transmitting aspect to a house
		/// Note: This does NOT account for good aspects, where relationship with house lord is checked
		/// TODO relationship aspect should be added get reference for it firsts
		/// </summary>
		public static bool IsBeneficPlanetAspectHouse(HouseName house, Time time)
		{
			//get all good planets
			var goodPlanets = BeneficPlanetList(time);

			//check if any good planet is aspecting the inputed house
			var goodFound = goodPlanets.FindAll(goodPlanet => IsHouseAspectedByPlanet(house, goodPlanet, time)).Any();

			return goodFound;

		}

		/// <summary>
		/// Checks if a planet is receiving aspects from an evil planet
		/// </summary>
		[API("AspectedByMalefics")]
		public static bool IsPlanetAspectedByMaleficPlanets(PlanetName lord, Time time)
		{
			//get list of evil planets
			var evilPlanets = MaleficPlanetList(time);

			//check if any of the evil planets is aspecting inputed planet
			var evilAspectFound = evilPlanets.FindAll(evilPlanet =>
				IsPlanetAspectedByPlanet(lord, evilPlanet, time)).Any();
			return evilAspectFound;

		}

		/// <summary>
		/// Checks if a planet is receiving aspects from an benefic planet
		/// </summary>
		[API("Checks if a planet is receiving aspects from an benefic planet")]
		public static bool IsPlanetAspectedByBeneficPlanets(PlanetName lord, Time time)
		{
			//get list of benefic planets
			var goodPlanets = BeneficPlanetList(time);

			//check if any of the benefic planets is aspecting inputed planet
			var goodAspectFound = goodPlanets.FindAll(goodPlanet =>
				IsPlanetAspectedByPlanet(lord, goodPlanet, time)).Any();

			return goodAspectFound;

		}

		/// <summary>
		/// Checks if a planet is receiving aspects from an enemy planet based on combined relationship
		/// </summary>
		[API("Checks if a planet is receiving aspects from an enemy planet based on combined relationship")]
		public static bool IsPlanetAspectedByEnemyPlanets(PlanetName inputPlanet, Time time)
		{
			//get all the planets aspecting inputed planet
			var planetsAspecting = PlanetsAspectingPlanet(time, inputPlanet);

			//check if any aspecting planet is an enemy
			foreach (var planet in planetsAspecting)
			{
				//get relationship of the 2 planets
				var aspectNature = PlanetCombinedRelationshipWithPlanet(inputPlanet, planet, time);
				var isEnemy = aspectNature == PlanetToPlanetRelationship.Enemy ||
							  aspectNature == PlanetToPlanetRelationship.BitterEnemy;

				//if enemy than end here as true
				if (isEnemy) { return true; }

			}

			//if control reaches here than no enemy planet found
			return false;


		}

		/// <summary>
		/// Checks if a planet is receiving aspects from a Friend planet based on combined relationship
		/// </summary>
		[API("Checks if a planet is receiving aspects from a Friend planet based on combined relationship")]
		public static bool IsPlanetAspectedByFriendPlanets(PlanetName inputPlanet, Time time)
		{
			//get all the planets aspecting inputed planet
			var planetsAspecting = PlanetsAspectingPlanet(time, inputPlanet);

			//check if any aspecting planet is an Friend
			foreach (var planet in planetsAspecting)
			{
				//get relationship of the 2 planets
				var aspectNature = PlanetCombinedRelationshipWithPlanet(inputPlanet, planet, time);
				var isFriend = aspectNature == PlanetToPlanetRelationship.Friend ||
							   aspectNature == PlanetToPlanetRelationship.BestFriend;

				//if Friend than end here as true
				if (isFriend) { return true; }

			}

			//if control reaches here than no Friend planet found
			return false;


		}

		/// <summary>
		/// Gets the Arudha Lagna Sign 
		/// 
		/// Reference Note:
		/// Arudha Lagna and planetary dispositions in reference to it have a strong bearing on the
		/// financial status of the person. In my own humble experience, Arudha Lagna should be given
		/// as much importance as the usual Janma Lagna. Arudha Lagna is the sign arrived at by counting
		/// as many signs from lord of Lagna as lord of Lagna is removed from Lagna.
		/// Thus if Aquarius is ascendant and its lord Saturn is in the 4th (Taurus)
		/// then the 4th from Taurus, viz., Leo becomes Arudha Lagna.
		/// </summary>
		[API("Gets the Arudha Lagna sign, bearing on the financial status", Category.StarsAboveMe)]
		public static ZodiacName ArudhaLagnaSign(Time time)
		{
			//get janma lagna
			var janmaLagna = HouseSignName(HouseName.House1, time);

			//get sign lord of janma lagna is in
			var lagnaLord = LordOfHouse(HouseName.House1, time);
			var lagnaLordSign = PlanetRasiSign(lagnaLord, time).GetSignName();

			//count the signs from janma to the sign the lord is in
			var janmaToLordCount = CountFromSignToSign(janmaLagna, lagnaLordSign);

			//use the above count to find arudha sign from lord's sign
			var arudhaSign = SignCountedFromInputSign(lagnaLordSign, janmaToLordCount);

			return arudhaSign;
		}

		/// <summary>
		/// Counts from start sign to end sign
		/// Example : Aquarius to Taurus is 4
		/// </summary>
		public static int CountFromSignToSign(ZodiacName startSign, ZodiacName endSign)
		{
			int count = 0;

			//get zodiac name & convert to its number equivalent
			var startSignNumber = (int)startSign;
			var endSignNumber = (int)endSign;

			//if start sign is more than end sign (meaning lower in the list)
			if (startSignNumber > endSignNumber)
			{
				//minus with 12, as though counting to the end
				int countToLastZodiac = (12 - startSignNumber) + 1; //plus 1 to count it self

				count = endSignNumber + countToLastZodiac;
			}
			else if (startSignNumber == endSignNumber)
			{
				count = 1;
			}
			//if start sign is lesser than end sign (meaning higher in the list)
			//we can minus like normal, and just add 1 to count it self
			else if (startSignNumber < endSignNumber)
			{
				count = (endSignNumber - startSignNumber) + 1; //plus 1 to count it self
			}

			return count;
		}

		/// <summary>
		/// Counts from start Constellation to end Constellation
		/// Example : Aquarius to Taurus is 4
		/// </summary>
		public static int CountFromConstellationToConstellation(PlanetConstellation start, PlanetConstellation end)
		{

			//get the number equivalent of the constellation
			int endConstellationNumber = end.GetConstellationNumber();

			int startConstellationNumber = start.GetConstellationNumber();

			int counter = 0;


			//Need to count from birthRulingConstellationNumber to dayRulingConstellationNumber

			//if start is more than end (meaning lower in the list)
			if (startConstellationNumber > endConstellationNumber)
			{
				//count from start to last constellation (27)
				int countToLastConstellation = (27 - startConstellationNumber) + 1; //plus 1 to count it self

				//to previous count add end constellation number
				counter = endConstellationNumber + countToLastConstellation;
			}
			else if (startConstellationNumber == endConstellationNumber)
			{
				counter = 1;
			}
			else if (startConstellationNumber < endConstellationNumber)
			{
				//if start sign is lesser than end sign (meaning higher in the list)
				//we can minus like normal, and just add 1 to count it self
				counter = (endConstellationNumber - startConstellationNumber) + 1; //plus 1 to count it self
			}


			return counter;
		}

		/// <summary>
		/// Checks if a planet is in a given house at a specified time 
		/// </summary>
		[API("Checks if a planet is in a given house at a specified time")]
		public static bool IsPlanetInHouse(Time time, PlanetName planet, HouseName houseNumber)
		{
			return HousePlanetIsIn(time, planet) == houseNumber;
		}

		/// <summary>
		/// Checks if a planet is in a given house at a specified time 
		/// </summary>
		[API("Checks if a planet list is in a given house at a specified time")]
		public static bool IsAllPlanetInHouse(Time time, List<PlanetName> planetList, HouseName houseNumber)
		{
			//calculate each planet, even if 1 planet is out, then return as false
			foreach (var planetName in planetList)
			{
				var tempVal = IsPlanetInHouse(time, planetName, houseNumber);
				if (tempVal == false) { return false; }
			}

			//if control reaches here than all planets in house
			return true;
		}

		/// <summary>
		/// Checks if any planet in list is at a given house at a specified time 
		/// </summary>
		[API("Checks if any planet in list is at a given house at a specified time ")]
		public static bool IsAnyPlanetInHouse(Time time, List<PlanetName> planetList, HouseName houseNumber)
		{
			//calculate each planet, even if 1 planet is out, then return as false
			foreach (var planetName in planetList)
			{
				var tempVal = IsPlanetInHouse(time, planetName, houseNumber);
				if (tempVal == true) { return true; }
			}

			// if control reaches here then no planet is in house
			return false;
		}

		/// <summary>
		/// Checks if a planet is in a longitude where it's in Debilitated
		/// Note : Rahu & ketu accounted for
		/// </summary>
		[API("Checks if a planet is in a longitude where it's in Debilitated")]
		public static bool IsPlanetDebilitated(PlanetName planet, Time time)
		{
			//get planet location
			var planetLongitude = PlanetNirayanaLongitude(time, planet);

			//convert planet longitude to zodiac sign
			var planetZodiac = ZodiacSignAtLongitude(planetLongitude);

			//get the longitude where planet is Debilited
			var point = PlanetDebilitationPoint(planet);

			//check if planet is in Debilitation sign
			var sameSign = planetZodiac.GetSignName() == point.GetSignName();

			//check only degree ignore minutes & seconds
			var sameDegree = planetZodiac.GetDegreesInSign().Degrees == point.GetDegreesInSign().Degrees;
			var planetIsDebilitated = sameSign && sameDegree;

			return planetIsDebilitated;
		}

		/// <summary>
		/// Checks if a planet is in a longitude where it's in Exaltation
		///
		/// NOTE:
		/// -   Rahu & ketu accounted for
		/// 
		/// -   Exaltation
		///     Each planet is held to be exalted when it is
		///     in a particular sign. The power to do good when in
		///     exaltation is greater than when in its own sign.
		///     Throughout the sign ascribed,
		///     the planet is exalted but in a particular degree
		///     its exaltation is at the maximum level.
		/// </summary>
		[API("Checks if a planet is in a longitude where it's in Exaltation")]
		public static bool IsPlanetExalted(PlanetName planet, Time time)
		{
			//get planet location
			var planetLongitude = PlanetNirayanaLongitude(time, planet);

			//convert planet longitude to zodiac sign
			var planetZodiac = ZodiacSignAtLongitude(planetLongitude);

			//get the longitude where planet is Exaltation
			var point = PlanetExaltationPoint(planet);

			//check if planet is in Exaltation sign
			var sameSign = planetZodiac.GetSignName() == point.GetSignName();

			//check only degree ignore minutes & seconds
			var sameDegree = planetZodiac.GetDegreesInSign().Degrees == point.GetDegreesInSign().Degrees;
			var planetIsExaltation = sameSign && sameDegree;

			return planetIsExaltation;
		}

		[API("name of vedic month", Category.StarsAboveMe)]
		public static LunarMonth LunarMonth(Time time)
		{

			return Library.LunarMonth.Empty;

			//TODO NEEDS WORK
			throw new NotImplementedException();


			//get this months full moon date
			var fullMoonTime = getFullMoonTime();

			//sunrise
			var x = SunriseTime(time);
			var y = MoonConstellation(x).GetConstellationName();

		Calculate:
			//get the constellation behind the moon
			var constellation = MoonConstellation(fullMoonTime).GetConstellationName();

			//go back one constellation
			//constellation = constellation - 1;

			switch (constellation)
			{
				case ConstellationName.Aswini:
					return Library.LunarMonth.Aswijam;
					break;
				case ConstellationName.Bharani:
					break;
				case ConstellationName.Krithika:
					return Library.LunarMonth.Karthikam;
					break;
				case ConstellationName.Rohini:
					break;
				case ConstellationName.Mrigasira:
				case ConstellationName.Aridra:
					return Library.LunarMonth.Margasiram;
					break;
				case ConstellationName.Punarvasu:
					break;
				case ConstellationName.Pushyami:
					return Library.LunarMonth.Pooshiam;
					break;
				case ConstellationName.Aslesha:
					break;
				case ConstellationName.Makha:
					return Library.LunarMonth.Magham;
					break;
				case ConstellationName.Pubba:
					return Library.LunarMonth.Phalgunam;
					break;
				case ConstellationName.Uttara:
					break;
				case ConstellationName.Hasta:
					break;
				case ConstellationName.Chitta:
					return Library.LunarMonth.Chitram;
					break;
				case ConstellationName.Swathi:
					break;
				case ConstellationName.Vishhaka:
					return Library.LunarMonth.Visakham;
					break;
				case ConstellationName.Anuradha:
					break;
				case ConstellationName.Jyesta:
					return Library.LunarMonth.Jaistam;
					break;
				case ConstellationName.Moola:
					break;
				case ConstellationName.Poorvashada:
					return Library.LunarMonth.Ashadam;
					break;
				case ConstellationName.Uttarashada:
					break;
				case ConstellationName.Sravana:
					return Library.LunarMonth.Sravanam;
					break;
				case ConstellationName.Dhanishta:
					break;
				case ConstellationName.Satabhisha:
					break;
				case ConstellationName.Poorvabhadra:
					return Library.LunarMonth.Bhadrapadam;
				case ConstellationName.Uttarabhadra:
					break;
				case ConstellationName.Revathi:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			throw new ArgumentOutOfRangeException();
			//switch (constellation)
			//{
			//    case ConstellationName.Aswini:
			//        return LunarMonth.Aswijam;
			//        break;
			//    case ConstellationName.Krithika:
			//        return LunarMonth.Karthikam;
			//        break;
			//    case ConstellationName.Mrigasira:
			//        return LunarMonth.Margasiram;
			//        break;
			//    case ConstellationName.Pushyami:
			//        return LunarMonth.Pooshiam;
			//        break;
			//    case ConstellationName.Makha:
			//        return LunarMonth.Magham;
			//        break;
			//    case ConstellationName.Pubba:
			//        return LunarMonth.Phalgunam;
			//        break;
			//    case ConstellationName.Chitta:
			//        return LunarMonth.Chitram;
			//        break;
			//    case ConstellationName.Vishhaka:
			//        return LunarMonth.Visakham;
			//        break;
			//    case ConstellationName.Jyesta:
			//        return LunarMonth.Jaistam;
			//        break;
			//    case ConstellationName.Poorvashada:
			//        return LunarMonth.Ashadam;
			//        break;
			//    case ConstellationName.Sravana:
			//        return LunarMonth.Sravanam;
			//        break;
			//    case ConstellationName.Poorvabhadra:
			//        return LunarMonth.Bhadrapadam;
			//        break;
			//    default:
			//        fullMoonTime = fullMoonTime.AddHours(1);
			//        goto Calculate;
			//}





			//FUNCTIONS

			Time getFullMoonTime()
			{
				//get the lunar date number
				int lunarDayNumber = LunarDay(time).GetLunarDateNumber();

				//start with input time
				var fullMoonTime = time;

				//full moon not yet pass
				if (lunarDayNumber < 15)
				{
					//go forward in time till find full moon
					while (!IsFullMoon(fullMoonTime))
					{
						fullMoonTime = fullMoonTime.AddHours(1);
					}

					return fullMoonTime;
				}
				else
				{
					//go backward in time till find full moon
					while (!IsFullMoon(fullMoonTime))
					{
						fullMoonTime = fullMoonTime.SubtractHours(1);
					}

					return fullMoonTime;

				}

			}
		}

		/// <summary>
		/// Checks if the moon is FULL, moon day 15
		/// </summary>
		[API("Checks if the moon is FULL, moon day 15", Category.StarsAboveMe)]
		public static bool IsFullMoon(Time time)
		{
			//get the lunar date number
			int lunarDayNumber = LunarDay(time).GetLunarDayNumber();

			//if day 15, it is full moon
			return lunarDayNumber == 15;
		}

		/// <summary>
		/// Check if it is a Water / Aquatic sign
		/// Water Signs: this category include Cancer, Scorpio and Pisces.
		/// </summary>
		[API("Check if it is a Water / Aquatic sign")]
		public static bool IsWaterSign(ZodiacName moonSign) => moonSign is ZodiacName.Cancer or ZodiacName.Scorpio or ZodiacName.Pisces;

		/// <summary>
		/// Check if it is a Fire sign
		/// Fire Signs: this sign encloses Aries, Leo and Sagittarius.
		/// </summary>
		[API("Check if it is a Fire sign, Aries, Leo and Sagittarius.")]
		public static bool IsFireSign(ZodiacName moonSign) => moonSign is ZodiacName.Aries or ZodiacName.Leo or ZodiacName.Sagittarius;

		/// <summary>
		/// Check if it is a Earth sign
		/// Earth Signs: it contains Taurus, Virgo and Capricorn.
		/// </summary>
		[API("Check if it is a Earth sign, Taurus, Virgo and Capricorn.")]
		public static bool IsEarthSign(ZodiacName moonSign) => moonSign is ZodiacName.Taurus or ZodiacName.Virgo or ZodiacName.Capricornus;

		/// <summary>
		/// Check if it is a Air / Windy sign
		/// Air Signs: this sign include Gemini, Libra and Aquarius.
		/// </summary>
		[API("Check if it is a Air / Windy sign, Gemini, Libra and Aquarius.")]
		public static bool IsAirSign(ZodiacName moonSign) => moonSign is ZodiacName.Gemini or ZodiacName.Libra or ZodiacName.Aquarius;

		/// <summary>
		/// WARNING! MARKED FOR DELETION : ERONEOUS RESULTS NOT SUITED FOR INTENDED PURPOSE
		/// METHOD NOT VERIFIED
		/// This methods perpose is to define the final good or bad
		/// nature of planet in antaram.
		///
		/// For now only data from chapter "Key-planets for Each Sign"
		/// If this proves to be inacurate, add more checks in this method.
		/// - bindu points
		/// 
		/// Similar to method GetDasaInfoForAscendant
		/// Data from pg 80 of Key-planets for Each Sign in Hindu Predictive Astrology
		/// TODO meant to determine nature of antram
		/// </summary>
		public static EventNature PlanetAntaramNature(Person person, PlanetName planet)
		{
			//todo account for rahu & ketu
			//rahu & ketu not sure for now, just return neutral
			if (planet == Rahu || planet == Ketu) { return EventNature.Neutral; }

			//get nature from person's lagna
			var planetNature = GetNatureFromLagna();

			//if nature is neutral then use nature of relation to current house
			//assumed that bad relation to sign is bad planet (todo upgrade to bindu points)
			//note: generaly speaking a neutral planet shloud not exist, either good or bad
			if (planetNature == EventNature.Neutral)
			{
				var _planetCurrentHouse = HousePlanetIsIn(person.BirthTime, planet);

				var _currentHouseRelation = PlanetRelationshipWithHouse((HouseName)_planetCurrentHouse, planet, person.BirthTime);

				switch (_currentHouseRelation)
				{
					case PlanetToSignRelationship.BestFriendVarga:
					case PlanetToSignRelationship.FriendVarga:
					case PlanetToSignRelationship.OwnVarga:
					case PlanetToSignRelationship.Moolatrikona:
						return EventNature.Good;
					case PlanetToSignRelationship.NeutralVarga:
						return EventNature.Neutral;
					case PlanetToSignRelationship.EnemyVarga:
					case PlanetToSignRelationship.BitterEnemyVarga:
						return EventNature.Bad;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			//else return nature from lagna
			return planetNature;


			//LOCAL FUNCTIONS

			EventNature GetNatureFromLagna()
			{
				var personLagna = HouseSignName(HouseName.House1, person.BirthTime);

				//get list of good and bad planets for a lagna
				dynamic planetData = GetPlanetData(personLagna);
				List<PlanetName> goodPlanets = planetData.Good;
				List<PlanetName> badPlanets = planetData.Bad;

				//check good planets first
				if (goodPlanets.Contains(planet))
				{
					return EventNature.Good;
				}

				//check bad planets next
				if (badPlanets.Contains(planet))
				{
					return EventNature.Bad;
				}

				//if control reaches here, then planet not
				//listed as good or bad, so just say neutral
				return EventNature.Neutral;
			}

			// data from chapter "Key-planets for Each Sign"
			object GetPlanetData(ZodiacName lagna)
			{
				List<PlanetName> good = null;
				List<PlanetName> bad = null;

				switch (lagna)
				{
					//Aries - Saturn, Mercury and Venus are ill-disposed.
					// Jupiter and the Sun are auspicious. The mere combination
					// of Jupiler and Saturn produces no beneficial results. Jupiter
					// is the Yogakaraka or the planet producing success. If Venus
					// becomes a maraka, he will not kill the native but planets like
					// Saturn will bring about death to the person.
					case ZodiacName.Aries:
						good = new List<PlanetName>() { Jupiter, Sun };
						bad = new List<PlanetName>() { Saturn, Mercury, Venus };
						break;
					//Taurus - Saturn is the most auspicious and powerful
					// planet. Jupiter, Venus and the Moon are evil planets. Saturn
					// alone produces Rajayoga. The native will be killed in the
					// periods and sub-periods of Jupiter, Venus and the Moon if
					// they get death-inflicting powers.
					case ZodiacName.Taurus:
						good = new List<PlanetName>() { Saturn };
						bad = new List<PlanetName>() { Jupiter, Venus, Moon };
						break;
					//Gemini - Mars, Jupiter and the Sun are evil. Venus alone
					// is most beneficial and in conjunction with Saturn in good signs
					// produces and excellent career of much fame. Combination
					// of Saturn and Jupiter produces similar results as in Aries.
					// Venus and Mercury, when well associated, cause Rajayoga.
					// The Moon will not kill the person even though possessed of
					// death-inflicting powers.
					case ZodiacName.Gemini:
						good = new List<PlanetName>() { Venus };
						bad = new List<PlanetName>() { Mars, Jupiter, Sun };
						break;
					//Cancer - Venus and Mercury are evil. Jupiter and Mars
					// give beneficial results. Mars is the Rajayogakaraka
					// (conferor of name and fame). The combination of Mars and Jupiter
					// also causes Rajayoga (combination for political success). The
					// Sun does not kill the person although possessed of maraka
					// powers. Venus and other inauspicious planets kill the native.
					// Mars in combination with the Moon or Jupiter in favourable
					// houses especially the 1st, the 5th, the 9th and the 10th
					// produces much reputation.
					case ZodiacName.Cancer:
						good = new List<PlanetName>() { Jupiter, Mars };
						bad = new List<PlanetName>() { Venus, Mercury };
						break;
					//Leo - Mars is the most auspicious and favourable planet.
					// The combination of Venus and Jupiter does not cause Rajayoga
					// but the conjunction of Jupiter and Mars in favourable
					// houses produce Rajayoga. Saturn, Venus and Mercury are
					// evil. Saturn does not kill the native when he has the maraka
					// power but Mercury and other evil planets inflict death when
					// they get maraka powers.
					case ZodiacName.Leo:
						good = new List<PlanetName>() { Mars };
						bad = new List<PlanetName>() { Saturn, Venus, Mercury };
						break;
					//Virgo - Venus alone is the most powerful. Mercury and
					// Venus when combined together cause Rajayoga. Mars and
					// the Moon are evil. The Sun does not kill the native even if
					// be becomes a maraka but Venus, the Moon and Jupiter will
					// inflict death when they are possessed of death-infticting power.
					case ZodiacName.Virgo:
						good = new List<PlanetName>() { Venus };
						bad = new List<PlanetName>() { Mars, Moon };
						break;
					// Libra - Saturn alone causes Rajayoga. Jupiter, the Sun
					// and Mars are inauspicious. Mercury and Saturn produce good.
					// The conjunction of the Moon and Mercury produces Rajayoga.
					// Mars himself will not kill the person. Jupiter, Venus
					// and Mars when possessed of maraka powers certainly kill the
					// nalive.
					case ZodiacName.Libra:
						good = new List<PlanetName>() { Saturn, Mercury };
						bad = new List<PlanetName>() { Jupiter, Sun, Mars };
						break;
					//Scorpio - Jupiter is beneficial. The Sun and the Moon
					// produce Rajayoga. Mercury and Venus are evil. Jupiter,
					// even if be becomes a maraka, does not inflict death. Mercury
					// and other evil planets, when they get death-inlflicting powers,
					// do not certainly spare the native.
					case ZodiacName.Scorpio:
						good = new List<PlanetName>() { Jupiter };
						bad = new List<PlanetName>() { Mercury, Venus };
						break;
					//Sagittarius - Mars is the best planet and in conjunction
					// with Jupiter, produces much good. The Sun and Mars also
					// produce good. Venus is evil. When the Sun and Mars
					// combine together they produce Rajayoga. Saturn does not
					// bring about death even when he is a maraka. But Venus
					// causes death when be gets jurisdiction as a maraka planet.
					case ZodiacName.Sagittarius:
						good = new List<PlanetName>() { Mars };
						bad = new List<PlanetName>() { Venus };
						break;
					//Capricornus - Venus is the most powerful planet and in
					// conjunction with Mercury produces Rajayoga. Mars, Jupiter
					// and the Moon are evil.
					case ZodiacName.Capricornus:
						good = new List<PlanetName>() { Venus };
						bad = new List<PlanetName>() { Mars, Jupiter, Moon };
						break;
					//Aquarius - Venus alone is auspicious. The combination of
					// Venus and Mars causes Rajayoga. Jupiter and the Moon are
					// evil.
					case ZodiacName.Aquarius:
						good = new List<PlanetName>() { Venus };
						bad = new List<PlanetName>() { Jupiter, Moon };
						break;
					//Pisces - The Moon and Mars are auspicious. Mars is
					// most powerful. Mars with the Moon or Jupiter causes Rajayoga.
					// Saturn, Venus, the Sun and Mercury are evil. Mars
					// himself does not kill the person even if he is a maraka.
					case ZodiacName.Pisces:
						good = new List<PlanetName>() { Moon, Mars };
						bad = new List<PlanetName>() { Saturn, Venus, Sun, Mercury };
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}


				return new { Good = good, Bad = bad };

			}
		}

		/// <summary>
		/// Soumyas
		/// Source : Astrology for beginners pg 30
		/// </summary>
		public static bool IsPlanetBeneficToLagna(PlanetName planetName, ZodiacName lagna)
		{
			switch (lagna)
			{
				case ZodiacName.Aries:
					return planetName == Sun || planetName == Mars || planetName == Jupiter;
				case ZodiacName.Taurus:
					return planetName == Sun || planetName == Mars
																|| planetName == Mercury || planetName == Saturn;
				case ZodiacName.Gemini:
					return planetName == Venus || planetName == Saturn;
				case ZodiacName.Cancer:
					return planetName == Mars || planetName == Jupiter;
				case ZodiacName.Leo:
					return planetName == Sun || planetName == Mars;
				case ZodiacName.Virgo:
					return planetName == Venus;
				case ZodiacName.Libra:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
				case ZodiacName.Scorpio:
					return planetName == Jupiter || planetName == Sun || planetName == Moon;
				case ZodiacName.Sagittarius:
					return planetName == Sun || planetName == Mars;
				case ZodiacName.Capricornus:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
				case ZodiacName.Aquarius:
					return planetName == Venus || planetName == Mars
																  || planetName == Sun || planetName == Saturn;
				case ZodiacName.Pisces:
					return planetName == Mars || planetName == Moon;
			}

			//control should not come here
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Kruras (Malefics)
		/// Source : Astrology for beginners pg 30
		/// </summary>
		public static bool IsPlanetMaleficToLagna(PlanetName planetName, ZodiacName lagna)
		{
			switch (lagna)
			{
				case ZodiacName.Aries:
					return planetName == Venus || planetName == Mercury || planetName == Saturn;
				case ZodiacName.Taurus:
					return planetName == Moon || planetName == Jupiter || planetName == Venus;
				case ZodiacName.Gemini:
					return planetName == Sun || planetName == Mars || planetName == Jupiter;
				case ZodiacName.Cancer:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
				case ZodiacName.Leo:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
				case ZodiacName.Virgo:
					return planetName == Mars || planetName == Moon || planetName == Jupiter;
				case ZodiacName.Libra:
					return planetName == Sun || planetName == Moon || planetName == Jupiter;
				case ZodiacName.Scorpio:
					return planetName == Mercury || planetName == Saturn;
				case ZodiacName.Sagittarius:
					return planetName == Saturn || planetName == Venus || planetName == Mercury;
				case ZodiacName.Capricornus:
					return planetName == Moon || planetName == Mars || planetName == Jupiter;
				case ZodiacName.Aquarius:
					return planetName == Jupiter || planetName == Moon;
				case ZodiacName.Pisces:
					return planetName == Sun || planetName == Mercury
																|| planetName == Venus || planetName == Saturn;
			}

			//control should not come here
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Yogakaraka (Planets indicating prosperity)
		/// Source : Astrology for beginners pg 30
		/// </summary>
		public static bool IsPlanetYogakarakaToLagna(PlanetName planetName, ZodiacName lagna)
		{
			switch (lagna)
			{
				case ZodiacName.Aries:
					return planetName == Sun;
				case ZodiacName.Taurus:
					return planetName == Saturn;
				case ZodiacName.Gemini:
					return planetName == Venus || planetName == Saturn;
				case ZodiacName.Cancer:
					return planetName == Mars;
				case ZodiacName.Leo:
					return planetName == Mars;
				case ZodiacName.Virgo:
					return planetName == Mercury || planetName == Venus;
				case ZodiacName.Libra:
					return planetName == Moon || planetName == Mercury || planetName == Saturn;
				case ZodiacName.Scorpio:
					return planetName == Sun || planetName == Moon;
				case ZodiacName.Sagittarius:
					return planetName == Sun || planetName == Mars;
				case ZodiacName.Capricornus:
					return planetName == Mercury || planetName == Venus;
				case ZodiacName.Aquarius:
					return planetName == Mars || planetName == Venus;
				case ZodiacName.Pisces:
					return planetName == Mars || planetName == Jupiter || planetName == Moon;
			}

			//control should not come here
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Yogakaraka (Planets indicating prosperity)
		/// Source : Astrology for beginners pg 30
		/// </summary>
		public static bool IsPlanetMarakaToLagna(PlanetName planetName, ZodiacName lagna)
		{
			switch (lagna)
			{
				case ZodiacName.Aries:
					return planetName == Mercury || planetName == Saturn;
				case ZodiacName.Taurus:
					return planetName == Jupiter || planetName == Venus;
				case ZodiacName.Gemini:
					return planetName == Mars || planetName == Jupiter;
				case ZodiacName.Cancer:
					return planetName == Mercury || planetName == Venus;
				case ZodiacName.Leo:
					return planetName == Mercury || planetName == Venus;
				case ZodiacName.Virgo:
					return planetName == Mars || planetName == Jupiter;
				case ZodiacName.Libra:
					return planetName == Jupiter;
				case ZodiacName.Scorpio:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
				case ZodiacName.Sagittarius:
					return planetName == Venus || planetName == Saturn;
				case ZodiacName.Capricornus:
					return planetName == Mars || planetName == Jupiter;
				case ZodiacName.Aquarius:
					return planetName == Mars;
				case ZodiacName.Pisces:
					return planetName == Mercury || planetName == Venus || planetName == Saturn;
			}

			//control should not come here
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Checks if planet is placed in own house
		/// meaning house sign owned by planet
		/// note: rahu and ketu return false always
		/// </summary>
		[API("Return true if planet is own house sign, planet is owner. Rahu and Ketu return false always")]
		public static bool IsPlanetInOwnHouse(Time time, PlanetName planetName)
		{
			//find out if planet is rahu or ketu, because not all calculations supported
			var isRahuKetu = planetName == Rahu || planetName == Ketu;

			//get current house
			var _planetCurrentHouse = HousePlanetIsIn(time, planetName);

			//relationship with current house
			var _currentHouseRelation = isRahuKetu ? 0 : PlanetRelationshipWithHouse(_planetCurrentHouse, planetName, time);

			//relation should be own
			if (_currentHouseRelation == PlanetToSignRelationship.OwnVarga)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// True if a planet is in a house sign owned by an enemy. Rahu and Ketu is false always
		/// </summary>
		[API("True if a planet is in a house sign owned by an enemy. Rahu and Ketu is false always")]
		public static bool IsPlanetInEnemyHouse(Time time, PlanetName planetName)
		{
			//find out if planet is rahu or ketu, because not all calculations supported
			var isRahuKetu = planetName == Rahu || planetName == Ketu;

			//get current house
			var _planetCurrentHouse = HousePlanetIsIn(time, planetName);

			//relationship with current house
			var _currentHouseRelation = isRahuKetu ? 0 : PlanetRelationshipWithHouse(_planetCurrentHouse, planetName, time);

			//relation should be own
			var isEnemy = _currentHouseRelation == PlanetToSignRelationship.EnemyVarga;
			var isSuperEnemy = _currentHouseRelation == PlanetToSignRelationship.BitterEnemyVarga;
			if (isEnemy || isSuperEnemy)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// True if a planet is in a house sign owned by a friend. Rahu and Ketu is false always
		/// </summary>
		[API("True if a planet is in a house sign owned by an friend. Rahu and Ketu is false always")]
		public static bool IsPlanetInFriendHouse(Time time, PlanetName planetName)
		{
			//find out if planet is rahu or ketu, because not all calculations supported
			var isRahuKetu = planetName == Rahu || planetName == Ketu;

			//get current house
			var _planetCurrentHouse = HousePlanetIsIn(time, planetName);

			//relationship with current house
			var _currentHouseRelation = isRahuKetu ? 0 : PlanetRelationshipWithHouse(_planetCurrentHouse, planetName, time);

			//relation should be own
			var isEnemy = _currentHouseRelation == PlanetToSignRelationship.EnemyVarga;
			var isSuperEnemy = _currentHouseRelation == PlanetToSignRelationship.BitterEnemyVarga;
			if (isEnemy || isSuperEnemy)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Swiss Ephemeris "swe_calc" wrapper for open API 
		/// </summary>
		[API("Get planet's Longitude, Latitude, DistanceAU, SpeedLongitude, SpeedLatitude...")]
		public static dynamic SwissEphemeris(Time time, PlanetName planetName)
		{
			//Converts LMT to UTC (GMT)
			//DateTimeOffset utcDate = lmtDateTime.ToUniversalTime();

			int iflag = 2;//SwissEph.SEFLG_SWIEPH;  //+ SwissEph.SEFLG_SPEED;
			double[] results = new double[6];
			string err_msg = "";
			double jul_day_ET;
			SwissEph ephemeris = new SwissEph();

			// Convert DOB to ET
			jul_day_ET = TimeToEphemerisTime(time);

			//convert planet name, compatible with Swiss Eph
			int swissPlanet = Tools.VedAstroToSwissEph(planetName);

			//Get planet long
			int ret_flag = ephemeris.swe_calc(jul_day_ET, swissPlanet, iflag, results, ref err_msg);

			//data in results at index 0 is longitude
			var sweCalcResults = new
			{
				Longitude = results[0],
				Latitude = results[1],
				DistanceAU = results[2],
				SpeedLongitude = results[3],
				SpeedLatitude = results[4],
				SpeedDistance = results[5]
			};

			return sweCalcResults;
		}

		/// <summary>
		/// Checks if a planet is same house (not nessarly conjunct) with the lord of a certain house
		/// Example : Is Sun joined with lord of 9th?
		/// </summary>
		public static bool IsPlanetSameHouseWithHouseLord(Time birthTime, int houseNumber, PlanetName planet)
		{
			//get house of the lord in question
			var houseLord = LordOfHouse((HouseName)houseNumber, birthTime);
			var houseLordHouse = HousePlanetIsIn(birthTime, houseLord);

			//get house of input planet
			var inputPlanetHouse = HousePlanetIsIn(birthTime, planet);

			//check if both are in same house
			if (inputPlanetHouse == houseLordHouse)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		public static IEnumerable<MethodInfo> TimeHouseCalcs()
		{
			var returnList = new List<MethodInfo>();

			//get all calculators that can work with the inputed data
			var calculatorClass = typeof(Calculate);

			var calculators1 = from calculatorInfo in calculatorClass.GetMethods()
							   let parameter = calculatorInfo.GetParameters()
							   where parameter.Length == 2 //only 2 params
									 && parameter[0].ParameterType == typeof(HouseName)  //planet name
									 && parameter[1].ParameterType == typeof(Time)        //birth time
							   select calculatorInfo;

			//second possible order, technically should be aligned todo
			var calculators3 = from calculatorInfo in calculatorClass.GetMethods()
							   let parameter = calculatorInfo.GetParameters()
							   where parameter.Length == 2 //only 2 params
									 && parameter[0].ParameterType == typeof(Time)  //birth time
									 && parameter[1].ParameterType == typeof(HouseName)        //planet name
							   select calculatorInfo;

			//these are for calculators with static tag data
			var calculators2 = from calculatorInfo in calculatorClass.GetMethods()
							   let parameter = calculatorInfo.GetParameters()
							   where parameter.Length == 1 //only 2 params
									 && parameter[0].ParameterType == typeof(HouseName)  //planet name
							   select calculatorInfo;


			returnList.AddRange(calculators1);
			returnList.AddRange(calculators2);
			returnList.AddRange(calculators3);

			return returnList;

		}

		/// <summary>
		/// Based on Shadvarga get nature of house for a person,
		/// nature in number form to for easy calculation into summary
		/// good = 1, bad = -1, neutral = 0
		/// specially made method for life chart summary
		/// </summary>
		public static int HouseNatureScore(Time personBirthTime, HouseName inputHouse)
		{
			//if no house then no score
			if (inputHouse == HouseName.Empty)
			{
				return 0;
			}

			//get house score
			var houseStrength = HouseStrength(inputHouse, personBirthTime).ToDouble();

			//based on score determine final nature
			switch (houseStrength)
			{
				//positive
				case > 550: return 2; //extra for power
				case >= 450: return 1;

				//negative
				case < 250: return -3; //if below is even worse
				case < 350: return -2; //if below is even worse
				case < 450: return -1;
				default:
					throw new Exception("No Strength Power defined!");
			}
		}

		public static double HouseNatureScoreMK4(Time personBirthTime, HouseName inputHouse)
		{
			//if no house then no score
			if (inputHouse == HouseName.Empty)
			{
				return 0;
			}

			//get house score
			var houseStrength = HouseStrength(inputHouse, personBirthTime).ToDouble();

			//weakest planet gives lowest score -2
			//strongest planet gives highest score 2
			//get range
			var highestHouseScore = HouseStrength(GetAllHousesOrderedByStrength(personBirthTime)[0], personBirthTime).ToDouble();
			var lowestHouseScore = HouseStrength(GetAllHousesOrderedByStrength(personBirthTime)[11], personBirthTime).ToDouble();

			var rangeBasedScore = houseStrength.Remap(lowestHouseScore, highestHouseScore, -3, 3);


			return rangeBasedScore;
		}

		public static double PlanetNatureScoreMK4(Time personBirthTime, PlanetName inputPlanet)
		{
			//if no house then no score
			if (inputPlanet == Empty) { return 0; }

			//get house score
			//var planetStrength = GetPlanetShadbalaPinda(inputPlanet, personBirthTime).ToDouble();

			//weakest planet gives lowest score -2
			//strongest planet gives highest score 2
			//get range
			//var highestPlanetScore = GetPlanetShadbalaPinda(GetAllPlanetOrderedByStrength(personBirthTime)[0], personBirthTime).ToDouble();
			//var weakestPlanet = GetAllPlanetOrderedByStrength(personBirthTime)[8];
			//var lowestPlanetScore = GetPlanetShadbalaPinda(weakestPlanet, personBirthTime).ToDouble();

			//find accurate planet strength relative to others
			//if above limit than strong else weak below 0
			var isBenefic = IsPlanetBeneficInShadbala(inputPlanet, personBirthTime);
			//var rangeBasedScore = 0.0;

			var x = isBenefic ? 1 : -1;

			return x;

			//if (isBenefic) //positive number
			//{
			//     rangeBasedScore = planetStrength.Remap(lowestPlanetScore, highestPlanetScore, 0, 2);

			//}
			//else // 0 or below
			//{
			//     rangeBasedScore = planetStrength.Remap(lowestPlanetScore, highestPlanetScore, -2, 0);
			//}

			//return rangeBasedScore;
		}

		/// <summary>
		/// Based on Shadvarga get nature of planet for a person,
		/// nature in number form to for easy calculation into summary
		/// good = 1, bad = -1, neutral = 0
		/// specially made method for life chart summary
		/// </summary>
		public static int PlanetNatureScore(Time personBirthTime, PlanetName inputPlanet)
		{
			//get house score
			var planetStrength = PlanetShadbalaPinda(inputPlanet, personBirthTime).ToDouble();


			//based on score determine final nature
			switch (planetStrength)
			{
				//positive
				case > 550: return 2; //extra for power
				case >= 450: return 1;

				//negative
				case < 250: return -3; //if below is even worse
				case < 350: return -2; //if below is even worse
				case < 450: return -1;
				default:
					throw new Exception("No Strength Power defined!");
			}
		}

		/// <summary>
		/// Get a person's varna or color (character)
		/// A person's varna can be observed in real life
		/// </summary>
		[API("Get a person's varna or color (character), from birth time")]
		public static Varna BirthVarna(Time birthTime)
		{
			//get ruling sign
			var ruleSign = PlanetRasiSign(Moon, birthTime).GetSignName();

			//get grade
			var maleGrade = GetGrade(ruleSign);

			return maleGrade;

			//higher grade is higher class
			Varna GetGrade(ZodiacName sign)
			{
				switch (sign)
				{   //Pisces, Scorpio and Cancer represent the highest development - Brahmin 
					case ZodiacName.Pisces:
					case ZodiacName.Scorpio:
					case ZodiacName.Cancer:
						return Varna.BrahminScholar;

					//Leo, Sagittarius and Libra indicate the second grade - or Kshatriya;
					case ZodiacName.Leo:
					case ZodiacName.Sagittarius:
					case ZodiacName.Libra:
						return Varna.KshatriyaWarrior;

					//Aries, Gemini and Aquarius suggest the third or the Vaisya;
					case ZodiacName.Aries:
					case ZodiacName.Gemini:
					case ZodiacName.Aquarius:
						return Varna.VaisyaWorkmen;

					//while Taurus, Virgo and Capricorn indicate the last grade, viz., Sudra
					case ZodiacName.Taurus:
					case ZodiacName.Virgo:
					case ZodiacName.Capricornus:
						return Varna.SudraServant;

					default: throw new Exception("");
				}
			}


		}

		/// <summary>
		/// Used for judging dasa good or bad, Bala book pg 110
		/// if planet has more Ishta than good = +1
		/// else if more Kashta than bad = -1
		/// </summary>
		public static double PlanetIshtaKashtaScore(PlanetName planet, Time birthTime)
		{
			var ishtaScore = PlanetIshtaScore(planet, birthTime);

			var kashtaScore = PlanetKashtaScore(planet, birthTime);

			//if more than good, else bad
			var ishtaMore = ishtaScore > kashtaScore;

			return ishtaMore ? 1 : -1;
		}

		[API("Kashta Phala (Bad Strength) of a Planet")]
		public static double PlanetKashtaScore(PlanetName planet, Time birthTime)
		{
			return 0;
			throw new NotImplementedException();
		}

		[API("Ishta Phala (Good Strength) of a Planet")]
		public static double PlanetIshtaScore(PlanetName planet, Time birthTime)
		{
			//The Ochcha Bala (exaltation strength) of a planet
			//is multiplied by its Chesta Bala(motional strength)
			//and then the square root of the product extracted.
			var ochchaBala = PlanetOchchaBala(planet, birthTime).ToDouble();
			var chestaBala = PlanetChestaBala(planet, birthTime, includeSunMoon: true).ToDouble();
			var product = ochchaBala * chestaBala;

			//Square root of the product extracted.
			//the result would represent the Ishta Phala.
			var ishtaScore = Math.Sqrt(product);

			return ishtaScore;
		}

	}
}


//--------------ARCHIVED CODE



//POSIBBLY VERY WRONG 
//public static bool IsMoonWeak(Time time)
//{
//    //Moon is weak or new moon from the 10th day of the dark half of the lunar month
//    //to the 5th day of the bright half of the next lunar month

//    //set moon as not weak at first
//    var moonIsWeak = false;

//    //get the lunar date number
//    int lunarDateNumber = AstronomicalCalculator.GetLunarDay(time).GetLunarDateNumber();

//    //10th day of dark half to 15th day of dark half = lunar date number 25 to 30
//    if (lunarDateNumber >= 25 && lunarDateNumber <= 30)
//    {
//        moonIsWeak = true;
//    }

//    //1st day of bright half to 5th day of bright half = lunar date number 1 to 5
//    if (lunarDateNumber >= 1 && lunarDateNumber <= 5)
//    {
//        moonIsWeak = true;
//    }

//    return moonIsWeak;
//}

//public static bool IsMoonStrong(Time time)
//{
//    //Moon is full from the 10th day of the bright half
//    //of the lunar month to the 5th day of the dark half
//    //of the same.
//    throw new NotImplementedException();
//}
