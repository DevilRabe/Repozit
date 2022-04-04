using System.ServiceModel;
namespace ChartiX
{
    class ServerUser
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public OperationContext opertionContext { get; set; }
    }
}
