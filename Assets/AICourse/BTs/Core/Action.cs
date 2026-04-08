using System;

namespace BTs
{
    public abstract class Action : Node
    {
        
        public Action() {
            Name = GetType().ToString();
            
            // Remove ACTION or ACTION_ prefix (case-insensitive)
            if (Name.Length >= 7 && Name.Substring(0, 7).Equals("ACTION_", StringComparison.OrdinalIgnoreCase))
            {
                Name = Name.Substring(7);
            }
            else if (Name.Length >= 6 && Name.Substring(0, 6).Equals("ACTION", StringComparison.OrdinalIgnoreCase))
            {
                Name = Name.Substring(6);
            }
            
        }
        public Action(string name) : base(name) { }

    }
}
