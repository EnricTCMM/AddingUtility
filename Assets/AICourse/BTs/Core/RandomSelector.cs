using System;


namespace BTs
{
    class RandomSelector : Selector
    {
        private Random alea = new Random();

        public override void Initialize()
        {
            base.Initialize();
            // after the "normal" initialization, just shuffle the children

            for (int i = 1; i <= children.Count; i++)
            {
                int a = alea.Next(0, children.Count);
                int b = alea.Next(0, children.Count);
                INode inter = children[a];
                children[a] = children[b];
                children[b] = inter;
            }
        }
    }
}
