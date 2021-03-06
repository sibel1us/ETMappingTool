﻿using System;
using System.Linq;
using ETMapHelper.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ETMapHelperTests
{
    [TestClass]
    public class MapParseTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var path = "E:/ET/map/ET/etmain/maps/fuelrun_test.map";
            var map = new Map(path);

            foreach (var ent in map.Entities.Where(e => e.ClassName == "light"))
            {
                int lightVal = int.Parse(ent.Props["light"]);
                int newVal = ((int)(lightVal * 1.2));

                System.Diagnostics.Debug.WriteLine($"Changed entity {ent.Id} light from {lightVal} to {newVal}");

                ent.Props["light"] = newVal.ToString();
            }

            map.WriteToFile(path);
        }

        // Everything commented out until some good test maps get sorted
        /*
        [TestMethod]
        public void ParseMap()
        {
            var path = "E:/ET/map/ET/etmain/maps/floodlit.map";
            var map = new Map(path);

            Console.WriteLine();
        }

        [TestMethod]
        public void ReplaceModel()
        {
            var path = "E:/ET/map/ET/etmain/_oldmaps2/battery_final.map";
            var map = new Map(path);

            string oldModel = "models/mapobjects/light/pendant10k.md3";
            string newModel = "REPLACED/MODEL/WITH/TOOL.md3";

            var replaced = map.ReplaceModel(oldModel, newModel);
            Console.WriteLine();
        }

        [TestMethod]
        public void DeleteSingleTextured()
        {
            var path = "E:/ET/map/ET/etmain/maps/floodlit.map";
            var map = new Map(path);
            var deleted = map.DeleteSingleTextured("battery/sand_disturb", false);

            Console.WriteLine();
        }

        [TestMethod]
        public void SaveMap()
        {
            var path = "E:/ET/map/ET/etmain/maps/floodlit.map";
            var outp = "E:/ET/map/ET/etmain/maps/floodlit_Tool.map";
            var map = new Map(path);
            map.WriteToFile(outp);
        }

        [TestMethod]
        public void SaveMap2()
        {
            var path = "E:/ET/map/ET/etmain/_oldmaps2/battery_final.map";
            var outp = "E:/ET/map/ET/etmain/_oldmaps2/battery_final_tool.map";
            var map = new Map(path);
            map.WriteToFile(outp);
        }

        [TestMethod]
        public void ParseEntityProperties()
        {
            var map = new Map(null);
            var first = "\"_floodlight\" \"1 1 1 512 4\"";
            var second = "\"classname\" \"worldspawn\"";

            var entity = new Entity();

            map.ParseProps(first, entity);
            map.ParseProps(second, entity);

            Assert.IsTrue(entity.Props.ContainsKey("_floodlight"));
            Assert.IsTrue(entity.Classname() != null);
            Assert.AreEqual(entity.Props[Tokens.classname], "worldspawn");
        }

        [TestMethod]
        public void ParsePatchValues()
        {
            var map = new Map(null);
            var patch = new Patch();

            map.ParsePatchValues("( 9 3 0 0 0 )", patch);

            Array.Equals(new int[] { 9, 3, 0, 0, 0 }, patch.Values);
        }

        [TestMethod]
        public void ParsePatchComponent()
        {
            var line = "( ( -368 312 0 1.875000 7 ) ( -368 312 112 1.875000 3.500000 ) ( -368 312 224 1.875000 0 ) )";
            var patch = new PatchComponent(line);

            Assert.AreEqual(line, patch.GetData());
        }

        [TestMethod]
        public void BrushFaceParse()
        {
            var line = "( -512 512 512 ) ( -512 -512 512 ) ( -512 -512 256 ) common/caulk 0 0 0 0.500000 0.500000 0 4 0";
            var face = new Face(line);

            Assert.AreEqual(line, face.GetData());
            Assert.IsFalse(face.Detail);

            line = "( -136 -64 264 ) ( -199 -64 305 ) ( -220 -12 305 ) battery/rock_graynoise 0 -384 0 0.500000 0.500000 134217728 0 0";
            face = new Face(line);

            Assert.AreEqual(line, face.GetData());
            Assert.IsTrue(face.Detail);
        }

        [TestMethod]
        public void ParseId()
        {
            var map = new Map(null);

            var first = "// brush 1";
            var second = "// entity 3432           ";
            var third = "// brush 1322 ";

            Assert.AreEqual(map.ParseId(first), 1);
            Assert.AreEqual(map.ParseId(second), 3432);
            Assert.AreEqual(map.ParseId(third), 1322);
        }

    */
    }
}
