﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;

using CodePlex.XPathParser;

using JetBrains.Annotations;

namespace Gardiner.XsltTools
{
    public class XPathTreeBuilder : IXPathBuilder<XElement>
    {
        public void StartBuild()
        {
        }

        public XElement EndBuild(XElement result)
        {
            return result;
        }

#pragma warning disable CA1720 // Identifier contains type name
        public XElement String(string value)
#pragma warning restore CA1720 // Identifier contains type name
        {
            return new XElement("string", new XAttribute("value", value));
        }

        public XElement Number(string value)
        {
            return new XElement("number", new XAttribute("value", value));
        }

        public XElement Operator(XPathOperator op, XElement left, XElement right)
        {
            if (op == XPathOperator.UnaryMinus)
            {
                return new XElement("negate", left);
            }

#pragma warning disable CA1305 // This rule is invalid for Enum
            return new XElement(op.ToString(), left, right);
#pragma warning restore CA1305 // This rule is invalid for Enum
        }

        public XElement Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
        {
#pragma warning disable CA1305 // This rule is invalid for Enum
            return new XElement(xpathAxis.ToString(),
                new XAttribute("nodeType", nodeType.ToString()),
                new XAttribute("prefix", prefix ?? "(null)"),
                new XAttribute("name", name ?? "(null)")
                );
#pragma warning restore CA1305 // This rule is invalid for Enum
        }

        public XElement JoinStep(XElement left, XElement right)
        {
            return new XElement("step", left, right);
        }

        public XElement Predicate(XElement node, XElement condition, bool reverseStep)
        {
            return new XElement("predicate", new XAttribute("reverse", reverseStep),
                node, condition
                );
        }

        public XElement Variable(string prefix, string name)
        {
            return new XElement("variable",
                new XAttribute("prefix", prefix ?? "(null)"),
                new XAttribute("name", name ?? "(null)")
                );
        }

        public XElement Function(string prefix, string name, [NotNull] IList<XElement> args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var xe = new XElement("function",
                new XAttribute("prefix", prefix ?? "(null)"),
                new XAttribute("name", name ?? "(null)")
                );
            foreach (var e in args)
            {
                xe.Add(e);
            }
            return xe;
        }
    }
}