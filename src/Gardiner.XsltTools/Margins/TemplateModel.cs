﻿using System;

namespace Gardiner.XsltTools.Margins
{
    public class TemplateModel
    {
        public string Name { get; set; }
        public string Mode { get; set; }
        public string Match { get; set; }
        public int Start { get; set; }

        private bool Equals(TemplateModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.InvariantCulture) && string.Equals(Mode, other.Mode, StringComparison.InvariantCulture) && string.Equals(Match, other.Match, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TemplateModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? StringComparer.InvariantCulture.GetHashCode(Name) : 0);
                hashCode = (hashCode*397) ^ (Mode != null ? StringComparer.InvariantCulture.GetHashCode(Mode) : 0);
                hashCode = (hashCode*397) ^ (Match != null ? StringComparer.InvariantCulture.GetHashCode(Match) : 0);
                return hashCode;
            }
        }
    }
}