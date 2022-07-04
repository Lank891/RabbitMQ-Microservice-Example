namespace Common.Messages
{
    /// <summary>
    /// Message representing addition
    /// </summary>
    public class AdditionMessage : Message
    {
        public double Term1 { get; set; }
        public double Term2 { get; set; }

        public double Result { get => Term1 + Term2; }

        public AdditionMessage(double term1, double term2)
        {
            Term1 = term1;
            Term2 = term2;
        }

        public AdditionMessage()
        {

        }

        public override Message.Type GetMessageType() => Message.Type.Addition;
    }
}
