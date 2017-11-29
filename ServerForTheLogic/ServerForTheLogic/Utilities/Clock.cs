﻿using Newtonsoft.Json;
using ServerForTheLogic.Infrastructure;
using ServerForTheLogic.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ConsoleDump;
using ServerForTheLogic.Econ;
using CitySimNetworkService;
using System.IO;

namespace ServerForTheLogic.Utilities
{
    [JsonObject(MemberSerialization.OptIn)]
    /// <summary>
    /// Holds the current time.  Intended for use by the City.
    /// <para/> Last edited:  2017-10-02
    /// </summary>
    public class Clock
    {
        //[JsonProperty]
        //Updater<ClientPacket> FullUpdater;
        //[JsonProperty]
        //Updater<Dictionary<int, Dictionary<Guid, Point>>> PartialUpdater;

        [JsonProperty]
        // Ticks every second to update the current time values.
        public Timer timer;
        private City city;

        /// <summary>
        /// The number of minutes that have passed in the current hour.
        /// minutes are 0.5 seconds
        /// </summary>
        [JsonProperty]
        public UInt32 NetMinutes { get; set; }

        /// <summary>
        /// The total number of hours since this Clock started.
        /// hours are 30 seconds
        /// </summary>
        [JsonProperty]
        public UInt32 NetHours { get; set; }

        /// <summary>
        /// The total number of days since this Clock started.
        /// days are 12 min
        /// </summary>
        [JsonProperty]
        private UInt32 NetDays { get; set; }

        /// <summary>
        /// The total number of years since this Clock started.
        /// </summary>
        [JsonProperty]
        private UInt32 NetYears { get; set; }

        [JsonProperty]
        /// <summary>
        /// The number of milliseconds between Clock "ticks."  In this case, 1 second = 1000.
        /// </summary>
        public const int INTERVAL = 100;

        private SimulationStateQueue FullUpdate;

        private SimulationStateQueue PartialUpdate;
        /// <summary>
        /// Constructs a Clock object.
        /// <para>Written by Andrew Busto </para>
        /// </summary>
        public Clock(City city, SimulationStateQueue full, SimulationStateQueue partial)
        {

            FullUpdate = full;
            PartialUpdate = partial;
             
            this.city = city;
            timer = new Timer();
           

            timer.Interval = INTERVAL;
            timer.Elapsed += TickMinute;

            timer.AutoReset = true;
            timer.Stop();
        }

        /// <summary>
        /// Increments netMins.  Set as an event handler for timer.
        /// </summary>
        /// <para>Written by Andrew Busto </para>
        /// <para>Last modified by Justin McLennan 2017-11-21</para>
        /// <param name="source"> Unused. </param>
        /// <param name="e"> Unused .</param>
        public void TickMinute(Object source, ElapsedEventArgs e)
        {
            NetMinutes++;
           // Console.WriteLine("Mins:\t" + NetMinutes);

            if (NetMinutes / 60 > NetHours)
            {
                TickHour();
            }
        }

        /// <summary>
        /// Updates netHours. Calling methods that are run every hour
        /// <para>Written by Andrew Busto </para>
        /// <para>Last modified by Justin McLennan 2017-11-21</para>
        /// </summary>
        /// <para/> Last edited:  2017-11-12
        private void TickHour()
        {
            NetHours = NetMinutes / 60;
            Console.WriteLine("Hours:\t" + NetHours);
            //Updater<Dictionary<Guid, Point>> updater = new Updater<Dictionary<Guid, Point>>();

            Console.WriteLine("Population = " + city.AllPeople.Count);
            foreach (Person p in city.AllPeople)
            {
                p.ConsumeProd();
            }
            ClientPacket packet = new ClientPacket(city);
            //packet.ConvertPacket();

            string output = packet.ConvertPartialPacket();

            PartialUpdate.Enqueue(output);
            
            Console.WriteLine(output);
            //Console.WriteLine("Market checker " + Market.BusinessesHiring.Count);

            if (NetHours / 24 > NetDays)
            {
                TickDay();
            }

            //PartialUpdater.SendPartialUpdate(city.PartialUpdateList, Formatting.None);
        }


        /// <summary>
        /// Updates netDays.
        /// </summary>
        /// <para>Written by Andrew Busto </para>
        /// <para/> Last edited:  2017-11-07
        private void TickDay()
        {
            foreach (Person p in city.AllPeople)
            {
                if (p.AgeDeathTick())
                {
                    city.AllPeople.Remove(p);
                }
            }
            NetDays = NetHours / 24;
            Console.WriteLine("Days:\t" + NetDays);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new LocationConverter());
            settings.Converters.Add(new BlockConverter());

            JsonSerializer serializer = JsonSerializer.Create(settings);

            using (StreamWriter sw = new StreamWriter(@"..\..\SerializedCity\city.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, city);
                sw.Close();
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }

            //string cityJson = JsonConvert.SerializeObject(city, Formatting.Indented, settings);
            ClientPacket packet = new ClientPacket(city);
            //packet.ConvertPacket();

            string output = packet.ConvertFullPacket();
            FullUpdate.Enqueue(output);


            if (NetDays / 365 > NetYears)
            {
                TickYear();
            }

           // FullUpdater.SendFullUpdate(new ClientPacket(city), Formatting.Indented);
        }

        /// <summary>
        /// Updates netYears.
        /// </summary>
        /// <para>Written by Andrew Busto </para>
        /// <para/> Last edited:  2017-11-07
        private void TickYear()
        {
            NetYears++;
            Console.WriteLine("Years:\t" + NetYears);
            foreach (Person p in city.AllPeople)
            {
                p.SetAge();
            }
        }
    }
}
