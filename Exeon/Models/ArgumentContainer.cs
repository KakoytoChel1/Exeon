namespace Exeon.Models
{
    public class ArgumentContainer<TParam1, TParam2>
    {
        public TParam1 Value1 { get; private set; }
        public TParam2 Value2 { get; private set; }

        public ArgumentContainer(TParam1 value1, TParam2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
