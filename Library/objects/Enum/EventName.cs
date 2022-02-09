﻿namespace Genso.Astrology.Library
{
    /// <summary>
    /// Staticly typed name list of events,
    /// This is not the primarly list, not all events here will be checked, only events in the XML list are checked
    /// Not all events here have to have a method
    /// </summary>
    public enum EventName
    {
        EmptyEvent,
        //Lunar Day
        LunarDay1_1stBrightHalf,
        LunarDay2_2ndBrightHalf,
        LunarDay3_3rdBrightHalf,
        LunarDay4_4thBrightHalf,
        LunarDay5_5thBrightHalf,
        LunarDay6_6thBrightHalf,
        LunarDay7_7thBrightHalf,
        LunarDay8_8thBrightHalf,
        LunarDay9_9thBrightHalf,
        LunarDay10_10thBrightHalf,
        LunarDay11_11thBrightHalf,
        LunarDay12_12thBrightHalf,
        LunarDay13_13thBrightHalf,
        LunarDay14_14thBrightHalf,
        LunarDay15_FullMoon,
        LunarDay16_1stDarkHalf,
        LunarDay17_2ndDarkHalf,
        LunarDay18_3rdDarkHalf,
        LunarDay19_4thDarkHalf,
        LunarDay20_5thDarkHalf,
        LunarDay21_6thDarkHalf,
        LunarDay22_7thDarkHalf,
        LunarDay23_8thDarkHalf,
        LunarDay24_9thDarkHalf,
        LunarDay25_10thDarkHalf,
        LunarDay26_11thDarkHalf,
        LunarDay27_12thDarkHalf,
        LunarDay28_13thDarkHalf,
        LunarDay29_14thDarkHalf,
        LunarDay30_NewMoon,

        GoodTarabala,
        BadTarabala,
        GoodChandrabala,
        BadChandrabala,
        SiddhaYogaSunday,
        SiddhaYogaMonday,
        SiddhaYogaTuesday,
        SiddhaYogaWednesday,
        SiddhaYogaThursday,
        SiddhaYogaFriday,
        SiddhaYogaSaturday,
        AmritaSiddhaYoga,
        PanchangaSuddhi,
        UgraYoga,
        SuryaSankramana,
        KarthariDosha,
        ShashtashtaRiphagathaChandraDosha,
        SagrahaChandraDosha,
        UdayasthaSuddhi,
        SiddhaYoga,
        SakunaKarana,
        BadNithyaYoga,
        LagnaThyajya,
        GoodPanchaka,
        BadPanchaka,
        BadTaraChandraPanchaka,
        GoodTaraChandraPanchaka,
        GoodTaraChandra,
        BadTaraChandra,
        BhriguShatka,
        Kujasthama,
        GoodHairCutting,
        GoodNailCutting,
        FixedConstellationRuling,
        SoftConstellationRuling,
        LightConstellationRuling,
        SharpConstellationRuling,
        MovableConstellationRuling,
        DreadfulConstellationRuling,
        MixedConstellationRuling,
        BadForStartingAllAgriculture,
        LagnaLordIsWeekdayLord,
        GoodLunarDayAgriculture,
        BadLagnaForAllAgriculture,
        GoodYogaForAllAgriculture,
        GoodAnySeedsSowing,
        Ekadashi,
        NotAuspiciousDay,
        GoodPlanetsInLagna,
        GoodForPlantingFloweringPlants,
        GoodForPlantingGarlic,
        GoodForPlantingPeachAndOthers,
        GoodForPlantingTomatoesAndOthers,
        GoodForPlantingGrains,
        GoodForPlantingOnionAndOthers,
        GoodForPlantingPepperAndOthers,
        GoodForPlantingPotatoAndOthers,
        GoodForPlantingGrainsAndOthers,
        GoodForPlantingPumpkinsAndOthers,
        GoodForPlantingTrees,
        GoodForPlantingFlowerSeeds,
        GoodForPlantingSugarcane,
        GoodForPlantingFruitTrees,
        GoodForPlantingFlowerTrees,
        GoodForPlantingFlowers,
        GoodForPlantingFlowerCuttings,
        GoodRullingConstellation,
        BadRullingConstellation,
        GoodTakingInjections,
        GoodSellingForProfit,
        BavaKarana,
        TaitulaKarana,
        BhadraKarana,
        JanmaNakshatraRulling,
        SunIsStrong,
        MoonIsStrong,
        MarsIsStrong,
        MercuryIsStrong,
        JupiterIsStrong,
        VenusIsStrong,
        SaturnIsStrong,
        House1IsStrong,
        House2IsStrong,
        House3IsStrong,
        House4IsStrong,
        House5IsStrong,
        House6IsStrong,
        House7IsStrong,
        House8IsStrong,
        House9IsStrong,
        House10IsStrong,
        House11IsStrong,
        House12IsStrong,
        BadForBuyingToolsUtensilsJewellery,
        GoodForBuyingBrassVessels,
        GoodForBuyingCopperVessels,
        GoodForBuyingSteelIronVessels,
        GoodForBuyingSilverVessels,
        GoodForBuyingJewellery,
        GoodPlanetsIn11th,
        GoodPlanetsInKendra,
        Sunrise,
        Sunset,
        Midday,
        TarabalaJanmaStrong,
        TarabalaSampatStrong,
        TarabalaVipatStrong,
        TarabalaKshemaStrong,
        TarabalaPratyakStrong,
        TarabalaSadhanaStrong,
        TarabalaNaidhanaStrong,
        TarabalaMitraStrong,
        TarabalaParamaMitraStrong,
        TarabalaJanmaMiddling,
        TarabalaSampatMiddling,
        TarabalaVipatMiddling,
        TarabalaKshemaMiddling,
        TarabalaPratyakMiddling,
        TarabalaSadhanaMiddling,
        TarabalaNaidhanaMiddling,
        TarabalaMitraMiddling,
        TarabalaParamaMitraMiddling,
        TarabalaJanmaWeak,
        TarabalaSampatWeak,
        TarabalaVipatWeak,
        TarabalaKshemaWeak,
        TarabalaPratyakWeak,
        TarabalaSadhanaWeak,
        TarabalaNaidhanaWeak,
        TarabalaMitraWeak,
        TarabalaParamaMitraWeak,
        Papashadvargas,
        House1LordInHouse1,
        House1LordInHouse2,
        House1LordInHouse3,
        House1LordInHouse4,
        House1LordInHouse5,
        House1LordInHouse6,
        House1LordInHouse7,
        House1LordInHouse8,
        House1LordInHouse9,
        House1LordInHouse10,
        House1LordInHouse11,
        House1LordInHouse12,
        House2LordInHouse1,
        House2LordInHouse2,
        House2LordInHouse3,
        House2LordInHouse4,
        House2LordInHouse5,
        House2LordInHouse6,
        House2LordInHouse7,
        House2LordInHouse8,
        House2LordInHouse9,
        House2LordInHouse10,
        House2LordInHouse11,
        House2LordInHouse12,
        House3LordInHouse1,
        House3LordInHouse2,
        House3LordInHouse3,
        House3LordInHouse4,
        House3LordInHouse5,
        House3LordInHouse6,
        House3LordInHouse7,
        House3LordInHouse8,
        House3LordInHouse9,
        House3LordInHouse10,
        House3LordInHouse11,
        House3LordInHouse12,
        CombinedEvent,
        GoodMoonSignForSelling,
        GoodWeekdayForSelling,
        House4LordInHouse1,
        House4LordInHouse2,
        House4LordInHouse3,
        House4LordInHouse4,
        House4LordInHouse5,
        House4LordInHouse6,
        House4LordInHouse7,
        House4LordInHouse8,
        House4LordInHouse9,
        House4LordInHouse10,
        House4LordInHouse11,
        House4LordInHouse12,
        House5LordInHouse1,
        House5LordInHouse2,
        House5LordInHouse3,
        House5LordInHouse4,
        House5LordInHouse5,
        House5LordInHouse6,
        House5LordInHouse7,
        House5LordInHouse8,
        House5LordInHouse9,
        House5LordInHouse10,
        House5LordInHouse11,
        House5LordInHouse12,
        House6LordInHouse1,
        House6LordInHouse2,
        House6LordInHouse3,
        House6LordInHouse4,
        House6LordInHouse5,
        House6LordInHouse6,
        House6LordInHouse7,
        House6LordInHouse8,
        House6LordInHouse9,
        House6LordInHouse10,
        House6LordInHouse11,
        House6LordInHouse12,
        House7LordInHouse1,
        House7LordInHouse2,
        House7LordInHouse3,
        House7LordInHouse4,
        House7LordInHouse5,
        House7LordInHouse6,
        House7LordInHouse7,
        House7LordInHouse8,
        House7LordInHouse9,
        House7LordInHouse10,
        House7LordInHouse11,
        House7LordInHouse12,
        House8LordInHouse1,
        House8LordInHouse2,
        House8LordInHouse3,
        House8LordInHouse4,
        House8LordInHouse5,
        House8LordInHouse6,
        House8LordInHouse7,
        House8LordInHouse8,
        House8LordInHouse9,
        House8LordInHouse10,
        House8LordInHouse11,
        House8LordInHouse12,
        House9LordInHouse1,
        House9LordInHouse2,
        House9LordInHouse3,
        House9LordInHouse4,
        House9LordInHouse5,
        House9LordInHouse6,
        House9LordInHouse7,
        House9LordInHouse8,
        House9LordInHouse9,
        House9LordInHouse10,
        House9LordInHouse11,
        House9LordInHouse12,
        House10LordInHouse1,
        House10LordInHouse2,
        House10LordInHouse3,
        House10LordInHouse4,
        House10LordInHouse5,
        House10LordInHouse6,
        House10LordInHouse7,
        House10LordInHouse8,
        House10LordInHouse9,
        House10LordInHouse10,
        House10LordInHouse11,
        House10LordInHouse12,
        House11LordInHouse1,
        House11LordInHouse2,
        House11LordInHouse3,
        House11LordInHouse4,
        House11LordInHouse5,
        House11LordInHouse6,
        House11LordInHouse7,
        House11LordInHouse8,
        House11LordInHouse9,
        House11LordInHouse10,
        House11LordInHouse11,
        House11LordInHouse12,
        House12LordInHouse1,
        House12LordInHouse2,
        House12LordInHouse3,
        House12LordInHouse4,
        House12LordInHouse5,
        House12LordInHouse6,
        House12LordInHouse7,
        House12LordInHouse8,
        House12LordInHouse9,
        House12LordInHouse10,
        House12LordInHouse11,
        House12LordInHouse12,
        AriesRising,
        TaurusRising,
        GeminiRising,
        CancerRising,
        LeoRising,
        VirgoRising,
        LibraRising,
        ScorpioRising,
        SagittariusRising,
        CapricornusRising,
        AquariusRising,
        PiscesRising,
        SunInHouse1,
        MoonInHouse1,
        MarsInHouse1,
        MercuryInHouse1,
        JupiterInHouse1,
        VenusInHouse1,
        SaturnInHouse1,
        RahuInHouse1,
        KetuInHouse1,
        SunInHouse2,
        MoonInHouse2,
        MarsInHouse2,
        MercuryInHouse2,
        JupiterInHouse2,
        VenusInHouse2,
        SaturnInHouse2,
        RahuInHouse2,
        KetuInHouse2,
        SunInHouse3,
        MoonInHouse3,
        MarsInHouse3,
        MercuryInHouse3,
        JupiterInHouse3,
        VenusInHouse3,
        SaturnInHouse3,
        RahuInHouse3,
        KetuInHouse3,
        SunInHouse4,
        MoonInHouse4,
        MarsInHouse4,
        MercuryInHouse4,
        JupiterInHouse4,
        VenusInHouse4,
        SaturnInHouse4,
        RahuInHouse4,
        KetuInHouse4,
        SunInHouse5,
        MoonInHouse5,
        MarsInHouse5,
        MercuryInHouse5,
        JupiterInHouse5,
        VenusInHouse5,
        SaturnInHouse5,
        RahuInHouse5,
        KetuInHouse5,
        SunInHouse6,
        MoonInHouse6,
        MarsInHouse6,
        MercuryInHouse6,
        JupiterInHouse6,
        VenusInHouse6,
        SaturnInHouse6,
        RahuInHouse6,
        KetuInHouse6,
        SunInHouse7,
        MoonInHouse7,
        MarsInHouse7,
        MercuryInHouse7,
        JupiterInHouse7,
        VenusInHouse7,
        SaturnInHouse7,
        RahuInHouse7,
        KetuInHouse7,
        SunInHouse8,
        MoonInHouse8,
        MarsInHouse8,
        MercuryInHouse8,
        JupiterInHouse8,
        VenusInHouse8,
        SaturnInHouse8,
        RahuInHouse8,
        KetuInHouse8,
        SunInHouse9,
        MoonInHouse9,
        MarsInHouse9,
        MercuryInHouse9,
        JupiterInHouse9,
        VenusInHouse9,
        SaturnInHouse9,
        RahuInHouse9,
        KetuInHouse9,
        SunInHouse10,
        MoonInHouse10,
        MarsInHouse10,
        MercuryInHouse10,
        JupiterInHouse10,
        VenusInHouse10,
        SaturnInHouse10,
        RahuInHouse10,
        KetuInHouse10,
        SunInHouse11,
        MoonInHouse11,
        MarsInHouse11,
        MercuryInHouse11,
        JupiterInHouse11,
        VenusInHouse11,
        SaturnInHouse11,
        RahuInHouse11,
        KetuInHouse11,
        SunInHouse12,
        MoonInHouse12,
        MarsInHouse12,
        MercuryInHouse12,
        JupiterInHouse12,
        VenusInHouse12,
        SaturnInHouse12,
        RahuInHouse12,
        KetuInHouse12,
        GeminiRisingWithEvilPlanet,
        AriesRisingWithEvilPlanet,
        Lord2WithEvilInHouse,
        SaturnIn2WithVenus,
        MoonMarsIn2WithSaturnAspect,
        MercuryAndEvilIn2WithMoonAspect,
        SunIn2WithNoSaturnAspect,
        MoonIn2WithMercuryAspect,
        Lord2And3In6WithEvilPlanet,
        Lord2InHouse1,
        Lord2InHouse1AndLord1InHouse2,
        Lord2InHouse3,
        Lord2InHouse4,
        Lord2InHouse5,
        Lord2InHouse6,
        Lord2InHouse7,
        Lord2InHouse8,
        Lord2InHouse9,
        Lord2InHouse10,
        Lord2InHouse11,
        Lord2InHouse12,
        MaleficIn11FromArudha,
        BeneficIn11FromArudha
    }
}