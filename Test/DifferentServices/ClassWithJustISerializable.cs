using System.Runtime.Serialization;

namespace Test.DifferentServices
{
    public class ClassWithJustISerializable : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}