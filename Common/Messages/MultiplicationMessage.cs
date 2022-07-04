namespace Common.Messages
{
    public class MultiplicationMessage : Message
    {
        public double Factor1 { get; set; }
        public double Factor2 { get; set; }

        public double Result { get => Factor1 * Factor2; }

        public MultiplicationMessage(double factor1, double factor2)
        {
            Factor1 = factor1;
            Factor2 = factor2;
        }

        public MultiplicationMessage()
        {

        }

        public override Message.Type GetMessageType() => Message.Type.Multiplication;
    }
}
