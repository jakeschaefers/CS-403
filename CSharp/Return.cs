namespace CSharp
{
    public class Return : Exception
    {
        public readonly object value;

        public Return(object value) : base(null, null)
        {
            this.value = value;
        }
    }
}