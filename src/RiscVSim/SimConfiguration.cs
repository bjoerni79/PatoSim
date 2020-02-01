using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace RiscVSim
{
    public class SimConfiguration
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public const string Filename = "simulation.xml";

        private string cpu;
        private string debug;
        private string memory;
        private string rvMode;
        private string rvModeL;
        private string verbose;

        public SimConfiguration()
        {

        }

        public void Init()
        {
            var fileExists = File.Exists(Filename);
            if (!fileExists)
            {
                Create();
            }
            else
            {
                Logger.Info("Config file detected");
            }

            /*
             * 
             * <?xml version="1.0" encoding="utf-8"?>
                <root>
                  <default>
                    <CPU>RV64I</CPU>
                    <Debug>Off</Debug>
                    <Verbose>Off</Verbose>
                    <Memory>Dynamic</Memory>
                    <RvMode>Off</RvMode>
                    <RvModeL>0</RvModeL>
                  </default>
                </root>
             */

            Logger.Info("Load Defaults");

            using (var configFileReader = File.OpenText(Filename))
            {
                // Create an XPath document and parse the file
                var document = new XPathDocument(configFileReader);
                var navigator = document.CreateNavigator();

                var cpuNav = navigator.SelectSingleNode("root/default/CPU");
                cpu = cpuNav.Value;
                Logger.Info("CPU = {cpu}", cpu);

                var verboseNav = navigator.SelectSingleNode("root/default/Verbose");
                verbose = verboseNav.Value;
                Logger.Info("Verbose = {verbose}", verbose);

                var debugNav = navigator.SelectSingleNode("root/default/Debug");
                debug = debugNav.Value;
                Logger.Info("Debug = {debug}", debug);

                var memoryNav = navigator.SelectSingleNode("root/default/Memory");
                memory = memoryNav.Value;
                Logger.Info("Memory = {memory}", memory);

                var rvModeNav = navigator.SelectSingleNode("root/default/RvMode");
                rvMode = rvModeNav.Value;

                var rvModeLNav = navigator.SelectSingleNode("root/default/RvModeL");
                rvModeL = rvModeLNav.Value;
                Logger.Info("RvMode = {mode} with -L = {lp}", rvMode, rvModeL);

            }

            

        }

        public string GetCpu()
        {
            return cpu;
        }

        public string GetDebug()
        {
            return debug;
        }

        public string GetMemory()
        {
            return memory;
        }

        public string GetVerbose()
        {
            return verbose;
        }

        public string GetRvMode()
        {
            return rvMode;
        }

        public string GetRvModeL()
        {
            return rvModeL;
        }

        private void Create()
        {
            Logger.Info("Create new config file");

            var setting = new XmlWriterSettings()
            {
                NewLineHandling = NewLineHandling.Entitize,
                 Indent=true
            };
            using (var writer = XmlWriter.Create(Filename,setting))
            {
                writer.WriteStartElement("root");
                writer.WriteStartElement("default");

                // Write CPU
                writer.WriteElementString("CPU", "RV32I");

                // Write Debug
                writer.WriteElementString("Debug", "Off");

                // Write Verbose
                writer.WriteElementString("Verbose", "Off");

                // Write Memory
                writer.WriteElementString("Memory", "Dynamic");

                // Write RVMode
                writer.WriteElementString("RvMode", "On");

                // Write RVModeL
                writer.WriteElementString("RvModeL", "0");

                //default
                writer.WriteEndElement();
                // root
                writer.WriteEndElement();
            }
        }
    }
}
