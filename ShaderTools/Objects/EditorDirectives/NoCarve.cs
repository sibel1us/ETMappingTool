﻿using ShaderTools.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTools.Objects.EditorDirectives
{
    [Format(Token.qer_nocarve)]
    [ClassDisplay(Name = "No Carve", Description = "Brush won't be affected by CSG subtract, useful for water and fog.")]
    public class NoCarve : IEditorDirective
    {
    }
}
