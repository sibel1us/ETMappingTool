﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTools
{
    /// <summary>
    /// Provides constant string expressions for shader display attributes, parsing and writing.
    /// </summary>
    public static class Token
    {
        static Token()
        {
        }

        #region File System
        public const string ETexe = "ET.exe";
        public const string etmain = "etmain";
        public const string textures = "textures";
        public const string scripts = "scripts";
        public const string models = "models";
        public const string pak0 = "pak0.pk3";
        #endregion File System

        #region General Shader Elements
        public const string commentPrefix = "//";
        public const string OpeningBrace = "{";
        public const string ClosingBrace = "}";
        public const string ShaderNameRegex = @"textures(\/([a-z0-9]([\w-]*[a-z0-9])?)){2,}";
        #endregion General Shader Elements

        #region Compiler Directives
        /// <summary>
        /// Prefix, used for parsing exclusively.
        /// </summary>
        public const string surfaceparm = "surfaceparm";
        public const string q3map = "q3map_";
        public const string tessSize = "tessSize";
        public const string q3map_tessSize = "q3map_tessSize";
        public const string q3map_backshader = "q3map_backshader";
        public const string q3map_globaltexture = "q3map_globaltexture";
        public const string q3map_sun = "q3map_sun";
        public const string q3map_surfacelight = "q3map_surfacelight";
        public const string q3map_lightimage = "q3map_lightimage";
        public const string q3map_cloneshader = "q3map_cloneshader";
        public const string q3map_invert = "q3map_invert";
        public const string q3map_vertexscale = "q3map_vertexscale";
        public const string q3map_texturesize = "q3map_texturesize";
        #endregion Compiler Directives

        #region Editor Directives
        /// <summary>:p
        /// Used for parsing exclusively.
        /// </summary>
        public const string qer = "qer_";
        public const string qer_editorimage = "qer_editorImage";
        public const string qer_nocarve = "qer_noCarve";
        public const string qer_trans = "qer_trans";
        #endregion Editor Directives

        #region General Directives
        public const string deformVertexes = "deformVertexes";
        public const string autosprite = "autosprite";
        public const string autosprite2 = "autosprite2";
        public const string wave = "wave";
        public const string normal = "normal";
        public const string bulge = "bulge";
        public const string move = "move";

        public const string polygonOffset = "polygonOffset";
        public const string lightgridAmbientMultiplier = "lightgridmulamb";
        public const string lightgridDirectionalMultiplier = "lightgridmuldir";
        public const string entityMergable = "entityMergable";
        public const string portal = "portal";
        public const string nofog = "nofog";
        public const string cull = "cull";
        public const string distancecull = "distancecull";
        public const string nocompress = "nocompress";
        public const string allowcompress = "allowcompress";
        public const string skyparms = "skyparms";
        public const string sunshader = "sunshader";
        public const string fogparms = "fogparms";
        public const string fogvars = "fogvars";
        public const string skyfogvars = "skyfogvars";
        public const string waterfogvars = "waterfogvars";

        /// <summary>implicitMap is recognized by ET any "implicit_" except "Mask" and "Blend". Case sensitive.</summary>
        public const string @implicit = "implicit";
        public const string implicitMap = "implicitMap";
        public const string implicitMask = "implicitMask";
        public const string implicitBlend = "implicitBlend";
        #endregion General Directives

        #region Stage Directives
        public const string map = "map";
        public const string animMap = "animMap";
        public const string clampMap = "clampMap";
        public const string videoMap = "videoMap";

        public const string blendFunc = "blendFunc";
        public const string add = "add";
        public const string filter = "filter";
        public const string blend = "blend";

        public const string rgbGen = "rgbGen";
        public const string tcMod = "tcMod";
        public const string map_lightmap = "$lightmap";
        public const string map_dlight = "$dlight";
        public const string map_whiteimage = "$whiteimage";
        public const string map_whiteimage2 = "*white";
        #endregion Stage Directives

        #region Miscellaneous
        #endregion Miscellaneous
    }
}
