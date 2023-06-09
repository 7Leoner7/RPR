﻿using RPR.Shapes;
using System;
using System.Collections.Generic;

namespace RPR.Model
{
    public interface IRule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> TextRules { get; }
    }

    public interface IRuleable
    {
        public List<Rule> Rules { get; }

        public bool IsFollowInnerRules { get; set; }

        public bool IsFollowOuterRules { get; set; }
    }

    public class Rule : IRule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        protected Action<ArgsSmartShapes> Instruction { get; set; }
        public List<string> TextRules { get; }

        public Rule()
        {
            TextRules = new List<string>();
        }

        public void SetInstrution(Action<ArgsSmartShapes> instruction) =>
            this.Instruction = instruction;

        public Action<ArgsSmartShapes> GetInstruction() => this.Instruction;
    }
}
