﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ETMapHelper.Exceptions;

namespace ETMapHelper.Maps
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Map
    {
        /// <summary>List of entities containing all the entities and brushes of the map.</summary>
        public List<Entity> Entities;

        /// <summary>The absolute path of the .map file, ie "C:/ET/etmain/maps/oasis.map"</summary>
        public string FileName;

        /// <summary>Used only for debugging.</summary>
        private string DebuggerDisplay
        {
            get
            {
                int entcount = 0;
                int brushcount = 0;

                foreach (var entity in Entities)
                {
                    entcount++;
                    foreach (var brush in entity.Brushes) brushcount++;
                }

                return $"Map \"{FileName}\", {entcount} entities, {brushcount} brushes";
            }
        }

        /// <summary>
        /// Creates a new Map-object from an already established .map file.
        /// </summary>
        /// <param name="filePath">Full path to the file.</param>
        public Map(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            FileName = Path.GetFileName(filePath);
            ReadFromFile(filePath);
        }

        /// <summary>
        /// Saves a Map-object back into a .map-file.
        /// </summary>
        /// <param name="filePath">Output file path.</param>
        public void WriteToFile(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var entity in Entities)
                {
                    writer.WriteLine(Tokens.Entity + entity.Id);                //  // entity id
                    writer.WriteLine(Tokens.CBL);                               //  {
                    foreach (var key in entity.Props.Keys)                      //  "classname" "worldspawn"
                        writer.WriteLine($"\"{key}\" \"{entity.Props[key]}\""); //  "_minlight" "8"

                    foreach (var brush in entity.Brushes)
                    {
                        writer.WriteLine(Tokens.Brush + brush.Id);               //  // brush id
                        writer.WriteLine(Tokens.CBL);                           //  {

                        foreach (var face in ((Brush)brush).Faces)           // ( 1 2 3 ) ( -1 -2 -3 ) ...
                        {
                            writer.WriteLine(face.GetData());               // ( 0 0 0 ) ( 2 -5 213 ) ...
                        }

                        writer.WriteLine(Tokens.CBR);                           // }
                    }
                    foreach (var patch in entity.Patches)
                    {
                        writer.WriteLine(Tokens.Brush + patch.Id);               //  // brush id
                        writer.WriteLine(Tokens.CBL);                           //  {

                        writer.WriteLine(Tokens.PathDef2);                  // patchDef2
                        writer.WriteLine(Tokens.CBL);                       // {
                        writer.WriteLine(patch.Texture);            // common/caulk
                        writer.WriteLine(patch.GetValues());        // ( 9 3 0 0 0 )
                        writer.WriteLine(Tokens.QL);                        // (
                        foreach (var comp in patch.Components)      // ( ( x y z ) ( x y z ) ...
                            writer.WriteLine(comp.GetData());               // ( ( x y z ) ( x y z ) ...
                        writer.WriteLine(Tokens.QR);                        // )
                        writer.WriteLine(Tokens.CBR);                       // }

                        writer.WriteLine(Tokens.CBR);                           // }

                    }
                    writer.WriteLine(Tokens.CBR);                               // }
                }
            }
        }

        /// <summary>
        /// Reads a .map file.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        public void ReadFromFile(string filePath)
        {
            Entities = new List<Entity>();

            string current = null; // Keeps track of what type of block we are in.
            string line = "";    // Current line read from the file.
            int lines = 0;       // Total lines read.

            bool openingBracket = false; // Expecting to see { on the next line.
            int patchState = 0;          // The depth of the patch object we are in.

            Entity entity = null;   // Current entity. If null, read for next entity.
            Brush brush = null;     // Current brush. Read brush faces are saved onto this.
            Patch patch = null;     // Current patch. Patch tex, props etc. are saved onto this.

            using (var reader = new StreamReader(filePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    lines++;
                    if (line.Trim().Length == 0) continue; // Skip empty lines.

                    // Expecting {, so accept no other input.
                    if (openingBracket)
                    {
                        if (!line.StartsWith(Tokens.CBL)) throw new ParseException($"Expecting {Tokens.CBL} on line {lines}.");
                        openingBracket = false;
                        continue;
                    }

                    // Currently not in any entity, brush or patch. Expecting to find entity definition.
                    if (current == null)
                    {
                        if (!line.StartsWith(Tokens.Entity)) throw new ParseException($"Error on line {lines}, expecting entity.");

                        // Create new entity object and set it as current.
                        current = nameof(Entity);
                        entity = new Entity();
                        entity.Id = ParseId(line);
                        openingBracket = true;
                        continue;
                    }

                    // Currently in an entity. Expecting closing bracket }, key/value pair or brush definition.
                    if (current == nameof(Entity))
                    {
                        // End of entity. Ensure the entity has a classname, save to list and initialize again.
                        if (line.StartsWith(Tokens.CBR))
                        {
                            if (entity.Classname == null)
                                throw new ParseException($"Trying to add entity {entity.Id} without classname on line {lines}.");

                            Entities.Add(entity);
                            entity = null;
                            current = null;
                            continue;
                        }

                        // Key/Value pair.
                        if (line.StartsWith(Tokens.Quote))
                        {
                            ParseProps(line, entity);
                            continue;
                        }

                        // New brush/patch definition.
                        if (line.StartsWith(Tokens.Brush))
                        {
                            current = "BrushOrPatch";
                            brush = new Brush();
                            brush.Id = ParseId(line);
                            openingBracket = true;
                            continue;
                        }

                        // None of the above.
                        throw new ParseException($"Unexpected line {lines}: \"{line}\"");
                    }

                    // Brush definition encountered, don't know if its a brush or a patch yet.
                    if (current == "BrushOrPatch")
                    {
                        // Patch definition, set patch as the current object and scrap the brush.
                        if (line.StartsWith(Tokens.PathDef2))
                        {
                            current = nameof(Patch);
                            patch = new Patch();
                            patch.Id = brush.Id;
                            brush = null;
                            openingBracket = true;
                            patchState = 1;
                            continue;
                        }

                        // If not a patch definition, this line should start with a ( for brush face.
                        if (line.StartsWith(Tokens.QL)) current = nameof(Brush);
                        else throw new ParseException($"Expecting patch definition or a brush face on line {lines}.");
                    }

                    // Reading a brush, after the definition line and {
                    if (current == nameof(Brush))
                    {
                        // End of brush }. Ensure the brush isn't broken, add to entity and set current back to parent entity.
                        if (line.StartsWith(Tokens.CBR))
                        {
                            if (brush.Faces.Count < 4)
                                throw new ParseException($"Trying to add broken brush {brush.Id} with {brush.Faces.Count} faces, on line {lines}.");

                            entity.Brushes.Add(brush);
                            brush = null;
                            current = nameof(Entity);
                            continue;
                        }

                        // Brush face definition, starts with (.
                        if (line.StartsWith(Tokens.QL))
                        {
                            ParseFace(line, brush);
                            continue;
                        }

                        // If neither of the above, something weird is in the brush.
                        throw new ParseException($"Unexpected line {lines}: \"{line}\"");
                    }

                    // Currently in a patch. Patches have an annoying multi-line structure, so keep track of reader's "position"
                    // with a tracking number.
                    if (current == nameof(Patch))
                    {
                        // After patchDef2 and the opening bracket
                        if (patchState == 1)
                        {
                            ParsePatchTexture(line, patch);
                            patchState = 2;
                            continue;
                        }

                        // After parsing texture.
                        if (patchState == 2)
                        {
                            ParsePatchValues(line, patch);
                            patchState = 3;
                            continue;
                        }

                        // Opening quote.
                        if (patchState == 3)
                        {
                            if (line.Trim() != Tokens.QL)
                                throw new ParseException($"Expecting {Tokens.QL} on line {lines}.");
                            patchState = 4;
                            continue;
                        }

                        // Reading vertex columns.
                        if (patchState == 4)
                        {
                            // End of patch, wait for another ).
                            if (line.StartsWith(Tokens.QR))
                            {
                                patchState = 5;
                                continue;
                            }

                            if (line.StartsWith(Tokens.QL))
                            {
                                ParsePatchComponent(line, patch);
                                continue;
                            }
                        }

                        // Waiting for final two )'s.
                        if (patchState > 4)
                        {
                            if (!line.StartsWith(Tokens.CBR))
                                throw new ParseException($"Expecting {Tokens.CBR} on line {lines}.");

                            // First of two )'s.
                            if (patchState == 5)
                            {
                                patchState++;
                                continue;
                            }

                            entity.Patches.Add(patch);
                            patch = null;
                            patchState = 0;
                            current = nameof(Entity);
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses an entity key/value pair.
        /// </summary>
        /// <param name="line">Example: "classname" "worldspawn"</param>
        /// <param name="entity">Target entity.</param>
        public void ParseProps(string line, Entity entity)
        {
            if (entity == null)
                throw new ParseException($"Trying to add properties onto a null entity.");

            string key = "";
            string value = "";
            var stringArray = line.ToLower().Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries);

            bool findingKey = true;

            foreach (var child in stringArray)
            {
                if (child.Trim().Length == 0) continue;

                if (findingKey)
                {
                    key = child.Trim();
                    findingKey = false;
                    continue;
                }

                value = child.Trim();
                break;
            }

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                throw new ParseException($"Entity key/value pair \"{key}\" \"{value}\" contains empty fields.");

            if (key.Equals(Tokens.classname))
                entity.Classname = key;
            else
                entity.Props.Add(key, value);
        }

        /// <summary>
        /// Parses a single brush face.
        /// </summary>
        /// <param name="line">Example: "( 0 0 0 ) ( 1 1 1 ) ( 2 2 2 ) common/caulk 0 0 0 0.5 0.5 0 4 0"</param>
        /// <param name="brush">Brush to save the face to.</param>
        public void ParseFace(string line, Brush brush)
        {
            Face face = new Face(line);
            brush.Faces.Add(face);
        }

        /// <summary>
        /// Parses a line containing a column of patch vertexes.
        /// </summary>
        /// <param name="line">Example: "( ( -368 372 0 0 6 ) ( -368 372 112 0 3 ) ( -368 372 224 0 0 ) )"</param>
        /// <param name="patch">Patch to save the component to.</param>
        public void ParsePatchComponent(string line, Patch patch)
        {
            var component = new PatchComponent(line);
            patch.Components.Add(component);
        }

        /// <summary>
        /// Saves the texture name from the line onto a patch.
        /// </summary>
        /// <param name="line">Example: "common/caulk"</param>
        /// <param name="patch">Patch to save the texture to.</param>
        public void ParsePatchTexture(string line, Patch patch)
        {
            patch.ParseTexture(line);
        }

        /// <summary>
        /// Parses the line after patch definition containing it's values.
        /// </summary>
        /// <param name="line">Example: "( 9 3 0 0 0 )"</param>
        /// <param name="patch">Patch to save the values to.</param>
        public void ParsePatchValues(string line, Patch patch)
        {
            var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var ex = $"Invalid patch definition line";

            for (int i = 0; i < split.Length; i++)
            {
                if (i == 0 && split[i] != Tokens.QL) throw new ParseException($"{ex} \"{line}\"");
                if (i == 6 && split[i] != Tokens.QR) throw new ParseException($"{ex} \"{line}\"");

                if (i == 0 || i == 6) continue;

                try
                {
                    patch.Values[i - 1] = int.Parse(split[i]);
                }
                catch
                {
                    throw new ParseException($"{ex}, \"{line}\" couldn't parse number.");
                }
            }
        }

        /// <summary>
        /// Parses Entity or Brush id from a line.
        /// </summary>
        /// <param name="line">Example: "// entity 3"</param>
        /// <returns>Parsed id</returns>
        public int ParseId(string line)
        {
            if (line.StartsWith(Tokens.Entity))
            {
                try
                {
                    return int.Parse(line.Substring(Tokens.Entity.Length).Trim());
                }
                catch
                {
                    throw new ParseException($"Couldn't parse entity id from line \"{line}\"");
                }
            }

            try
            {
                return int.Parse(line.Substring(Tokens.Brush.Length).Trim());
            }
            catch
            {
                throw new ParseException($"Couldn't parse brush id from line \"{line}\"");
            }
        }


    }
}