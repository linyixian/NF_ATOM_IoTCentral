//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NF_ATOM_IoTCentral
{
    
    internal partial class Resource
    {
        private static System.Resources.ResourceManager manager;
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((Resource.manager == null))
                {
                    Resource.manager = new System.Resources.ResourceManager("NF_ATOM_IoTCentral.Resource", typeof(Resource).Assembly);
                }
                return Resource.manager;
            }
        }
        internal static byte[] GetBytes(Resource.BinaryResources id)
        {
            return ((byte[])(nanoFramework.Runtime.Native.ResourceUtility.GetObject(ResourceManager, id)));
        }
        [System.SerializableAttribute()]
        internal enum BinaryResources : short
        {
            BaltimoreCyberTrustRoot = -10127,
        }
    }
}
